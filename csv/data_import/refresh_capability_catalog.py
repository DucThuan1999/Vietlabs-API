#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Runbook: hard-delete catalog/năng lực cũ (con → cha) rồi import lại từ workbook v2 (cha → con).

  python3 refresh_capability_catalog.py --dry-run
  python3 refresh_capability_catalog.py --execute --backup-file /path/to/backup.bak

Bước import gọi các script:
  import_analysis_items_capability_vietlabs_xlsx.py --sheet all
  import_department_capability_vietlabs_xlsx.py --all-branches
  import_subcontractor_capability_ntp_xlsx.py
"""
from __future__ import annotations

import argparse
import csv
import json
import os
import subprocess
import sys
from datetime import datetime, timezone
from pathlib import Path
from typing import Any, Dict, List, Optional, Tuple

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

import import_analysis_item as iai
import capability_workbook_paths as cwp

try:
    import pyodbc
except ImportError:
    print("Cần: pip install pyodbc", file=sys.stderr)
    sys.exit(1)

_SCRIPT_DIR = Path(__file__).resolve().parent
_REPO = _SCRIPT_DIR.parents[2]
_LOG_DIR = _REPO / "data" / "capability_refresh_logs"

# Thứ tự xóa: con → cha (mỗi bước có nhãn và SQL)
DELETE_STEPS: List[Tuple[str, str]] = [
    ("order_template_package_analysis_item", "DELETE FROM order_template_package_analysis_item"),
    ("order_template_item", "DELETE FROM order_template_item"),
    (
        "order_template_analysis_group",
        "IF OBJECT_ID(N'order_template_analysis_group', N'U') IS NOT NULL DELETE FROM order_template_analysis_group",
    ),
    ("order_template_package", "DELETE FROM order_template_package"),
    ("order_template", "DELETE FROM order_template"),
    ("order_sample_package_analysis_item", "DELETE FROM order_sample_package_analysis_item"),
    ("order_sample_item", "DELETE FROM order_sample_item"),
    (
        "order_sample_analysis_group",
        "IF OBJECT_ID(N'order_sample_analysis_group', N'U') IS NOT NULL DELETE FROM order_sample_analysis_group",
    ),
    ("order_sample_package", "DELETE FROM order_sample_package"),
    ("order_sample", "DELETE FROM order_sample"),
    ("quotation_non_nd107_item", "DELETE FROM quotation_non_nd107_item"),
    ("quotation_item", "DELETE FROM quotation_item"),
    (
        "quotation_analysis_group",
        "IF OBJECT_ID(N'quotation_analysis_group', N'U') IS NOT NULL DELETE FROM quotation_analysis_group",
    ),
    ("package_analysis_item", "DELETE FROM package_analysis_item"),
    (
        "package_analysis_group",
        "IF OBJECT_ID(N'package_analysis_group', N'U') IS NOT NULL DELETE FROM package_analysis_group",
    ),
    ("employee_analysis_capability", "DELETE FROM employee_analysis_capability"),
    (
        "department_analysis_capability_designation",
        "DELETE FROM department_analysis_capability_designation",
    ),
    (
        "subcontractor_capability_designation",
        "DELETE FROM subcontractor_capability_designation",
    ),
    (
        "analysis_item_designation",
        "IF OBJECT_ID(N'analysis_item_designation', N'U') IS NOT NULL DELETE FROM analysis_item_designation",
    ),
    ("department_analysis_capability", "DELETE FROM department_analysis_capability"),
    ("subcontractor_capability", "DELETE FROM subcontractor_capability"),
    ("analysis_item_tat", "DELETE FROM analysis_item_tat"),
    ("analysis_item", "DELETE FROM analysis_item"),
    ("package", "DELETE FROM package"),
    ("analysis_group", "DELETE FROM analysis_group"),
    ("sample_matrix", "DELETE FROM sample_matrix"),
    ("sample_matrix_group", "DELETE FROM sample_matrix_group"),
    ("equipment_type", "DELETE FROM equipment_type"),
    ("reference_method", "DELETE FROM reference_method"),
    ("standard", "DELETE FROM standard"),
    ("unit_of_measure", "DELETE FROM unit_of_measure"),
]

COUNT_TABLES = [
    "analysis_item",
    "analysis_item_tat",
    "department_analysis_capability",
    "department_analysis_capability_designation",
    "subcontractor_capability",
    "subcontractor_capability_designation",
    "analysis_group",
    "sample_matrix",
    "sample_matrix_group",
    "equipment_type",
    "reference_method",
    "standard",
    "unit_of_measure",
    "quotation_item",
    "package_analysis_item",
]

ORPHAN_CHECKS = [
    (
        "dac_orphan_analysis_item",
        """SELECT COUNT(*) FROM department_analysis_capability dac
           LEFT JOIN analysis_item ai ON dac.analysis_item_id = ai.analysis_item_id
           WHERE ai.analysis_item_id IS NULL""",
    ),
    (
        "sc_orphan_analysis_item",
        """SELECT COUNT(*) FROM subcontractor_capability sc
           LEFT JOIN analysis_item ai ON sc.analysis_item_id = ai.analysis_item_id
           WHERE ai.analysis_item_id IS NULL""",
    ),
    (
        "tat_orphan_analysis_item",
        """SELECT COUNT(*) FROM analysis_item_tat t
           LEFT JOIN analysis_item ai ON t.analysis_item_id = ai.analysis_item_id
           WHERE ai.analysis_item_id IS NULL""",
    ),
    (
        "dac_des_orphan_dac",
        """SELECT COUNT(*) FROM department_analysis_capability_designation d
           LEFT JOIN department_analysis_capability dac
             ON d.department_analysis_capability_id = dac.department_analysis_capability_id
           WHERE dac.department_analysis_capability_id IS NULL""",
    ),
    (
        "sc_des_orphan_sc",
        """SELECT COUNT(*) FROM subcontractor_capability_designation d
           LEFT JOIN subcontractor_capability sc
             ON d.subcontractor_capability_id = sc.subcontractor_capability_id
           WHERE sc.subcontractor_capability_id IS NULL""",
    ),
]

SCHEMA_MIGRATION_SQL = """
IF COL_LENGTH('dbo.analysis_item', 'display_short_name') IS NULL
BEGIN
    ALTER TABLE dbo.analysis_item ADD display_short_name NVARCHAR(MAX) NULL;
END
"""


def ts() -> str:
    return datetime.now(timezone.utc).strftime("%Y%m%d_%H%M%S")


def table_exists(cur, name: str) -> bool:
    cur.execute(
        "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = ? AND TABLE_TYPE = 'BASE TABLE'",
        name,
    )
    return cur.fetchone() is not None


def count_rows(cur, table: str) -> Optional[int]:
    if not table_exists(cur, table):
        return None
    cur.execute(f"SELECT COUNT(*) FROM [{table}]")
    return int(cur.fetchone()[0])


def audit_counts(cur) -> Dict[str, Optional[int]]:
    return {t: count_rows(cur, t) for t in COUNT_TABLES}


def count_delete_impact(cur, label: str, sql: str) -> Optional[int]:
    if label.startswith("quotation_analysis_group") or label.startswith("package_analysis_group"):
        tbl = label
        if not table_exists(cur, tbl):
            return None
    if label.startswith("analysis_item_designation"):
        if not table_exists(cur, "analysis_item_designation"):
            return None
    # Estimate rows that would be deleted (table full wipe)
    return count_rows(cur, label)


def run_delete_phase(cur, dry_run: bool) -> Dict[str, Optional[int]]:
    results: Dict[str, Optional[int]] = {}
    for label, sql in DELETE_STEPS:
        tbl = label
        if not table_exists(cur, tbl):
            if "IF OBJECT_ID" in sql:
                print(f"  [skip] {label}: bảng không tồn tại")
                results[label] = None
                continue
            print(f"  [skip] {label}: bảng không tồn tại")
            results[label] = None
            continue
        cnt = count_rows(cur, tbl)
        results[label] = cnt
        if dry_run:
            print(f"  [dry-run] DELETE {label}: {cnt} dòng")
        else:
            print(f"  DELETE {label}: {cnt} dòng...")
            cur.execute(sql)
    return results


def ensure_schema(cur, dry_run: bool) -> None:
    cur.execute(
        """SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = 'analysis_item' AND COLUMN_NAME = 'display_short_name'"""
    )
    if cur.fetchone():
        print("  Schema OK: analysis_item.display_short_name đã có")
        return
    print("  Thiếu cột display_short_name — chạy migration SQL...")
    if dry_run:
        print("  [dry-run] Sẽ ALTER TABLE analysis_item ADD display_short_name")
        return
    cur.execute(SCHEMA_MIGRATION_SQL)


def run_orphan_audit(cur) -> Dict[str, int]:
    out: Dict[str, int] = {}
    for name, sql in ORPHAN_CHECKS:
        cur.execute(sql)
        out[name] = int(cur.fetchone()[0])
    return out


def run_import_scripts(xlsx: str, dry_run: bool) -> int:
    py = sys.executable
    scripts = [
        [
            py,
            str(_SCRIPT_DIR / "import_analysis_items_capability_vietlabs_xlsx.py"),
            "--xlsx",
            xlsx,
            "--sheet",
            "all",
        ],
        [
            py,
            str(_SCRIPT_DIR / "import_department_capability_vietlabs_xlsx.py"),
            "--xlsx",
            xlsx,
            "--all-branches",
        ],
    ]
    for ntp_sheet in ("NTP", "NTP bổ sung"):
        scripts.append(
            [
                py,
                str(_SCRIPT_DIR / "import_subcontractor_capability_ntp_xlsx.py"),
                "--xlsx",
                xlsx,
                "--sheet",
                ntp_sheet,
            ]
        )
    if dry_run:
        for cmd in scripts:
            full = cmd + ["--dry-run"]
            print(f"  [dry-run] {' '.join(full)}")
        return 0

    rc = 0
    for cmd in scripts:
        print(f"\n>>> {' '.join(cmd)}")
        proc = subprocess.run(cmd, cwd=str(_SCRIPT_DIR))
        if proc.returncode != 0:
            print(f"Lỗi: script trả về {proc.returncode}")
            rc = proc.returncode
    return rc


def run_workbook_audit(xlsx: str, json_out: Path) -> Dict[str, Any]:
    py = sys.executable
    audit_script = _SCRIPT_DIR / "audit_capability_workbook_v2.py"
    cmd = [py, str(audit_script), "--xlsx", xlsx, "--json-out", str(json_out)]
    print(f">>> {' '.join(cmd)}")
    proc = subprocess.run(cmd, cwd=str(_SCRIPT_DIR), capture_output=True, text=True)
    if proc.stdout:
        print(proc.stdout)
    if proc.stderr:
        print(proc.stderr, file=sys.stderr)
    if proc.returncode != 0:
        return {"error": proc.returncode}
    if json_out.is_file():
        return json.loads(json_out.read_text(encoding="utf-8"))
    return {}


def write_audit_log(path: Path, payload: Dict[str, Any]) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(payload, ensure_ascii=False, indent=2), encoding="utf-8")


def main() -> None:
    parser = argparse.ArgumentParser(description="Refresh toàn bộ catalog/năng lực từ workbook v2")
    parser.add_argument("--xlsx", default=None, help="Workbook v2 (mặc định: data/Danh mục Năng lực v2.xlsx)")
    parser.add_argument(
        "--dry-run",
        action="store_true",
        help="Chỉ audit + in kế hoạch xóa/import, không ghi DB",
    )
    parser.add_argument(
        "--execute",
        action="store_true",
        help="Thực thi xóa + import (mặc định: dry-run nếu không có --execute)",
    )
    parser.add_argument(
        "--skip-delete",
        action="store_true",
        help="Bỏ qua phase xóa (chỉ schema + import)",
    )
    parser.add_argument(
        "--skip-import",
        action="store_true",
        help="Chỉ xóa + audit, không import",
    )
    parser.add_argument(
        "--backup-file",
        default=None,
        help="Đường dẫn file backup .bak (bắt buộc khi --execute trừ khi --skip-backup-check)",
    )
    parser.add_argument(
        "--skip-backup-check",
        action="store_true",
        help="Không yêu cầu --backup-file khi execute",
    )
    parser.add_argument(
        "--connection-string",
        default=None,
        help="Override ODBC connection string",
    )
    args = parser.parse_args()

    execute = args.execute and not args.dry_run
    dry_run = not execute

    xlsx = cwp.resolve_xlsx_arg(args.xlsx)
    if not xlsx or not os.path.isfile(xlsx):
        print(f"Lỗi: không tìm thấy workbook: {args.xlsx or xlsx}")
        sys.exit(1)

    if execute and not args.skip_backup_check and not args.backup_file:
        print("Lỗi: --execute yêu cầu --backup-file hoặc --skip-backup-check")
        sys.exit(1)
    if execute and args.backup_file and not os.path.isfile(args.backup_file):
        print(f"Cảnh báo: backup file không tồn tại: {args.backup_file}")

    _LOG_DIR.mkdir(parents=True, exist_ok=True)
    log_stem = f"refresh_{ts()}"
    audit_json = _LOG_DIR / f"{log_stem}_workbook_audit.json"
    report_json = _LOG_DIR / f"{log_stem}_report.json"

    print("=" * 70)
    print("REFRESH CAPABILITY CATALOG")
    print("=" * 70)
    print(f"  Mode: {'EXECUTE' if execute else 'DRY-RUN'}")
    print(f"  Workbook: {xlsx}")
    print(f"  Log dir: {_LOG_DIR}")

    wb_audit = run_workbook_audit(xlsx, audit_json)
    report: Dict[str, Any] = {
        "timestamp": datetime.now(timezone.utc).isoformat(),
        "mode": "execute" if execute else "dry-run",
        "xlsx": xlsx,
        "workbook_audit": wb_audit,
    }

    conn_str = args.connection_string or iai.CONNECTION_STRING
    try:
        conn = pyodbc.connect(conn_str)
    except pyodbc.Error as e:
        print(f"Lỗi kết nối DB: {e}")
        report["connection_error"] = str(e)
        write_audit_log(report_json, report)
        sys.exit(1)

    conn.autocommit = False
    cur = conn.cursor()

    try:
        print("\n--- Counts TRƯỚC ---")
        before = audit_counts(cur)
        for k, v in before.items():
            print(f"  {k}: {v}")
        report["counts_before"] = before

        print("\n--- Schema ---")
        ensure_schema(cur, dry_run)
        if execute:
            conn.commit()

        if not args.skip_delete:
            print("\n--- Phase DELETE (con → cha) ---")
            delete_plan = run_delete_phase(cur, dry_run)
            report["delete"] = delete_plan
            if execute:
                conn.commit()
                print("  DELETE committed.")

        if not args.skip_import:
            print("\n--- Phase IMPORT (cha → con) ---")
            import_rc = run_import_scripts(xlsx, dry_run)
            report["import_return_code"] = import_rc
            if execute and import_rc != 0:
                raise RuntimeError(f"Import script failed with code {import_rc}")

        print("\n--- Counts SAU ---")
        after = audit_counts(cur)
        for k, v in after.items():
            print(f"  {k}: {v}")
        report["counts_after"] = after

        print("\n--- Orphan FK audit ---")
        orphans = run_orphan_audit(cur)
        for k, v in orphans.items():
            flag = " OK" if v == 0 else " FAIL"
            print(f"  {k}: {v}{flag}")
        report["orphans"] = orphans

        if execute:
            conn.commit()
        elif not dry_run:
            conn.rollback()
    except Exception as e:
        conn.rollback()
        report["error"] = str(e)
        write_audit_log(report_json, report)
        print(f"Lỗi: {e}")
        conn.close()
        sys.exit(1)

    conn.close()
    write_audit_log(report_json, report)
    print(f"\nBáo cáo: {report_json}")

    orphan_total = sum(report.get("orphans", {}).values())
    if orphan_total > 0 and execute:
        print(f"Cảnh báo: còn {orphan_total} orphan FK")
        sys.exit(2)
    print("\nHoàn tất.")


if __name__ == "__main__":
    main()
