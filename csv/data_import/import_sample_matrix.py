#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script để import dữ liệu sample_matrix từ CSV vào database
- Đọc sample_matrix.csv và sample_matrix_group.csv
- Convert name_vi sang Sentence case
- Mapping sample_matrix_group (tên) sang sample_matrix_group_id (ID)
- Insert vào database với encoding UTF-8
"""

import csv
import pyodbc
import sys
import os
import re
from datetime import datetime, timezone
from typing import Dict, Optional, Tuple

# Cấu hình kết nối database (lấy từ appsettings.json)
CONNECTION_STRING = (
    "Driver={ODBC Driver 17 for SQL Server};"
    "Server=192.168.2.21,49938;"
    "Database=VietLabs;"
    "UID=sa;"
    "PWD=Sbt@2024;"
    "TrustServerCertificate=yes;"
    "Encrypt=yes;"
)

# Đường dẫn đến file CSV (từ folder data_import, cần lên 1 cấp)
CSV_DIR = os.path.join(os.path.dirname(os.path.dirname(__file__)), "csv")
SAMPLE_MATRIX_CSV = os.path.join(CSV_DIR, "sample_matrix.csv")
SAMPLE_MATRIX_GROUP_CSV = os.path.join(CSV_DIR, "sample_matrix_group.csv")


def preserve_excel_text(text: str) -> str:
    """Giữ nguyên hoa thường từ Excel, chỉ trim khoảng trắng."""
    return text.strip() if text else text


def to_sentence_case(text: str) -> str:
    """
    Chuyển đổi text sang Sentence case (chữ cái đầu viết hoa, các chữ còn lại viết thường)
    Ví dụ: "BAO BÌ" -> "Bao bì", "NƯỚC ĂN UỐNG" -> "Nước ăn uống"
    """
    if not text or not text.strip():
        return text
    
    # Loại bỏ khoảng trắng thừa
    text = text.strip()
    
    # Chuyển toàn bộ sang chữ thường trước
    text_lower = text.lower()
    
    # Chuyển chữ cái đầu của mỗi từ thành chữ hoa
    words = text_lower.split()
    words_capitalized = [word.capitalize() for word in words]
    
    return " ".join(words_capitalized)


def normalize_text(text: str) -> str:
    """
    Chuẩn hóa text để so sánh (loại bỏ khoảng trắng, dấu câu, chuyển sang uppercase)
    """
    if not text:
        return ""
    # Loại bỏ khoảng trắng, dấu câu, chuyển sang uppercase
    text = text.strip().upper()
    # Loại bỏ các ký tự đặc biệt và khoảng trắng (giữ lại chữ cái, số, dấu tiếng Việt)
    text = re.sub(r'[^\w]', '', text)
    return text


def load_sample_matrix_groups(csv_path: str) -> Tuple[Dict[str, str], Dict[str, str]]:
    """
    Đọc file sample_matrix_group.csv và tạo mapping từ name_vi (normalized) sang ID
    Returns: Tuple (mapping từ normalized name_vi sang ID, mapping từ ID sang name_vi)
    """
    mapping = {}
    id_to_name = {}
    
    try:
        with open(csv_path, 'r', encoding='utf-8-sig') as f:
            # Đọc với delimiter là semicolon
            reader = csv.DictReader(f, delimiter=';')
            
            for row in reader:
                # Lấy các cột (có thể có khoảng trắng trong tên cột)
                group_id = row.get('sample matrix_group_id', '').strip()
                name_vi = row.get('name_vi', '').strip()
                
                if group_id and name_vi:
                    id_to_name[group_id] = preserve_excel_text(name_vi)
                    
                    # Tạo mapping với normalized name_vi
                    normalized_name = normalize_text(name_vi)
                    if normalized_name:
                        mapping[normalized_name] = group_id
                        
                        # Thêm các biến thể mapping để tăng khả năng match
                        # Mapping với sentence case
                        sentence_case = to_sentence_case(name_vi)
                        mapping[normalize_text(sentence_case)] = group_id
                        
                        # Mapping với uppercase (trường hợp trong CSV có thể là uppercase)
                        mapping[normalize_text(name_vi.upper())] = group_id
                        
    except FileNotFoundError:
        print(f"Loi: Khong tim thay file {csv_path}")
        sys.exit(1)
    except Exception as e:
        print(f"Loi khi doc file {csv_path}: {e}")
        sys.exit(1)
    
    return mapping, id_to_name


def process_sample_matrix_csv(csv_path: str, group_mapping: Dict[str, str], id_to_name: Dict[str, str]) -> list:
    """
    Đọc file sample_matrix.csv, xử lý và trả về danh sách records để insert
    """
    records = []
    errors = []
    
    try:
        with open(csv_path, 'r', encoding='utf-8-sig') as f:
            # Đọc tất cả các dòng
            lines = f.readlines()
            
            # Tìm dòng header (dòng đầu tiên không phải comment)
            header_line = None
            start_index = 0
            for i, line in enumerate(lines):
                line_stripped = line.strip()
                if line_stripped and not line_stripped.startswith(';') and 'sample_matrix_id' in line_stripped:
                    header_line = line_stripped
                    start_index = i + 1
                    break
            
            if not header_line:
                print(f"Loi: Khong tim thay header trong file {csv_path}")
                return records
            
            # Parse header
            header_fields = [field.strip() for field in header_line.split(';')]
            
            # Đọc các dòng dữ liệu
            for i in range(start_index, len(lines)):
                line = lines[i].strip()
                if not line or line.startswith(';'):
                    continue
                
                # Parse dòng dữ liệu
                values = [v.strip() for v in line.split(';')]
                if len(values) < len(header_fields):
                    # Bổ sung các giá trị trống nếu thiếu
                    values.extend([''] * (len(header_fields) - len(values)))
                
                # Tạo dictionary từ header và values
                row = dict(zip(header_fields, values))
                
                # Kiểm tra nếu là dòng trống hoặc không có dữ liệu
                if not row or all(not v or v.strip() == '' for v in row.values()):
                    continue
                
                sample_matrix_id = row.get('sample_matrix_id', '').strip()
                sample_matrix_code = row.get('sample_matrix_code', '').strip()
                name_vi_raw = row.get('name_vi', '').strip()
                sample_matrix_group_name = row.get('sample_matrix_group', '').strip()
                
                # Bỏ qua nếu thiếu thông tin cần thiết
                if not sample_matrix_id or not name_vi_raw:
                    continue
                
                name_vi = preserve_excel_text(name_vi_raw)
                
                # Tìm sample_matrix_group_id từ mapping
                normalized_group_name = normalize_text(sample_matrix_group_name)
                sample_matrix_group_id = group_mapping.get(normalized_group_name)
                
                if not sample_matrix_group_id:
                    errors.append(f"Khong tim thay sample_matrix_group_id cho '{sample_matrix_group_name}' (row: {sample_matrix_id})")
                    continue
                
                # Tạo record
                group_name_excel = preserve_excel_text(sample_matrix_group_name) if sample_matrix_group_name else None
                group_name_sentence = id_to_name.get(sample_matrix_group_id) or group_name_excel
                
                record = {
                    'sample_matrix_id': sample_matrix_id,
                    'sample_matrix_code': sample_matrix_code if sample_matrix_code else None,
                    'name_vi': name_vi,
                    'name_en': None,  # Có thể thêm sau nếu cần
                    'sample_matrix_group_id': sample_matrix_group_id,
                    'sample_matrix_group': group_name_sentence,
                    'registered_matrix': None,  # Có thể thêm sau nếu cần
                    'status': 'Active',
                    'notes': None,
                    'created_at': datetime.now(timezone.utc),
                    'updated_at': None
                }
                
                records.append(record)
                
    except FileNotFoundError:
        print(f"Loi: Khong tim thay file {csv_path}")
        sys.exit(1)
    except Exception as e:
        print(f"Loi khi doc file {csv_path}: {e}")
        sys.exit(1)
    
    if errors:
        print("\nCanh bao - Cac loi mapping:")
        for error in errors:
            print(f"  - {error}")
    
    return records


def insert_records(connection, records: list):
    """
    Insert records vào database
    """
    cursor = connection.cursor()
    
    inserted_count = 0
    updated_count = 0
    error_count = 0
    
    for record in records:
        try:
            # Kiểm tra xem record đã tồn tại chưa
            check_query = """
                SELECT sample_matrix_id 
                FROM sample_matrix 
                WHERE sample_matrix_id = ?
            """
            cursor.execute(check_query, record['sample_matrix_id'])
            exists = cursor.fetchone()
            
            if exists:
                # Update record nếu đã tồn tại
                update_query = """
                    UPDATE sample_matrix
                    SET sample_matrix_code = ?,
                        name_vi = ?,
                        name_en = ?,
                        sample_matrix_group_id = ?,
                        sample_matrix_group = ?,
                        registered_matrix = ?,
                        status = ?,
                        notes = ?,
                        updated_at = ?
                    WHERE sample_matrix_id = ?
                """
                cursor.execute(update_query, (
                    record['sample_matrix_code'],
                    record['name_vi'],
                    record['name_en'],
                    record['sample_matrix_group_id'],
                    record['sample_matrix_group'],
                    record['registered_matrix'],
                    record['status'],
                    record['notes'],
                    datetime.now(timezone.utc),
                    record['sample_matrix_id']
                ))
                updated_count += 1
            else:
                # Insert record mới
                insert_query = """
                    INSERT INTO sample_matrix (
                        sample_matrix_id,
                        sample_matrix_code,
                        name_vi,
                        name_en,
                        sample_matrix_group_id,
                        sample_matrix_group,
                        registered_matrix,
                        status,
                        notes,
                        created_at,
                        updated_at
                    ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
                """
                cursor.execute(insert_query, (
                    record['sample_matrix_id'],
                    record['sample_matrix_code'],
                    record['name_vi'],
                    record['name_en'],
                    record['sample_matrix_group_id'],
                    record['sample_matrix_group'],
                    record['registered_matrix'],
                    record['status'],
                    record['notes'],
                    record['created_at'],
                    record['updated_at']
                ))
                inserted_count += 1
                
        except Exception as e:
            error_count += 1
            print(f"Loi khi xu ly record {record['sample_matrix_id']}: {e}")
            continue
    
    connection.commit()
    
    print(f"\nKet qua:")
    print(f"  - Da insert: {inserted_count} records")
    print(f"  - Da update: {updated_count} records")
    print(f"  - Loi: {error_count} records")


def main():
    # Thiết lập encoding cho console trên Windows
    if sys.platform == 'win32':
        import io
        sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8', errors='replace')
        sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8', errors='replace')
    
    print("=" * 60)
    print("Script import sample_matrix tu CSV vao database")
    print("=" * 60)
    
    # Kiểm tra file CSV tồn tại
    if not os.path.exists(SAMPLE_MATRIX_CSV):
        print(f"Loi: Khong tim thay file {SAMPLE_MATRIX_CSV}")
        sys.exit(1)
    
    if not os.path.exists(SAMPLE_MATRIX_GROUP_CSV):
        print(f"Loi: Khong tim thay file {SAMPLE_MATRIX_GROUP_CSV}")
        sys.exit(1)
    
    # Load mapping từ sample_matrix_group
    print("\n1. Dang load mapping tu sample_matrix_group...")
    group_mapping, id_to_name = load_sample_matrix_groups(SAMPLE_MATRIX_GROUP_CSV)
    print(f"   Da load {len(group_mapping)} mappings")
    
    # Process sample_matrix CSV
    print("\n2. Dang xu ly file sample_matrix.csv...")
    records = process_sample_matrix_csv(SAMPLE_MATRIX_CSV, group_mapping, id_to_name)
    print(f"   Da xu ly {len(records)} records")
    
    if not records:
        print("Khong co records nao de insert!")
        sys.exit(1)
    
    # Kết nối database và insert
    print("\n3. Dang ket noi database...")
    try:
        connection = pyodbc.connect(CONNECTION_STRING)
        print("   Ket noi thanh cong!")
        
        print("\n4. Dang insert/update records...")
        insert_records(connection, records)
        
        connection.close()
        print("\nHoan thanh!")
        
    except pyodbc.Error as e:
        print(f"Loi ket noi database: {e}")
        sys.exit(1)
    except Exception as e:
        print(f"Loi: {e}")
        sys.exit(1)


if __name__ == "__main__":
    main()

