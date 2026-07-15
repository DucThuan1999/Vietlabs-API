#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""Shared header matching for capability workbook columns (NĐ 107 / chi nhánh PTN)."""
from __future__ import annotations

import re
from typing import Any, Dict, List, Tuple


def norm_header_cell(h: Any) -> str:
    if h is None:
        return ""
    s = str(h).replace("\n", " ").replace('"', "").replace("'", "").strip()
    return re.sub(r"\s+", " ", s).lower()


def _has_nd107_marker(nh: str) -> bool:
    return "107" in nh or "đ 107" in nh or "nđ 107" in nh


def matches_nd107_hcm(nh: str) -> bool:
    if not nh or "năng lực" not in nh:
        return False
    if "hcm" in nh:
        return True
    if any(token in nh for token in ("hồ chí minh", "ho chi minh", "tp. hồ chí minh")):
        return True
    return "năng lực hcm" in nh and _has_nd107_marker(nh)


def matches_nd107_ct(nh: str) -> bool:
    if not nh or "năng lực" not in nh:
        return False
    if "cần thơ" in nh or "can tho" in nh:
        return True
    return "năng lực ct" in nh and _has_nd107_marker(nh)


def matches_nd107_bl(nh: str) -> bool:
    if not nh or "năng lực" not in nh:
        return False
    if "bạc liêu" in nh or "bac lieu" in nh:
        return True
    return "năng lực bl" in nh and _has_nd107_marker(nh)


def matches_nd107_cm(nh: str) -> bool:
    if not nh or "năng lực" not in nh:
        return False
    if "cà mau" in nh or "ca mau" in nh:
        return True
    return "năng lực cm" in nh and _has_nd107_marker(nh)


ND107_COLUMN_MATCHERS: Tuple[Tuple[str, Any], ...] = (
    ("nd107_hcm", matches_nd107_hcm),
    ("nd107_ct", matches_nd107_ct),
    ("nd107_bl", matches_nd107_bl),
    ("nd107_cm", matches_nd107_cm),
)


def map_nd107_columns(pairs: List[Tuple[str, int]]) -> Dict[str, int]:
    """Map nd107_* keys from normalized (header, index) pairs."""
    cap: Dict[str, int] = {}
    for nh, idx in pairs:
        for key, matcher in ND107_COLUMN_MATCHERS:
            if key not in cap and matcher(nh):
                cap[key] = idx
                break
    return cap


def map_designation_columns(pairs: List[Tuple[str, int]], cap: Dict[str, int]) -> None:
    """Fill designation column keys on *cap* in place."""
    for nh, idx in pairs:
        if "cục bvtv" in nh or "cuc bvtv" in nh:
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
