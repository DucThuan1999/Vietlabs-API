#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script import nhân viên từ csv/employee.csv vào bảng employee.
- CSV delimiter: ; (chấm phẩy)
- Cột (theo header): STT; Mã Nhân Viên; Tên Nhân Viên; Email; SĐT; Phòng Ban; Chức Vụ; Quản lý trực tiếp
- Đọc theo tên cột header nên có thể đổi thứ tự cột.
- Map Chức vụ -> employee_title_id (theo name_vi trong employee_title)
- Map Phòng Ban -> department_id (theo name_vi hoặc department_code trong department)
- Map cột "Quản lý trực tiếp" -> manager_id (theo full_name sau khi đã insert hết)
- SĐT ghi vào cột mobile (không còn ghi vào notes)
- Tạo account cho mọi nhân viên chưa có tài khoản: user_name = email (nếu không có email thì dùng employee_code), quyền PERM-USER, mật khẩu mặc định 123456
"""

import base64
import csv
import hashlib
import os
import sys
import uuid
import pyodbc
from typing import Dict, List, Optional, Tuple

# Mật khẩu mặc định cho account mới (nhân viên đổi sau khi đăng nhập lần đầu)
DEFAULT_PASSWORD = "123456"

# Cấu hình kết nối database (sửa cho đúng môi trường của bạn)
CONNECTION_STRING = (
    "Driver={ODBC Driver 17 for SQL Server};"
    "Server=192.168.2.21,49938;"
    "Database=VietLabs;"
    "UID=sa;"
    "PWD=Sbt@2024;"
    "TrustServerCertificate=yes;"
    "Encrypt=yes;"
)

# Thư mục csv (parent của data_import) = .../Vietlabs-API/csv
CSV_DIR = os.path.dirname(os.path.dirname(__file__))
EMPLOYEE_CSV = os.path.join(CSV_DIR, "employee.csv")


def hash_password(password: str) -> str:
    """Hash mật khẩu giống AuthController: SHA256 rồi Base64."""
    h = hashlib.sha256(password.encode("utf-8")).digest()
    return base64.b64encode(h).decode("ascii")


def normalize_name(text: str) -> str:
    """Chuẩn hóa tên để so sánh: bỏ dấu cách thừa, strip, upper."""
    if not text:
        return ""
    return " ".join(text.strip().split()).upper()


def load_employee_titles(cursor) -> Dict[str, str]:
    """Load employee_title: name_vi (normalized) -> employee_title_id (guid string)."""
    cursor.execute("""
        SELECT employee_title_id, name_vi, title_code
        FROM employee_title
        WHERE status = N'Active'
    """)
    rows = cursor.fetchall()
    result = {}
    for row in rows:
        eid, name_vi, code = str(row[0]), (row[1] or "").strip(), (row[2] or "").strip()
        key = normalize_name(name_vi)
        if key:
            result[key] = eid
        if code:
            result[code.upper()] = eid
    return result


def load_departments(cursor) -> Dict[str, str]:
    """Load department: name_vi (normalized) và department_code -> department_id (guid string)."""
    cursor.execute("""
        SELECT department_id, name_vi, department_code
        FROM department
        WHERE status = N'Active'
    """)
    rows = cursor.fetchall()
    result = {}
    for row in rows:
        dept_id = str(row[0])
        name_vi = (row[1] or "").strip()
        code = (row[2] or "").strip()
        key = normalize_name(name_vi)
        if key:
            result[key] = dept_id
        if code:
            result[code.upper()] = dept_id
    return result


# Tên header CSV (strip để khớp linh hoạt)
HEADER_STT = "STT"
HEADER_MA_NV = "Mã Nhân Viên"
HEADER_TEN_NV = "Tên Nhân Viên"
HEADER_EMAIL = "Email"
HEADER_SDT = "SĐT"
HEADER_PHONG_BAN = "Phòng Ban"
HEADER_BO_PHAN = "Bộ phận"
HEADER_CHUC_VU = "Chức Vụ"
HEADER_QUAN_LY_TRUC_TIEP = "Quản lý trực tiếp"


def load_sections(cursor) -> Dict[str, str]:
    """Load section: name_vi (normalized) và section_code -> section_id (guid string)."""
    cursor.execute("""
        SELECT section_id, name_vi, section_code
        FROM section
        WHERE status = N'Active'
    """)
    rows = cursor.fetchall()
    result = {}
    for row in rows:
        sec_id = str(row[0])
        name_vi = (row[1] or "").strip()
        code = (row[2] or "").strip()
        key = normalize_name(name_vi)
        if key:
            result[key] = sec_id
        if code:
            result[code.upper()] = sec_id
    return result


def cleanup_stale_employees(cursor, csv_codes: set):
    """Xóa nhân viên trong DB không có trong CSV và dọn dẹp liên kết."""
    print("--- Bắt đầu dọn dẹp nhân viên không có trong CSV ---")
    cursor.execute("SELECT employee_id, employee_code, full_name FROM employee")
    db_employees = cursor.fetchall()
    
    to_delete = []
    for eid, code, name in db_employees:
        c = (code or "").strip().upper()
        if c and c not in csv_codes:
            to_delete.append((str(eid), c, name))
            
    if not to_delete:
        print("Không có nhân viên nào thừa cần xóa.")
        return

    print(f"Phát hiện {len(to_delete)} nhân viên cần xóa khỏi DB.")

    audit_tables = [
        ('client_industry', 'created_by'), ('client_industry', 'updated_by'),
        ('employee_title', 'created_by'), ('employee_title', 'updated_by'),
        ('branch', 'updated_by'), ('department', 'updated_by'),
        ('employee', 'updated_by'), ('section', 'updated_by'),
        ('client_history', 'changed_by_account_id'), ('quotation_history', 'changed_by_account_id'),
        ('quotation', 'employee_id'), ('employee', 'manager_id'),
        ('employee_analysis_capability', 'employee_id'), ('module_approver', 'approver_employee_id')
    ]

    deleted_count = 0
    skipped_count = 0

    for emp_id, code, name in to_delete:
        try:
            # Lấy account_id
            cursor.execute("SELECT account_id FROM account WHERE employee_id = ?", (emp_id,))
            acc_row = cursor.fetchone()
            acc_id = str(acc_row[0]) if acc_row else None

            # 1. Try to nullify references
            for table, col in audit_tables:
                # Nếu là audit field (account_id)
                if acc_id and col in ['created_by', 'updated_by', 'changed_by_account_id']:
                    try:
                        cursor.execute(f"UPDATE [{table}] SET [{col}] = NULL WHERE [{col}] = ?", (acc_id,))
                    except: pass # Bỏ qua nếu không cho NULL
                # Nếu là employee_id direct ref
                elif col in ['employee_id', 'manager_id', 'approver_employee_id']:
                    try:
                        cursor.execute(f"UPDATE [{table}] SET [{col}] = NULL WHERE [{col}] = ?", (emp_id,))
                    except: pass

            # 2. Xóa Account related
            if acc_id:
                cursor.execute("DELETE FROM refresh_token WHERE account_id = ?", (acc_id,))
                cursor.execute("DELETE FROM account_module_grant WHERE account_id = ?", (acc_id,))
                cursor.execute("DELETE FROM account WHERE account_id = ?", (acc_id,))
            
            # 3. Xóa Employee
            cursor.execute("DELETE FROM employee WHERE employee_id = ?", (emp_id,))
            deleted_count += 1
            print(f"  [Xóa] {code} - {name}")

        except Exception as e:
            skipped_count += 1
            print(f"  [Bỏ qua] {code} - {name} (Lỗi: có dữ liệu liên kết không thể xóa)")

    print(f"Hoàn tất: Đã xóa {deleted_count}, Bỏ qua {skipped_count} nhân viên.")


def _cell(d: Dict[str, str], key: str) -> str:
    """Lấy giá trị ô theo tên cột (strip cả key và value)."""
    val = d.get(key) or d.get(key.strip()) if key else ""
    return (val or "").strip()


def parse_csv_row_dict(row_dict: Dict[str, str]) -> Optional[Tuple[str, str, str, str, str, str, str, str]]:
    """
    Parse 1 dòng CSV (đã chuyển thành dict theo header).
    Trả về (employee_code, full_name, email, phone, department, section, title, manager_name) hoặc None nếu bỏ qua.
    Cột "Quản lý trực tiếp" -> manager_name (dùng để set manager_id sau khi insert).
    """
    stt = _cell(row_dict, HEADER_STT)
    code = _cell(row_dict, HEADER_MA_NV)
    name = _cell(row_dict, HEADER_TEN_NV)
    email = _cell(row_dict, HEADER_EMAIL)
    phone = _cell(row_dict, HEADER_SDT)
    department = _cell(row_dict, HEADER_PHONG_BAN)
    section = _cell(row_dict, HEADER_BO_PHAN)
    title = _cell(row_dict, HEADER_CHUC_VU)
    manager_name = _cell(row_dict, HEADER_QUAN_LY_TRUC_TIEP)

    # Bỏ qua dòng trống hoặc không có mã / tên
    if not code and not name:
        return None
    if not code:
        code = f"NV{stt}" if stt and stt.isdigit() else ""
    if not name:
        return None

    return (code, name, email, phone, department, section, title, manager_name)


def read_employees_from_csv(path: str) -> List[Tuple[str, str, str, str, str, str, str, str]]:
    """Đọc file CSV theo header (delimiter ;), cột 'Quản lý trực tiếp' map -> manager_id."""
    rows_data = []
    with open(path, "r", encoding="utf-8-sig") as f:
        reader = csv.DictReader(f, delimiter=";")
        for row in reader:
            parsed = parse_csv_row_dict(row)
            if parsed:
                rows_data.append(parsed)
    return rows_data


def run():
    if not os.path.isfile(EMPLOYEE_CSV):
        print(f"Không tìm thấy file: {EMPLOYEE_CSV}")
        return 1

    conn = pyodbc.connect(CONNECTION_STRING)
    conn.autocommit = False
    cursor = conn.cursor()

    try:
        title_map = load_employee_titles(cursor)
        print(f"Đã load {len(title_map)} chức vụ từ employee_title.")

        department_map = load_departments(cursor)
        print(f"Đã load {len(department_map)} phòng ban từ department.")

        section_map = load_sections(cursor)
        print(f"Đã load {len(section_map)} bộ phận từ section.")

        data = read_employees_from_csv(EMPLOYEE_CSV)
        print(f"Đọc được {len(data)} dòng nhân viên từ CSV.")

        # Tập hợp mã NV từ CSV để cleanup sau này
        csv_codes = {code.strip().upper() for code, _, _, _, _, _, _, _ in data if code}

        # Kiểm tra mã nhân viên đã tồn tại chưa
        cursor.execute("SELECT employee_code FROM employee WHERE employee_code IS NOT NULL")
        existing_codes = {str(r[0]).strip().upper() for r in cursor.fetchall() if r[0]}

        inserted = 0
        skipped = 0
        updated_mobile = 0
        # Lưu (employee_code, full_name, manager_name) để cập nhật manager_id sau
        pending_managers: List[Tuple[str, str, str]] = []

        for code, full_name, email, phone, department, section, title, manager_name in data:
            code_upper = code.upper()
            mobile = (phone or "").strip() or None

            # Map Phòng Ban (CSV) -> department_id
            department_id = None
            if department:
                dept_key = normalize_name(department)
                department_id = department_map.get(dept_key) or department_map.get(department.strip().upper())

            # Map Bộ phận (CSV) -> section_id
            section_id = None
            if section:
                sec_key = normalize_name(section)
                section_id = section_map.get(sec_key) or section_map.get(section.strip().upper())

            employee_id = None
            if code_upper in existing_codes:
                # Tìm ID nhân viên đã có
                cursor.execute("SELECT employee_id FROM employee WHERE UPPER(LTRIM(RTRIM(employee_code))) = ?", (code_upper,))
                row = cursor.fetchone()
                if row:
                    employee_id = str(row[0])
                    employee_title_id = None
                    title_key = normalize_name(title)
                    if title_key and title_key in title_map:
                        employee_title_id = title_map[title_key]

                    # Nhân viên đã tồn tại: cập nhật mobile, department_id, section_id, title từ CSV
                    cursor.execute(
                        """
                        UPDATE employee 
                        SET mobile = ?, department_id = ?, section_id = ?, 
                            employee_title_id = ?, title = ? 
                        WHERE employee_id = ?
                        """,
                        (mobile, department_id, section_id, employee_title_id, title or None, employee_id)
                    )
                    if cursor.rowcount > 0:
                        updated_mobile += 1
                skipped += 1
            else:
                employee_id = str(uuid.uuid4())
                employee_title_id = None
                title_key = normalize_name(title)
                if title_key and title_key in title_map:
                    employee_title_id = title_map[title_key]

                # role: DB không cho NULL, dùng chuỗi rỗng nếu chưa có
                role_value = ""

                # SĐT từ CSV -> mobile; notes để trống (cột notes không cho NULL)
                notes_value = ""

                cursor.execute("""
                    INSERT INTO employee (
                        employee_id, employee_code, department_id, section_id, role, full_name,
                        employee_title_id, title, email, mobile, notes, status, manager_id
                    )
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, N'Active', NULL)
                """, (
                    employee_id, code or None, department_id, section_id, role_value, full_name,
                    employee_title_id, title or None, email or None, mobile, notes_value
                ))
                inserted += 1
                existing_codes.add(code_upper)

            if employee_id:
                pending_managers.append((employee_id, full_name, manager_name))

        conn.commit()
        print(f"Đã insert {inserted} nhân viên, cập nhật mobile cho {updated_mobile} nhân viên có sẵn, bỏ qua (trùng mã, đã xử lý) {skipped}.")

        # Build map full_name (normalized) -> employee_id cho quản lý
        cursor.execute("SELECT employee_id, full_name FROM employee")
        name_to_id: Dict[str, str] = {}
        for r in cursor.fetchall():
            eid, name = str(r[0]), (r[1] or "").strip()
            key = normalize_name(name)
            if key:
                name_to_id[key] = eid

        # Cập nhật manager_id
        updated_manager = 0
        for employee_id, full_name, manager_name in pending_managers:
            if not manager_name:
                continue
            key = normalize_name(manager_name)
            manager_id = name_to_id.get(key)
            if manager_id and manager_id != employee_id:
                cursor.execute(
                    "UPDATE employee SET manager_id = ? WHERE employee_id = ?",
                    (manager_id, employee_id)
                )
                updated_manager += 1

        conn.commit()
        print(f"Đã cập nhật manager_id cho {updated_manager} nhân viên.")

        # Tạo account cho nhân viên chưa có
        cursor.execute(
            """
            SELECT e.employee_id, e.employee_code, e.full_name, e.email
            FROM employee e
            WHERE e.status = N'Active'
              AND NOT EXISTS (SELECT 1 FROM account a WHERE a.employee_id = e.employee_id)
              AND (e.email IS NOT NULL AND LTRIM(RTRIM(e.email)) <> N''
                   OR (e.employee_code IS NOT NULL AND LTRIM(RTRIM(ISNULL(e.employee_code, N''))) <> N''))
            """
        )
        employees_without_account = cursor.fetchall()
        existing_usernames = set()
        cursor.execute("SELECT user_name FROM account WHERE user_name IS NOT NULL")
        for r in cursor.fetchall():
            if r[0]:
                existing_usernames.add(str(r[0]).strip().lower())

        created_accounts = 0
        for row in employees_without_account:
            emp_id = str(row[0])
            emp_code = (row[1] or "").strip()
            full_name = (row[2] or "").strip()
            email = (row[3] or "").strip()
            # Username = email; nếu không có email thì fallback sang employee_code
            user_name = email if email else emp_code
            if not user_name:
                continue
            if user_name.lower() in existing_usernames:
                continue
            account_id = str(uuid.uuid4())
            password_hash = hash_password(DEFAULT_PASSWORD)
            cursor.execute(
                """
                INSERT INTO account (account_id, employee_id, user_name, password_hash, status)
                VALUES (?, ?, ?, ?, N'Active')
                """,
                (account_id, emp_id, user_name, password_hash),
            )
            created_accounts += 1
            existing_usernames.add(user_name.lower())

        conn.commit()
        print(f"Đã tạo {created_accounts} account, mật khẩu mặc định: {DEFAULT_PASSWORD!r}.")

        # Bước cuối: Xóa nhân viên không có trong CSV
        cleanup_stale_employees(cursor, csv_codes)
        conn.commit()

    except Exception as e:
        conn.rollback()
        print(f"Lỗi: {e}")
        raise
    finally:
        cursor.close()
        conn.close()

    print("Hoàn tất import employee.")
    return 0


if __name__ == "__main__":
    sys.exit(run())
