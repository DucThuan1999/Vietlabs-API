#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Chuyển ô Excel (openpyxl Cell) sang:
- plain text (cho name_vi / name_en / short_name — search/filter)
- Tiptap JSON document (cho display_name_vi / display_name_en / display_short_name)

Hỗ trợ:
- CellRichText (nhiều run, mỗi run có font riêng)
- Font cấp cell (italic / superscript / subscript qua vertAlign)
- Xuống dòng trong ô -> nhiều paragraph
"""
from __future__ import annotations

import json
import re
from typing import Any, Dict, List, Optional, Tuple

try:
    from openpyxl.cell.rich_text import CellRichText, TextBlock
except ImportError:  # pragma: no cover
    CellRichText = None  # type: ignore
    TextBlock = None  # type: ignore

DOC_TYPE = "doc"
FORMAT_MARKS = frozenset({"italic", "subscript", "superscript"})


def cell_str(v: Any) -> str:
    if v is None:
        return ""
    if isinstance(v, float) and v == int(v):
        return str(int(v))
    return str(v)


def _marks_from_font(font) -> List[Dict[str, str]]:
    if font is None:
        return []
    marks: List[Dict[str, str]] = []
    if getattr(font, "i", None):
        marks.append({"type": "italic"})
    vert = getattr(font, "vertAlign", None)
    if vert == "superscript":
        marks.append({"type": "superscript"})
    elif vert == "subscript":
        marks.append({"type": "subscript"})
    return marks


def _text_node(text: str, marks: List[Dict[str, str]]) -> Dict[str, Any]:
    node: Dict[str, Any] = {"type": "text", "text": text}
    if marks:
        node["marks"] = marks
    return node


def _plain_from_doc(doc: Dict[str, Any]) -> str:
    parts: List[str] = []
    content = doc.get("content") or []
    if not isinstance(content, list):
        return ""
    for i, block in enumerate(content):
        if not isinstance(block, dict):
            continue
        if block.get("type") == "paragraph":
            for child in block.get("content") or []:
                if isinstance(child, dict) and child.get("type") == "text":
                    parts.append(child.get("text") or "")
        if i < len(content) - 1:
            parts.append("\n")
    plain = "".join(parts)
    return re.sub(r"\s+", " ", plain.replace("\n", " ")).strip()


def _has_format_marks(doc: Dict[str, Any]) -> bool:
    def walk(node: Any) -> bool:
        if not isinstance(node, dict):
            return False
        if node.get("type") == "text":
            for m in node.get("marks") or []:
                if isinstance(m, dict) and m.get("type") in FORMAT_MARKS:
                    return True
        for child in node.get("content") or []:
            if walk(child):
                return True
        return False

    return walk(doc)


def _runs_from_rich_text(rich: CellRichText) -> List[Tuple[str, List[Dict[str, str]]]]:
    runs: List[Tuple[str, List[Dict[str, str]]]] = []
    for part in rich:
        if isinstance(part, str):
            runs.append((part, []))
        elif TextBlock is not None and isinstance(part, TextBlock):
            runs.append((part.text or "", _marks_from_font(part.font)))
        else:
            runs.append((str(part), []))
    return runs


def _doc_from_text_runs(
    runs: List[Tuple[str, List[Dict[str, str]]]],
    default_marks: List[Dict[str, str]],
) -> Dict[str, Any]:
    """Một paragraph duy nhất từ các run (không có \\n)."""
    nodes: List[Dict[str, Any]] = []
    for text, marks in runs:
        if not text:
            continue
        merged = marks or list(default_marks)
        nodes.append(_text_node(text, merged))
    return {"type": DOC_TYPE, "content": [{"type": "paragraph", "content": nodes}]}


def _doc_from_plain_text(text: str, default_marks: List[Dict[str, str]]) -> Dict[str, Any]:
    lines = text.split("\n")
    paragraphs: List[Dict[str, Any]] = []
    for line in lines:
        content: List[Dict[str, Any]] = []
        if line:
            content.append(_text_node(line, default_marks))
        paragraphs.append({"type": "paragraph", "content": content})
    if not paragraphs:
        paragraphs = [{"type": "paragraph", "content": []}]
    return {"type": DOC_TYPE, "content": paragraphs}


def cell_to_plain_and_display(cell) -> Tuple[Optional[str], Optional[str]]:
    """
    Returns (plain_text, tiptap_json_string).
    plain_text: collapsed whitespace for search.
    display: JSON string luôn được tạo khi có nội dung; null nếu ô rỗng.
    """
    if cell is None:
        return None, None

    val = cell.value
    if val is None or (isinstance(val, str) and not val.strip()):
        return None, None

    default_marks = _marks_from_font(getattr(cell, "font", None))

    if CellRichText is not None and isinstance(val, CellRichText):
        raw = str(val)
        if "\n" in raw:
            # Hiếm: rich text nhiều dòng — tách paragraph, giữ run trong từng dòng
            line_docs = []
            for line in raw.split("\n"):
                if not line and not default_marks:
                    line_docs.append({"type": "paragraph", "content": []})
                    continue
                runs = _runs_from_rich_text(val) if line == raw else [(line, default_marks)]
                if line != raw:
                    runs = [(line, default_marks)]
                nodes = []
                for t, m in runs:
                    if t:
                        nodes.append(_text_node(t, m or default_marks))
                line_docs.append({"type": "paragraph", "content": nodes})
            doc = {"type": DOC_TYPE, "content": line_docs or [{"type": "paragraph", "content": []}]}
        else:
            runs = _runs_from_rich_text(val)
            doc = _doc_from_text_runs(runs, default_marks)
    else:
        text = cell_str(val)
        doc = _doc_from_plain_text(text, default_marks)

    plain = _plain_from_doc(doc)
    if not plain:
        return None, None

    display = json.dumps(doc, ensure_ascii=False)
    return plain, display


def cell_to_plain_and_display_optional(cell) -> Tuple[Optional[str], Optional[str]]:
    """
    Giống cell_to_plain_and_display nhưng display = None nếu doc không có mark đặc biệt
    và chỉ một dòng (tương thích UI cũ). Import năng lực v2 luôn ghi display.
    """
    plain, display = cell_to_plain_and_display(cell)
    if not display:
        return plain, None
    try:
        doc = json.loads(display)
    except json.JSONDecodeError:
        return plain, display
    multi_para = len(doc.get("content") or []) > 1
    if multi_para or _has_format_marks(doc):
        return plain, display
    return plain, None
