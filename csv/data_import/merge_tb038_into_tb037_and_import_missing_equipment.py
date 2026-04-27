#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
1) Gộp trùng lặp TB-038 -> TB-037:
   - UPDATE analysis_item SET equipment_type_id = (id TB-037) WHERE equipment_type_id = (id TB-038)
   - DELETE equipment_type WHERE equipment_type_code = N'TB-038'

2) Chuẩn hóa TB-037 theo danh mục CSV (Calculation / Tính toán).

3) INSERT 3 loại thiết bị còn thiếu (Elisa, Kháng vi khuẩn, Phát hiện) với mã TB-xxx
   tiếp theo sau số TB lớn nhất trên DB (thường TB-038, TB-039, TB-040 sau khi xóa TB-038).

  python3 merge_tb038_into_tb037_and_import_missing_equipment.py --dry-run
  python3 merge_tb038_into_tb037_and_import_missing_equipment.py
"""

from __future__ import annotations

import argparse
import os
import re
import sys
import uuid
from datetime import datetime, timezone
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

_RE_TB = re.compile(r"^TB-(\d+)$", re.IGNORECASE)

# Sau gộp TB-037 đúng theo data/Danh_sach_loai_thiet_bi.csv
TB037_NAME_EN = "Calculation"
TB037_NAME_VI = "Tính toán"

NEW_ROWS: List[Tuple[str, str]] = [
    ("Elisa", "Elisa"),
    ("Kháng vi khuẩn", "Antibacterial"),
    ("Phát hiện", "Detection"),
]


def _conn_str(args: argparse.Namespace) -> str:
    return (args.conn or os.environ.get("VIETLABS_SQL_ODBC", "").strip() or CONNECTION_STRING).strip()


def resolve_id_by_code(cur: "pyodbc.Cursor", code: str) -> Optional[str]:
    cur.execute(
        "SELECT CONVERT(VARCHAR(36), equipment_type_id) FROM dbo.equipment_type WHERE equipment_type_code = ?",
        (code,),
    )
    row = cur.fetchone()
    return str(row[0]) if row else None


def max_tb_suffix_excluding(cur: "pyodbc.Cursor", exclude_equipment_type_id: Optional[str]) -> int:
    """Số TB-xxx lớn nhất, bỏ qua dòng sẽ xóa (TB-038) để mã mới tiếp nối đúng (TB-038,039,040)."""
    if exclude_equipment_type_id:
        cur.execute(
            """
            SELECT equipment_type_code
            FROM dbo.equipment_type
            WHERE equipment_type_code LIKE N'TB-%'
              AND equipment_type_id <> CAST(? AS UNIQUEIDENTIFIER)
            """,
            (exclude_equipment_type_id,),
        )
    else:
        cur.execute("SELECT equipment_type_code FROM dbo.equipment_type WHERE equipment_type_code LIKE N'TB-%'")
    mx = 0
    for (code,) in cur.fetchall():
        m = _RE_TB.match((code or "").strip())
        if m:
            mx = max(mx, int(m.group(1), 10))
    return mx


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--conn", default="")
    parser.add_argument("--dry-run", action="store_true")
    args = parser.parse_args()

    cs = _conn_str(args)
    if not cs or pyodbc is None:
        print("Thiếu connection string hoặc pyodbc.", file=sys.stderr)
        return 2

    conn = pyodbc.connect(cs, autocommit=False)
    try:
        cur = conn.cursor()
        id37 = resolve_id_by_code(cur, "TB-037")
        id38 = resolve_id_by_code(cur, "TB-038")
        if not id37 or not id38:
            print(f"Thiếu TB-037 hoặc TB-038 trên DB (TB-037={id37!r}, TB-038={id38!r}).", file=sys.stderr)
            return 1
        if id37.casefold() == id38.casefold():
            print("TB-037 và TB-038 trùng cùng một id — không làm gì.", file=sys.stderr)
            return 1

        cur.execute(
            "SELECT COUNT(*) FROM dbo.analysis_item WHERE equipment_type_id = ?",
            (id38,),
        )
        n_ai = int(cur.fetchone()[0])

        mx_keep = max_tb_suffix_excluding(cur, id38)
        start_num = mx_keep + 1
        codes = [f"TB-{start_num + i:03d}" for i in range(len(NEW_ROWS))]

        print("Kế hoạch:")
        print(f"  analysis_item đang dùng TB-038: {n_ai} dòng -> chuyển sang TB-037 ({id37})")
        print(f"  DELETE equipment_type TB-038 ({id38})")
        print(f"  UPDATE TB-037 name_vi={TB037_NAME_VI!r}, name_en={TB037_NAME_EN!r}")
        for (vi, en), c in zip(NEW_ROWS, codes):
            print(f"  INSERT {c}: name_vi={vi!r}, name_en={en!r}")

        if args.dry_run:
            conn.rollback()
            print("\n[dry-run] Không ghi DB.")
            return 0

        now = datetime.now(timezone.utc)
        cur.execute(
            "UPDATE dbo.analysis_item SET equipment_type_id = CAST(? AS UNIQUEIDENTIFIER), updated_at = ? WHERE equipment_type_id = CAST(? AS UNIQUEIDENTIFIER)",
            (id37, now, id38),
        )
        cur.execute("DELETE FROM dbo.equipment_type WHERE equipment_type_id = CAST(? AS UNIQUEIDENTIFIER)", (id38,))
        if cur.rowcount != 1:
            raise RuntimeError(f"DELETE TB-038: rowcount={cur.rowcount} (kỳ vọng 1)")

        cur.execute(
            """
            UPDATE dbo.equipment_type
            SET name_vi = ?, name_en = ?
            WHERE equipment_type_id = CAST(? AS UNIQUEIDENTIFIER)
            """,
            (TB037_NAME_VI, TB037_NAME_EN, id37),
        )

        for (nvi, nen), code in zip(NEW_ROWS, codes):
            new_id = str(uuid.uuid4())
            cur.execute(
                """
                INSERT INTO dbo.equipment_type
                    (equipment_type_id, equipment_type_code, name_vi, name_en, status)
                VALUES
                    (CAST(? AS UNIQUEIDENTIFIER), ?, ?, ?, N'Active')
                """,
                (new_id, code, nvi, nen),
            )

        conn.commit()
        print(f"\nĐã commit: gộp TB-038->{id37[:8]}…, cập nhật TB-037, thêm {len(NEW_ROWS)} dòng ({', '.join(codes)}).")
        return 0
    except Exception as ex:
        conn.rollback()
        print(f"Lỗi: {ex}", file=sys.stderr)
        return 1
    finally:
        conn.close()


if __name__ == "__main__":
    raise SystemExit(main())
