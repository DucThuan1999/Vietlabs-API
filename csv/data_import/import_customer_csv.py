#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Import Client.csv / Customer.csv → SQL Server: client, contact, client_debt, client_forecast.

Mặc định: [data/Client.csv](data/Client.csv) — dữ liệu bắt đầu ngay sau header (1 dòng header kỹ thuật).

Customer.csv: sau header còn 3 dòng mô tả/template → script tự bỏ qua khi cột 1 dòng đầu trống.

Delimiter ';', utf-8-sig, 51 cột. agentClientId = mã đại lý (internal_code). Trùng internal_code: log + skip.

Usage:
  python import_customer_csv.py [--csv PATH] [--duplicate-log PATH]

  Optional env: VIETLABS_SQL_CONNECTION (ODBC connection string)
  --commit-every N: commit mỗi N dòng (mặc định 200; 0 = một transaction lớn).
  Ctrl+C: cố commit phần đã insert + chạy agent pass2, tránh rollback khi kết nối đứt.
"""

from __future__ import annotations

import argparse
import csv
import itertools
import os
import re
import sys
import unicodedata
import uuid
from datetime import datetime, timezone
from decimal import Decimal, InvalidOperation
from typing import Any, Dict, List, Optional, Set, Tuple

try:
    import pyodbc
except ImportError:
    print("Cần cài: pip install pyodbc")
    sys.exit(1)

try:
    from dateutil import parser as date_parser
except ImportError:
    date_parser = None  # type: ignore


DEFAULT_CSV = os.path.normpath(
    os.path.join(os.path.dirname(__file__), "..", "..", "..", "data", "Client.csv")
)

DEFAULT_CONNECTION = (
    "Driver={ODBC Driver 17 for SQL Server};"
    "Server=192.168.2.21,49938;"
    "Database=VietLabs;"
    "UID=sa;"
    "PWD=Sbt@2024;"
    "TrustServerCertificate=yes;"
    "Encrypt=yes;"
    "Login Timeout=120;"
)

EXPECTED_COLS = 51

# Header sau normalize_header() của [data/Client.csv](data/Client.csv) — đối chiếu khi đổi file nguồn.
EXPECTED_CLIENT_CSV_HEADERS: Tuple[str, ...] = (
    "companyName",
    "companyNameEn",
    "internalCode",
    "taxCode",
    "representativeName",
    "representativeEmail",
    "representativePhone",
    "representativeTitle",
    "bankName",
    "bankAccountNumber",
    "bankAccountName",
    "address",
    "ward",
    "province",
    "country",
    "clientIndustryId",
    "profession",
    "scale",
    "customerType",
    "discountRate",
    "commissionRate",
    "Column1",
    "salesOwnerName",
    "salesOwnerEmail",
    "salesOwnerPhone",
    "csoOwnerName",
    "csoOwnerEmail",
    "csoOwnerPhone",
    # Renamed in CSV: agentClientId -> agentClientCode (still means: agent internal_code)
    "agentClientCode",
    "isBlacklisted",
    "blacklistReason",
    "notes",
    "issueInvoice",
    "contactFullName",
    "contactEmail",
    "contactPhone",
    "contactDepartment",
    "contactTitle",
    "contactIsPrimary",
    "contactIsSampleSender",
    "contactIsResultReceiver",
    "contactIsPayer",
    "contactNotes",
    "Hình thức thanh toán(Thu tiền khi gửi mẫu/ Thu tiền trả kết quả/ Công nợ)",
    "Thời hạn Công nợ (ngày)",
    "Hạn mức dư nợ",
    "Tình trạng hợp đồng hiệu lực ngày",
    "Tình trạng hợp đồng kết thúc ngày",
    "Forecast Từ ngày",
    "Forecast Đến ngày",
    "Forecast",
)


def warn_if_header_differs_from_client_template(header: List[str]) -> None:
    """In cảnh báo nếu tên cột (sau normalize) không khớp template Client.csv."""
    if len(header) != len(EXPECTED_CLIENT_CSV_HEADERS):
        return
    mismatches: List[Tuple[int, str, str]] = []
    for i, (exp, got) in enumerate(zip(EXPECTED_CLIENT_CSV_HEADERS, header)):
        if exp != got:
            mismatches.append((i + 1, exp, got))
    if not mismatches:
        return
    print(
        f"Cảnh báo: {len(mismatches)} cột header lệch so với template Client.csv (kiểm tra file nguồn):",
        flush=True,
    )
    for idx, exp, got in mismatches[:20]:
        print(f"  cột {idx}: kỳ vọng {exp!r} ; thực tế {got!r}", flush=True)
    if len(mismatches) > 20:
        print(f"  ... và {len(mismatches) - 20} cột khác", flush=True)


def connection_string() -> str:
    return os.environ.get("VIETLABS_SQL_CONNECTION", DEFAULT_CONNECTION)


def connect_sql_server(conn_s: str):
    """
    pyodbc.connect với timeout dài. Nếu vẫn HYT00 Login timeout (hay gặp trên macOS)
    dù sqlcmd vẫn đăng nhập được, thử lại với Encrypt=optional.
    """
    last_exc: Optional[Exception] = None
    candidates = [conn_s]
    patched = re.sub(
        r"Encrypt\s*=\s*yes\b", "Encrypt=optional", conn_s, count=1, flags=re.I
    )
    if patched != conn_s:
        candidates.append(patched)
    for cs in candidates:
        try:
            return pyodbc.connect(cs, timeout=120)
        except pyodbc.Error as e:
            last_exc = e
    assert last_exc is not None
    raise last_exc


def table_columns(cursor, table: str) -> Set[str]:
    """Ưu tiên schema dbo (SQL Server); nếu không có bảng dbo thì lấy mọi schema (fallback)."""
    cursor.execute(
        """
        SELECT LOWER(c.COLUMN_NAME)
        FROM INFORMATION_SCHEMA.COLUMNS c
        INNER JOIN INFORMATION_SCHEMA.TABLES t
          ON c.TABLE_SCHEMA = t.TABLE_SCHEMA AND c.TABLE_NAME = t.TABLE_NAME
        WHERE t.TABLE_SCHEMA = N'dbo'
          AND t.TABLE_NAME = ?
          AND t.TABLE_TYPE = N'BASE TABLE'
        """,
        (table,),
    )
    cols = {str(r[0]).lower() for r in cursor.fetchall()}
    if cols:
        return cols
    cursor.execute(
        """
        SELECT LOWER(c.COLUMN_NAME)
        FROM INFORMATION_SCHEMA.COLUMNS c
        INNER JOIN INFORMATION_SCHEMA.TABLES t
          ON c.TABLE_SCHEMA = t.TABLE_SCHEMA AND c.TABLE_NAME = t.TABLE_NAME
        WHERE t.TABLE_NAME = ?
          AND t.TABLE_TYPE = N'BASE TABLE'
        """,
        (table,),
    )
    return {str(r[0]).lower() for r in cursor.fetchall()}


def strip_cell(s: Any) -> str:
    if s is None:
        return ""
    return str(s).replace("\r\n", "\n").strip()


def split_contact_emails(raw: str) -> List[str]:
    """Một ô có thể chứa nhiều email (CSV dùng ; hoặc ,) — tách và loại trùng không phân biệt hoa thường."""
    if not strip_cell(raw):
        return []
    seen: Set[str] = set()
    out: List[str] = []
    for part in re.split(r"[;,]+", raw):
        e = strip_cell(part)
        if not e:
            continue
        k = e.casefold()
        if k in seen:
            continue
        seen.add(k)
        out.append(e)
    return out


def parse_percent_decimal(s: str) -> Optional[Decimal]:
    s = strip_cell(s)
    if not s:
        return None
    s = s.replace("%", "").replace(",", ".").strip()
    try:
        return Decimal(s)
    except InvalidOperation:
        return None


def parse_bool(s: str) -> bool:
    t = strip_cell(s).lower()
    if t in ("true", "1", "yes", "có", "x"):
        return True
    return False


def parse_int_safe(s: str) -> Optional[int]:
    s = strip_cell(s)
    if not s:
        return None
    try:
        return int(float(s.replace(",", ".")))
    except ValueError:
        return None


def parse_decimal_safe(s: str) -> Optional[Decimal]:
    s = strip_cell(s)
    if not s:
        return None
    s = s.replace(",", ".")
    try:
        return Decimal(s)
    except InvalidOperation:
        return None


def parse_date(s: str) -> Optional[datetime]:
    s = strip_cell(s)
    if not s:
        return None
    if date_parser:
        try:
            dt = date_parser.parse(s, dayfirst=True, yearfirst=False)
            return dt
        except (ValueError, TypeError, OverflowError):
            pass
    for fmt in ("%d-%b-%y", "%d-%b-%Y", "%d/%m/%Y", "%d-%m-%Y", "%Y-%m-%d"):
        try:
            return datetime.strptime(s, fmt)
        except ValueError:
            continue
    return None


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


def is_row_empty(d: Dict[str, str]) -> bool:
    if strip_cell(d.get("companyName", "")) or strip_cell(d.get("internalCode", "")):
        return False
    return not any(strip_cell(v) for k, v in d.items() if not k.startswith("__"))


def agent_code_from_row(d: Dict[str, str]) -> str:
    return strip_cell(d.get("agentClientId") or d.get("agentClientCode") or "")


def normalize_employee_name(text: str) -> str:
    """Giống import_employee.normalize_name: so khớp UPPER + gộp khoảng trắng."""
    if not text:
        return ""
    return " ".join(text.strip().split()).upper()


def load_employee_name_lookup(cursor) -> Dict[str, Tuple[str, Optional[str], Optional[str]]]:
    """
    full_name (chuẩn hóa) -> (full_name trong DB, email, mobile).
    Trùng tên sau chuẩn hóa: giữ bản ghi đầu tiên (theo thứ tự truy vấn).
    """
    cursor.execute(
        """
        SELECT full_name, email, mobile
        FROM employee
        WHERE (status = N'Active' OR status IS NULL)
          AND full_name IS NOT NULL
          AND LTRIM(RTRIM(CAST(full_name AS NVARCHAR(MAX)))) <> N''
        """
    )
    m: Dict[str, Tuple[str, Optional[str], Optional[str]]] = {}
    for fn, em, mob in cursor.fetchall():
        key = normalize_employee_name(strip_cell(fn))
        if not key or key in m:
            continue
        m[key] = (
            strip_cell(fn),
            strip_cell(em) or None,
            strip_cell(mob) or None,
        )
    return m


def resolve_owner_from_employee(
    d: Dict[str, str],
    name_key: str,
    email_key: str,
    phone_key: str,
    emp_lookup: Optional[Dict[str, Tuple[str, Optional[str], Optional[str]]]],
) -> Tuple[Optional[str], Optional[str], Optional[str]]:
    """
    Tên NV Sale/CS trong CSV: tra employee theo full_name (UPPER).
    Khớp -> dùng full_name chuẩn từ DB; email/SĐT ưu tiên CSV, không có thì lấy từ employee.
    """
    raw_name = strip_cell(d.get(name_key, ""))
    if not raw_name:
        return None, nv(d, email_key), nv(d, phone_key)
    em = nv(d, email_key)
    ph = nv(d, phone_key)
    if emp_lookup:
        k = normalize_employee_name(raw_name)
        if k and k in emp_lookup:
            dbn, dbe, dbm = emp_lookup[k]
            return (dbn, em or dbe, ph or dbm)
    return (raw_name, em, ph)


def build_client_payload(
    d: Dict[str, str],
    agent_client_id_sql: Optional[str],
    client_columns: Set[str],
    default_country_vi: str,
    emp_lookup: Optional[Dict[str, Tuple[str, Optional[str], Optional[str]]]] = None,
) -> Dict[str, Any]:
    """Map CSV dict → DB column name → value. Omitted keys = not in table."""
    discount = parse_percent_decimal(d.get("discountRate", ""))
    commission = parse_percent_decimal(d.get("commissionRate", ""))
    if commission is None:
        commission = Decimal("0")

    cid = str(uuid.uuid4()).upper()
    now = datetime.now(timezone.utc).replace(tzinfo=None)

    raw_industry = strip_cell(d.get("clientIndustryId", ""))
    client_industry_id = None
    if raw_industry:
        try:
            client_industry_id = str(uuid.UUID(raw_industry))
        except ValueError:
            client_industry_id = None

    internal_code = strip_cell(d.get("internalCode", ""))

    csv_country = strip_cell(d.get("country", ""))
    country_val: Optional[str] = csv_country if csv_country else (default_country_vi or None)

    sn, se, sp = resolve_owner_from_employee(
        d, "salesOwnerName", "salesOwnerEmail", "salesOwnerPhone", emp_lookup
    )
    cn, ce, cp = resolve_owner_from_employee(
        d, "csoOwnerName", "csoOwnerEmail", "csoOwnerPhone", emp_lookup
    )

    # Người đại diện chỉ map vào client.representative_*; nếu CSV không có cột representative* thì backfill từ contact.
    # Nếu CSV không có cột representative* nhưng có khối liên hệ → điền representative từ contact (một email đầu nếu multi).
    rep_name = nv(d, "representativeName")
    rep_email = nv(d, "representativeEmail")
    rep_phone = nv(d, "representativePhone")
    rep_title = nv(d, "representativeTitle")
    if not (rep_name or rep_email or rep_phone or rep_title):
        c_emails = split_contact_emails(d.get("contactEmail", ""))
        c_email = c_emails[0] if c_emails else None
        c_name = nv(d, "contactFullName")
        c_phone = nv(d, "contactPhone")
        c_title = nv(d, "contactTitle")
        if c_name or c_email or c_phone or c_title:
            rep_name = rep_name or c_name
            rep_email = rep_email or c_email
            rep_phone = rep_phone or c_phone
            rep_title = rep_title or c_title

    vals: Dict[str, Any] = {
        "client_id": cid,
        "company_name": strip_cell(d.get("companyName", "")) or "",
        "created_date": now,
        "status": "Active",
        "is_blacklisted": parse_bool(d.get("isBlacklisted", "")),
    }
    if "commission_rate" in client_columns:
        vals["commission_rate"] = commission

    optional_str = {
        "company_name_en": strip_cell(d.get("companyNameEn", "")) or None,
        "internal_code": internal_code or None,
        "tax_code": nv(d, "taxCode"),
        "bank_name": nv(d, "bankName"),
        "bank_account_number": nv(d, "bankAccountNumber"),
        "bank_account_name": nv(d, "bankAccountName"),
        "address": nv(d, "address"),
        "ward": nv(d, "ward"),
        "province": nv(d, "province"),
        "country": country_val,
        "profession": nv(d, "profession"),
        "scale": nv(d, "scale"),
        "customer_type": normalize_customer_type(nv(d, "customerType")),
        "representative_name": rep_name,
        "representative_email": rep_email,
        "representative_phone": rep_phone,
        "representative_title": rep_title,
        "sales_owner_name": sn,
        "sales_owner_email": se,
        "sales_owner_phone": sp,
        "cso_owner_name": cn,
        "cso_owner_email": ce,
        "cso_owner_phone": cp,
        "blacklist_reason": nv(d, "blacklistReason"),
        "notes": nv(d, "notes"),
        "issue_invoice": nv(d, "issueInvoice"),
    }
    for col, v in optional_str.items():
        if col in client_columns:
            vals[col] = v

    if "city" in client_columns and "province" not in client_columns:
        vals["city"] = optional_str.get("province")

    if "discount_rate" in client_columns:
        vals["discount_rate"] = discount if discount is not None else Decimal("0")

    if "client_industry_id" in client_columns:
        vals["client_industry_id"] = client_industry_id

    if agent_client_id_sql and "agent_client_id" in client_columns:
        vals["agent_client_id"] = agent_client_id_sql

    return {k: v for k, v in vals.items() if k in client_columns}


def nv(d: Dict[str, str], key: str) -> Optional[str]:
    s = strip_cell(d.get(key, ""))
    return s or None


# Trùng khớp CustomerManagementDetail.jsx (Radio.Group): filter UI dùng so sánh chuỗi tuyệt đối.
_CUSTOMER_TYPE_CANONICAL = (
    "Cá nhân",
    "Doanh nghiệp",
    "Nhà nước",
    "Đại lý",
    "CTV",
)
_CUSTOMER_TYPE_BY_CASEFOLD = {
    unicodedata.normalize("NFC", t).casefold(): t for t in _CUSTOMER_TYPE_CANONICAL
}


def normalize_customer_type(raw: Optional[str]) -> Optional[str]:
    """Chuẩn hóa loại KH: NFC + trim, map biến thể hoa/thường → chuỗi canonical trên frontend."""
    if not raw:
        return None
    s = unicodedata.normalize("NFC", str(raw).strip())
    if not s:
        return None
    canon = _CUSTOMER_TYPE_BY_CASEFOLD.get(s.casefold())
    return canon if canon is not None else s


# Trùng khớp CustomerManagementDetail.jsx (PaymentMethod Select).
_PAYMENT_METHOD_CANONICAL = (
    "Thu tiền khi gửi mẫu",
    "Thu tiền trả kết quả",
    "Công nợ",
)
_PAYMENT_METHOD_BY_CASEFOLD = {
    " ".join(unicodedata.normalize("NFC", t).split()).casefold(): t for t in _PAYMENT_METHOD_CANONICAL
}


def normalize_payment_method(raw: Optional[str]) -> Optional[str]:
    """Chuẩn hóa hình thức thanh toán: NFC, gom khoảng trắng, map hoa/thường → giá trị Select trên UI."""
    if not raw:
        return None
    s = unicodedata.normalize("NFC", str(raw).strip())
    if not s:
        return None
    collapsed = " ".join(s.split())
    canon = _PAYMENT_METHOD_BY_CASEFOLD.get(collapsed.casefold())
    return canon if canon is not None else collapsed


def sql_insert(
    table: str, payload: Dict[str, Any], cursor
) -> Tuple[str, List[Any]]:
    cols = [c for c, v in payload.items() if v is not None]
    placeholders = ", ".join(["?"] * len(cols))
    colsql = ", ".join(cols)
    sql = f"INSERT INTO {table} ({colsql}) VALUES ({placeholders})"
    params = [payload[c] for c in cols]
    return sql, params


def load_existing_internal_codes(cursor) -> Dict[str, str]:
    cursor.execute(
        "SELECT internal_code, CAST(client_id AS NVARCHAR(36)) FROM client WHERE internal_code IS NOT NULL"
    )
    m: Dict[str, str] = {}
    for code, cid in cursor.fetchall():
        c = strip_cell(code)
        if c:
            m[c] = strip_cell(cid)
    return m


def load_default_country_name_vi(cursor) -> str:
    """
    Tên hiển thị quốc gia mặc định (Việt Nam) từ bảng country (alpha_2 = VN).
    Không có bản ghi → 'Nước Việt Nam' như yêu cầu nghiệp vụ.
    """
    cursor.execute(
        """
        SELECT TOP 1 full_name_vi
        FROM country
        WHERE UPPER(LTRIM(RTRIM(alpha_2))) = N'VN'
          AND (status = N'Active' OR status IS NULL)
        ORDER BY sequence_number ASC
        """
    )
    row = cursor.fetchone()
    if row and row[0]:
        return strip_cell(row[0])
    return "Nước Việt Nam"


def main() -> int:
    ap = argparse.ArgumentParser(description="Import Client.csv / Customer.csv into VietLabs SQL Server")
    ap.add_argument("--csv", default=DEFAULT_CSV, help="Path tới Client.csv hoặc Customer.csv")
    ap.add_argument(
        "--duplicate-log",
        default="",
        help="CSV path for duplicate internal_code rows (default: next to input)",
    )
    ap.add_argument(
        "--dry-run",
        action="store_true",
        help="Chỉ đọc CSV, thống kê và log trùng; không kết nối database",
    )
    ap.add_argument(
        "--commit-every",
        type=int,
        default=200,
        metavar="N",
        help="Commit sau mỗi N khách (0 = chỉ commit một lần cuối). Giảm mất dữ liệu khi mạng lỗi hoặc dừng giữa chừng.",
    )
    args = ap.parse_args()
    csv_path = os.path.abspath(args.csv)
    if not os.path.isfile(csv_path):
        print(f"Không tìm thấy file: {csv_path}")
        return 1

    dup_log = args.duplicate_log
    if not dup_log:
        base = os.path.splitext(os.path.basename(csv_path))[0]
        ts = datetime.now().strftime("%Y%m%d_%H%M%S")
        dup_log = os.path.join(os.path.dirname(csv_path), f"{base}_import_duplicates_{ts}.csv")

    conn_s = connection_string()
    skipped_empty = 0
    skipped_no_company = 0
    parsed_rows: List[Tuple[int, Dict[str, str]]] = []

    with open(csv_path, "r", encoding="utf-8-sig", newline="") as f:
        reader = csv.reader(f, delimiter=";")
        try:
            header_row = next(reader)
        except StopIteration:
            print("File rỗng")
            return 1
        header = [normalize_header(h) for h in header_row]
        if len(header) != EXPECTED_COLS:
            print(f"Cảnh báo: header có {len(header)} cột, kỳ vọng {EXPECTED_COLS}")
        else:
            warn_if_header_differs_from_client_template(header)

        try:
            first_after_header = next(reader)
        except StopIteration:
            print("File chỉ có header, không có dòng dữ liệu")
            return 1

        if strip_cell(first_after_header[0] if first_after_header else ""):
            # Client.csv: dòng đầu sau header là khách hàng
            row_iter = itertools.chain([first_after_header], reader)
            line_no = 1
        else:
            # Customer.csv: bỏ thêm 2 dòng (cùng 3 dòng sau header trước dữ liệu)
            for _ in range(2):
                try:
                    next(reader)
                except StopIteration:
                    print("File thiếu dòng dữ liệu sau phần mô tả")
                    return 1
            row_iter = reader
            line_no = 4

        for row in row_iter:
            line_no += 1
            if len(row) != EXPECTED_COLS:
                if len(row) < EXPECTED_COLS:
                    row = row + [""] * (EXPECTED_COLS - len(row))
                else:
                    row = row[:EXPECTED_COLS]

            d = enrich_row_with_indexed_fields(header, row)
            if is_row_empty(d):
                skipped_empty += 1
                continue
            company = strip_cell(d.get("companyName", ""))
            if not company:
                skipped_no_company += 1
                continue
            parsed_rows.append((line_no, d))

    code_to_id_db: Dict[str, str] = {}
    conn = None
    cursor = None
    if args.dry_run:
        print("[dry-run] không kiểm tra trùng internal_code trên database")
    else:
        try:
            conn = connect_sql_server(conn_s)
        except pyodbc.Error as e:
            print("Kết nối SQL Server thất bại:", e, file=sys.stderr)
            print(
                "Gợi ý: bật VPN / mạng tới máy chủ, kiểm tra Server và cổng trong chuỗi kết nối, "
                "firewall cho phép SQL.",
                file=sys.stderr,
            )
            print(
                "Nếu sqlcmd vẫn vào được nhưng pyodbc báo Login timeout: "
                "đặt VIETLABS_SQL_CONNECTION với Encrypt=optional (script đã thử tự động nếu có Encrypt=yes).",
                file=sys.stderr,
            )
            if not os.environ.get("VIETLABS_SQL_CONNECTION"):
                print(
                    "Bạn đang dùng chuỗi mặc định trong script. "
                    "Đặt chuỗi đúng môi trường: export VIETLABS_SQL_CONNECTION='Driver=...;Server=...;...'",
                    file=sys.stderr,
                )
            return 1
        conn.autocommit = False
        cursor = conn.cursor()
        code_to_id_db = load_existing_internal_codes(cursor)

    seen_batch_codes: Set[str] = set()
    rows_buffer: List[Tuple[int, Dict[str, str], str]] = []
    skipped_dup = 0
    for line_no, d in parsed_rows:
        internal_code = strip_cell(d.get("internalCode", ""))
        if internal_code:
            if internal_code in code_to_id_db:
                skipped_dup += 1
                rows_buffer.append((line_no, d, "already_in_database"))
                continue
            if internal_code in seen_batch_codes:
                skipped_dup += 1
                rows_buffer.append((line_no, d, "duplicate_in_file"))
                continue
            seen_batch_codes.add(internal_code)

        rows_buffer.append((line_no, d, ""))

    # Write duplicate log
    dup_rows = [(ln, dr, reason) for ln, dr, reason in rows_buffer if reason]
    if dup_rows:
        with open(dup_log, "w", encoding="utf-8", newline="") as lf:
            w = csv.writer(lf, delimiter=";")
            w.writerow(["line", "reason", "internalCode", "companyName", "agentClientCode"])
            for ln, dr, reason in dup_rows:
                w.writerow(
                    [
                        ln,
                        reason,
                        strip_cell(dr.get("internalCode", "")),
                        strip_cell(dr.get("companyName", "")),
                        agent_code_from_row(dr),
                    ]
                )
        print(f"Đã ghi log trùng internal_code: {dup_log} ({len(dup_rows)} dòng)")

    to_import = [(ln, dr) for ln, dr, reason in rows_buffer if not reason]

    if args.dry_run:
        with_contact_clients = sum(1 for _, d in to_import if contact_has_data(d))
        contact_rows = sum(count_contact_inserts_for_row(d) for _, d in to_import)
        with_debt = sum(1 for _, d in to_import if debt_has_data_from_indices(d))
        with_fc = sum(1 for _, d in to_import if forecast_has_data(d))
        print(
            f"[dry-run] import dự kiến: {len(to_import)} client, "
            f"{with_contact_clients} client có ít nhất một contact, "
            f"{contact_rows} dòng contact (sau tách đa email), "
            f"{with_debt} có client_debt, {with_fc} có forecast; "
            f"skip trùng {skipped_dup}, rỗng {skipped_empty}, thiếu tên {skipped_no_company}"
        )
        return 0

    assert conn is not None and cursor is not None

    client_cols = table_columns(cursor, "client")
    contact_cols = table_columns(cursor, "contact")
    debt_cols = table_columns(cursor, "client_debt")
    forecast_cols = table_columns(cursor, "client_forecast")
    default_country_vi = load_default_country_name_vi(cursor)
    emp_lookup = load_employee_name_lookup(cursor)

    for col in ("customer_type", "profession", "scale"):
        if col not in client_cols:
            print(
                f"Cảnh báo: bảng dbo.client không có cột «{col}» — "
                "dữ liệu CSV tương ứng sẽ không được INSERT (cần migration DB).",
                flush=True,
            )

    inserted_clients = 0
    inserted_contacts = 0
    inserted_debts = 0
    inserted_forecasts = 0
    agent_warn: List[str] = []

    commit_every = max(0, int(args.commit_every))
    merged = 0

    try:
        for line_no, d in to_import:
            internal_code = strip_cell(d.get("internalCode", ""))
            agent_code = agent_code_from_row(d)

            agent_guid: Optional[str] = None
            if agent_code:
                agent_guid = code_to_id_db.get(agent_code)
                if not agent_guid:
                    agent_warn.append(
                        f"line {line_no}: agentClientCode '{agent_code}' chưa có trong DB — "
                        "để trống FK tạm thời, pass 2 sẽ gán nếu mã xuất hiện trong batch"
                    )

            payload = build_client_payload(
                d, agent_guid, client_cols, default_country_vi, emp_lookup
            )
            sql, params = sql_insert("client", payload, cursor)
            try:
                cursor.execute(sql, params)
            except Exception as e:
                try:
                    conn.rollback()
                except Exception:
                    pass
                print(f"Lỗi INSERT client dòng {line_no}: {e}")
                raise

            new_id = payload["client_id"]
            if internal_code:
                code_to_id_db[internal_code] = new_id

            inserted_clients += 1

            if contact_has_data(d):
                inserted_contacts += ins_contacts_for_row(
                    cursor,
                    new_id,
                    d,
                    contact_cols,
                    line_no=line_no,
                )

            if debt_has_data_from_indices(d):
                merge_client_debt(
                    cursor,
                    new_id,
                    d,
                    debt_cols,
                )
                inserted_debts += 1

            if forecast_has_data(d) and ins_forecast(cursor, new_id, d, forecast_cols):
                inserted_forecasts += 1

            if commit_every > 0 and inserted_clients % commit_every == 0:
                conn.commit()
                print(f"  ... đã commit batch (~{inserted_clients} client)", flush=True)

        merged = merge_agent_fk_second_pass(
            cursor, to_import, code_to_id_db, client_cols
        )
        conn.commit()

        print(
            f"Xong: client +{inserted_clients}, contact +{inserted_contacts}, "
            f"client_debt upsert {inserted_debts}, client_forecast +{inserted_forecasts}, "
            f"agent FK pass2 cập nhật {merged}, skip trùng {skipped_dup}, skip rỗng {skipped_empty}, "
            f"skip thiếu tên {skipped_no_company}"
        )
        if agent_warn:
            print("Cảnh báo agent (một phần đã xử lý ở pass 2):")
            for w in agent_warn[:20]:
                print(" ", w)
            if len(agent_warn) > 20:
                print(f"  ... và {len(agent_warn) - 20} cảnh báo khác")
    except KeyboardInterrupt:
        print("\nCtrl+C: đang commit phần đã xử lý (không rollback để tránh lỗi kết nối)...", flush=True)
        try:
            merged = merge_agent_fk_second_pass(
                cursor, to_import, code_to_id_db, client_cols
            )
            conn.commit()
            print(
                f"Đã lưu một phần: ~{inserted_clients} client, agent FK pass2={merged}. "
                "Chạy lại import sẽ bỏ qua internal_code đã có trong DB.",
                flush=True,
            )
        except Exception as ex:
            print(f"Không commit được sau khi dừng: {ex}", flush=True)
        return 130
    except Exception:
        try:
            if conn:
                conn.rollback()
        except Exception:
            pass
        raise
    finally:
        try:
            if conn:
                conn.close()
        except Exception:
            pass
    return 0


def contact_has_data(d: Dict[str, str]) -> bool:
    """Chỉ khối cột 34–43 (người liên hệ). Người đại diện (5–8) → bảng client.representative_*."""
    keys = (
        "contactFullName",
        "contactEmail",
        "contactPhone",
        "contactDepartment",
        "contactTitle",
        "contactIsPrimary",
        "contactIsSampleSender",
        "contactIsResultReceiver",
        "contactIsPayer",
        "contactNotes",
    )
    for k in keys:
        if k.startswith("contactIs"):
            if parse_bool(d.get(k, "")):
                return True
        elif strip_cell(d.get(k, "")):
            return True
    return False


def count_contact_inserts_for_row(d: Dict[str, str]) -> int:
    """Số bản ghi contact sẽ INSERT cho một dòng CSV (tách nhiều email)."""
    if not contact_has_data(d):
        return 0
    emails = split_contact_emails(d.get("contactEmail", ""))
    return len(emails) if len(emails) > 1 else 1


def ins_contacts_for_row(
    cursor,
    client_id: str,
    d: Dict[str, str],
    contact_cols: Set[str],
    *,
    line_no: int = 0,
) -> int:
    """Một client có thể có nhiều Contact nếu cột email chứa nhiều địa chỉ."""
    emails = split_contact_emails(d.get("contactEmail", ""))
    if len(emails) > 1:
        for em in emails:
            ins_contact(
                cursor,
                client_id,
                {**d, "contactEmail": em},
                contact_cols,
                line_no=line_no,
            )
        return len(emails)
    if len(emails) == 1:
        ins_contact(
            cursor,
            client_id,
            {**d, "contactEmail": emails[0]},
            contact_cols,
            line_no=line_no,
        )
        return 1
    ins_contact(cursor, client_id, d, contact_cols, line_no=line_no)
    return 1


def ins_contact(
    cursor,
    client_id: str,
    d: Dict[str, str],
    contact_cols: Set[str],
    *,
    line_no: int = 0,
) -> None:
    fn = strip_cell(d.get("contactFullName", ""))
    if not fn:
        fn = (
            strip_cell(d.get("contactEmail", ""))
            or strip_cell(d.get("contactPhone", ""))
            or "-"
        )

    cid_u = uuid.UUID(str(client_id).strip())
    new_contact_id = uuid.uuid4()

    row: Dict[str, Any] = {
        "contact_id": new_contact_id,
        "client_id": cid_u,
        "full_name": fn,
        "is_primary": parse_bool(d.get("contactIsPrimary", "")),
        "is_sample_sender": parse_bool(d.get("contactIsSampleSender", "")),
        "is_result_receiver": parse_bool(d.get("contactIsResultReceiver", "")),
        "is_payer": parse_bool(d.get("contactIsPayer", "")),
    }
    opt = {
        "email": nv(d, "contactEmail"),
        "phone": nv(d, "contactPhone"),
        "department": nv(d, "contactDepartment"),
        "title": nv(d, "contactTitle"),
        "notes": nv(d, "contactNotes"),
    }
    for k, v in opt.items():
        if k in contact_cols:
            row[k] = v
    if "created_at" in contact_cols:
        row["created_at"] = datetime.now(timezone.utc).replace(tzinfo=None)

    for bit_col in (
        "is_primary",
        "is_sample_sender",
        "is_result_receiver",
        "is_payer",
    ):
        if bit_col in contact_cols and bit_col not in row:
            row[bit_col] = False

    row = {k: v for k, v in row.items() if k in contact_cols}
    row = {k: v for k, v in row.items() if v is not None}

    sql, params = sql_insert("contact", row, cursor)
    try:
        cursor.execute(sql, params)
    except Exception as e:
        raise RuntimeError(f"Lỗi INSERT contact (CSV khoảng dòng {line_no}): {e}") from e


def debt_has_data_from_indices(d: Dict[str, str]) -> bool:
    """Columns 44–48 mapped by header index — use raw dict keys from plan."""
    c44 = strip_cell(d.get("__debt_payment", ""))
    c45 = strip_cell(d.get("__debt_term", ""))
    c46 = strip_cell(d.get("__debt_limit", ""))
    c47 = strip_cell(d.get("__contract_start", ""))
    c48 = strip_cell(d.get("__contract_end", ""))
    if c44 or c45 or c46 or c47 or c48:
        return True
    return False


# Cột 17–19 (1-based): profession, scale, customerType — template Customer.csv dòng 1.
CLIENT_CATALOG_FIELDS_1BASE: List[Tuple[int, str]] = [
    (17, "profession"),
    (18, "scale"),
    (19, "customerType"),
]

# Customer.csv: cột 34–43 (1-based) = contact; luôn lấy theo vị trí để không lệch khi header có nhiều ô trùng tên rỗng.
CONTACT_CSV_COLUMNS_1BASE: List[Tuple[int, str]] = [
    (34, "contactFullName"),
    (35, "contactEmail"),
    (36, "contactPhone"),
    (37, "contactDepartment"),
    (38, "contactTitle"),
    (39, "contactIsPrimary"),
    (40, "contactIsSampleSender"),
    (41, "contactIsResultReceiver"),
    (42, "contactIsPayer"),
    (43, "contactNotes"),
]


def enrich_row_with_indexed_fields(header: List[str], row: List[str]) -> Dict[str, str]:
    d = row_to_dict(header, row)
    for col1, key in CLIENT_CATALOG_FIELDS_1BASE:
        idx0 = col1 - 1
        if idx0 < len(row):
            d[key] = row[idx0] if row[idx0] is not None else ""
    for col1, key in CONTACT_CSV_COLUMNS_1BASE:
        idx0 = col1 - 1
        if idx0 < len(row):
            d[key] = row[idx0] if row[idx0] is not None else ""
    indices_extra = {44: "__debt_payment", 45: "__debt_term", 46: "__debt_limit", 47: "__contract_start", 48: "__contract_end", 49: "__fc_from", 50: "__fc_to", 51: "__fc_amount"}
    for idx1, key in indices_extra.items():
        idx0 = idx1 - 1
        if idx0 < len(row):
            d[key] = row[idx0]
    return d


def merge_client_debt(
    cursor,
    client_id: str,
    d: Dict[str, str],
    debt_cols: Set[str],
) -> None:
    pay = normalize_payment_method(nv(d, "__debt_payment"))
    term = parse_int_safe(d.get("__debt_term", "")) or 0
    credit = parse_decimal_safe(d.get("__debt_limit", "")) or Decimal("0")
    eff = parse_date(d.get("__contract_start", ""))
    end = parse_date(d.get("__contract_end", ""))

    payload: Dict[str, Any] = {
        "client_debt_id": str(uuid.uuid4()).upper(),
        "client_id": client_id,
        "payment_method": pay,
        "total_debt": Decimal("0"),
        "debt_term_days": term,
        "credit_limit": credit,
        "contract_effective_date": eff,
        "contract_end_date": end,
        "created_at": datetime.now(timezone.utc).replace(tzinfo=None),
    }
    payload = {k: v for k, v in payload.items() if k in debt_cols}

    cursor.execute("SELECT client_debt_id FROM client_debt WHERE client_id = ?", (client_id,))
    ex = cursor.fetchone()
    if ex:
        sets = [f"{k} = ?" for k in payload if k not in ("client_debt_id", "client_id", "created_at")]
        if not sets:
            return
        vals = [payload[k] for k in payload if k not in ("client_debt_id", "client_id", "created_at")]
        sql = f"UPDATE client_debt SET {', '.join(sets)} WHERE client_id = ?"
        cursor.execute(sql, vals + [client_id])
    else:
        sql, pr = sql_insert("client_debt", payload, cursor)
        cursor.execute(sql, pr)


def forecast_has_data(d: Dict[str, str]) -> bool:
    return any(
        strip_cell(d.get(k, ""))
        for k in ("__fc_from", "__fc_to", "__fc_amount")
    )


def ins_forecast(
    cursor,
    client_id: str,
    d: Dict[str, str],
    forecast_cols: Set[str],
) -> bool:
    fd = parse_date(d.get("__fc_from", ""))
    td = parse_date(d.get("__fc_to", ""))
    amt = parse_decimal_safe(d.get("__fc_amount", "")) or Decimal("0")
    if not fd or not td:
        return False
    payload: Dict[str, Any] = {
        "client_forecast_id": str(uuid.uuid4()).upper(),
        "client_id": client_id,
        "from_date": fd,
        "to_date": td,
        "forecast_amount": amt,
        "created_at": datetime.now(timezone.utc).replace(tzinfo=None),
    }
    payload = {k: v for k, v in payload.items() if k in forecast_cols}
    req = {"client_forecast_id", "client_id", "from_date", "to_date", "forecast_amount"}
    if not req.issubset(payload.keys()):
        return False
    sql, pr = sql_insert("client_forecast", payload, cursor)
    cursor.execute(sql, pr)
    return True


def merge_agent_fk_second_pass(
    cursor,
    to_import: List[Tuple[int, Dict[str, str]]],
    code_to_id: Dict[str, str],
    client_cols: Set[str],
) -> int:
    if "agent_client_id" not in client_cols:
        return 0
    n = 0
    for _ln, d in to_import:
        code = agent_code_from_row(d)
        if not code:
            continue
        internal = strip_cell(d.get("internalCode", ""))
        if not internal:
            continue
        agent_id = code_to_id.get(code)
        if not agent_id:
            continue
        cid = code_to_id.get(internal)
        if not cid:
            continue
        cursor.execute(
            "UPDATE client SET agent_client_id = ? WHERE client_id = ?",
            (agent_id, cid),
        )
        if cursor.rowcount and cursor.rowcount > 0:
            n += 1
    return n


if __name__ == "__main__":
    sys.exit(main())
