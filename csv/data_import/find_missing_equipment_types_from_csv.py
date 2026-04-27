#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
So sánh data/Danh_sach_loai_thiet_bi.csv với bảng equipment_type trên SQL Server.

Chỉ đọc cột NameEn, NameVi (bỏ qua các cột khác trong file).

Khớp tên (không phân biệt hoa thường, đã trim; "-" / n/a coi như rỗng):
  NameVi CSV ↔ name_vi DB, NameEn CSV ↔ name_en DB, và khớp chéo Vi/En nếu cùng chuỗi.

Chế độ:
  --mode csv-not-in-db (mặc định): dòng CSV không tìm thấy equipment_type tương ứng trên DB.
  --mode db-not-in-csv: dòng equipment_type trên DB không khớp bất kỳ dòng CSV nào.

  python3 find_missing_equipment_types_from_csv.py
  python3 find_missing_equipment_types_from_csv.py --mode db-not-in-csv
  python3 find_missing_equipment_types_from_csv.py --csv /path/to/Danh_sach_loai_thiet_bi.csv
"""

from __future__ import annotations

import argparse
import csv
import os
import sys
from pathlib import Path
from typing import List, Optional, Set, Tuple

DbEquipmentRow = Tuple[str, str, str, str]  # id, code, name_vi, name_en

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


def load_csv_rows(csv_path: Path) -> List[Tuple[str, str]]:
    """Chỉ NameVi, NameEn. Bỏ qua dòng không có cả hai nội dung hữu ích."""
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
            if not norm(vi) and not norm(en):
                continue
            rows.append((vi, en))
    return rows


def load_db_name_sets(cur: "pyodbc.Cursor") -> Tuple[Set[str], Set[str]]:
    cur.execute(
        """
        SELECT name_vi, name_en
        FROM dbo.equipment_type
        """
    )
    vis: Set[str] = set()
    ens: Set[str] = set()
    for a, b in cur.fetchall():
        va = norm(str(a) if a is not None else "")
        vb = norm(str(b) if b is not None else "")
        if va:
            vis.add(va)
        if vb:
            ens.add(vb)
    return vis, ens


def load_db_equipment_rows(cur: "pyodbc.Cursor") -> List[DbEquipmentRow]:
    cur.execute(
        """
        SELECT CONVERT(VARCHAR(36), equipment_type_id),
               LTRIM(RTRIM(ISNULL(equipment_type_code, N''))),
               name_vi,
               name_en
        FROM dbo.equipment_type
        """
    )
    out: List[DbEquipmentRow] = []
    for rid, code, nvi, nen in cur.fetchall():
        out.append(
            (
                str(rid),
                str(code or ""),
                str(nvi).strip() if nvi is not None else "",
                str(nen).strip() if nen is not None else "",
            )
        )
    return out


def rows_match(vi_a: str, en_a: str, vi_b: str, en_b: str) -> bool:
    """Hai cặp (vi,en) coi là khớp nếu có ít nhất một cặp tên chuẩn hóa trùng (kể cả chéo Vi/En)."""
    a, b = norm(vi_a), norm(en_a)
    c, d = norm(vi_b), norm(en_b)
    if a and a == c:
        return True
    if b and b == d:
        return True
    if a and a == d:
        return True
    if b and b == c:
        return True
    return False


def db_row_in_csv(db_vi: str, db_en: str, csv_rows: List[Tuple[str, str]]) -> bool:
    return any(rows_match(cv, ce, db_vi, db_en) for cv, ce in csv_rows)


def in_db(nvi: str, nen: str, db_vis: Set[str], db_ens: Set[str]) -> bool:
    kv, ke = norm(nvi), norm(nen)
    if kv and kv in db_vis:
        return True
    if ke and ke in db_ens:
        return True
    # Trường hợp DB chỉ có một trong hai cột: khớp chéo tên (cùng chuỗi trên Vi/En)
    if kv and kv in db_ens:
        return True
    if ke and ke in db_vis:
        return True
    return False


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--csv",
        type=Path,
        default=_DEFAULT_CSV,
        help="Đường dẫn CSV (mặc định: data/Danh_sach_loai_thiet_bi.csv ở root repo)",
    )
    parser.add_argument("--conn", default="", help="ODBC (mặc định: VIETLABS_SQL_ODBC hoặc CONNECTION_STRING trong file)")
    parser.add_argument(
        "--mode",
        choices=("csv-not-in-db", "db-not-in-csv"),
        default="csv-not-in-db",
        help="csv-not-in-db: CSV thiếu trên DB | db-not-in-csv: DB thừa so với CSV",
    )
    args = parser.parse_args()

    csv_path = args.csv.resolve()
    if not csv_path.is_file():
        print(f"Không tìm thấy file: {csv_path}", file=sys.stderr)
        return 2

    conn_str = (args.conn or os.environ.get("VIETLABS_SQL_ODBC", "").strip() or CONNECTION_STRING).strip()
    if not conn_str:
        print("Thiếu connection string.", file=sys.stderr)
        return 2
    if pyodbc is None:
        print("Cần pyodbc.", file=sys.stderr)
        return 2

    try:
        csv_rows = load_csv_rows(csv_path)
    except Exception as ex:
        print(f"Lỗi đọc CSV: {ex}", file=sys.stderr)
        return 2

    conn = pyodbc.connect(conn_str)
    try:
        cur = conn.cursor()
        if args.mode == "csv-not-in-db":
            db_vis, db_ens = load_db_name_sets(cur)
        else:
            db_rows = load_db_equipment_rows(cur)
    finally:
        conn.close()

    print(f"File: {csv_path}")
    print(f"Chế độ: {args.mode}")

    if args.mode == "csv-not-in-db":
        missing = [(vi, en) for vi, en in csv_rows if not in_db(vi, en, db_vis, db_ens)]
        print(f"Tổng dòng CSV (có NameVi/NameEn khác rỗng/-): {len(csv_rows)}")
        print(f"Dòng CSV chưa khớp DB: {len(missing)}")
        if missing:
            print("\nNameVi\tNameEn")
            for vi, en in missing:
                print(f"{vi}\t{en}")
    else:
        extra = [r for r in db_rows if not db_row_in_csv(r[2], r[3], csv_rows)]
        print(f"Tổng dòng CSV (có NameVi/NameEn khác rỗng/-): {len(csv_rows)}")
        print(f"Dòng equipment_type trên DB không có trên CSV: {len(extra)}")
        if extra:
            print("\nequipment_type_id\tequipment_type_code\tname_vi\tname_en")
            for eid, code, nvi, nen in extra:
                print(f"{eid}\t{code}\t{nvi}\t{nen}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
