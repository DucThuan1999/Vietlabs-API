#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
INSERT đơn vị tính từ data/Danh_sach_don_vi_tinh.csv vào dbo.unit_of_measure.

- Chỉ INSERT: bỏ qua dòng đã tồn tại (khớp import_analysis_item.normalize_text trên
  name_vi / name_en / unit_of_measure_code — cùng logic augment_master_maps / Capability).
- Mã: dùng cột UnitOfMeasureCode trên CSV nếu còn trống trên DB; nếu trùng mã hoặc không hợp lệ
  thì gán DVT-xxx tiếp theo (MAX số của mọi mã DVT-<số> trên DB + các dòng vừa insert trong batch).
- Cột tùy chọn: sequence_number, notes — chỉ ghi nếu INFORMATION_SCHEMA có cột.

  python3 import_unit_of_measure_from_csv.py --dry-run
  python3 import_unit_of_measure_from_csv.py
  python3 import_unit_of_measure_from_csv.py --csv /path/to/Danh_sach_don_vi_tinh.csv
"""

from __future__ import annotations

import argparse
import csv
import os
import re
import sys
import uuid
from datetime import datetime, timezone
from pathlib import Path
from typing import Dict, List, Optional, Set, Tuple

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

import import_analysis_item as iai

try:
    import pyodbc
except ImportError:
    pyodbc = None  # type: ignore


CONNECTION_STRING = (
    "Driver={ODBC Driver 17 for SQL Server};"
    "Server=192.168.2.21,49938;"
    "Database=VietLabs;"
    "UID=sa;"
    "PWD=Sbt@2024;"
    "TrustServerCertificate=yes;"
    "Encrypt=yes;"
    "Login Timeout=60;"
)

_REPO_ROOT = Path(__file__).resolve().parents[3]
_DEFAULT_CSV = _REPO_ROOT / "data" / "Danh_sach_don_vi_tinh.csv"

_RE_DVT = re.compile(r"^DVT-(\d+)$", re.IGNORECASE)


def _blank_name_vi(s: Optional[str]) -> bool:
    if s is None:
        return True
    t = str(s).strip().lower()
    return t in ("", "-", "—", "n/a", "na")


def fetch_table_columns(cur: "pyodbc.Cursor", table: str, schema: str = "dbo") -> Set[str]:
    cur.execute(
        """
        SELECT LOWER(COLUMN_NAME)
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = ? AND TABLE_NAME = ?
        """,
        schema,
        table,
    )
    return {r[0] for r in cur.fetchall()}


def load_uom_normalized_keys(cur: "pyodbc.Cursor") -> Set[str]:
    cur.execute(
        """
        SELECT name_vi, name_en, unit_of_measure_code
        FROM dbo.unit_of_measure
        """
    )
    keys: Set[str] = set()
    for nvi, nen, code in cur.fetchall():
        for x in (nvi, nen, code):
            if x is None or not str(x).strip():
                continue
            k = iai.normalize_text(str(x).strip())
            if k:
                keys.add(k)
    return keys


def load_existing_codes_upper(cur: "pyodbc.Cursor") -> Set[str]:
    cur.execute(
        "SELECT LTRIM(RTRIM(ISNULL(unit_of_measure_code, N''))) FROM dbo.unit_of_measure"
    )
    return {str(r[0] or "").strip().upper() for r in cur.fetchall() if str(r[0] or "").strip()}


def max_dvt_numeric_suffix(cur: "pyodbc.Cursor") -> int:
    cur.execute(
        "SELECT unit_of_measure_code FROM dbo.unit_of_measure WHERE unit_of_measure_code LIKE N'DVT-%'"
    )
    mx = 0
    for (code,) in cur.fetchall():
        m = _RE_DVT.match((code or "").strip())
        if m:
            mx = max(mx, int(m.group(1), 10))
    return mx


def pick_column(fieldnames: List[str], *needles: str) -> str:
    """Chọn header: khớp chuỗi needle (lower) trong header đã strip lower."""
    fn = list(fieldnames or [])
    low = [(h, (h or "").strip().lower()) for h in fn if h]
    for needle in needles:
        n = needle.lower().strip()
        for orig, hl in low:
            if n in hl.replace(" ", "").replace("_", "") or n in hl:
                return orig
    raise ValueError(f"Không tìm thấy cột chứa {needles!r}. Header: {fieldnames!r}")


def load_csv_rows(csv_path: Path) -> List[Dict[str, str]]:
    rows: List[Dict[str, str]] = []
    with open(csv_path, "r", encoding="utf-8-sig", newline="") as f:
        reader = csv.DictReader(f, delimiter=";")
        if not reader.fieldnames:
            raise ValueError("CSV không có header.")
        fn = list(reader.fieldnames)
        k_en = pick_column(fn, "nameen", "name_en", "tên tiếng anh", "tên anh")
        k_vi = pick_column(fn, "namevi", "name_vi", "tiếng việt", "tên việt")
        k_notes = pick_column(fn, "notes", "ghi chú")
        k_seq = pick_column(fn, "sequencenumber", "sequence_number", "số thứ tự", "stt", "sequence")
        k_status = pick_column(fn, "status", "trạng thái")
        k_code = pick_column(fn, "unitofmeasurecode", "unit_of_measure_code", "mã")

        for r in reader:
            rows.append(
                {
                    "name_en": (r.get(k_en) or "").strip(),
                    "name_vi": (r.get(k_vi) or "").strip(),
                    "notes": (r.get(k_notes) or "").strip(),
                    "sequence": (r.get(k_seq) or "").strip(),
                    "status": (r.get(k_status) or "").strip() or "Active",
                    "code": (r.get(k_code) or "").strip(),
                }
            )
    return rows


def parse_int_optional(s: str) -> Optional[int]:
    if not s:
        return None
    try:
        return int(str(s).strip(), 10)
    except ValueError:
        return None


def csv_row_keys(nvi: str, nen: str, code: str) -> Set[str]:
    out: Set[str] = set()
    for x in (nvi, nen, code):
        if not x or not str(x).strip():
            continue
        k = iai.normalize_text(str(x).strip())
        if k:
            out.add(k)
    return out


def pick_insert_code(
    csv_code: str,
    used_codes: Set[str],
    auto_counter: List[int],
) -> Tuple[str, bool]:
    """
    Returns (code, used_csv_code).
    auto_counter is single-element list [current_max] mutated when assigning DVT-auto.
    """
    c = (csv_code or "").strip().upper()
    if c and c not in used_codes:
        used_codes.add(c)
        return c, True
    auto_counter[0] += 1
    code = f"DVT-{auto_counter[0]:03d}"
    while code.upper() in used_codes:
        auto_counter[0] += 1
        code = f"DVT-{auto_counter[0]:03d}"
    used_codes.add(code.upper())
    return code, False


def main() -> int:
    ap = argparse.ArgumentParser(description="Import unit_of_measure từ Danh_sach_don_vi_tinh.csv")
    ap.add_argument("--csv", type=Path, default=_DEFAULT_CSV)
    ap.add_argument("--conn", default="")
    ap.add_argument("--dry-run", action="store_true")
    args = ap.parse_args()

    csv_path = args.csv.resolve()
    if not csv_path.is_file():
        print(f"Không tìm thấy: {csv_path}", file=sys.stderr)
        return 2

    try:
        csv_rows = load_csv_rows(csv_path)
    except Exception as ex:
        print(f"Lỗi đọc CSV: {ex}", file=sys.stderr)
        return 2

    conn_str = (args.conn or os.environ.get("VIETLABS_SQL_ODBC", "").strip() or CONNECTION_STRING).strip()
    if not conn_str or pyodbc is None:
        print("Thiếu connection string hoặc pyodbc.", file=sys.stderr)
        return 2

    conn = pyodbc.connect(conn_str, autocommit=False)
    now = datetime.now(timezone.utc)
    try:
        cur = conn.cursor()
        db_keys = load_uom_normalized_keys(cur)
        used_codes = load_existing_codes_upper(cur)
        mx = max_dvt_numeric_suffix(cur)
        tbl = fetch_table_columns(cur, "unit_of_measure")

        seen_keys = set(db_keys)
        pending: List[Dict[str, str]] = []
        for r in csv_rows:
            nvi = r["name_vi"].strip()
            if _blank_name_vi(nvi):
                continue
            nen = r["name_en"].strip() if r["name_en"].strip() else nvi
            k_vi = iai.normalize_text(nvi)
            k_en = iai.normalize_text(nen)
            if k_vi in seen_keys or k_en in seen_keys:
                continue
            pending.append(r)
            seen_keys.add(k_vi)
            seen_keys.add(k_en)

        if not pending:
            print("Không có dòng CSV mới (đã khớp name_vi/name_en trên DB theo normalize_text).")
            conn.rollback()
            return 0

        auto_n = [mx]
        print(f"File: {csv_path}")
        print(f"Dòng CSV (hợp lệ name_vi): {sum(1 for r in csv_rows if not _blank_name_vi(r['name_vi']))}")
        print(f"Sẽ INSERT: {len(pending)} dòng (MAX DVT số hiện tại: {mx}).")

        has_seq = "sequence_number" in tbl
        has_notes = "notes" in tbl

        for r in pending:
            nvi = r["name_vi"].strip()
            nen = r["name_en"].strip() if r["name_en"].strip() else nvi
            notes = r["notes"].strip() if r["notes"].strip() else None
            status = r["status"] if r["status"] else "Active"
            seq = parse_int_optional(r["sequence"])
            csv_code = r["code"].strip()
            code, from_csv = pick_insert_code(csv_code, used_codes, auto_n)
            if csv_code and not from_csv:
                print(f"  [mã CSV {csv_code!r} bị trùng/không dùng được -> {code}]")

            uid = str(uuid.uuid4())
            print(f"  {code}  name_vi={nvi!r}  name_en={nen!r}  seq={seq}")

            if args.dry_run:
                for k in csv_row_keys(nvi, nen, code):
                    db_keys.add(k)
                continue

            if has_seq and has_notes:
                cur.execute(
                    """
                    INSERT INTO dbo.unit_of_measure
                        (unit_of_measure_id, unit_of_measure_code, name_vi, name_en,
                         status, notes, sequence_number, created_at)
                    VALUES
                        (CAST(? AS UNIQUEIDENTIFIER), ?, ?, ?, ?, ?, ?, ?)
                    """,
                    (uid, code, nvi, nen, status, notes, seq, now),
                )
            elif has_seq:
                cur.execute(
                    """
                    INSERT INTO dbo.unit_of_measure
                        (unit_of_measure_id, unit_of_measure_code, name_vi, name_en,
                         status, sequence_number, created_at)
                    VALUES
                        (CAST(? AS UNIQUEIDENTIFIER), ?, ?, ?, ?, ?, ?)
                    """,
                    (uid, code, nvi, nen, status, seq, now),
                )
            elif has_notes:
                cur.execute(
                    """
                    INSERT INTO dbo.unit_of_measure
                        (unit_of_measure_id, unit_of_measure_code, name_vi, name_en,
                         status, notes, created_at)
                    VALUES
                        (CAST(? AS UNIQUEIDENTIFIER), ?, ?, ?, ?, ?, ?)
                    """,
                    (uid, code, nvi, nen, status, notes, now),
                )
            else:
                cur.execute(
                    """
                    INSERT INTO dbo.unit_of_measure
                        (unit_of_measure_id, unit_of_measure_code, name_vi, name_en, status, created_at)
                    VALUES
                        (CAST(? AS UNIQUEIDENTIFIER), ?, ?, ?, ?, ?)
                    """,
                    (uid, code, nvi, nen, status, now),
                )

            for k in csv_row_keys(nvi, nen, code):
                db_keys.add(k)

        if args.dry_run:
            conn.rollback()
            print("\n[dry-run] Không ghi DB.")
            return 0

        conn.commit()
        print(f"\nĐã commit: INSERT {len(pending)} đơn vị tính.")
        return 0
    except Exception as ex:
        conn.rollback()
        print(f"Lỗi: {ex}", file=sys.stderr)
        return 1
    finally:
        conn.close()


if __name__ == "__main__":
    raise SystemExit(main())
