#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Cập nhật section_code theo cấu trúc: {branch_code}-{department_code}-BP-{số thứ tự 2 chữ số}.
- Gom theo department_id, sắp xếp: ưu tiên số trong hậu tố -BP-xx của mã cũ, sau đó theo name_vi.
- Hai bước: gán mã tạm (tránh trùng khi có unique index), rồi gán mã đích.
Dùng cùng CONNECTION_STRING với import_employee.py — sửa nếu môi trường khác.
"""

import os
import re
import sys
import uuid

import pyodbc

# Giữ đồng bộ với import_employee.py
CONNECTION_STRING = (
    "Driver={ODBC Driver 17 for SQL Server};"
    "Server=192.168.2.21,49938;"
    "Database=VietLabs;"
    "UID=sa;"
    "PWD=Sbt@2024;"
    "TrustServerCertificate=yes;"
    "Encrypt=yes;"
)

BP_SUFFIX = re.compile(r"-BP-(\d+)$", re.IGNORECASE)


def fetch_sections(cursor):
    cursor.execute(
        """
        SELECT
            s.section_id,
            s.section_code,
            s.name_vi,
            d.department_id,
            d.department_code,
            b.branch_code
        FROM section s
        INNER JOIN department d ON s.department_id = d.department_id
        INNER JOIN branch b ON d.branch_id = b.branch_id
        WHERE s.status = N'Active'
        ORDER BY d.department_id, s.name_vi
        """
    )
    rows = []
    for r in cursor.fetchall():
        rows.append(
            {
                "section_id": str(r[0]),
                "section_code": (r[1] or "").strip(),
                "name_vi": (r[2] or "").strip(),
                "department_id": str(r[3]),
                "department_code": (r[4] or "").strip(),
                "branch_code": (r[5] or "").strip(),
            }
        )
    return rows


def sort_key(row):
    m = BP_SUFFIX.search(row["section_code"])
    n = int(m.group(1)) if m else 10**9
    return (n, row["name_vi"] or "")


def build_plans(rows):
    by_dept = {}
    for row in rows:
        by_dept.setdefault(row["department_id"], []).append(row)
    plans = []
    for dept_id, dept_rows in by_dept.items():
        dept_rows.sort(key=sort_key)
        bc = dept_rows[0]["branch_code"]
        dc = dept_rows[0]["department_code"]
        if not bc or not dc:
            for r in dept_rows:
                plans.append(
                    {
                        "section_id": r["section_id"],
                        "old": r["section_code"],
                        "new": None,
                        "error": "Thiếu branch_code hoặc department_code",
                    }
                )
            continue
        for i, r in enumerate(dept_rows, start=1):
            # Tránh lặp chi nhánh khi department_code đã là "{branch}-{phòng}"
            bu, du = bc.upper(), dc.upper()
            if du.startswith(bu + "-"):
                base = dc
            else:
                base = f"{bc}-{dc}"
            new_code = f"{base}-BP-{i:02d}"
            plans.append(
                {
                    "section_id": r["section_id"],
                    "old": r["section_code"],
                    "new": new_code,
                    "error": None,
                }
            )
    return plans


def main():
    dry_run = "--dry-run" in sys.argv
    conn = pyodbc.connect(CONNECTION_STRING)
    conn.autocommit = False
    cursor = conn.cursor()
    rows = fetch_sections(cursor)
    plans = build_plans(rows)
    to_apply = [p for p in plans if p["new"] and p["old"] != p["new"]]
    errors = [p for p in plans if p["error"]]

    for p in errors:
        print(f"[SKIP] {p['section_id']}: {p['error']}")

    print(f"Tổng section: {len(plans)}, cần đổi mã: {len(to_apply)}, dry_run={dry_run}")
    for p in to_apply[:30]:
        print(f"  {p['old']!r} -> {p['new']!r}")
    if len(to_apply) > 30:
        print(f"  ... và {len(to_apply) - 30} dòng khác")

    if dry_run:
        conn.close()
        return 0

    if not to_apply and not errors:
        print("Không có thay đổi.")
        conn.close()
        return 0

    try:
        # Bước 1: mã tạm duy nhất
        for p in to_apply:
            tmp = f"__MIG_{uuid.uuid4().hex[:24]}__"
            cursor.execute(
                "UPDATE section SET section_code = ? WHERE section_id = ?",
                (tmp, p["section_id"]),
            )
        # Bước 2: mã đích
        for p in to_apply:
            cursor.execute(
                "UPDATE section SET section_code = ?, updated_at = SYSUTCDATETIME() WHERE section_id = ?",
                (p["new"], p["section_id"]),
            )
        conn.commit()
        print("Đã commit cập nhật section_code.")
    except Exception as e:
        conn.rollback()
        print(f"Lỗi, đã rollback: {e}")
        return 1
    finally:
        conn.close()
    return 0


if __name__ == "__main__":
    sys.exit(main())
