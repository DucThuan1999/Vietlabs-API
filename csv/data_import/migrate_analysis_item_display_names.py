#!/usr/bin/env python3
"""
Di chuyển JSON Tiptap từ name_vi/name_en sang display_name_vi/display_name_en;
ghi plain text trở lại name_vi/name_en.

  python3 migrate_analysis_item_display_names.py --dry-run
  python3 migrate_analysis_item_display_names.py
"""
from __future__ import annotations

import argparse
import json
import re

try:
    import pyodbc
except ImportError:
    pyodbc = None  # type: ignore

DOC_TYPE = "doc"


def is_json_doc(s: str | None) -> bool:
    if not s:
        return False
    t = s.strip()
    if not t.startswith("{"):
        return False
    try:
        o = json.loads(t)
        return isinstance(o, dict) and o.get("type") == DOC_TYPE
    except json.JSONDecodeError:
        return False


def plain_from_doc(node, parts: list[str]) -> None:
    if not node:
        return
    if node.get("type") == "text":
        parts.append(node.get("text") or "")
        return
    if node.get("type") == "hardBreak":
        parts.append("\n")
        return
    content = node.get("content")
    if not isinstance(content, list):
        return
    for i, child in enumerate(content):
        plain_from_doc(child, parts)
        if node.get("type") == DOC_TYPE and child.get("type") == "paragraph" and i < len(content) - 1:
            parts.append("\n")


def plain_from_json(s: str) -> str:
    try:
        doc = json.loads(s.strip())
    except json.JSONDecodeError:
        return s.strip()
    parts: list[str] = []
    plain_from_doc(doc, parts)
    return re.sub(r"\n+", " ", "".join(parts)).strip()


def migrate_column(cur, col_name: str, display_col: str, dry_run: bool) -> int:
    cur.execute(
        f"""
        SELECT analysis_item_id, {col_name}
        FROM dbo.analysis_item
        WHERE {col_name} IS NOT NULL
          AND LTRIM(RTRIM({col_name})) LIKE '{{%'
        """
    )
    rows = cur.fetchall()
    n = 0
    for row in rows:
        aid, raw = row[0], row[1]
        if not is_json_doc(raw):
            continue
        plain = plain_from_json(raw)
        if dry_run:
            print(f"  [{aid}] {col_name}: JSON -> display, plain={plain[:80]!r}...")
        else:
            cur.execute(
                f"""
                UPDATE dbo.analysis_item
                SET {display_col} = ?, {col_name} = ?
                WHERE analysis_item_id = ?
                """,
                (raw, plain or None, aid),
            )
        n += 1
    return n


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--dry-run", action="store_true")
    parser.add_argument("--connection-string", default="", help="ODBC connection string")
    args = parser.parse_args()

    if pyodbc is None:
        raise SystemExit("Cần cài pyodbc: pip install pyodbc")

    conn_str = args.connection_string.strip()
    if not conn_str:
        raise SystemExit("Cung cấp --connection-string (ODBC)")

    conn = pyodbc.connect(conn_str)
    cur = conn.cursor()
    try:
        n_vi = migrate_column(cur, "name_vi", "display_name_vi", args.dry_run)
        n_en = migrate_column(cur, "name_en", "display_name_en", args.dry_run)
        if not args.dry_run:
            conn.commit()
        print(f"Đã xử lý name_vi: {n_vi}, name_en: {n_en}")
    finally:
        cur.close()
        conn.close()


if __name__ == "__main__":
    main()
