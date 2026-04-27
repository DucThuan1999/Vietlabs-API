#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Đổi mã loại thiết bị trên SQL Server: TP-xxx -> TB-xxx (cùng số thứ tự).

- Chỉ xử lý mã khớp dạng ^TP-<số>$ (không đổi chuỗi chứa TP- ở giữa).
- Hai bước: gán mã tạm duy nhất rồi gán TB-… để tránh trùng mã trong lúc UPDATE.
- Nếu đã tồn tại một dòng khác có TB-<số> giống: mặc định dừng (dry-run báo; chạy thật không ghi).
  Dùng --ignore-collisions để bỏ qua các dòng xung đột (giữ nguyên mã TP-).

Kết nối: sửa CONNECTION_STRING; hoặc VIETLABS_SQL_ODBC / --conn.

  python3 rename_equipment_type_tp_to_tb.py --dry-run
  python3 rename_equipment_type_tp_to_tb.py
  python3 rename_equipment_type_tp_to_tb.py --ignore-collisions
"""

from __future__ import annotations

import argparse
import os
import re
import sys
from datetime import datetime, timezone
from collections import defaultdict
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

_RE_TP = re.compile(r"^TP-(\d+)$", re.IGNORECASE)


def _conn_str(args: argparse.Namespace) -> str:
    return (args.conn or os.environ.get("VIETLABS_SQL_ODBC", "").strip() or CONNECTION_STRING).strip()


def _fetch_all(cur: "pyodbc.Cursor") -> List[Tuple[str, str, Optional[str]]]:
    cur.execute(
        """
        SELECT CONVERT(VARCHAR(36), equipment_type_id) AS id,
               LTRIM(RTRIM(ISNULL(equipment_type_code, N''))) AS code,
               name_vi
        FROM dbo.equipment_type
        """
    )
    return [(str(r[0]), str(r[1] or ""), r[2]) for r in cur.fetchall()]


def _target_tb_code(tp_code: str) -> Optional[str]:
    m = _RE_TP.match(tp_code.strip())
    if not m:
        return None
    n = int(m.group(1), 10)
    return f"TB-{n:03d}"


def plan_updates(
    rows: List[Tuple[str, str, Optional[str]]],
) -> Tuple[List[Tuple[str, str, str]], List[str]]:
    """
    Returns (planned list of (id, old_code, new_code)), conflict_messages.
    """
    code_to_id: Dict[str, str] = {}
    for eid, code, _ in rows:
        if not code:
            continue
        k = code.casefold()
        code_to_id[k] = eid

    tp_dup: Dict[str, List[str]] = defaultdict(list)
    for eid, code, _ in rows:
        if code and _RE_TP.match(code.strip()):
            tp_dup[code.strip().casefold()].append(eid)

    planned: List[Tuple[str, str, str]] = []
    conflicts: List[str] = []

    for tp_key, ids in tp_dup.items():
        if len(ids) > 1:
            conflicts.append(f"Trùng mã {tp_key!r} trên nhiều equipment_type_id: {ids}")

    skip_ids = {i for ids in tp_dup.values() for i in ids if len(ids) > 1}

    for eid, code, name_vi in rows:
        if eid in skip_ids:
            continue
        new_c = _target_tb_code(code)
        if not new_c:
            continue
        other = code_to_id.get(new_c.casefold())
        if other and other.casefold() != eid.casefold():
            conflicts.append(
                f"equipment_type_id={eid} code={code!r} -> {new_c!r} "
                f"nhưng id={other} đã dùng mã {new_c!r} (name_vi gợi ý: {name_vi!r})"
            )
            continue
        planned.append((eid, code, new_c))

    return planned, conflicts


def run_updates(cur: "pyodbc.Cursor", planned: List[Tuple[str, str, str]], updated_at) -> None:
    """Hai pha: mã tạm theo PK, rồi gán TB-xxx."""
    for eid, _old, new_c in planned:
        tmp = f"__MIG_TP__{eid.replace('-', '')}"
        cur.execute(
            """
            UPDATE dbo.equipment_type
            SET equipment_type_code = ?,
                updated_at = ?
            WHERE equipment_type_id = ? AND equipment_type_code = ?
            """,
            (tmp, updated_at, eid, _old),
        )
        if cur.rowcount != 1:
            raise RuntimeError(f"Phase1: kỳ vọng 1 dòng, rowcount={cur.rowcount} id={eid} old={_old!r}")

    for eid, _old, new_c in planned:
        tmp = f"__MIG_TP__{eid.replace('-', '')}"
        cur.execute(
            """
            UPDATE dbo.equipment_type
            SET equipment_type_code = ?,
                updated_at = ?
            WHERE equipment_type_id = ? AND equipment_type_code = ?
            """,
            (new_c, updated_at, eid, tmp),
        )
        if cur.rowcount != 1:
            raise RuntimeError(f"Phase2: kỳ vọng 1 dòng, rowcount={cur.rowcount} id={eid} -> {new_c!r}")


def main() -> int:
    parser = argparse.ArgumentParser(description="Đổi equipment_type_code TP-xxx sang TB-xxx.")
    parser.add_argument("--conn", default="", help="ODBC connection string")
    parser.add_argument("--dry-run", action="store_true", help="Chỉ in kế hoạch, không UPDATE.")
    parser.add_argument(
        "--ignore-collisions",
        action="store_true",
        help="Bỏ qua các dòng bị trùng mã TB đã tồn tại ở equipment_type khác.",
    )
    args = parser.parse_args()

    cs = _conn_str(args)
    if not cs:
        print("Thiếu connection string.", file=sys.stderr)
        return 2
    if pyodbc is None:
        print("Cần pyodbc.", file=sys.stderr)
        return 2

    conn = pyodbc.connect(cs, autocommit=False)
    try:
        cur = conn.cursor()
        rows = _fetch_all(cur)
        planned_all, conflicts = plan_updates(rows)

        if conflicts:
            print("Cảnh báo / xung đột:", file=sys.stderr)
            for c in conflicts:
                print(" ", c, file=sys.stderr)
            if not args.ignore_collisions and not args.dry_run:
                print(
                    "\nCó xung đột — không ghi DB. Sửa dữ liệu hoặc chạy với --ignore-collisions "
                    "(chỉ đổi các dòng không xung đột; dòng trùng TB hoặc trùng mã TP bị bỏ qua).",
                    file=sys.stderr,
                )
                conn.rollback()
                return 1
            if args.ignore_collisions:
                print(
                    "\n[--ignore-collisions] Vẫn chạy UPDATE cho các dòng không nằm trong nhóm xung đột ở trên.",
                    file=sys.stderr,
                )

        planned = planned_all

        if not planned:
            print("Không có dòng nào mã dạng TP-<số> cần đổi (hoặc toàn bộ bị xung đột).")
            conn.rollback()
            return 0

        print("Sẽ đổi mã (equipment_type_id, cũ -> mới, name_vi):")
        id_set = {p[0] for p in planned}
        name_by_id = {eid: nv for eid, _, nv in rows}
        for eid, old_c, new_c in planned:
            print(f"  {eid}  {old_c!r} -> {new_c!r}  ({name_by_id.get(eid)!r})")

        if args.dry_run:
            print(f"\n[dry-run] {len(planned)} dòng — không ghi DB.")
            conn.rollback()
            return 0

        run_updates(cur, planned, datetime.now(timezone.utc))
        conn.commit()
        print(f"\nĐã commit: cập nhật {len(planned)} dòng equipment_type.")
        return 0
    except Exception as ex:
        conn.rollback()
        print(f"Lỗi: {ex}", file=sys.stderr)
        return 1
    finally:
        conn.close()


if __name__ == "__main__":
    raise SystemExit(main())
