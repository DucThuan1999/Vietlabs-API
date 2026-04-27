#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Gỡ toàn bộ liên kết Đơn vị tính trên chỉ tiêu (analysis_item), sau đó xóa hết dbo.unit_of_measure.

Dùng khi muốn nhập lại danh mục ĐVT và liên kết CT ↔ ĐVT từ đầu.
Không sửa quotation_item.Unit (snapshot chuỗi).

  cd Vietlabs-API/csv/data_import
  python3 reset_unit_of_measures_and_analysis_item_links.py --dry-run
  python3 reset_unit_of_measures_and_analysis_item_links.py

Kết nối: VIETLABS_SQL_ODBC hoặc --conn hoặc CONNECTION_STRING trong file.
"""

from __future__ import annotations

import argparse
import os
import sys

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


SQL_COUNTS = """
SELECT
    (SELECT COUNT(*) FROM dbo.analysis_item
     WHERE unit_of_measure_id IS NOT NULL
        OR standard_quantity_unit_of_measure_id IS NOT NULL) AS analysis_item_with_uom_fk,
    (SELECT COUNT(*) FROM dbo.unit_of_measure) AS unit_of_measure_rows;
"""

SQL_RUN = """
UPDATE dbo.analysis_item
SET
    unit_of_measure_id = NULL,
    standard_quantity_unit_of_measure_id = NULL,
    updated_at = SYSUTCDATETIME()
WHERE unit_of_measure_id IS NOT NULL
   OR standard_quantity_unit_of_measure_id IS NOT NULL;

DELETE FROM dbo.unit_of_measure;
"""


def main() -> int:
    ap = argparse.ArgumentParser(
        description="Xóa toàn bộ unit_of_measure và gỡ FK ĐVT trên analysis_item."
    )
    ap.add_argument(
        "--conn",
        default="",
        help="ODBC string (mặc định: VIETLABS_SQL_ODBC hoặc CONNECTION_STRING trong file).",
    )
    ap.add_argument("--dry-run", action="store_true", help="Chỉ in số liệu, không ghi DB.")
    args = ap.parse_args()

    conn_str = (args.conn or os.environ.get("VIETLABS_SQL_ODBC", "").strip() or CONNECTION_STRING).strip()
    if not conn_str:
        print("Thiếu connection string.", file=sys.stderr)
        return 2
    if pyodbc is None:
        print("Cần pyodbc: pip install pyodbc", file=sys.stderr)
        return 2

    conn = pyodbc.connect(conn_str, autocommit=False)
    try:
        cur = conn.cursor()
        cur.execute(SQL_COUNTS)
        row = cur.fetchone()
        if not row:
            print("Không đọc được thống kê.", file=sys.stderr)
            return 2
        n_ai, n_uom = int(row[0]), int(row[1])
        print("Thống kê hiện tại:")
        print(f"  analysis_item còn FK ĐVT (một trong hai cột): {n_ai}")
        print(f"  unit_of_measure (sẽ xóa hết):              {n_uom}")

        if n_ai == 0 and n_uom == 0:
            print("Không có gì để làm (đã trống).")
            conn.rollback()
            return 0

        if args.dry_run:
            print("\n[dry-run] Không ghi DB.")
            conn.rollback()
            return 0

        cur.execute(SQL_RUN)
        conn.commit()
        print("\nĐã commit: gỡ FK ĐVT trên analysis_item + DELETE toàn bộ unit_of_measure.")
        return 0
    except Exception as ex:
        conn.rollback()
        print(f"Lỗi: {ex}", file=sys.stderr)
        return 1
    finally:
        conn.close()


if __name__ == "__main__":
    raise SystemExit(main())
