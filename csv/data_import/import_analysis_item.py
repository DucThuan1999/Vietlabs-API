#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script để import dữ liệu analysis_item từ CSV vào database
- Đọc analysis_item.csv
- Tự động tạo mới AnalysisGroup, EquipmentType, SampleMatrix nếu không tìm thấy
- Generate analysis_item_code theo format 'AI-0001'
- Parse unit_price từ CSV
- Convert name_vi sang Sentence case
- Insert TAT vào analysis_item_tat
"""

import csv
import pyodbc
import sys
import os
import re
import uuid
from datetime import datetime, timezone
from typing import Dict, Optional, Tuple

# Cấu hình kết nối database
CONNECTION_STRING = (
    "Driver={ODBC Driver 17 for SQL Server};"
    "Server=192.168.2.21,49938;"
    "Database=VietLabs;"
    "UID=sa;"
    "PWD=Sbt@2024;"
    "TrustServerCertificate=yes;"
    "Encrypt=yes;"
    "Login Timeout=60;"
)

# Đường dẫn đến file CSV (từ folder data_import, cần lên 1 cấp)
CSV_DIR = os.path.join(os.path.dirname(os.path.dirname(__file__)), "csv")
ANALYSIS_ITEM_CSV = os.path.join(CSV_DIR, "analysis_item.csv")

# Cột giá nhóm chuẩn trên Capability.xlsx (export CSV); ưu tiên tên mới, fallback tên cũ
WHOLE_GROUP_PRICE_KEYS = (
    "Giá nhóm chuẩn_new",
    "Analysis Group Whole group standard",
)


def row_first_non_empty(row: dict, keys: tuple) -> str:
    for k in keys:
        v = row.get(k)
        if v is not None and str(v).strip():
            return str(v).strip()
    return ""


def to_sentence_case(text: str) -> str:
    """Chuyển đổi text sang Sentence case"""
    if not text or not text.strip():
        return text
    
    text = text.strip()
    text_lower = text.lower()
    words = text_lower.split()
    words_capitalized = [word.capitalize() for word in words]
    
    return " ".join(words_capitalized)


def normalize_text(text: str) -> str:
    """Chuẩn hóa text để so sánh"""
    if not text:
        return ""
    text = text.strip().upper()
    text = re.sub(r'[^\w]', '', text)
    return text


def parse_decimal_with_unit(value: str) -> Tuple[Optional[float], Optional[str]]:
    """
    Parse giá trị có thể chứa số và đơn vị
    Ví dụ: "0.05 µg/mL" -> (0.05, "µg/mL")
           "0,25 µg/g" -> (0.25, "µg/g")
           "<10" -> (None, None) hoặc (10, None)
    """
    if not value or not value.strip():
        return None, None
    
    value = value.strip()
    
    # Xử lý trường hợp <10, >10, etc
    if value.startswith('<') or value.startswith('>'):
        # Extract số
        num_str = re.sub(r'[<>]', '', value).strip()
        try:
            num = float(num_str.replace(',', '.'))
            return num, None
        except:
            return None, None
    
    # Extract số (hỗ trợ cả dấu phẩy và chấm)
    match = re.search(r'([\d,\.]+)', value)
    if match:
        num_str = match.group(1).replace(',', '.')
        try:
            num = float(num_str)
            # Extract đơn vị (phần còn lại sau số)
            unit = value[match.end():].strip()
            return num, unit if unit else None
        except:
            pass
    
    return None, None


def parse_price(value: str) -> Optional[float]:
    """
    Parse giá tiền từ string
    Ví dụ: "140.000" -> 140000.0
           "1,500,000" -> 1500000.0
    """
    if not value or not value.strip():
        return None
    
    # Loại bỏ khoảng trắng và ký tự không phải số, dấu phẩy, dấu chấm
    value = value.strip().replace(' ', '')
    
    # Nếu có dấu chấm ở cuối (phân cách hàng nghìn), loại bỏ
    # Nếu có dấu phẩy ở cuối (phân cách hàng nghìn), loại bỏ
    # Giữ lại dấu chấm nếu là số thập phân
    
    # Xử lý trường hợp "140.000" (dấu chấm phân cách hàng nghìn)
    if '.' in value and ',' not in value:
        parts = value.split('.')
        if len(parts) == 2 and len(parts[1]) == 3:
            # Có thể là phân cách hàng nghìn
            value = value.replace('.', '')
        else:
            # Có thể là số thập phân
            pass
    
    # Xử lý trường hợp "1,500,000" (dấu phẩy phân cách hàng nghìn)
    if ',' in value:
        parts = value.split(',')
        if len(parts) > 1 and all(len(p) <= 3 for p in parts[1:]):
            # Có thể là phân cách hàng nghìn
            value = value.replace(',', '')
        else:
            # Có thể là số thập phân
            value = value.replace(',', '.')
    
    try:
        return float(value)
    except:
        return None


def load_mappings(connection) -> Dict:
    """Load tất cả mappings từ database"""
    cursor = connection.cursor()
    
    mappings = {
        'analysis_groups': {},
        'equipment_types': {},
        'sample_matrices': {},
        'sample_matrix_groups': {}
    }
    
    # Load AnalysisGroups
    cursor.execute("SELECT analysis_group_id, name_vi, name_en FROM analysis_group")
    for row in cursor.fetchall():
        group_id, name_vi, name_en = row
        if name_vi:
            mappings['analysis_groups'][normalize_text(name_vi)] = group_id
        if name_en:
            mappings['analysis_groups'][normalize_text(name_en)] = group_id
    
    # Load EquipmentTypes
    cursor.execute("SELECT equipment_type_id, name_vi, name_en FROM equipment_type")
    for row in cursor.fetchall():
        eq_id, name_vi, name_en = row
        if name_vi:
            mappings['equipment_types'][normalize_text(name_vi)] = eq_id
        if name_en:
            mappings['equipment_types'][normalize_text(name_en)] = eq_id
    
    # Load SampleMatrices
    cursor.execute("""
        SELECT sm.sample_matrix_id, sm.name_vi, sm.sample_matrix_group_id, smg.name_vi as group_name
        FROM sample_matrix sm
        LEFT JOIN sample_matrix_group smg ON sm.sample_matrix_group_id = smg.sample_matrix_group_id
    """)
    for row in cursor.fetchall():
        matrix_id, name_vi, group_id, group_name = row
        if name_vi and group_id:
            # Key kết hợp name và group_id
            key = f"{normalize_text(name_vi)}_{group_id}"
            mappings['sample_matrices'][key] = (matrix_id, group_id)
    
    # Load SampleMatrixGroups
    cursor.execute("SELECT sample_matrix_group_id, name_vi FROM sample_matrix_group")
    for row in cursor.fetchall():
        group_id, name_vi = row
        if name_vi:
            mappings['sample_matrix_groups'][normalize_text(name_vi)] = group_id
    
    return mappings


def get_or_create_analysis_group(connection, name: str, mappings: Dict) -> str:
    """Lấy hoặc tạo mới AnalysisGroup"""
    cursor = connection.cursor()
    
    normalized = normalize_text(name)
    if normalized in mappings['analysis_groups']:
        return mappings['analysis_groups'][normalized]
    
    # Tạo mới
    group_id = str(uuid.uuid4())
    name_vi = to_sentence_case(name)
    name_en = name_vi  # Sử dụng name_vi làm name_en nếu không có
    
    # Generate code theo format NCT-0001 (seed Layer0; legacy GPCT-/AG- vẫn tính max)
    max_num = get_max_analysis_group_code(connection)
    next_num = max_num + 1
    group_code = f"NCT-{next_num:04d}"
    
    cursor.execute("""
        INSERT INTO analysis_group (analysis_group_id, analysis_group_code, name_vi, name_en, status, created_at)
        VALUES (?, ?, ?, ?, 'Active', ?)
    """, group_id, group_code, name_vi, name_en, datetime.now(timezone.utc))
    
    connection.commit()
    
    # Cập nhật mapping
    mappings['analysis_groups'][normalized] = group_id
    
    return group_id


def get_max_equipment_type_code(connection) -> int:
    """Lấy số thứ tự lớn nhất cho equipment_type_code (TB-001 chuẩn; còn TP-/GPTP-/ET- là legacy)."""
    cursor = connection.cursor()
    
    max_num = 0
    # TB- (chuẩn), TP-/GPTP-/ET- (code cũ) — gộp max để mã TB mới không đụng số cũ
    for prefix in ["TB", "TP", "GPTP", "ET"]:
        try:
            cursor.execute("""
                SELECT TOP 1 equipment_type_code 
                FROM equipment_type 
                WHERE equipment_type_code LIKE ?
                ORDER BY CAST(SUBSTRING(equipment_type_code, CHARINDEX('-', equipment_type_code) + 1, LEN(equipment_type_code)) AS INT) DESC
            """, f"{prefix}-%")
            
            row = cursor.fetchone()
            if row and row[0]:
                try:
                    num_str = row[0].split('-')[1] if '-' in row[0] else row[0]
                    num = int(num_str)
                    if num > max_num:
                        max_num = num
                except:
                    pass
        except:
            pass
    
    return max_num


def get_or_create_equipment_type(connection, name: str, mappings: Dict) -> str:
    """Lấy hoặc tạo mới EquipmentType"""
    cursor = connection.cursor()
    
    normalized = normalize_text(name)
    if normalized in mappings['equipment_types']:
        return mappings['equipment_types'][normalized]
    
    # Tạo mới
    eq_id = str(uuid.uuid4())
    name_vi = to_sentence_case(name) if name else name
    name_en = name_vi  # Sử dụng name_vi làm name_en nếu không có
    
    # Generate code theo format TB-001
    max_num = get_max_equipment_type_code(connection)
    next_num = max_num + 1
    eq_code = f"TB-{next_num:03d}"
    
    cursor.execute("""
        INSERT INTO equipment_type (equipment_type_id, equipment_type_code, name_vi, name_en, status)
        VALUES (?, ?, ?, ?, 'Active')
    """, eq_id, eq_code, name_vi, name_en)
    
    connection.commit()
    
    # Cập nhật mapping
    mappings['equipment_types'][normalized] = eq_id
    
    return eq_id


def get_or_create_sample_matrix_group(connection, name: str, mappings: Dict) -> str:
    """Lấy hoặc tạo mới SampleMatrixGroup"""
    cursor = connection.cursor()
    
    if not name or not name.strip():
        # Sử dụng mẫu mặc định
        default_name = "Mẫu chung"
        normalized = normalize_text(default_name)
        if normalized in mappings['sample_matrix_groups']:
            return mappings['sample_matrix_groups'][normalized]
        name = default_name
    
    normalized = normalize_text(name)
    if normalized in mappings['sample_matrix_groups']:
        return mappings['sample_matrix_groups'][normalized]
    
    # Tạo mới SampleMatrixGroup
    group_id = str(uuid.uuid4())
    group_name = to_sentence_case(name)
    
    # Generate code theo format GPNM-0001
    max_num = get_max_sample_matrix_group_code(connection)
    next_num = max_num + 1
    group_code = f"GPNM-{next_num:04d}"
    
    cursor.execute("""
        INSERT INTO sample_matrix_group (sample_matrix_group_id, sample_matrix_group_code, name_vi, name_en, status, created_at)
        VALUES (?, ?, ?, ?, 'Active', ?)
    """, group_id, group_code, group_name, group_name, datetime.now(timezone.utc))
    
    connection.commit()
    
    # Cập nhật mapping
    mappings['sample_matrix_groups'][normalized] = group_id
    
    return group_id


def get_or_create_sample_matrix(connection, matrix_name: str, group_id: str, mappings: Dict) -> str:
    """Lấy hoặc tạo mới SampleMatrix trong một SampleMatrixGroup cụ thể"""
    cursor = connection.cursor()
    
    if not matrix_name or not matrix_name.strip():
        # Tìm mẫu đầu tiên trong group
        cursor.execute("""
            SELECT TOP 1 sample_matrix_id
            FROM sample_matrix
            WHERE sample_matrix_group_id = ? AND status = 'Active'
            ORDER BY created_at
        """, group_id)
        row = cursor.fetchone()
        if row:
            return row[0]
        # Tạo mẫu mặc định
        matrix_name = "Mẫu chung"
    
    # Tìm trong mapping
    normalized = normalize_text(matrix_name)
    key = f"{normalized}_{group_id}"  # Key kết hợp name và group_id
    
    # Kiểm tra trong database
    cursor.execute("""
        SELECT sample_matrix_id
        FROM sample_matrix
        WHERE sample_matrix_group_id = ? AND name_vi = ?
    """, group_id, to_sentence_case(matrix_name))
    row = cursor.fetchone()
    if row:
        matrix_id = row[0]
        # Cập nhật mapping
        mappings['sample_matrices'][key] = (matrix_id, group_id)
        return matrix_id
    
    # Tạo mới SampleMatrix
    matrix_id = str(uuid.uuid4())
    matrix_name_vi = to_sentence_case(matrix_name)
    
    # Generate code theo format NM-0001
    max_num = get_max_sample_matrix_code(connection)
    next_num = max_num + 1
    matrix_code = f"NM-{next_num:04d}"
    
    # Lấy tên nhóm để insert vào cột sample_matrix_group
    cursor.execute("SELECT name_vi FROM sample_matrix_group WHERE sample_matrix_group_id = ?", group_id)
    group_row = cursor.fetchone()
    group_name = group_row[0] if group_row else matrix_name_vi
    
    cursor.execute("""
        INSERT INTO sample_matrix (sample_matrix_id, sample_matrix_code, name_vi, name_en, sample_matrix_group_id, sample_matrix_group, status, created_at)
        VALUES (?, ?, ?, ?, ?, ?, 'Active', ?)
    """, matrix_id, matrix_code, matrix_name_vi, matrix_name_vi, group_id, group_name, datetime.now(timezone.utc))
    
    connection.commit()
    
    # Cập nhật mapping
    mappings['sample_matrices'][key] = (matrix_id, group_id)
    
    return matrix_id


def get_max_code_number(connection, table_name: str, code_column: str, prefix: str) -> int:
    """Lấy số thứ tự lớn nhất cho một loại code"""
    cursor = connection.cursor()
    
    try:
        query = f"""
            SELECT TOP 1 {code_column}
            FROM {table_name}
            WHERE {code_column} LIKE ?
            ORDER BY CAST(SUBSTRING({code_column}, {len(prefix) + 2}, LEN({code_column})) AS INT) DESC
        """
        cursor.execute(query, f"{prefix}-%")
        
        row = cursor.fetchone()
        if row and row[0]:
            try:
                # Extract số từ "PREFIX-0001"
                num_str = row[0].split('-')[1] if '-' in row[0] else row[0]
                return int(num_str)
            except:
                pass
    except:
        pass
    
    return 0


def get_max_analysis_item_code(connection) -> int:
    """Lấy số thứ tự lớn nhất cho analysis_item_code (CT-0001 hoặc AI-0001)"""
    cursor = connection.cursor()
    
    max_num = 0
    # Tìm trong cả CT- và AI- (code cũ)
    for prefix in ["CT", "AI"]:
        try:
            cursor.execute("""
                SELECT TOP 1 analysis_item_code 
                FROM analysis_item 
                WHERE analysis_item_code LIKE ?
                ORDER BY CAST(SUBSTRING(analysis_item_code, CHARINDEX('-', analysis_item_code) + 1, LEN(analysis_item_code)) AS INT) DESC
            """, f"{prefix}-%")
            
            row = cursor.fetchone()
            if row and row[0]:
                try:
                    num_str = row[0].split('-')[1] if '-' in row[0] else row[0]
                    num = int(num_str)
                    if num > max_num:
                        max_num = num
                except:
                    pass
        except:
            pass
    
    return max_num


def get_max_analysis_group_code(connection) -> int:
    """Lấy số thứ tự lớn nhất cho analysis_group_code (NCT-0001, GPCT-, AG-)"""
    cursor = connection.cursor()
    
    max_num = 0
    for prefix in ["NCT", "GPCT", "AG"]:
        try:
            cursor.execute("""
                SELECT TOP 1 analysis_group_code 
                FROM analysis_group 
                WHERE analysis_group_code LIKE ?
                ORDER BY CAST(SUBSTRING(analysis_group_code, CHARINDEX('-', analysis_group_code) + 1, LEN(analysis_group_code)) AS INT) DESC
            """, f"{prefix}-%")
            
            row = cursor.fetchone()
            if row and row[0]:
                try:
                    num_str = row[0].split('-')[1] if '-' in row[0] else row[0]
                    num = int(num_str)
                    if num > max_num:
                        max_num = num
                except:
                    pass
        except:
            pass
    
    return max_num


def get_max_sample_matrix_code(connection) -> int:
    """Lấy số thứ tự lớn nhất cho sample_matrix_code (NM-0001)"""
    cursor = connection.cursor()
    
    try:
        cursor.execute("""
            SELECT TOP 1 sample_matrix_code 
            FROM sample_matrix 
            WHERE sample_matrix_code LIKE 'NM-%'
            ORDER BY CAST(SUBSTRING(sample_matrix_code, CHARINDEX('-', sample_matrix_code) + 1, LEN(sample_matrix_code)) AS INT) DESC
        """)
        
        row = cursor.fetchone()
        if row and row[0]:
            try:
                num_str = row[0].split('-')[1] if '-' in row[0] else row[0]
                return int(num_str)
            except:
                pass
    except:
        pass
    
    return 0


def get_max_sample_matrix_group_code(connection) -> int:
    """Lấy số thứ tự lớn nhất cho sample_matrix_group_code (GPNM-0001)"""
    cursor = connection.cursor()
    
    try:
        cursor.execute("""
            SELECT TOP 1 sample_matrix_group_code 
            FROM sample_matrix_group 
            WHERE sample_matrix_group_code LIKE 'GPNM-%'
            ORDER BY CAST(SUBSTRING(sample_matrix_group_code, CHARINDEX('-', sample_matrix_group_code) + 1, LEN(sample_matrix_group_code)) AS INT) DESC
        """)
        
        row = cursor.fetchone()
        if row and row[0]:
            try:
                num_str = row[0].split('-')[1] if '-' in row[0] else row[0]
                return int(num_str)
            except:
                pass
    except:
        pass
    
    return 0


def process_analysis_item_csv(csv_path: str, connection, mappings: Dict, start_code_num: int) -> list:
    """Đọc và xử lý CSV"""
    records = []
    errors = []
    current_code_num = start_code_num
    
    try:
        with open(csv_path, 'r', encoding='utf-8-sig') as f:
            lines = f.readlines()
            
            # Tìm header
            header_line = None
            start_index = 0
            for i, line in enumerate(lines):
                line_stripped = line.strip()
                if line_stripped and ('sample_matrix' in line_stripped or 'analysis_item_name_vi' in line_stripped):
                    header_line = line_stripped
                    start_index = i + 1
                    break
            
            if not header_line:
                print(f"Loi: Khong tim thay header trong file {csv_path}")
                return records
            
            # Parse header
            header_fields = [field.strip() for field in header_line.split(';')]
            
            # Đọc dữ liệu
            for i in range(start_index, len(lines)):
                line = lines[i].strip()
                if not line or line.startswith(';') and not any(c.isalnum() for c in line):
                    continue
                
                values = [v.strip() for v in line.split(';')]
                if len(values) < len(header_fields):
                    values.extend([''] * (len(header_fields) - len(values)))
                
                row = dict(zip(header_fields, values))
                
                # Kiểm tra dòng trống
                if not row or all(not v or v.strip() == '' for v in row.values()):
                    continue
                
                # Parse các trường
                sample_matrix_group_name = row.get('sample_matrix_group', '').strip()
                sample_matrix_name = row.get('sample_matrix', '').strip()
                analysis_group_name = row.get('analysis_group', '').strip()
                name_vi_raw = row.get('analysis_item_name_vi', '').strip()
                name_en = row.get('analysis_item_name_en', '').strip()
                published_group_code = row.get('public_group_code', '').strip()
                equipment_type_name = row.get('Equipment_type', '').strip()
                lod_raw = row.get('LOD', '').strip()
                loq_raw = row.get('LOQ', '').strip()
                tat_normal = row.get('TAT_Normal', '').strip()
                tat_fast = row.get('TAT_Fast', '').strip()
                tat_urgent = row.get('TAT_Urgent', '').strip()
                unit_price_raw = row.get('unit_price', '').strip()
                whole_group_standard = row_first_non_empty(row, WHOLE_GROUP_PRICE_KEYS)
                
                # Bỏ qua nếu thiếu thông tin cần thiết
                if not name_vi_raw:
                    continue
                
                # Convert name_vi sang Sentence case
                name_vi = to_sentence_case(name_vi_raw)
                
                # name_en bắt buộc, nếu không có thì dùng name_vi
                if not name_en or not name_en.strip():
                    name_en = name_vi
                
                # Mapping AnalysisGroup
                if analysis_group_name:
                    analysis_group_id = get_or_create_analysis_group(connection, analysis_group_name, mappings)
                else:
                    errors.append(f"Khong co analysis_group cho record: {name_vi}")
                    continue
                
                # Mapping EquipmentType
                if equipment_type_name:
                    equipment_type_id = get_or_create_equipment_type(connection, equipment_type_name, mappings)
                else:
                    errors.append(f"Khong co Equipment_type cho record: {name_vi}")
                    continue
                
                # Mapping SampleMatrixGroup
                sample_matrix_group_id = get_or_create_sample_matrix_group(
                    connection, sample_matrix_group_name, mappings
                )
                
                # Mapping SampleMatrix (phải thuộc về group tương ứng)
                sample_matrix_id = get_or_create_sample_matrix(
                    connection, sample_matrix_name, sample_matrix_group_id, mappings
                )
                
                # Parse LOD/LOQ
                lod_value, lod_unit = parse_decimal_with_unit(lod_raw)
                loq_value, loq_unit = parse_decimal_with_unit(loq_raw) if loq_raw else (None, None)
                
                # Parse unit_price (bắt buộc, default = 0.0)
                unit_price = parse_price(unit_price_raw) if unit_price_raw else 0.0
                if unit_price is None:
                    unit_price = 0.0
                
                # Parse TAT
                tat_normal_value = int(tat_normal) if tat_normal and tat_normal.isdigit() else None
                tat_fast_value = int(tat_fast) if tat_fast and tat_fast.isdigit() else None
                tat_urgent_value = int(tat_urgent) if tat_urgent and tat_urgent.isdigit() else None
                
                # Generate analysis_item_code theo format CT-0001
                current_code_num += 1
                analysis_item_code = f"CT-{current_code_num:04d}"
                
                # Tạo record
                record = {
                    'analysis_item_id': str(uuid.uuid4()),
                    'analysis_item_code': analysis_item_code,
                    'name_vi': name_vi,
                    'name_en': name_en if name_en else None,
                    'published_group_code': published_group_code if published_group_code else None,
                    'equipment_type_id': equipment_type_id,
                    'analysis_group_id': analysis_group_id,
                    'sample_matrix_id': sample_matrix_id,
                    'sample_matrix_group_id': sample_matrix_group_id,
                    'lod': lod_value,
                    'loq': loq_value,
                    'unit': lod_unit or loq_unit,  # Sử dụng unit từ LOD hoặc LOQ
                    'unit_price': unit_price,
                    'status': 'Active',
                    'tat_normal': tat_normal_value,
                    'tat_fast': tat_fast_value,
                    'tat_urgent': tat_urgent_value,
                    'whole_group_standard': whole_group_standard,
                    'created_at': datetime.now(timezone.utc)
                }
                
                records.append(record)
                
    except Exception as e:
        print(f"Loi khi doc file {csv_path}: {e}")
        import traceback
        traceback.print_exc()
        return records
    
    if errors:
        print("\nCanh bao:")
        for error in errors[:10]:  # Chỉ hiển thị 10 lỗi đầu
            print(f"  - {error}")
        if len(errors) > 10:
            print(f"  ... va {len(errors) - 10} loi khac")
    
    return records


def insert_records(connection, records: list):
    """Insert records vào database"""
    cursor = connection.cursor()
    
    inserted_count = 0
    updated_count = 0
    error_count = 0
    tat_inserted = 0
    tat_updated = 0
    
    for record in records:
        try:
            # Kiểm tra xem đã tồn tại chưa (theo code)
            check_query = """
                SELECT analysis_item_id 
                FROM analysis_item 
                WHERE analysis_item_code = ?
            """
            cursor.execute(check_query, record['analysis_item_code'])
            exists = cursor.fetchone()
            
            if exists:
                # Update - giữ nguyên analysis_item_id cũ
                analysis_item_id = exists[0]
                update_query = """
                    UPDATE analysis_item
                    SET name_vi = ?,
                        name_en = ?,
                        published_group_code = ?,
                        equipment_type_id = ?,
                        analysis_group_id = ?,
                        sample_matrix_id = ?,
                        sample_matrix_group_id = ?,
                        lod = ?,
                        loq = ?,
                        unit = ?,
                        unit_price = ?,
                        status = ?,
                        updated_at = ?
                    WHERE analysis_item_id = ?
                """
                cursor.execute(update_query, (
                    record['name_vi'],
                    record['name_en'],
                    record['published_group_code'],
                    record['equipment_type_id'],
                    record['analysis_group_id'],
                    record['sample_matrix_id'],
                    record['sample_matrix_group_id'],
                    record['lod'],
                    record['loq'],
                    record['unit'],
                    record['unit_price'],
                    record['status'],
                    datetime.now(timezone.utc),
                    analysis_item_id
                ))
                updated_count += 1
            else:
                # Insert mới
                insert_query = """
                    INSERT INTO analysis_item (
                        analysis_item_id, analysis_item_code, name_vi, name_en,
                        published_group_code, equipment_type_id, analysis_group_id,
                        sample_matrix_id, sample_matrix_group_id,
                        lod, loq, unit, unit_price, status,
                        created_at, updated_at
                    ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
                """
                cursor.execute(insert_query, (
                    record['analysis_item_id'],
                    record['analysis_item_code'],
                    record['name_vi'],
                    record['name_en'],
                    record['published_group_code'],
                    record['equipment_type_id'],
                    record['analysis_group_id'],
                    record['sample_matrix_id'],
                    record['sample_matrix_group_id'],
                    record['lod'],
                    record['loq'],
                    record['unit'],
                    record['unit_price'],
                    record['status'],
                    record['created_at'],
                    None
                ))
                analysis_item_id = record['analysis_item_id']
                inserted_count += 1
            
            # Insert TAT nếu có
            for tat_type, tat_value in [
                ('Normal', record['tat_normal']),
                ('Fast', record['tat_fast']),
                ('Urgent', record['tat_urgent'])
            ]:
                if tat_value is not None and tat_value > 0:
                    try:
                        # Kiểm tra TAT đã tồn tại chưa
                        cursor.execute("""
                            SELECT analysis_item_tat_id 
                            FROM analysis_item_tat 
                            WHERE analysis_item_id = ? AND tat_type = ?
                        """, analysis_item_id, tat_type)
                        
                        existing_tat = cursor.fetchone()
                        
                        if existing_tat:
                            # Update
                            cursor.execute("""
                                UPDATE analysis_item_tat
                                SET tat_value = ?, updated_at = ?
                                WHERE analysis_item_id = ? AND tat_type = ?
                            """, tat_value, datetime.now(timezone.utc), analysis_item_id, tat_type)
                            tat_updated += 1
                        else:
                            # Insert mới
                            tat_id = str(uuid.uuid4())
                            cursor.execute("""
                                INSERT INTO analysis_item_tat (
                                    analysis_item_tat_id, analysis_item_id, tat_type, 
                                    tat_value, tat_unit, created_at
                                ) VALUES (?, ?, ?, ?, 'Hours', ?)
                            """, tat_id, analysis_item_id, tat_type, tat_value, datetime.now(timezone.utc))
                            tat_inserted += 1
                    except Exception as tat_error:
                        print(f"  Loi khi insert TAT {tat_type} cho {record.get('name_vi', 'unknown')}: {tat_error}")
                        # Tiếp tục xử lý record khác
            
            # Update whole_group_standard_price nếu có
            if record.get('whole_group_standard'):
                try:
                    price = parse_price(record['whole_group_standard'])
                    if price:
                        cursor.execute("""
                            UPDATE analysis_group
                            SET whole_group_standard_price = ?
                            WHERE analysis_group_id = ?
                        """, price, record['analysis_group_id'])
                except:
                    pass
                    
        except Exception as e:
            error_count += 1
            print(f"Loi khi xu ly record {record.get('name_vi', 'unknown')}: {e}")
            continue
    
    connection.commit()
    
    print(f"\nKet qua:")
    print(f"  - Da insert: {inserted_count} analysis_item records")
    print(f"  - Da update: {updated_count} analysis_item records")
    print(f"  - Da insert: {tat_inserted} TAT records (moi)")
    print(f"  - Da update: {tat_updated} TAT records (cap nhat)")
    print(f"  - Loi: {error_count} records")


def main():
    # Thiết lập encoding cho console trên Windows
    if sys.platform == 'win32':
        import io
        sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8', errors='replace')
        sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8', errors='replace')
    
    print("=" * 60)
    print("Script import analysis_item tu CSV vao database")
    print("=" * 60)
    
    # Kiểm tra file CSV
    if not os.path.exists(ANALYSIS_ITEM_CSV):
        print(f"Loi: Khong tim thay file {ANALYSIS_ITEM_CSV}")
        sys.exit(1)
    
    # Kết nối database
    print("\n1. Dang ket noi database...")
    try:
        connection = pyodbc.connect(CONNECTION_STRING)
        print("   Ket noi thanh cong!")
        
        # Load mappings
        print("\n2. Dang load mappings tu database...")
        mappings = load_mappings(connection)
        print(f"   - AnalysisGroups: {len(mappings['analysis_groups'])}")
        print(f"   - EquipmentTypes: {len(mappings['equipment_types'])}")
        print(f"   - SampleMatrices: {len(mappings['sample_matrices'])}")
        
        # Lấy số thứ tự bắt đầu cho analysis_item_code (CT-0001)
        print("\n3. Dang lay so thu tu bat dau...")
        start_code_num = get_max_analysis_item_code(connection)
        print(f"   So thu tu bat dau cho CT: {start_code_num + 1}")
        
        # Process CSV
        print("\n4. Dang xu ly file CSV...")
        records = process_analysis_item_csv(ANALYSIS_ITEM_CSV, connection, mappings, start_code_num)
        print(f"   Da xu ly {len(records)} records")
        
        if not records:
            print("Khong co records nao de insert!")
            connection.close()
            sys.exit(1)
        
        # Insert vào database
        print("\n5. Dang insert/update records...")
        insert_records(connection, records)
        
        connection.close()
        print("\nHoan thanh!")
        
    except pyodbc.Error as e:
        print(f"Loi ket noi database: {e}")
        sys.exit(1)
    except Exception as e:
        print(f"Loi: {e}")
        import traceback
        traceback.print_exc()
        sys.exit(1)


if __name__ == "__main__":
    main()

