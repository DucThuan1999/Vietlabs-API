#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
So sánh name_vi: data/Danh_sach_tieu_chuan_qui_chuan.csv ↔ bảng dbo.standard (SQL Server).

Chỉ dùng cột "Tên tiếng việt" trong CSV (các cột khác không đọc cho so khớp).

Chuẩn hóa so sánh: trim + lower (Unicode). Chuỗi rỗng, "-", "n/a" bỏ qua.

In hai nhóm:
  - Có trên CSV, không có name_vi tương ứng trên DB
  - Có trên DB, không có trên CSV

  python3 compare_standard_name_vi_csv_db.py
  python3 compare_standard_name_vi_csv_db.py --csv /path/to/file.csv
"""

from __future__ import annotations

import argparse
import csv
import os
import sys
from pathlib import Path
from typing import Dict, List, Optional, Set, Tuple

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
_DEFAULT_CSV = _REPO_ROOT / "data" / "Danh_sach_tieu_chuan_qui_chuan.csv"


def norm(s: Optional[str]) -> str:
    if s is None:
        return ""
    t = str(s).strip().lower()
    if t in ("-", "—", "n/a", "na", ""):
        return ""
    return t


def pick_name_vi_column(fieldnames: List[str]) -> str:
    for h in fieldnames or []:
        if not h:
            continue
        h0 = h.strip().lower()
        if "tiếng" in h0 and "việt" in h0:
            return h
        if h0.replace(" ", "") in ("namevi", "name_vi"):
            return h
    raise ValueError(f"Không tìm thấy cột tên tiếng Việt. Header: {fieldnames!r}")


def load_csv_name_vi(path: Path) -> Tuple[Set[str], Dict[str, str]]:
    """(tập norm, map norm -> một bản ghi hiển thị)."""
    norms: Set[str] = set()
    display: Dict[str, str] = {}
    with open(path, "r", encoding="utf-8-sig", newline="") as f:
        reader = csv.DictReader(f, delimiter=";")
        if not reader.fieldnames:
            raise ValueError("CSV không có header.")
        key_vi = pick_name_vi_column(list(reader.fieldnames))
        for row in reader:
            raw = (row.get(key_vi) or "").strip()
            k = norm(raw)
            if not k:
                continue
            norms.add(k)
            display.setdefault(k, raw)
    return norms, display


def load_db_name_vi(cur: "pyodbc.Cursor") -> Tuple[Set[str], Dict[str, List[Tuple[str, str]]]]:
    """(tập norm, map norm -> [(standard_code, name_vi gốc), ...])"""
    cur.execute(
        """
        SELECT LTRIM(RTRIM(ISNULL(standard_code, N''))), LTRIM(RTRIM(ISNULL(name_vi, N'')))
        FROM dbo.standard
        """
    )
    norms: Set[str] = set()
    by_norm: Dict[str, List[Tuple[str, str]]] = {}
    for code, nvi in cur.fetchall():
        c, v = str(code or ""), str(nvi or "")
        k = norm(v)
        if not k:
            continue
        norms.add(k)
        by_norm.setdefault(k, []).append((c, v))
    return norms, by_norm


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--csv", type=Path, default=_DEFAULT_CSV)
    parser.add_argument("--conn", default="")
    args = parser.parse_args()

    csv_path = args.csv.resolve()
    if not csv_path.is_file():
        print(f"Không tìm thấy: {csv_path}", file=sys.stderr)
        return 2

    try:
        csv_norms, csv_disp = load_csv_name_vi(csv_path)
    except Exception as ex:
        print(f"Lỗi đọc CSV: {ex}", file=sys.stderr)
        return 2

    conn_str = (args.conn or os.environ.get("VIETLABS_SQL_ODBC", "").strip() or CONNECTION_STRING).strip()
    if not conn_str or pyodbc is None:
        print("Thiếu connection string hoặc pyodbc.", file=sys.stderr)
        return 2

    conn = pyodbc.connect(conn_str)
    try:
        cur = conn.cursor()
        db_norms, db_by_norm = load_db_name_vi(cur)
    finally:
        conn.close()

    only_csv = sorted(csv_norms - db_norms)
    only_db = sorted(db_norms - csv_norms)

    print(f"File CSV: {csv_path}")
    print(f"Số name_vi (chuẩn hóa) duy nhất trên CSV: {len(csv_norms)}")
    print(f"Số name_vi (chuẩn hóa) duy nhất trên DB (có nội dung): {len(db_norms)}")
    print()

    print(f"=== Có trên CSV, không có trên DB ({len(only_csv)}) ===")
    if only_csv:
        for k in only_csv:
            print(csv_disp.get(k, k))
    else:
        print("(không có)")

    print()
    print(f"=== Có trên DB, không có trên CSV ({len(only_db)}) ===")
    if only_db:
        for k in only_db:
            rows = db_by_norm.get(k, [])
            for code, nvi in rows:
                pref = f"[{code}] " if code else ""
                print(f"{pref}{nvi}")
    else:
        print("(không có)")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
