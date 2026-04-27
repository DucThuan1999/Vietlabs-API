#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
INSERT các dòng *mới* từ data/Danh_sach_tieu_chuan_qui_chuan.csv vào dbo.standard.

- Chỉ INSERT: bỏ qua dòng đã tồn tại (khớp name_vi sau trim + lower).
- Mã tiêu chuẩn: luôn gán TC-xxx tiếp theo sau MAX(số) của mọi mã dạng TC-<số> trên DB
  (không dùng cột "Mã tiêu chuẩn" trong CSV cho dòng insert).
- Đọc từ CSV: Tên tiếng anh, Tên tiếng việt, Notes, Số thứ tự, Status (cột Mã chỉ tham chiếu file, không ghi theo CSV).

  python3 import_new_standards_from_csv.py --dry-run
  python3 import_new_standards_from_csv.py
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

_RE_TC = re.compile(r"^TC-(\d+)$", re.IGNORECASE)


def norm(s: Optional[str]) -> str:
    if s is None:
        return ""
    t = str(s).strip().lower()
    if t in ("-", "—", "n/a", "na", ""):
        return ""
    return t


def _pick(fieldnames: List[str], *needles: str) -> str:
    """Chọn cột: chứa chuỗi needle (lower) trong header đã strip lower."""
    fn = list(fieldnames or [])
    low = [(h, (h or "").strip().lower()) for h in fn if h]
    for needle in needles:
        n = needle.lower()
        for orig, hl in low:
            if n in hl:
                return orig
    raise ValueError(f"Không tìm thấy cột chứa {needles!r}. Header: {fieldnames!r}")


def load_csv_rows(csv_path: Path) -> List[Dict[str, str]]:
    rows: List[Dict[str, str]] = []
    with open(csv_path, "r", encoding="utf-8-sig", newline="") as f:
        reader = csv.DictReader(f, delimiter=";")
        if not reader.fieldnames:
            raise ValueError("CSV không có header.")
        fn = list(reader.fieldnames)
        k_en = _pick(fn, "tiếng anh", "name_en", "nameen")
        k_vi = _pick(fn, "tiếng việt", "name_vi", "namevi")
        k_notes = _pick(fn, "notes", "ghi chú")
        k_stt = _pick(fn, "số thứ tự", "stt", "sequence")
        k_status = _pick(fn, "status", "trạng thái")

        for r in reader:
            rows.append(
                {
                    "name_en": (r.get(k_en) or "").strip(),
                    "name_vi": (r.get(k_vi) or "").strip(),
                    "notes": (r.get(k_notes) or "").strip(),
                    "sequence": (r.get(k_stt) or "").strip(),
                    "status": (r.get(k_status) or "").strip() or "Active",
                }
            )
    return rows


def load_db_name_vi_norms(cur: "pyodbc.Cursor") -> Set[str]:
    cur.execute(
        "SELECT LTRIM(RTRIM(ISNULL(name_vi, N''))) FROM dbo.standard"
    )
    out: Set[str] = set()
    for (nvi,) in cur.fetchall():
        k = norm(str(nvi))
        if k:
            out.add(k)
    return out


def max_tc_suffix(cur: "pyodbc.Cursor") -> int:
    cur.execute(
        "SELECT standard_code FROM dbo.standard WHERE standard_code LIKE N'TC-%'"
    )
    mx = 0
    for (code,) in cur.fetchall():
        m = _RE_TC.match((code or "").strip())
        if m:
            mx = max(mx, int(m.group(1), 10))
    return mx


def parse_int_optional(s: str) -> Optional[int]:
    if not s:
        return None
    try:
        return int(str(s).strip())
    except ValueError:
        return None


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--csv", type=Path, default=_DEFAULT_CSV)
    parser.add_argument("--conn", default="")
    parser.add_argument("--dry-run", action="store_true")
    args = parser.parse_args()

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
        db_vi = load_db_name_vi_norms(cur)
        mx = max_tc_suffix(cur)

        to_insert: List[Dict[str, str]] = []
        for r in csv_rows:
            k = norm(r["name_vi"])
            if not k:
                continue
            if k in db_vi:
                continue
            to_insert.append(r)

        if not to_insert:
            print("Không có dòng CSV mới (name_vi chưa có trên DB).")
            conn.rollback()
            return 0

        start = mx + 1
        print(f"MAX mã TC-* hiện tại: TC-{mx:03d}  ->  dòng mới bắt đầu từ TC-{start:03d}")
        print(f"Sẽ INSERT: {len(to_insert)} dòng.")

        for i, r in enumerate(to_insert):
            code = f"TC-{start + i:03d}"
            seq = parse_int_optional(r["sequence"])
            nvi = r["name_vi"].strip()
            nen = r["name_en"].strip() if r["name_en"].strip() else None
            notes = r["notes"].strip() if r["notes"].strip() else None
            status = r["status"] if r["status"] else "Active"
            sid = str(uuid.uuid4())

            print(f"  {code}  name_vi={nvi!r}  name_en={nen!r}  seq={seq}")

            if args.dry_run:
                continue

            cur.execute(
                """
                INSERT INTO dbo.standard
                    (standard_id, sequence_number, standard_code, name_vi, name_en, status, notes, created_at)
                VALUES
                    (CAST(? AS UNIQUEIDENTIFIER), ?, ?, ?, ?, ?, ?, ?)
                """,
                (sid, seq, code, nvi, nen, status, notes, now),
            )

        if args.dry_run:
            conn.rollback()
            print("\n[dry-run] Không ghi DB.")
            return 0

        conn.commit()
        print(f"\nĐã commit: INSERT {len(to_insert)} dòng standard.")
        return 0
    except Exception as ex:
        conn.rollback()
        print(f"Lỗi: {ex}", file=sys.stderr)
        return 1
    finally:
        conn.close()


if __name__ == "__main__":
    raise SystemExit(main())
