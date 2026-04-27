#!/usr/bin/env python3
# -*- coding: utf-8 -*-

"""
Test mapping agentClientCode (agent internalCode) coverage in Client.csv.

Context:
- data/Client.csv uses delimiter ';' (utf-8-sig).
- Column `agentClientCode` contains the agent's internalCode (NOT a UUID).
- For DB, `client.agent_client_id` should store agent's client_id (UUID),
  resolved by joining on agent.internal_code.

Run (from this folder):
  python3 test_agent_mapping.py --csv ../../../data/Client.csv

Or (from repo root):
  python3 Vietlabs-API/csv/data_import/test_agent_mapping.py --csv data/Client.csv
"""

from __future__ import annotations

import argparse
import csv
from collections import Counter
from typing import List, Tuple


def strip_cell(s: str) -> str:
    return (s or "").strip()


def read_csv_rows(path: str) -> Tuple[List[str], List[List[str]]]:
    with open(path, "r", encoding="utf-8-sig", newline="") as f:
        r = csv.reader(f, delimiter=";")
        header = [strip_cell(c).lstrip("\ufeff") for c in next(r)]
        rows = list(r)
    return header, rows


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--csv", default="../../../data/Client.csv", help="Path to Client.csv")
    ap.add_argument(
        "--write-missing",
        default="",
        help="Write missing agent codes (not present as internalCode in the same file) to a text file",
    )
    ap.add_argument(
        "--write-sql",
        default="",
        help="Write helper SQL (fix agent_client_id stored as internalCode string) to a .sql file",
    )
    args = ap.parse_args()

    header, rows = read_csv_rows(args.csv)
    try:
        idx_internal = header.index("internalCode")
    except ValueError as e:
        print(f"Header thiếu cột cần thiết: {e}")
        print("Header hiện có:", header)
        return 2

    # CSV renamed: agentClientId -> agentClientCode (keep backward compatibility)
    agent_header_name = "agentClientCode" if "agentClientCode" in header else "agentClientId"
    if agent_header_name not in header:
        print("Header thiếu cột agentClientCode/agentClientId")
        print("Header hiện có:", header)
        return 2
    idx_agent = header.index(agent_header_name)

    internal_codes = set()
    agent_codes: List[str] = []

    for row in rows:
        if len(row) < len(header):
            row = row + [""] * (len(header) - len(row))

        internal = strip_cell(row[idx_internal])
        if internal:
            internal_codes.add(internal)

        agent = strip_cell(row[idx_agent])
        if agent:
            agent_codes.append(agent)

    c = Counter(agent_codes)
    distinct_agents = sorted(c.keys())
    present = [a for a in distinct_agents if a in internal_codes]
    missing = [a for a in distinct_agents if a not in internal_codes]

    print("CSV:", args.csv)
    print("rows:", len(rows))
    print(f"{agent_header_name} non-empty:", len(agent_codes))
    print(f"distinct {agent_header_name}:", len(distinct_agents))
    print("present as internalCode in same file:", len(present))
    print("missing from internalCode in same file:", len(missing))
    print()
    print(f"Top {agent_header_name} counts:")
    for k, v in c.most_common(20):
        print(f"  {k}: {v}")

    if args.write_missing:
        with open(args.write_missing, "w", encoding="utf-8") as f:
            for code in missing:
                f.write(code + "\n")
        print(f"\nWrote missing agent codes to: {args.write_missing}")

    if args.write_sql:
        sql = """\
-- Fix agent_client_id đang lưu nhầm internalCode (chuỗi).
-- Cập nhật: client.agent_client_id = agent.client_id dựa theo agent.internal_code.
-- (SQL Server)

UPDATE c
SET c.agent_client_id = a.client_id
FROM dbo.client c
JOIN dbo.client a
  ON LTRIM(RTRIM(a.internal_code)) = LTRIM(RTRIM(CAST(c.agent_client_id AS NVARCHAR(100))))
WHERE c.agent_client_id IS NOT NULL
  AND a.client_id IS NOT NULL;
"""
        with open(args.write_sql, "w", encoding="utf-8") as f:
            f.write(sql)
        print(f"\nWrote SQL to: {args.write_sql}")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())

