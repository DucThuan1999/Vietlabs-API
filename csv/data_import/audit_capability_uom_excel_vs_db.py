#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Đọc data/Capability.xlsx (sheet Vietlabs), so với DB:

1) Chỉ tiêu (mã CT-*) có ô **Đơn vị tính** (hoặc ĐVT khối lượng) trên Excel nhưng
   trên DB `analysis_item.unit_of_measure_id` / `standard_quantity_unit_of_measure_id`
   vẫn NULL.

2) Các **chuỗi ĐVT** xuất hiện trên Excel mà không khớp bất kỳ dòng nào trong
   `dbo.unit_of_measure` (theo `import_analysis_item.normalize_text` trên name_vi, name_en, mã).

  cd Vietlabs-API/csv/data_import
  python3 audit_capability_uom_excel_vs_db.py
  python3 audit_capability_uom_excel_vs_db.py --xlsx /path/Capability.xlsx --sheet Vietlabs
"""

from __future__ import annotations

import argparse
import os
import sys
from pathlib import Path
from typing import Dict, List, Optional, Set, Tuple

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

import import_analysis_item as iai

import import_uom_from_capability_xlsx_and_audit as uom_cap

try:
    import pyodbc
except ImportError:
    pyodbc = None  # type: ignore


_REPO_ROOT = Path(__file__).resolve().parents[3]
_DEFAULT_XLSX = _REPO_ROOT / "data" / "Capability.xlsx"


def load_uom_master_keys(cur: "pyodbc.Cursor") -> Set[str]:
    cur.execute(
        """
        SELECT name_vi, name_en, unit_of_measure_code
        FROM dbo.unit_of_measure
        """
    )
    keys: Set[str] = set()
    for nvi, nen, code in cur.fetchall():
        for x in (nvi, nen, code):
            if x is None or not str(x).strip():
                continue
            k = iai.normalize_text(str(x).strip())
            if k:
                keys.add(k)
    return keys


def main() -> int:
    ap = argparse.ArgumentParser(
        description="Kiểm tra ĐVT trên Capability.xlsx vs liên kết analysis_item và danh mục unit_of_measure."
    )
    ap.add_argument("--xlsx", type=Path, default=_DEFAULT_XLSX)
    ap.add_argument("--sheet", default="Vietlabs")
    ap.add_argument("--conn", default="")
    ap.add_argument(
        "--max-print",
        type=int,
        default=60,
        help="Giới hạn số dòng in mỗi nhóm (0 = in hết).",
    )
    args = ap.parse_args()

    xlsx_path = args.xlsx.resolve()
    if not xlsx_path.is_file():
        print(f"Không tìm thấy: {xlsx_path}", file=sys.stderr)
        return 2

    conn_str = (
        args.conn or os.environ.get("VIETLABS_SQL_ODBC", "").strip() or iai.CONNECTION_STRING
    ).strip()
    if not conn_str or pyodbc is None:
        print("Thiếu connection string hoặc pyodbc.", file=sys.stderr)
        return 2

    try:
        header, data = uom_cap.read_sheet_rows(xlsx_path, args.sheet)
    except Exception as ex:
        print(f"Lỗi đọc Excel: {ex}", file=sys.stderr)
        return 2

    try:
        ct_map, warns = uom_cap.build_ct_to_excel_uoms(header, data)
    except Exception as ex:
        print(f"Lỗi parse cột Excel: {ex}", file=sys.stderr)
        return 2

    codes = sorted(ct_map.keys())
    conn = pyodbc.connect(conn_str)
    try:
        cur = conn.cursor()
        db_rows = uom_cap.fetch_analysis_item_uom_fk(cur, codes)
        master_keys = load_uom_master_keys(cur)
    finally:
        conn.close()

    for w in warns[:30]:
        print(f"[cảnh báo Excel] {w}")
    if len(warns) > 30:
        print(f"... +{len(warns) - 30} cảnh báo khác.")

    missing_main: List[str] = []
    missing_sq: List[str] = []
    missing_ct_row: List[str] = []

    excel_uom_samples: Set[str] = set()
    excel_sq_samples: Set[str] = set()

    for ct in codes:
        main_raw, sq_raw = ct_map[ct]
        row = db_rows.get(ct)
        if row is None:
            missing_ct_row.append(ct)
            continue
        uid, sid = row
        if main_raw and not uom_cap._is_blank_uom_cell(main_raw):
            excel_uom_samples.add(main_raw.strip())
            if not uid:
                missing_main.append(ct)
        if sq_raw and not uom_cap._is_blank_uom_cell(sq_raw):
            excel_sq_samples.add(sq_raw.strip())
            if not sid:
                missing_sq.append(ct)

    lim = args.max_print

    def _print_list(title: str, items: List[str], extra: str = "") -> None:
        print()
        print(f"=== {title} ({len(items)}) ===")
        if not items:
            print("(không có)")
            return
        show = items if lim == 0 else items[:lim]
        for c in show:
            mm, sq = ct_map.get(c, ("", ""))
            dbu = db_rows.get(c)
            print(
                f"  {c}  excel_ĐVT={mm!r}  excel_ĐVT_KL={sq!r}  "
                f"DB_uom={dbu[0] if dbu else None}  DB_sq_uom={dbu[1] if dbu else None}"
                f"{extra}"
            )
        if lim > 0 and len(items) > lim:
            print(f"  ... +{len(items) - lim} mã nữa.")

    print("=" * 72)
    print("Capability.xlsx ↔ DB — Đơn vị tính")
    print("=" * 72)
    print(f"File: {xlsx_path}  sheet={args.sheet!r}")
    print(f"Mã CT (có ít nhất một ô ĐVT/ĐVT KL không rỗng trên Excel): {len(ct_map)}")

    _print_list(
        "A) Có ĐVT trên Excel nhưng analysis_item.unit_of_measure_id IS NULL",
        missing_main,
    )
    _print_list(
        "B) Có ĐVT khối lượng trên Excel nhưng standard_quantity_unit_of_measure_id IS NULL",
        missing_sq,
    )
    _print_list(
        "C) Mã CT trên Excel nhưng không có analysis_item tương ứng trên DB",
        missing_ct_row,
    )

    unresolved_main: List[str] = []
    unresolved_sq: List[str] = []
    for s in sorted(excel_uom_samples):
        if iai.normalize_text(s) not in master_keys:
            unresolved_main.append(s)
    for s in sorted(excel_sq_samples):
        if iai.normalize_text(s) not in master_keys:
            unresolved_sq.append(s)

    print()
    print(f"=== D) Chuỗi ĐVT trên Excel không khớp danh mục unit_of_measure ({len(unresolved_main)}) ===")
    if not unresolved_main:
        print("(không có — mọi ĐVT chính đều resolve được vào master)")
    else:
        show = unresolved_main if lim == 0 else unresolved_main[:lim]
        for s in show:
            print(f"  {s!r}  (normalize={iai.normalize_text(s)!r})")
        if lim > 0 and len(unresolved_main) > lim:
            print(f"  ... +{len(unresolved_main) - lim} chuỗi nữa.")

    only_sq = sorted(set(unresolved_sq) - set(unresolved_main))
    if only_sq:
        print()
        print(f"=== E) Chỉ xuất hiện ở cột ĐVT khối lượng, không có trong master ({len(only_sq)}) ===")
        show = only_sq if lim == 0 else only_sq[:lim]
        for s in show:
            print(f"  {s!r}")
        if lim > 0 and len(only_sq) > lim:
            print(f"  ... +{len(only_sq) - lim} chuỗi nữa.")

    print()
    if missing_main or missing_sq or missing_ct_row:
        print(
            "Gợi ý: gán FK bằng import chỉ tiêu từ Capability "
            "(import_analysis_items_capability_vietlabs_xlsx.py) hoặc cập nhật thủ công trên UI."
        )
    if unresolved_main or only_sq:
        print(
            "Gợi ý: chạy import_uom_from_capability_xlsx_and_audit.py (bước import — "
            "gồm cột ĐVT khối lượng) hoặc import_unit_of_measure_from_csv.py."
        )

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
