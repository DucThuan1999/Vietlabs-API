#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Gán analysis_item.unit_of_measure_id và standard_quantity_unit_of_measure_id
theo data/Capability.xlsx (sheet Vietlabs — cột ĐVT / Đơn vị tính và ĐVT khối lượng).

Resolve chuỗi Excel → id bằng cùng logic import Capability (normalize_text + bảng unit_of_measure).

Chế độ:
  --mode only-null (mặc định): chỉ UPDATE khi FK hiện tại đang NULL (hoặc GUID rỗng)
                               và Excel có chuỗi ĐVT resolve được.
  --mode sync:                 nếu Excel có chuỗi và resolve được thì ghi đè FK;
                               nếu ô Excel trống thì không đổi cột tương ứng trên DB.

  python3 link_analysis_item_unit_of_measure_from_capability_xlsx.py --dry-run
  python3 link_analysis_item_unit_of_measure_from_capability_xlsx.py
  python3 link_analysis_item_unit_of_measure_from_capability_xlsx.py --mode sync --dry-run

Kết nối: VIETLABS_SQL_ODBC hoặc --conn hoặc CONNECTION_STRING trong file.
"""

from __future__ import annotations

import argparse
import os
import sys
from datetime import datetime, timezone
from pathlib import Path
from typing import Dict, List, Optional, Tuple

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

import import_analysis_item as iai
import import_analysis_items_capability_vietlabs_xlsx as cap_imp

import import_uom_from_capability_xlsx_and_audit as uom_cap

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
_DEFAULT_XLSX = _REPO_ROOT / "data" / "Capability.xlsx"

_ZERO = "00000000-0000-0000-0000-000000000000"


def _conn_str(args_conn: str) -> str:
    return (args_conn or os.environ.get("VIETLABS_SQL_ODBC", "").strip() or CONNECTION_STRING).strip()


def _guid_str(v: object) -> str:
    if v is None:
        return ""
    return str(v).strip()


def _is_empty_fk(v: object) -> bool:
    s = _guid_str(v).lower()
    return not s or s == _ZERO.lower()


def fetch_analysis_items_uom_by_codes(
    cur: "pyodbc.Cursor", codes: List[str]
) -> Dict[str, Tuple[str, Optional[str], Optional[str]]]:
    """analysis_item_code -> (analysis_item_id, unit_of_measure_id, standard_quantity_unit_of_measure_id)."""
    out: Dict[str, Tuple[str, Optional[str], Optional[str]]] = {}
    if not codes:
        return out
    CHUNK = 500
    for i in range(0, len(codes), CHUNK):
        chunk = codes[i : i + CHUNK]
        ph = ",".join(["?"] * len(chunk))
        cur.execute(
            f"""
            SELECT LTRIM(RTRIM(analysis_item_code)),
                   CONVERT(varchar(36), analysis_item_id) AS aid,
                   CONVERT(varchar(36), unit_of_measure_id) AS uid,
                   CONVERT(varchar(36), standard_quantity_unit_of_measure_id) AS sid
            FROM dbo.analysis_item
            WHERE analysis_item_code IN ({ph})
            """,
            *chunk,
        )
        for code, aid, uid, sid in cur.fetchall():
            c = str(code).strip()
            out[c] = (
                str(aid).strip(),
                str(uid).strip() if uid else None,
                str(sid).strip() if sid else None,
            )
    return out


def resolve_uom_id(mappings: dict, text: str) -> Optional[str]:
    if not text or not str(text).strip():
        return None
    if uom_cap._is_blank_uom_cell(str(text)):
        return None
    rid = cap_imp.resolve_master(mappings, "unit_of_measures", text)
    if rid is None:
        return None
    return str(rid).strip()


def main() -> int:
    ap = argparse.ArgumentParser(
        description="Gán ĐVT cho chỉ tiêu (analysis_item) theo Capability.xlsx."
    )
    ap.add_argument("--xlsx", type=Path, default=_DEFAULT_XLSX)
    ap.add_argument("--sheet", default="Vietlabs")
    ap.add_argument("--conn", default="")
    ap.add_argument("--dry-run", action="store_true")
    ap.add_argument(
        "--mode",
        choices=("only-null", "sync"),
        default="only-null",
        help="only-null: chỉ fill FK đang trống. sync: ghi đè khi Excel có ĐVT resolve được.",
    )
    ap.add_argument(
        "--commit-every",
        type=int,
        default=500,
        help="Commit theo batch (0 = một commit cuối).",
    )
    ap.add_argument("--limit", type=int, default=0, help="Giới hạn số mã CT xử lý (0 = không giới hạn).")
    args = ap.parse_args()

    xlsx_path = args.xlsx.resolve()
    if not xlsx_path.is_file():
        print(f"Không tìm thấy: {xlsx_path}", file=sys.stderr)
        return 2

    conn_str = _conn_str(args.conn)
    if not conn_str or pyodbc is None:
        print("Thiếu connection string hoặc pyodbc.", file=sys.stderr)
        return 2

    try:
        header, data = uom_cap.read_sheet_rows(xlsx_path, args.sheet)
        ct_map, warns = uom_cap.build_ct_to_excel_uoms(header, data)
    except Exception as ex:
        print(f"Lỗi đọc Excel / map cột: {ex}", file=sys.stderr)
        return 2

    codes = sorted(ct_map.keys())
    if args.limit and args.limit > 0:
        codes = codes[: int(args.limit)]

    conn = pyodbc.connect(conn_str, autocommit=False)
    now = datetime.now(timezone.utc)
    try:
        cur = conn.cursor()
        mappings: dict = {}
        cap_imp.augment_master_maps(conn, mappings)

        items = fetch_analysis_items_uom_by_codes(cur, codes)
        print("=" * 72)
        print("Link AnalysisItem ↔ Đơn vị tính (Capability.xlsx)")
        print("=" * 72)
        print(f"XLSX: {xlsx_path}  sheet={args.sheet!r}")
        print(f"Mode: {args.mode}  dry-run: {args.dry_run}")
        print(f"Mã CT trên Excel (có merge ĐVT/ĐVT KL): {len(ct_map)}")
        print(f"Khớp analysis_item trên DB: {len(items)}")
        print(f"unit_of_measures keys trong map: {len(mappings.get('unit_of_measures') or {})}")
        for w in warns[:15]:
            print(f"  [xlsx] {w}")
        if len(warns) > 15:
            print(f"  ... +{len(warns) - 15} cảnh báo.")

        updated = 0
        skipped = 0
        missing_item = 0
        unresolved_main = 0
        unresolved_sq = 0

        commit_every = max(0, int(args.commit_every))
        pending_commit = 0

        for ct in codes:
            main_raw, sq_raw = ct_map[ct]
            row = items.get(ct)
            if not row:
                missing_item += 1
                continue

            aid, cur_u, cur_sq = row
            cur_u_s = _guid_str(cur_u) if cur_u else ""
            cur_sq_s = _guid_str(cur_sq) if cur_sq else ""

            target_u: Optional[str] = None
            target_sq: Optional[str] = None
            want_u = False
            want_sq = False

            if main_raw and not uom_cap._is_blank_uom_cell(main_raw):
                ru = resolve_uom_id(mappings, main_raw)
                if ru is None:
                    unresolved_main += 1
                else:
                    if args.mode == "sync":
                        if ru.lower() != cur_u_s.lower():
                            target_u = ru
                            want_u = True
                    else:
                        if _is_empty_fk(cur_u):
                            target_u = ru
                            want_u = True

            if sq_raw and not uom_cap._is_blank_uom_cell(sq_raw):
                rs = resolve_uom_id(mappings, sq_raw)
                if rs is None:
                    unresolved_sq += 1
                else:
                    if args.mode == "sync":
                        if rs.lower() != cur_sq_s.lower():
                            target_sq = rs
                            want_sq = True
                    else:
                        if _is_empty_fk(cur_sq):
                            target_sq = rs
                            want_sq = True

            if not want_u and not want_sq:
                skipped += 1
                continue

            new_u = target_u if want_u else cur_u
            new_sq = target_sq if want_sq else cur_sq

            if args.dry_run:
                updated += 1
                if updated <= 25:
                    print(
                        f"  [dry] {ct}  uom={new_u!r} sq_uom={new_sq!r}  "
                        f"(excel ĐVT={main_raw!r} KL={sq_raw!r})"
                    )
                continue

            cur.execute(
                """
                UPDATE dbo.analysis_item
                SET unit_of_measure_id = ?,
                    standard_quantity_unit_of_measure_id = ?,
                    updated_at = ?
                WHERE analysis_item_id = CAST(? AS uniqueidentifier)
                """,
                (new_u, new_sq, now, aid),
            )
            updated += 1
            pending_commit += 1
            if commit_every > 0 and pending_commit >= commit_every:
                conn.commit()
                pending_commit = 0
                print(f"  ... committed batch, total updates {updated}")

        if args.dry_run:
            conn.rollback()
        else:
            conn.commit()

        print("\nKết quả:")
        print(f"  Updated (hoặc dry-run sẽ sửa): {updated}")
        print(f"  Skipped (không cần đổi / only-null đã có FK): {skipped}")
        print(f"  Không có analysis_item cho mã CT: {missing_item}")
        print(f"  Excel có ĐVT chính nhưng không resolve được master: {unresolved_main}")
        print(f"  Excel có ĐVT KL nhưng không resolve được master: {unresolved_sq}")
        if args.dry_run:
            print("\nChạy lại không --dry-run để ghi DB.")
        return 0
    except Exception as ex:
        conn.rollback()
        print(f"Lỗi: {ex}", file=sys.stderr)
        return 1
    finally:
        conn.close()


if __name__ == "__main__":
    raise SystemExit(main())
