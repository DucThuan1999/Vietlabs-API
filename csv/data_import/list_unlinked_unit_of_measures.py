#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Liệt kê đơn vị tính (dbo.unit_of_measure) *không có liên kết* trong DB.

Theo schema EF hiện tại, chỉ bảng analysis_item tham chiếu ĐVT qua:
  - unit_of_measure_id
  - standard_quantity_unit_of_measure_id

Một dòng unit_of_measure được coi là "không liên kết" nếu không có analysis_item nào
dùng id đó ở một trong hai cột trên.

  python3 list_unlinked_unit_of_measures.py
  python3 list_unlinked_unit_of_measures.py --tsv
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

SQL = """
SELECT
    CONVERT(VARCHAR(36), u.unit_of_measure_id) AS id,
    LTRIM(RTRIM(ISNULL(u.unit_of_measure_code, N''))) AS code,
    LTRIM(RTRIM(ISNULL(u.name_vi, N''))) AS name_vi,
    LTRIM(RTRIM(ISNULL(u.name_en, N''))) AS name_en,
    LTRIM(RTRIM(ISNULL(u.status, N''))) AS status
FROM dbo.unit_of_measure u
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.analysis_item ai
    WHERE ai.unit_of_measure_id = u.unit_of_measure_id
       OR ai.standard_quantity_unit_of_measure_id = u.unit_of_measure_id
)
ORDER BY u.unit_of_measure_code, u.name_vi;
"""


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--conn", default="")
    parser.add_argument("--tsv", action="store_true", help="In ra TSV (tab)")
    args = parser.parse_args()

    conn_str = (args.conn or os.environ.get("VIETLABS_SQL_ODBC", "").strip() or CONNECTION_STRING).strip()
    if not conn_str or pyodbc is None:
        print("Thiếu connection string hoặc pyodbc.", file=sys.stderr)
        return 2

    conn = pyodbc.connect(conn_str)
    try:
        cur = conn.cursor()
        cur.execute(SQL)
        rows = cur.fetchall()
    finally:
        conn.close()

    print(f"Tổng đơn vị tính không có liên kết analysis_item: {len(rows)}")
    if not rows:
        return 0

    if args.tsv:
        print("unit_of_measure_id\tunit_of_measure_code\tname_vi\tname_en\tstatus")
        for r in rows:
            print("\t".join(str(x) if x is not None else "" for x in r))
    else:
        print()
        print("unit_of_measure_id\tcode\tname_vi\tname_en\tstatus")
        for r in rows:
            print("\t".join(str(x) if x is not None else "" for x in r))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
