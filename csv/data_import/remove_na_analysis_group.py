#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Cập nhật DB: xóa nhóm chỉ tiêu sentinel (-, NA, N/A, …), gán analysis_group_id = NULL cho chỉ tiêu liên quan.

Giống logic migration 20260413120000_RemoveNaAnalysisGroupAndNullableAnalysisItemGroup,
mở rộng thêm các giá trị trống trên Excel V3.

Kết nối: cùng style các script trong thư mục này — sửa CONNECTION_STRING bên dưới.
Có thể ghi đè bằng biến môi trường VIETLABS_SQL_ODBC hoặc tham số --conn (không bắt buộc).

  python remove_na_analysis_group.py --dry-run
  python remove_na_analysis_group.py
"""

from __future__ import annotations

import argparse
import os
import sys
from typing import List, Tuple

try:
    import pyodbc
except ImportError:
    pyodbc = None  # type: ignore


# Cùng layout với link_analysis_item_analysis_group.py — chỉnh Server/Database/UID/PWD theo môi trường.
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

# Khớp import_analysis_item.is_blank_analysis_group_cell (sau trim + bỏ khoảng trắng, so sánh UPPER).
SENTINEL_WHERE = """
(
    LTRIM(RTRIM(ISNULL(name_vi, N''))) IN (N'-', N'--')
    OR UPPER(REPLACE(LTRIM(RTRIM(ISNULL(name_vi, N''))), N' ', N'')) IN (
        N'NA', N'N/A', N'NONE', N'NULL', N'KHÔNG', N'KHONGCO'
    )
)
"""

SQL_PREVIEW = f"""
SELECT analysis_group_id, analysis_group_code, name_vi, name_en
FROM analysis_group
WHERE {SENTINEL_WHERE};
"""

SQL_STEPS = [
    f"""
    DECLARE @ids TABLE (id UNIQUEIDENTIFIER NOT NULL);
    INSERT INTO @ids (id)
    SELECT analysis_group_id FROM analysis_group WHERE {SENTINEL_WHERE};

    UPDATE ai SET analysis_group_id = NULL, updated_at = SYSUTCDATETIME()
    FROM analysis_item ai INNER JOIN @ids i ON ai.analysis_group_id = i.id;
    """,
    f"""
    DECLARE @ids TABLE (id UNIQUEIDENTIFIER NOT NULL);
    INSERT INTO @ids (id)
    SELECT analysis_group_id FROM analysis_group WHERE {SENTINEL_WHERE};

    DELETE pag FROM package_analysis_group pag INNER JOIN @ids i ON pag.analysis_group_id = i.id;
    """,
    f"""
    DECLARE @ids TABLE (id UNIQUEIDENTIFIER NOT NULL);
    INSERT INTO @ids (id)
    SELECT analysis_group_id FROM analysis_group WHERE {SENTINEL_WHERE};

    UPDATE qi SET analysis_group_id = NULL, updated_at = SYSUTCDATETIME()
    FROM quotation_item qi INNER JOIN @ids i ON qi.analysis_group_id = i.id;
    """,
    f"""
    DECLARE @ids TABLE (id UNIQUEIDENTIFIER NOT NULL);
    INSERT INTO @ids (id)
    SELECT analysis_group_id FROM analysis_group WHERE {SENTINEL_WHERE};

    IF OBJECT_ID(N'quotation_analysis_group', N'U') IS NOT NULL
        DELETE qag FROM quotation_analysis_group qag INNER JOIN @ids i ON qag.analysis_group_id = i.id;
    """,
    f"""
    DECLARE @ids TABLE (id UNIQUEIDENTIFIER NOT NULL);
    INSERT INTO @ids (id)
    SELECT analysis_group_id FROM analysis_group WHERE {SENTINEL_WHERE};

    DELETE ag FROM analysis_group ag INNER JOIN @ids i ON ag.analysis_group_id = i.id;
    """,
]


def ensure_nullable_analysis_group_id(cur: "pyodbc.Cursor") -> None:
    cur.execute(
        """
        SELECT c.is_nullable
        FROM sys.columns c
        INNER JOIN sys.tables t ON c.object_id = t.object_id
        INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
        WHERE s.name = N'dbo' AND t.name = N'analysis_item' AND c.name = N'analysis_group_id';
        """
    )
    row = cur.fetchone()
    if not row:
        raise RuntimeError("Không tìm thấy cột dbo.analysis_item.analysis_group_id.")
    if row[0] != 1:
        print(
            "Cột analysis_item.analysis_group_id đang NOT NULL — đang ALTER sang NULL...",
            file=sys.stderr,
        )
        cur.execute("ALTER TABLE dbo.analysis_item ALTER COLUMN analysis_group_id uniqueidentifier NULL;")
        cur.connection.commit()


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Xóa nhóm chỉ tiêu sentinel (-, NA, N/A, …) và gỡ FK chỉ tiêu / báo giá / gói."
    )
    parser.add_argument(
        "--conn",
        default="",
        help="ODBC connection string (mặc định: VIETLABS_SQL_ODBC nếu có, không thì CONNECTION_STRING trong file)",
    )
    parser.add_argument("--dry-run", action="store_true", help="Chỉ in nhóm sẽ bị xóa, không ghi DB.")
    args = parser.parse_args()

    conn_str = (args.conn or os.environ.get("VIETLABS_SQL_ODBC", "").strip() or CONNECTION_STRING).strip()
    if not conn_str:
        print("Thiếu connection string.", file=sys.stderr)
        return 2
    if pyodbc is None:
        print("Cần cài pyodbc: pip install pyodbc", file=sys.stderr)
        return 2

    conn = pyodbc.connect(conn_str, autocommit=False)
    try:
        cur = conn.cursor()
        cur.execute(SQL_PREVIEW)
        rows: List[Tuple] = cur.fetchall()
        if not rows:
            print("Không có analysis_group sentinel nào (-, NA, N/A, …).")
            return 0
        print("Nhóm sẽ xóa (analysis_group_id, code, name_vi, name_en):")
        for r in rows:
            print(" ", r)
        if args.dry_run:
            print("[dry-run] Không ghi thay đổi.")
            conn.rollback()
            return 0

        ensure_nullable_analysis_group_id(cur)
        for step_sql in SQL_STEPS:
            cur.execute(step_sql)
        conn.commit()
        print("Đã commit: gỡ liên kết + xóa nhóm sentinel.")
        return 0
    except Exception as ex:
        conn.rollback()
        print(f"Lỗi: {ex}", file=sys.stderr)
        return 1
    finally:
        conn.close()


if __name__ == "__main__":
    raise SystemExit(main())
