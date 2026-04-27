#!/usr/bin/env python3
# -*- coding: utf-8 -*-

"""
Remap agent FK for clients based on Client.csv.

CSV semantics:
- internalCode: client's internal_code
- agentClientCode: agent's internal_code (NOT UUID)

DB semantics:
- dbo.client.internal_code: internal code
- dbo.client.client_id: UUID (uniqueidentifier stored as string in pyodbc fetch)
- dbo.client.agent_client_id: FK -> agent's client_id (UUID)

This script:
- Loads mapping {internal_code -> client_id} from DB
- Reads Client.csv and for each row with (internalCode, agentClientCode),
  resolves agent_client_id = client_id(agent internal_code)
- Updates dbo.client.agent_client_id for that client

Run:
  python3 remap_agent_fk_from_csv.py --csv ../../../data/Client.csv --dry-run
  python3 remap_agent_fk_from_csv.py --csv ../../../data/Client.csv
  python3 remap_agent_fk_from_csv.py --csv ../../../data/Client.csv --only-null

Connection:
  Uses env VIETLABS_SQL_CONNECTION if set; otherwise uses the same DEFAULT_CONNECTION
  as import_customer_csv.py (if present in that module).
"""

from __future__ import annotations

import argparse
import csv
import os
from typing import Dict, List, Tuple


def strip_cell(s: str) -> str:
    return (s or "").strip()


def normalize_header(h: str) -> str:
    return strip_cell(h).lstrip("\ufeff")


def row_to_dict(header: List[str], row: List[str]) -> Dict[str, str]:
    if len(row) < len(header):
        row = row + [""] * (len(header) - len(row))
    if len(row) > len(header):
        row = row[: len(header)]
    out: Dict[str, str] = {}
    for k, v in zip(header, row):
        out[normalize_header(k)] = v if v is not None else ""
    return out


def agent_code_from_row(d: Dict[str, str]) -> str:
    # New CSV header uses agentClientCode; keep backward compatibility.
    return strip_cell(d.get("agentClientCode") or d.get("agentClientId") or "")


def load_existing_internal_codes(cursor) -> Dict[str, str]:
    cursor.execute(
        "SELECT internal_code, CAST(client_id AS NVARCHAR(36)) FROM client WHERE internal_code IS NOT NULL"
    )
    m: Dict[str, str] = {}
    for code, cid in cursor.fetchall():
        c = strip_cell(code)
        i = strip_cell(cid)
        if c and i and c not in m:
            m[c] = i
    return m


def read_csv_dicts(path: str) -> List[Tuple[int, Dict[str, str]]]:
    out: List[Tuple[int, Dict[str, str]]] = []
    with open(path, "r", encoding="utf-8-sig", newline="") as f:
        r = csv.reader(f, delimiter=";")
        header = [normalize_header(c) for c in next(r)]
        for line_no, row in enumerate(r, start=2):
            d = row_to_dict(header, row)
            out.append((line_no, d))
    return out


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--csv", default="../../../data/Client.csv")
    ap.add_argument("--dry-run", action="store_true", help="Do not UPDATE, only report")
    ap.add_argument(
        "--only-null",
        action="store_true",
        help="Only update rows where agent_client_id is currently NULL",
    )
    ap.add_argument("--limit", type=int, default=0, help="Only process first N data rows (0=all)")
    args = ap.parse_args()

    # Lazy import pyodbc so the script can still print usage without it.
    try:
        import pyodbc  # type: ignore
    except Exception:
        print("Cần cài: pip install pyodbc")
        return 2

    conn_str = os.getenv("VIETLABS_SQL_CONNECTION", "").strip()
    if not conn_str:
        try:
            from import_customer_csv import DEFAULT_CONNECTION  # type: ignore

            conn_str = DEFAULT_CONNECTION
        except Exception:
            print("Thiếu VIETLABS_SQL_CONNECTION và không import được DEFAULT_CONNECTION.")
            return 2

    rows = read_csv_dicts(args.csv)
    if args.limit and args.limit > 0:
        rows = rows[: args.limit]

    conn = pyodbc.connect(conn_str, autocommit=False)
    cur = conn.cursor()

    code_to_id = load_existing_internal_codes(cur)

    updated = 0
    already_set = 0
    skipped_no_agent = 0
    skipped_no_internal = 0
    missing_agent_in_db = 0
    missing_client_in_db = 0

    for _ln, d in rows:
        internal = strip_cell(d.get("internalCode", ""))
        agent_code = agent_code_from_row(d)

        if not internal:
            skipped_no_internal += 1
            continue
        if not agent_code:
            skipped_no_agent += 1
            continue

        client_id = code_to_id.get(internal)
        if not client_id:
            missing_client_in_db += 1
            continue

        agent_id = code_to_id.get(agent_code)
        if not agent_id:
            missing_agent_in_db += 1
            continue

        if not args.dry_run:
            if args.only_null:
                cur.execute(
                    "UPDATE client SET agent_client_id = ? WHERE client_id = ? AND agent_client_id IS NULL",
                    (agent_id, client_id),
                )
            else:
                cur.execute(
                    "UPDATE client SET agent_client_id = ? WHERE client_id = ?",
                    (agent_id, client_id),
                )
            if cur.rowcount and cur.rowcount > 0:
                updated += 1
            else:
                already_set += 1
        else:
            updated += 1

    if args.dry_run:
        conn.rollback()
    else:
        conn.commit()

    print("CSV:", args.csv)
    print("dry_run:", bool(args.dry_run))
    print("only_null:", bool(args.only_null))
    print("would_update/updated:", updated)
    if not args.dry_run:
        print("no_change (already set / not matched by WHERE):", already_set)
    print("skipped_no_internalCode:", skipped_no_internal)
    print("skipped_no_agentClientCode:", skipped_no_agent)
    print("missing_client_internalCode_in_db:", missing_client_in_db)
    print("missing_agent_internalCode_in_db:", missing_agent_in_db)

    try:
        cur.close()
        conn.close()
    except Exception:
        pass

    return 0


if __name__ == "__main__":
    raise SystemExit(main())

