#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Cập nhật dbo.equipment_type.name_en từ data/Danh_sach_loai_thiet_bi.csv.

Chỉ đọc cột NameVi, NameEn (bỏ qua các cột khác).

Khớp dòng DB theo name_vi (so sánh không phân biệt hoa thường, đã trim).
Mỗi dòng CSV có NameVi hợp lệ: UPDATE name_en = NameEn CSV (chuỗi rỗng -> NULL).

  python3 sync_equipment_type_name_en_from_csv.py --dry-run
  python3 sync_equipment_type_name_en_from_csv.py
"""

from __future__ import annotations

import argparse
import csv
import os
import sys
from pathlib import Path
from typing import List, Optional, Tuple

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
_DEFAULT_CSV = _REPO_ROOT / "data" / "Danh_sach_loai_thiet_bi.csv"


def norm(s: Optional[str]) -> str:
    if s is None:
        return ""
    t = str(s).strip().lower()
    if t in ("-", "—", "n/a", "na"):
        return ""
    return t


def load_csv_name_vi_en(csv_path: Path) -> List[Tuple[str, str]]:
    """Chỉ NameVi, NameEn. Bỏ dòng không có NameVi (không khớp được)."""
    rows: List[Tuple[str, str]] = []
    with open(csv_path, "r", encoding="utf-8-sig", newline="") as f:
        reader = csv.DictReader(f, delimiter=";")
        if not reader.fieldnames:
            raise ValueError("CSV không có header.")
        fields = {h.strip(): h for h in reader.fieldnames if h}
        key_vi = next((fields[k] for k in fields if k.lower() == "namevi"), None)
        key_en = next((fields[k] for k in fields if k.lower() == "nameen"), None)
        if not key_vi or not key_en:
            raise ValueError(f"CSV cần cột NameVi và NameEn. Có: {reader.fieldnames!r}")

        for r in reader:
            vi = (r.get(key_vi) or "").strip()
            en = (r.get(key_en) or "").strip()
            if not norm(vi):
                continue
            rows.append((vi, en))
    return rows


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--csv", type=Path, default=_DEFAULT_CSV)
    parser.add_argument("--conn", default="")
    parser.add_argument("--dry-run", action="store_true")
    parser.add_argument("-v", "--verbose", action="store_true", help="Dry-run: in từng equipment_type_code khớp.")
    args = parser.parse_args()

    csv_path = args.csv.resolve()
    if not csv_path.is_file():
        print(f"Không tìm thấy: {csv_path}", file=sys.stderr)
        return 2

    conn_str = (args.conn or os.environ.get("VIETLABS_SQL_ODBC", "").strip() or CONNECTION_STRING).strip()
    if not conn_str or pyodbc is None:
        print("Thiếu connection string hoặc pyodbc.", file=sys.stderr)
        return 2

    try:
        rows = load_csv_name_vi_en(csv_path)
    except Exception as ex:
        print(f"Lỗi đọc CSV: {ex}", file=sys.stderr)
        return 2

    conn = pyodbc.connect(conn_str, autocommit=False)
    updated = 0
    no_match = 0
    try:
        cur = conn.cursor()
        for vi, en in rows:
            key = vi.strip()
            new_en = en.strip() if en.strip() else None

            cur.execute(
                """
                SELECT COUNT(*)
                FROM dbo.equipment_type
                WHERE LOWER(LTRIM(RTRIM(ISNULL(name_vi, N'')))) = LOWER(LTRIM(?))
                """,
                (key,),
            )
            cnt = int(cur.fetchone()[0])
            if cnt == 0:
                no_match += 1
                print(f"[không khớp name_vi] {vi!r} -> name_en CSV {en!r}")
                continue

            if args.dry_run:
                if args.verbose:
                    cur.execute(
                        """
                        SELECT equipment_type_code, name_vi, name_en
                        FROM dbo.equipment_type
                        WHERE LOWER(LTRIM(RTRIM(ISNULL(name_vi, N'')))) = LOWER(LTRIM(?))
                        """,
                        (key,),
                    )
                    for code, db_vi, db_en in cur.fetchall():
                        print(f"  [dry-run] {code!r} name_vi={db_vi!r} name_en {db_en!r} -> {new_en!r}")
                else:
                    print(f"  [dry-run] name_vi={vi!r} -> name_en={new_en!r}  ({cnt} dòng DB)")
                updated += cnt
                continue

            cur.execute(
                """
                UPDATE dbo.equipment_type
                SET name_en = ?
                WHERE LOWER(LTRIM(RTRIM(ISNULL(name_vi, N'')))) = LOWER(LTRIM(?))
                """,
                (new_en, key),
            )
            updated += cur.rowcount

        if args.dry_run:
            conn.rollback()
            print(f"\n[dry-run] Sẽ cập nhật ~{updated} dòng (theo số bản ghi khớp name_vi). Không khớp: {no_match}.")
            return 0

        conn.commit()
        print(f"File CSV: {csv_path}")
        print(f"Đã UPDATE name_en: {updated} dòng equipment_type.")
        if no_match:
            print(f"Cảnh báo: {no_match} dòng CSV không tìm thấy name_vi tương ứng trên DB.")
        return 0
    except Exception as ex:
        conn.rollback()
        print(f"Lỗi: {ex}", file=sys.stderr)
        return 1
    finally:
        conn.close()


if __name__ == "__main__":
    raise SystemExit(main())
