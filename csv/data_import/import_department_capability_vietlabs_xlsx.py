#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Import năng lực Vietlabs từ Capability.xlsx (sheet Vietlabs) vào:
- Mỗi dòng = một chỉ tiêu; có thể có nhiều DAC (theo HCM/CT/BL/CM và chỉ định).

Quy tắc ô NĐ 107 / ô chỉ định (ISO, Cục BVTV, …):
- Giá trị "Chưa có" / rỗng / x → không ghi NĐ 107; không DELETE designation.
- Giá trị là ngày → ghi nd_107 + ngày (DAC) hoặc expired_date (designation); INSERT DAC nếu chưa có bản ghi
  cho bộ (department, branch_id, analysis_item_id).
- Nếu NĐ 107 là Chưa có nhưng có ít nhất một ô chỉ định HCM (hoặc ISO CT) là ngày: vẫn tạo DAC thiếu với
  nd_107 = false, nd_107_expired_date = NULL rồi ghi designation như bình thường.

Nguồn dữ liệu:
- Kỹ thuật (UI): lọc theo chỉ tiêu, load từ analysis_item.laboratory_technique_id → laboratory_technique.
  Import chỉ kiểm tra chỉ tiêu đã gán FK kỹ thuật — không ghi cột Ghi chú (notes) trên DAC.
- Phòng ban: cột Bộ phận phụ trách — department.name_vi chứa chuỗi đó (CHARINDEX).
  Với mỗi site NĐ 107 (HCM/CT/BL/CM) chỉ tra phòng ban khi ô NĐ 107 site đó là ngày (ô Chưa có thì bỏ qua, không báo thiếu phòng ban).
  Giá trị "Cổ điển" hoặc "Quang phổ" được gộp khi tra cứu thành "Quang phổ - Cổ điển".
- Chi nhánh mỗi DAC: HCM→SG, CT→CT, BL→BL, CM→CM (SITES_ND107_FULL). Mặc định chỉ SG nếu
  IMPORT_BRANCH_SG_ONLY = True; lệnh --all-branches bật đủ 4 chi nhánh cho lần chạy đó.

Log xử lý tay: mặc định ghi CSV cạnh file xlsx (hoặc --failure-log / --no-failure-log).
"""

from __future__ import annotations

import argparse
import csv
import os
import re
import sys
import uuid
from datetime import datetime, timezone
from typing import Any, Dict, List, Optional, Tuple

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

import import_analysis_item as iai

try:
    import openpyxl
except ImportError:
    print("Can cai: pip install openpyxl")
    sys.exit(1)

DEFAULT_XLSX = os.path.normpath(
    os.path.join(os.path.dirname(__file__), "..", "..", "..", "data", "Capability.xlsx")
)


def default_failure_log_path(xlsx_path: str) -> str:
    d = os.path.dirname(os.path.abspath(xlsx_path))
    base = os.path.splitext(os.path.basename(xlsx_path))[0]
    ts = datetime.now().strftime("%Y%m%d_%H%M%S")
    return os.path.join(d, f"{base}_capability_import_failures_{ts}.csv")

# Tạm thời True: chỉ INSERT/UPDATE DAC & chỉ định HCM (chi nhánh SG). Đặt False để bật lại CT/BL/CM.
IMPORT_BRANCH_SG_ONLY = True

# (tên site trên sheet, cột capmap NĐ 107, branch_code)
SITES_ND107_FULL: List[Tuple[str, str, str]] = [
    ("HCM", "nd107_hcm", "SG"),
    ("CT", "nd107_ct", "CT"),
    ("BL", "nd107_bl", "BL"),
    ("CM", "nd107_cm", "CM"),
]

DESIGNATION_CODES = {
    "iso": "ISO",
    "cuc_bvtv": "CUC_BVTV",
    "bo_cong_thuong": "BO_CONG_THUONG",
    "nafi": "NAFI",
    "cuc_chan_nuoi": "CUC_CHAN_NUOI",
}


def norm_header_cell(h: Any) -> str:
    if h is None:
        return ""
    s = str(h).replace("\n", " ").replace('"', "").replace("'", "").strip()
    s = re.sub(r"\s+", " ", s).lower()
    return s


def build_maps(header_row: Tuple[Any, ...]) -> Tuple[Dict[str, int], Dict[str, int]]:
    pairs: List[Tuple[str, int]] = [
        (norm_header_cell(v), i) for i, v in enumerate(header_row)
    ]

    def find(pred) -> Optional[int]:
        for nh, idx in pairs:
            if nh and pred(nh):
                return idx
        return None

    colmap: Dict[str, int] = {}
    c = find(lambda nh: "mã chỉ tiêu" in nh)
    if c is not None:
        colmap["code"] = c
    lt = find(
        lambda nh: "bộ phận phụ trách" in nh
        or ("kỹ thuật" in nh and "bộ phận" in nh)
    )
    if lt is not None:
        colmap["lab"] = lt

    cap: Dict[str, int] = {}
    for nh, idx in pairs:
        if "năng lực hcm" in nh and ("107" in nh or "đ 107" in nh or "nđ 107" in nh):
            cap["nd107_hcm"] = idx
        elif "năng lực ct" in nh and ("107" in nh or "đ 107" in nh or "nđ 107" in nh):
            cap["nd107_ct"] = idx
        elif "năng lực bl" in nh and ("107" in nh or "đ 107" in nh or "nđ 107" in nh):
            cap["nd107_bl"] = idx
        elif "năng lực cm" in nh and ("107" in nh or "đ 107" in nh or "nđ 107" in nh):
            cap["nd107_cm"] = idx
        elif "cục bvtv" in nh or "cuc bvtv" in nh:
            cap["cuc_bvtv"] = idx
        elif "bộ công thương" in nh or "bo cong thuong" in nh:
            cap["bo_cong_thuong"] = idx
        elif "nafi" in nh:
            cap["nafi"] = idx
        elif "chăn nuôi" in nh or "chan nuoi" in nh:
            cap["cuc_chan_nuoi"] = idx

    iso_idxs = sorted(
        idx for nh, idx in pairs if "iso" in nh and "(" in nh and "a" in nh
    )
    if len(iso_idxs) >= 1:
        cap["iso_hcm"] = iso_idxs[0]
    if len(iso_idxs) >= 2:
        cap["iso_ct"] = iso_idxs[1]

    return colmap, cap


def cell_str(v: Any) -> str:
    if v is None:
        return ""
    if isinstance(v, float) and v == int(v):
        return str(int(v))
    return str(v).strip()


def is_chua_co(s: str) -> bool:
    t = s.strip().lower()
    return (
        not t
        or "chưa có" in t
        or "chua co" in t
        or t in ("na", "n/a", "-", "x", "**x**")
    )


def parse_vn_date_string(s: str) -> Optional[datetime]:
    s = s.strip()
    for _sep in ("/", "-", "."):
        if _sep in s:
            parts = re.split(r"[/.\-]", s)
            if len(parts) == 3:
                try:
                    d, m, y = int(parts[0]), int(parts[1]), int(parts[2])
                    if y < 100:
                        y += 2000
                    return datetime(y, m, d)
                except (ValueError, OverflowError):
                    return None
    return None


def parse_nd107_cell(v: Any) -> Tuple[bool, Optional[datetime]]:
    if v is None:
        return False, None
    if isinstance(v, datetime):
        return True, v
    s = cell_str(v)
    if is_chua_co(s):
        return False, None
    dt = parse_vn_date_string(s)
    if dt:
        return True, dt
    return False, None


def parse_designation_cell(v: Any) -> Optional[datetime]:
    if v is None:
        return None
    if isinstance(v, datetime):
        return v
    s = cell_str(v)
    if is_chua_co(s):
        return None
    return parse_vn_date_string(s)


def fetch_analysis_item_for_import(
    cur, analysis_item_code: str
) -> Optional[Tuple[str, Optional[str]]]:
    """
    Trả về (analysis_item_id, laboratory_technique_id) hoặc None.
    Bắt buộc có laboratory_technique_id để khớp frontend (kỹ thuật theo chỉ tiêu).
    """
    cur.execute(
        """
        SELECT CAST(ai.analysis_item_id AS nvarchar(50)),
               CAST(ai.laboratory_technique_id AS nvarchar(50))
        FROM analysis_item ai
        WHERE ai.analysis_item_code = ?
        """,
        analysis_item_code,
    )
    r = cur.fetchone()
    if not r:
        return None
    aid = str(r[0]).strip()
    lid = str(r[1]).strip() if r[1] is not None else None
    if not lid:
        lid = None
    return aid, lid


def normalize_bo_phan_for_department_lookup(bo_phan_raw: str) -> str:
    """
    Excel có thể ghi tách "Cổ điển" / "Quang phổ"; trong DB thường là một phòng
    "Quang phổ - Cổ điển". CHARINDEX trên name_vi dùng chuỗi gộp này.
    """
    t = (bo_phan_raw or "").strip()
    if not t:
        return t
    key = re.sub(r"\s+", " ", t).casefold()
    if key in ("cổ điển", "quang phổ"):
        return "Quang phổ - Cổ điển"
    return t


def resolve_department_by_name_vi(
    cur,
    branch_code: str,
    bo_phan_text: str,
) -> Optional[Tuple[str, str]]:
    """
    Chi nhánh = branch_code (SG / CT / BL / CM theo cột năng lực tương ứng).
    Phòng ban: department.name_vi chứa chuỗi Bộ phận phụ trách (không dùng mã DEP-*).
    """
    bp = normalize_bo_phan_for_department_lookup(bo_phan_text or "")
    if len(bp) < 2:
        return None
    bp_clean = bp.replace("%", "").replace("_", "").replace("[", "").strip()
    if len(bp_clean) < 2:
        return None
    needle = bp_clean.lower()[:200]
    cur.execute(
        """
        SELECT TOP 1 d.department_id, CAST(d.branch_id AS nvarchar(50))
        FROM department d
        INNER JOIN branch b ON d.branch_id = b.branch_id
        WHERE b.branch_code = ?
          AND (d.status IS NULL OR d.status = 'Active')
          AND CHARINDEX(?, LOWER(ISNULL(d.name_vi, N''))) > 0
        ORDER BY LEN(d.name_vi), d.department_code
        """,
        branch_code,
        needle,
    )
    r = cur.fetchone()
    if not r:
        return None
    return str(r[0]), str(r[1]).strip()


def assert_dac_table_schema(cur) -> None:
    cur.execute(
        """
        SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'department_analysis_capability'
        """
    )
    cols = {str(r[0]).lower() for r in cur.fetchall()}
    need = {"nd_107", "nd_107_expired_date"}
    missing = need - cols
    if missing:
        print(
            "Loi: bang department_analysis_capability thieu cot:",
            ", ".join(sorted(missing)),
        )
        print("Can chay migration EF hoac ALTER TABLE cho khop model.")
        sys.exit(1)


def load_designation_ids(cur) -> Dict[str, str]:
    cur.execute(
        "SELECT designation_code, designation_id FROM designation WHERE designation_code IS NOT NULL"
    )
    m: Dict[str, str] = {}
    for code, did in cur.fetchall():
        if code:
            m[str(code).strip().upper()] = str(did)
    return m


def find_dac_id(
    cur,
    department_id: str,
    branch_id_str: str,
    analysis_item_id: str,
) -> Optional[str]:
    cur.execute(
        """
        SELECT department_analysis_capability_id FROM department_analysis_capability
        WHERE department_id = ? AND branch_id = ? AND analysis_item_id = ?
        """,
        department_id,
        branch_id_str,
        analysis_item_id,
    )
    r = cur.fetchone()
    return str(r[0]) if r else None


def upsert_dac_with_date(
    cur,
    department_id: str,
    branch_id_str: str,
    analysis_item_id: str,
    nd107_exp: datetime,
    dry_run: bool,
) -> Tuple[str, bool]:
    """Chỉ gọi khi ô Excel là ngày hợp lệ: nd_107 = true, nd_107_expired_date = ngày. Không ghi notes."""
    cur.execute(
        """
        SELECT department_analysis_capability_id FROM department_analysis_capability
        WHERE department_id = ? AND branch_id = ? AND analysis_item_id = ?
        """,
        department_id,
        branch_id_str,
        analysis_item_id,
    )
    row = cur.fetchone()
    now = datetime.now(timezone.utc)
    if row:
        dac_id = str(row[0])
        if not dry_run:
            cur.execute(
                """
                UPDATE department_analysis_capability
                SET nd_107 = ?, nd_107_expired_date = ?, updated_at = ?
                WHERE department_analysis_capability_id = ?
                """,
                True,
                nd107_exp,
                now,
                dac_id,
            )
        return dac_id, False
    dac_id = str(uuid.uuid4())
    if not dry_run:
        cur.execute(
            """
            INSERT INTO department_analysis_capability (
                department_analysis_capability_id, department_id, branch_id, analysis_item_id,
                nd_107, nd_107_expired_date, status, created_at
            ) VALUES (?, ?, ?, ?, ?, ?, 'Active', ?)
            """,
            dac_id,
            department_id,
            branch_id_str,
            analysis_item_id,
            True,
            nd107_exp,
            now,
        )
    return dac_id, True


def ensure_dac_without_nd107(
    cur,
    department_id: str,
    branch_id_str: str,
    analysis_item_id: str,
    dry_run: bool,
) -> Tuple[str, bool]:
    """
    DAC chưa tồn tại: INSERT nd_107 = false, nd_107_expired_date = NULL (ĐK hoạt động Chưa có trên UI).
    Đã tồn tại: trả về id, không đổi nd_107 / ngày (tránh ghi đè bản ghi đã có NĐ 107).
    """
    cur.execute(
        """
        SELECT department_analysis_capability_id FROM department_analysis_capability
        WHERE department_id = ? AND branch_id = ? AND analysis_item_id = ?
        """,
        department_id,
        branch_id_str,
        analysis_item_id,
    )
    row = cur.fetchone()
    if row:
        return str(row[0]), False
    dac_id = str(uuid.uuid4())
    now = datetime.now(timezone.utc)
    if not dry_run:
        cur.execute(
            """
            INSERT INTO department_analysis_capability (
                department_analysis_capability_id, department_id, branch_id, analysis_item_id,
                nd_107, nd_107_expired_date, status, created_at
            ) VALUES (?, ?, ?, ?, ?, NULL, 'Active', ?)
            """,
            dac_id,
            department_id,
            branch_id_str,
            analysis_item_id,
            False,
            now,
        )
    return dac_id, True


def sync_designation(
    cur,
    dac_id: str,
    designation_id: str,
    expired: Optional[datetime],
    dry_run: bool,
) -> None:
    cur.execute(
        """
        SELECT department_analysis_capability_designation_id
        FROM department_analysis_capability_designation
        WHERE department_analysis_capability_id = ? AND designation_id = ?
        """,
        dac_id,
        designation_id,
    )
    ex = cur.fetchone()
    if expired is None:
        # Chưa có / không phải ngày: bỏ qua, không xóa bản ghi designation hiện có
        return
    if ex:
        if not dry_run:
            cur.execute(
                """
                UPDATE department_analysis_capability_designation
                SET expired_date = ?
                WHERE department_analysis_capability_designation_id = ?
                """,
                expired,
                str(ex[0]),
            )
    elif not dry_run:
        did = str(uuid.uuid4())
        cur.execute(
            """
            INSERT INTO department_analysis_capability_designation (
                department_analysis_capability_designation_id,
                department_analysis_capability_id, designation_id, expired_date
            ) VALUES (?, ?, ?, ?)
            """,
            did,
            dac_id,
            designation_id,
            expired,
        )


def process(
    xlsx_path: str,
    connection,
    dry_run: bool,
    max_rows: Optional[int],
    only_codes: Optional[List[str]] = None,
    failure_log_path: Optional[str] = None,
    all_branches: bool = False,
) -> None:
    sites_nd107 = SITES_ND107_FULL if all_branches else SITES_ND107_FULL[:1]

    wb = openpyxl.load_workbook(xlsx_path, read_only=True, data_only=True)
    ws = wb["Vietlabs"]
    it = ws.iter_rows(values_only=True)
    header = next(it)
    colmap, capmap = build_maps(tuple(header))

    if "code" not in colmap or "lab" not in colmap:
        print("Loi: thieu cot Ma chi tieu hoac Bo phan phu trach")
        wb.close()
        return

    cur = connection.cursor()
    assert_dac_table_schema(cur)
    des_ids = load_designation_ids(cur)

    only_cf: Optional[set] = None
    if only_codes:
        only_cf = {c.strip().casefold() for c in only_codes if c and str(c).strip()}

    stats = {
        "rows_ok": 0,
        "skip": 0,
        "error": 0,
        "dac_insert": 0,
        "dac_update": 0,
        "dac_skip_chua_co": 0,
        "dac_skip_no_dept": 0,
        "dacd_write": 0,
        "dacd_skip_chua_co": 0,
    }
    failure_rows: List[Tuple[int, str, str]] = []

    def log_issue(row_idx: int, item_code: str, reason: str) -> None:
        failure_rows.append((row_idx, item_code, reason))

    for ridx, row in enumerate(it, start=2):
        if max_rows is not None and stats["rows_ok"] >= max_rows:
            break
        if not row:
            stats["skip"] += 1
            continue

        def gv(key: str) -> str:
            i = colmap.get(key)
            if i is None or i >= len(row):
                return ""
            return cell_str(row[i])

        def cv(key: str) -> Any:
            i = capmap.get(key)
            if i is None or i >= len(row):
                return None
            return row[i]

        code = gv("code").strip()
        if not code:
            stats["skip"] += 1
            continue
        if only_cf is not None and code.casefold() not in only_cf:
            continue

        bo_phan = gv("lab").strip()
        if len(bo_phan) < 2:
            print(f"  Dong {ridx} {code}: cot Bo phan phu trach trong hoac qua ngan")
            log_issue(
                ridx,
                code,
                "Cot Bo phan phu trach trong hoac qua ngan (< 2 ky tu)",
            )
            stats["error"] += 1
            continue

        ai_row = fetch_analysis_item_for_import(cur, code)
        if not ai_row:
            print(f"  Dong {ridx} {code}: chua co analysis_item")
            log_issue(ridx, code, "Chua co analysis_item trong DB (ma chi tieu)")
            stats["error"] += 1
            continue
        analysis_item_id, lt_id = ai_row
        if not lt_id:
            print(
                f"  Dong {ridx} {code}: chi tieu chua gan laboratory_technique_id (can cho frontend load ky thuat theo chi tieu)"
            )
            log_issue(
                ridx,
                code,
                "Chua gan laboratory_technique_id tren analysis_item",
            )
            stats["error"] += 1
            continue

        hcm_dac_id: Optional[str] = None
        ct_dac_id: Optional[str] = None

        try:
            for site_name, cap_key, branch_code in sites_nd107:
                if cap_key not in capmap:
                    continue
                # Chi tra phòng ban khi ô NĐ 107 site này có ngày — tránh log lỗi khi ô là Chưa có/rỗng
                nd_ok, nd_dt = parse_nd107_cell(cv(cap_key))
                if not nd_ok or nd_dt is None:
                    stats["dac_skip_chua_co"] += 1
                    continue
                resolved = resolve_department_by_name_vi(cur, branch_code, bo_phan)
                if not resolved:
                    stats["dac_skip_no_dept"] += 1
                    print(
                        f"  Dong {ridx} {code}: khong tim phong ban name_vi khop Bo phan phu trach @ branch {branch_code}"
                    )
                    log_issue(
                        ridx,
                        code,
                        f"NĐ 107 {site_name} ({branch_code}): khong tim phong ban "
                        f"name_vi chua chuoi Bo phan phu trach (gia tri Excel: {bo_phan!r})",
                    )
                    continue
                dept_id, dac_branch_id = resolved
                dac_id, inserted = upsert_dac_with_date(
                    cur,
                    dept_id,
                    dac_branch_id,
                    analysis_item_id,
                    nd_dt,
                    dry_run,
                )
                if inserted:
                    stats["dac_insert"] += 1
                else:
                    stats["dac_update"] += 1
                if site_name == "HCM":
                    hcm_dac_id = dac_id
                elif site_name == "CT":
                    ct_dac_id = dac_id

            def hcm_has_designation_date() -> bool:
                for ck in (
                    "iso_hcm",
                    "cuc_bvtv",
                    "bo_cong_thuong",
                    "nafi",
                    "cuc_chan_nuoi",
                ):
                    if ck not in capmap:
                        continue
                    if parse_designation_cell(cv(ck)) is not None:
                        return True
                return False

            if hcm_dac_id is None and hcm_has_designation_date():
                hcm_res = resolve_department_by_name_vi(cur, "SG", bo_phan)
                if hcm_res:
                    d0, b0 = hcm_res
                    hcm_dac_id = find_dac_id(cur, d0, b0, analysis_item_id)
                    if hcm_dac_id is None:
                        hcm_dac_id, ins_nd = ensure_dac_without_nd107(
                            cur, d0, b0, analysis_item_id, dry_run
                        )
                        if ins_nd:
                            stats["dac_insert"] += 1

            if all_branches:
                if ct_dac_id is None and "iso_ct" in capmap:
                    if parse_designation_cell(cv("iso_ct")) is not None:
                        ct_res = resolve_department_by_name_vi(
                            cur, "CT", bo_phan
                        )
                        if ct_res:
                            d1, b1 = ct_res
                            ct_dac_id = find_dac_id(
                                cur, d1, b1, analysis_item_id
                            )
                            if ct_dac_id is None:
                                ct_dac_id, ins_ct = ensure_dac_without_nd107(
                                    cur, d1, b1, analysis_item_id, dry_run
                                )
                                if ins_ct:
                                    stats["dac_insert"] += 1

            if hcm_dac_id:
                hcm_specs = [
                    ("iso_hcm", "iso"),
                    ("cuc_bvtv", "cuc_bvtv"),
                    ("bo_cong_thuong", "bo_cong_thuong"),
                    ("nafi", "nafi"),
                    ("cuc_chan_nuoi", "cuc_chan_nuoi"),
                ]
                for ck, dk in hcm_specs:
                    if ck not in capmap:
                        continue
                    dcode = DESIGNATION_CODES[dk]
                    did = des_ids.get(dcode)
                    if not did:
                        exp = parse_designation_cell(cv(ck))
                        if exp is not None:
                            log_issue(
                                ridx,
                                code,
                                f"Chi dinh HCM ({ck}): bang designation khong co ma {dcode}",
                            )
                        continue
                    exp = parse_designation_cell(cv(ck))
                    if exp is None:
                        stats["dacd_skip_chua_co"] += 1
                        continue
                    sync_designation(cur, hcm_dac_id, did, exp, dry_run)
                    stats["dacd_write"] += 1
            elif hcm_has_designation_date():
                print(
                    f"  Dong {ridx} {code}: canh bao — co ngay chi dinh HCM nhung khong tim duoc phong ban @ SG (Bo phan phu trach)"
                )
                log_issue(
                    ridx,
                    code,
                    "Co ngay chi dinh HCM nhung khong tim phong ban @ SG "
                    f"(Bo phan phu trach Excel: {bo_phan!r})",
                )

            if all_branches:
                if ct_dac_id and "iso_ct" in capmap:
                    did = des_ids.get("ISO")
                    exp_ct = parse_designation_cell(cv("iso_ct"))
                    if did:
                        if exp_ct is None:
                            stats["dacd_skip_chua_co"] += 1
                        else:
                            sync_designation(
                                cur, ct_dac_id, did, exp_ct, dry_run
                            )
                            stats["dacd_write"] += 1
                    elif exp_ct is not None:
                        log_issue(
                            ridx,
                            code,
                            "Chi dinh ISO CT: bang designation khong co ma ISO",
                        )
                elif "iso_ct" in capmap and parse_designation_cell(cv("iso_ct")):
                    print(
                        f"  Dong {ridx} {code}: canh bao — co ngay ISO CT nhung khong tim duoc phong ban @ CT (Bo phan phu trach)"
                    )
                    log_issue(
                        ridx,
                        code,
                        "Co ngay ISO CT nhung khong tim phong ban @ CT "
                        f"(Bo phan phu trach Excel: {bo_phan!r})",
                    )

            if not dry_run:
                connection.commit()
            stats["rows_ok"] += 1
        except Exception as e:
            stats["error"] += 1
            print(f"  Dong {ridx} {code}: {e}")
            log_issue(ridx, code, f"Exception: {e!s}")
            try:
                connection.rollback()
            except Exception:
                pass

    wb.close()

    if failure_log_path and failure_rows:
        with open(
            failure_log_path, "w", encoding="utf-8-sig", newline=""
        ) as lf:
            w = csv.writer(lf)
            w.writerow(["sheet_row", "analysis_item_code", "reason"])
            w.writerows(failure_rows)

    print("\nKet qua import nang luc Vietlabs:")
    for k, v in stats.items():
        print(f"  {k}: {v}")
    if failure_log_path:
        if failure_rows:
            print(
                f"\nLog can xu ly tay ({len(failure_rows)} dong): {failure_log_path}"
            )
        else:
            print("\nKhong co dong loi — khong tao file log.")


def main():
    parser = argparse.ArgumentParser(
        description="Import nang luc Vietlabs -> department_analysis_capability (+ designation)"
    )
    parser.add_argument(
        "--xlsx",
        default=DEFAULT_XLSX if os.path.isfile(DEFAULT_XLSX) else None,
    )
    parser.add_argument("--dry-run", action="store_true")
    parser.add_argument("--max-rows", type=int, default=None, metavar="N")
    parser.add_argument(
        "--only-code",
        action="append",
        dest="only_codes",
        metavar="CODE",
        help="Chi xu ly ma chi tieu (lap lai tuy chon de nhieu ma)",
    )
    parser.add_argument(
        "--failure-log",
        default=None,
        metavar="PATH",
        help=(
            "File CSV: sheet_row, analysis_item_code, reason. "
            "Mac dinh: cung thu muc file xlsx, ten <ten_file>_capability_import_failures_<timestamp>.csv"
        ),
    )
    parser.add_argument(
        "--no-failure-log",
        action="store_true",
        help="Khong ghi file CSV loi/can xu ly tay",
    )
    parser.add_argument(
        "--all-branches",
        action="store_true",
        help="Bat day du SG/CT/BL/CM + ISO CT (bo qua che do chi SG trong IMPORT_BRANCH_SG_ONLY)",
    )
    args = parser.parse_args()

    if not args.xlsx or not os.path.isfile(args.xlsx):
        print("Loi: can --xlsx tro toi file Capability.xlsx (duong dan that).")
        if args.xlsx:
            print(f"  File khong ton tai: {args.xlsx}")
        sys.exit(1)

    if sys.platform == "win32":
        import io

        sys.stdout = io.TextIOWrapper(
            sys.stdout.buffer, encoding="utf-8", errors="replace"
        )

    all_branches = args.all_branches or not IMPORT_BRANCH_SG_ONLY

    print("=" * 60)
    if all_branches:
        print("Import nang luc Vietlabs (branch: SG / CT / BL / CM)")
    else:
        print("Import nang luc Vietlabs (CHI CHI NHANH SG — IMPORT_BRANCH_SG_ONLY)")
    print("=" * 60)

    if args.no_failure_log:
        fl_path: Optional[str] = None
    elif args.failure_log:
        fl_path = args.failure_log
    else:
        fl_path = default_failure_log_path(args.xlsx)

    conn = iai.pyodbc.connect(iai.CONNECTION_STRING)
    try:
        process(
            args.xlsx,
            conn,
            args.dry_run,
            args.max_rows,
            args.only_codes,
            fl_path,
            all_branches,
        )
    finally:
        conn.close()


if __name__ == "__main__":
    main()
