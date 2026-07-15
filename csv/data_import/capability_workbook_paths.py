#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""Resolve default path to Danh mục Năng lực v2.xlsx (Unicode-safe)."""
from __future__ import annotations

import os
from pathlib import Path

_REPO = Path(__file__).resolve().parents[3]
_DATA = _REPO / "data"


def resolve_default_capability_xlsx() -> str:
    """Return path to v2 workbook, or legacy Capability.xlsx, or composed default."""
    globs = list(_DATA.glob("*N*ng l*c v2.xlsx"))
    if globs:
        return str(globs[0])
    v2 = _DATA / "Danh mục Năng lực v2.xlsx"
    if v2.is_file():
        return str(v2)
    legacy = _DATA / "Capability.xlsx"
    if legacy.is_file():
        return str(legacy)
    return str(v2)


def resolve_xlsx_arg(xlsx: str | None) -> str | None:
    if xlsx and os.path.isfile(xlsx):
        return xlsx
    default = resolve_default_capability_xlsx()
    if os.path.isfile(default):
        return default
    return xlsx
