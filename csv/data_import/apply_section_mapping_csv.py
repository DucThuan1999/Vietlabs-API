#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Áp dữ liệu từ CSV (Section mapping) vào bảng employee: department_id, section_id, employee_title_id.
Cột CSV (delimiter ;): Mã Nhân Viên, Chi Nhánh, Phòng Ban, Bộ phận, Chức Vụ.

Quy tắc:
- Chi nhánh: khớp branch.name_vi (strip, so sánh không phân biệt hoa thường).
- Phòng ban: so sánh name_vi sau khi normalize — bỏ khoảng trắng và dấu '-' (cả – —).
- Bộ phận: khớp section.name_vi trong đúng phòng ban (strip, không phân biệt hoa thường).
- Chức vụ: khớp employee_title.name_vi (strip, không phân biệt hoa thường).

Dòng lỗi / không map được ghi ra file CSV failures (mặc định cùng thư mục với file nguồn).
"""

from __future__ import annotations

import argparse
import csv
import os
import re
import sys
from datetime import datetime
from typing import Dict, List, Optional, Tuple

import pyodbc

CONNECTION_STRING = (
    "Driver={ODBC Driver 17 for SQL Server};"
    "Server=192.168.2.21,49938;"
    "Database=VietLabs;"
    "UID=sa;"
    "PWD=Sbt@2024;"
    "TrustServerCertificate=yes;"
    "Encrypt=yes;"
)

DEFAULT_CSV = os.path.normpath(
    os.path.join(
        os.path.dirname(__file__),
        "..",
        "..",
        "..",
        "data",
        "Section mapping.csv",
    )
)

HYPHENS = re.compile(r"[\s\-–—]+")


def strip_ci(s: Optional[str]) -> str:
    if not s:
        return ""
    return " ".join(str(s).split()).strip()


def norm_branch_key(s: str) -> str:
    return strip_ci(s).casefold()


def norm_dept_compare(s: str) -> str:
    """Phòng ban: bỏ space và hyphen (mọi loại) giữa các ký tự — dùng cho so sánh."""
    t = strip_ci(s).casefold()
    return HYPHENS.sub("", t)


def norm_section_key(s: str) -> str:
    return strip_ci(s).casefold()


def norm_title_key(s: str) -> str:
    return strip_ci(s).casefold()


def load_branches(cursor) -> Tuple[Dict[str, List[str]], List[Tuple]]:
    """name_vi (casefold strip) -> [branch_id,...]"""
    cursor.execute(
        "SELECT branch_id, name_vi FROM branch WHERE status = N'Active'"
    )
    m: Dict[str, List[str]] = {}
    rows = []
    for bid, nv in cursor.fetchall():
        bid_s = str(bid)
        k = norm_branch_key(nv or "")
        if not k:
            continue
        m.setdefault(k, []).append(bid_s)
        rows.append((bid_s, nv or ""))
    return m, rows


def load_departments(cursor) -> List[dict]:
    cursor.execute(
        """
        SELECT department_id, branch_id, name_vi
        FROM department
        WHERE status = N'Active'
        """
    )
    out = []
    for did, bid, nv in cursor.fetchall():
        out.append(
            {
                "id": str(did),
                "branch_id": str(bid),
                "name_vi": nv or "",
                "dept_cmp": norm_dept_compare(nv or ""),
            }
        )
    return out


def load_sections(cursor) -> List[dict]:
    cursor.execute(
        """
        SELECT section_id, department_id, name_vi
        FROM section
        WHERE status = N'Active'
        """
    )
    out = []
    for sid, did, nv in cursor.fetchall():
        out.append(
            {
                "id": str(sid),
                "department_id": str(did),
                "name_vi": nv or "",
                "sec_key": norm_section_key(nv or ""),
            }
        )
    return out


def load_titles(cursor) -> Tuple[Dict[str, List[str]], List[str]]:
    cursor.execute(
        "SELECT employee_title_id, name_vi FROM employee_title WHERE status = N'Active'"
    )
    m: Dict[str, List[str]] = {}
    dup_notes = []
    for tid, nv in cursor.fetchall():
        k = norm_title_key(nv or "")
        if not k:
            continue
        m.setdefault(k, []).append(str(tid))
    for k, ids in m.items():
        if len(ids) > 1:
            dup_notes.append(f"employee_title name_vi trùng key sau normalize: {k!r} ({len(ids)} id)")
    return m, dup_notes


def load_employees_by_code(cursor) -> Dict[str, str]:
    cursor.execute(
        "SELECT employee_id, employee_code FROM employee WHERE status = N'Active'"
    )
    by_code: Dict[str, str] = {}
    for eid, code in cursor.fetchall():
        c = strip_ci(code or "")
        if c:
            by_code[c.casefold()] = str(eid)
    return by_code


def read_mapping_rows(path: str) -> List[dict]:
    with open(path, "r", encoding="utf-8-sig", newline="") as f:
        reader = csv.DictReader(f, delimiter=";")
        fieldnames = [x.strip() if x else x for x in (reader.fieldnames or [])]
        # Chuẩn hóa key
        rows = []
        for raw in reader:
            row = {((k or "").strip()): (v if v is None else str(v).strip()) for k, v in raw.items()}
            code = strip_ci(row.get("Mã Nhân Viên") or row.get("Mã nhân viên") or "")
            if not code:
                continue
            rows.append(
                {
                    "stt": row.get("STT", ""),
                    "ma_nv": code,
                    "ten": row.get("Tên Nhân Viên") or row.get("Tên nhân viên") or "",
                    "chi_nhanh": row.get("Chi Nhánh") or row.get("Chi nhánh") or "",
                    "phong_ban": row.get("Phòng Ban") or row.get("Phòng ban") or "",
                    "bo_phan": row.get("Bộ phận") or row.get("Bộ phận") or "",
                    "chuc_vu": row.get("Chức Vụ") or row.get("Chức vụ") or "",
                }
            )
        return rows


def resolve_branch(branch_map: Dict[str, List[str]], name: str) -> Tuple[Optional[str], str]:
    k = norm_branch_key(name)
    if not k:
        return None, "Chi nhánh trống"
    ids = branch_map.get(k)
    if not ids:
        return None, f"Không tìm thấy chi nhánh name_vi={name!r}"
    if len(ids) > 1:
        return None, f"Nhiều chi nhánh trùng tên sau normalize: {name!r} -> {ids}"
    return ids[0], ""


def resolve_department(
    departments: List[dict], branch_id: str, pb_csv: str
) -> Tuple[Optional[str], str]:
    target = norm_dept_compare(pb_csv)
    if not target:
        return None, "Phòng ban trống"
    matches = [d for d in departments if d["branch_id"] == branch_id and d["dept_cmp"] == target]
    if not matches:
        return None, f"Không có phòng ban khớp (branch_id={branch_id}, phòng ban CSV={pb_csv!r}, key={target!r})"
    if len(matches) > 1:
        names = [m["name_vi"] for m in matches]
        return None, f"Nhiều phòng ban khớp: {names}"
    return matches[0]["id"], ""


def resolve_section(
    sections: List[dict], department_id: str, bo_phan: str
) -> Tuple[Optional[str], str]:
    k = norm_section_key(bo_phan)
    if not k:
        return None, "Bộ phận trống"
    matches = [s for s in sections if s["department_id"] == department_id and s["sec_key"] == k]
    if not matches:
        return None, (
            f"Không có bộ phận name_vi khớp trong phòng ban (department_id={department_id}, "
            f"bộ phận CSV={bo_phan!r})"
        )
    if len(matches) > 1:
        return None, f"Nhiều bộ phận trùng tên trong phòng ban: {[m['name_vi'] for m in matches]}"
    return matches[0]["id"], ""


def resolve_title(title_map: Dict[str, List[str]], chuc_vu: str) -> Tuple[Optional[str], str]:
    k = norm_title_key(chuc_vu)
    if not k:
        return None, "Chức vụ trống"
    ids = title_map.get(k)
    if not ids:
        return None, f"Không tìm thấy chức vụ name_vi={chuc_vu!r}"
    if len(ids) > 1:
        return None, f"Nhiều chức vụ trùng tên sau normalize: {chuc_vu!r}"
    return ids[0], ""


def main():
    p = argparse.ArgumentParser(description="Apply Section mapping CSV -> employee")
    p.add_argument("--csv", default=DEFAULT_CSV, help="Đường dẫn Section mapping.csv")
    p.add_argument("--dry-run", action="store_true", help="Chỉ kiểm tra, không UPDATE")
    p.add_argument(
        "--failures",
        default="",
        help="File CSV log lỗi (mặc định: cạnh file nguồn, timestamp)",
    )
    args = p.parse_args()
    csv_path = os.path.abspath(args.csv)
    if not os.path.isfile(csv_path):
        print(f"Không thấy file: {csv_path}", file=sys.stderr)
        return 1

    ts = datetime.now().strftime("%Y%m%d_%H%M%S")
    failures_path = args.failures or os.path.join(
        os.path.dirname(csv_path), f"section_mapping_apply_failures_{ts}.csv"
    )

    rows = read_mapping_rows(csv_path)
    conn = pyodbc.connect(CONNECTION_STRING)
    conn.autocommit = False
    cur = conn.cursor()

    branch_map, _ = load_branches(cur)
    departments = load_departments(cur)
    sections = load_sections(cur)
    title_map, title_warnings = load_titles(cur)
    emp_by_code = load_employees_by_code(cur)

    for w in title_warnings:
        print("WARN:", w)

    ok = 0
    fail_rows: List[dict] = []

    for r in rows:
        emp_id = emp_by_code.get(r["ma_nv"].casefold())
        if not emp_id:
            fail_rows.append(
                {
                    **r,
                    "ly_do": "Không tìm thấy nhân viên (employee_code, Active)",
                    "employee_id": "",
                    "department_id": "",
                    "section_id": "",
                    "employee_title_id": "",
                }
            )
            continue

        bid, err = resolve_branch(branch_map, r["chi_nhanh"])
        if err:
            fail_rows.append({**r, "ly_do": err, "employee_id": emp_id, "department_id": "", "section_id": "", "employee_title_id": ""})
            continue

        did, err = resolve_department(departments, bid, r["phong_ban"])
        if err:
            fail_rows.append({**r, "ly_do": err, "employee_id": emp_id, "department_id": "", "section_id": "", "employee_title_id": ""})
            continue

        sid, err = resolve_section(sections, did, r["bo_phan"])
        if err:
            fail_rows.append({**r, "ly_do": err, "employee_id": emp_id, "department_id": did, "section_id": "", "employee_title_id": ""})
            continue

        tid, err = resolve_title(title_map, r["chuc_vu"])
        if err:
            fail_rows.append({**r, "ly_do": err, "employee_id": emp_id, "department_id": did, "section_id": sid, "employee_title_id": ""})
            continue

        if args.dry_run:
            ok += 1
            continue

        cur.execute(
            """
            UPDATE employee
            SET department_id = ?, section_id = ?, employee_title_id = ?, updated_at = SYSUTCDATETIME()
            WHERE employee_id = ?
            """,
            (did, sid, tid, emp_id),
        )
        if cur.rowcount != 1:
            fail_rows.append(
                {
                    **r,
                    "ly_do": f"UPDATE rowcount={cur.rowcount}",
                    "employee_id": emp_id,
                    "department_id": did,
                    "section_id": sid,
                    "employee_title_id": tid,
                }
            )
            continue
        ok += 1

    if fail_rows:
        fieldnames = [
            "stt",
            "ma_nv",
            "ten",
            "chi_nhanh",
            "phong_ban",
            "bo_phan",
            "chuc_vu",
            "ly_do",
            "employee_id",
            "department_id",
            "section_id",
            "employee_title_id",
        ]
        with open(failures_path, "w", encoding="utf-8-sig", newline="") as out:
            w = csv.DictWriter(out, fieldnames=fieldnames, delimiter=";")
            w.writeheader()
            for fr in fail_rows:
                w.writerow({k: fr.get(k, "") for k in fieldnames})
        print(f"Đã ghi {len(fail_rows)} dòng lỗi -> {failures_path}")

    if args.dry_run:
        print(f"DRY-RUN: sẽ áp được {ok} dòng, lỗi {len(fail_rows)} (không ghi DB)")
        conn.close()
        return 0 if not fail_rows else 2

    conn.commit()
    print(f"Cập nhật thành công: {ok} nhân viên. Lỗi: {len(fail_rows)}.")
    conn.close()
    return 0 if not fail_rows else 2


if __name__ == "__main__":
    sys.exit(main())
