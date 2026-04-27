#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Cập nhật dbo.standard.name_en từ data/Danh_sach_tieu_chuan_qui_chuan.csv.

Chỉ đọc cột "Tên tiếng việt" (khớp name_vi) và "Tên tiếng anh" (ghi name_en).

Khớp: LOWER(LTRIM(RTRIM(name_vi))) = LOWER(LTRIM(Tên tiếng việt CSV)).
name_en rỗng sau trim -> NULL. Cập nhật updated_at nếu cột tồn tại (bỏ qua lỗi nếu không có).

  python3 sync_standard_name_en_from_csv.py --dry-run
  python3 sync_standard_name_en_from_csv.py
"""

from __future__ import annotations

import argparse
import csv
import os
import sys
from datetime import datetime, timezone
from pathlib import Path
from typing import Dict, List, Optional, Tuple

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


def _pick(fieldnames: List[str], *needles: str) -> str:
    fn = list(fieldnames or [])
    low = [(h, (h or "").strip().lower()) for h in fn if h]
    for needle in needles:
        n = needle.lower()
        for orig, hl in low:
            if n in hl:
                return orig
    raise ValueError(f"Không tìm thấy cột chứa {needles!r}. Header: {fieldnames!r}")


def load_csv_vi_to_en(csv_path: Path) -> List[Tuple[str, Optional[str]]]:
    """
    Danh sách (name_vi_csv_trimmed, name_en hoặc None).
    Trùng name_vi (sau norm): giữ bản ghi cuối trong file.
    """
    merged: Dict[str, Tuple[str, Optional[str]]] = {}
    with open(csv_path, "r", encoding="utf-8-sig", newline="") as f:
        reader = csv.DictReader(f, delimiter=";")
        if not reader.fieldnames:
            raise ValueError("CSV không có header.")
        fn = list(reader.fieldnames)
        k_en = _pick(fn, "tiếng anh", "name_en", "nameen")
        k_vi = _pick(fn, "tiếng việt", "name_vi", "namevi")
        for r in reader:
            vi = (r.get(k_vi) or "").strip()
            en_raw = (r.get(k_en) or "").strip()
            k = norm(vi)
            if not k:
                continue
            new_en = en_raw if en_raw else None
            merged[k] = (vi, new_en)
    return list(merged.values())


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--csv", type=Path, default=_DEFAULT_CSV)
    parser.add_argument("--conn", default="")
    parser.add_argument("--dry-run", action="store_true")
    parser.add_argument("-v", "--verbose", action="store_true")
    args = parser.parse_args()

    csv_path = args.csv.resolve()
    if not csv_path.is_file():
        print(f"Không tìm thấy: {csv_path}", file=sys.stderr)
        return 2

    try:
        pairs = load_csv_vi_to_en(csv_path)
    except Exception as ex:
        print(f"Lỗi đọc CSV: {ex}", file=sys.stderr)
        return 2

    conn_str = (args.conn or os.environ.get("VIETLABS_SQL_ODBC", "").strip() or CONNECTION_STRING).strip()
    if not conn_str or pyodbc is None:
        print("Thiếu connection string hoặc pyodbc.", file=sys.stderr)
        return 2

    conn = pyodbc.connect(conn_str, autocommit=False)
    now = datetime.now(timezone.utc)
    updated = 0
    no_match = 0
    try:
        cur = conn.cursor()
        has_updated_at = False
        try:
            cur.execute(
                """
                SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'standard' AND COLUMN_NAME = N'updated_at'
                """
            )
            has_updated_at = cur.fetchone() is not None
        except Exception:
            has_updated_at = False

        for vi_key, new_en in pairs:
            cur.execute(
                """
                SELECT COUNT(*)
                FROM dbo.standard
                WHERE LOWER(LTRIM(RTRIM(ISNULL(name_vi, N'')))) = LOWER(LTRIM(?))
                """,
                (vi_key,),
            )
            cnt = int(cur.fetchone()[0])
            if cnt == 0:
                no_match += 1
                print(f"[không khớp name_vi] {vi_key!r} -> name_en CSV {new_en!r}")
                continue

            if args.dry_run:
                if args.verbose:
                    cur.execute(
                        """
                        SELECT standard_code, name_vi, name_en
                        FROM dbo.standard
                        WHERE LOWER(LTRIM(RTRIM(ISNULL(name_vi, N'')))) = LOWER(LTRIM(?))
                        """,
                        (vi_key,),
                    )
                    for code, db_vi, db_en in cur.fetchall():
                        print(f"  [dry-run] {code!r} name_vi={db_vi!r} name_en {db_en!r} -> {new_en!r}")
                else:
                    print(f"  [dry-run] name_vi={vi_key!r} -> name_en={new_en!r}  ({cnt} dòng DB)")
                updated += cnt
                continue

            if has_updated_at:
                cur.execute(
                    """
                    UPDATE dbo.standard
                    SET name_en = ?, updated_at = ?
                    WHERE LOWER(LTRIM(RTRIM(ISNULL(name_vi, N'')))) = LOWER(LTRIM(?))
                    """,
                    (new_en, now, vi_key),
                )
            else:
                cur.execute(
                    """
                    UPDATE dbo.standard
                    SET name_en = ?
                    WHERE LOWER(LTRIM(RTRIM(ISNULL(name_vi, N'')))) = LOWER(LTRIM(?))
                    """,
                    (new_en, vi_key),
                )
            updated += cur.rowcount

        if args.dry_run:
            conn.rollback()
            print(f"\n[dry-run] Sẽ cập nhật ~{updated} dòng standard. Không khớp name_vi: {no_match}.")
            return 0

        conn.commit()
        print(f"File CSV: {csv_path}")
        print(f"Đã UPDATE name_en: {updated} dòng standard.")
        if no_match:
            print(f"Cảnh báo: {no_match} dòng CSV không có name_vi tương ứng trên DB.")
        return 0
    except Exception as ex:
        conn.rollback()
        print(f"Lỗi: {ex}", file=sys.stderr)
        return 1
    finally:
        conn.close()


if __name__ == "__main__":
    raise SystemExit(main())
