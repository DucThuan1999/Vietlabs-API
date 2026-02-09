# Hướng dẫn import sample_matrix từ CSV

Script này dùng để import dữ liệu từ file CSV vào bảng `sample_matrix` trong database.

## Yêu cầu

1. Python 3.6 trở lên
2. Thư viện `pyodbc` (sẽ tự động cài đặt khi chạy script)
3. ODBC Driver 17 for SQL Server (hoặc driver tương thích)
4. File CSV trong thư mục `csv/`:
   - `sample_matrix.csv`
   - `sample_matrix_group.csv`

## Cài đặt

### Cài đặt Python dependencies

```bash
pip install -r requirements.txt
```

Hoặc cài đặt trực tiếp:

```bash
pip install pyodbc
```

### Cài đặt ODBC Driver

**Windows:**
- Tải và cài đặt [ODBC Driver 17 for SQL Server](https://docs.microsoft.com/en-us/sql/connect/odbc/download-odbc-driver-for-sql-server)

**Linux:**
```bash
# Ubuntu/Debian
sudo apt-get install unixodbc-dev
sudo apt-get install odbcinst1debian2

# CentOS/RHEL
sudo yum install unixODBC-devel
```

## Cấu hình

Chỉnh sửa connection string trong file `import_sample_matrix.py` nếu cần:

```python
CONNECTION_STRING = (
    "Driver={ODBC Driver 17 for SQL Server};"
    "Server=192.168.2.21,49938;"
    "Database=VietLabs;"
    "UID=sa;"
    "PWD=Sbt@2024;"
    "TrustServerCertificate=yes;"
    "Encrypt=yes;"
)
```

## Chạy script

### Windows

Chạy file batch:
```bash
run_import_sample_matrix.bat
```

Hoặc chạy trực tiếp:
```bash
python import_sample_matrix.py
```

### Linux/Mac

```bash
python3 import_sample_matrix.py
```

## Chức năng

1. **Đọc mapping từ `sample_matrix_group.csv`**: Tạo mapping từ tên nhóm (name_vi) sang ID
2. **Xử lý `sample_matrix.csv`**:
   - Convert `name_vi` sang Sentence case (ví dụ: "BAO BÌ" → "Bao bì")
   - Mapping `sample_matrix_group` (tên) sang `sample_matrix_group_id` (ID)
   - Xử lý encoding UTF-8
3. **Insert/Update vào database**:
   - Nếu record đã tồn tại (theo `sample_matrix_id`), sẽ update
   - Nếu chưa tồn tại, sẽ insert mới

## Lưu ý

- File CSV phải có encoding UTF-8 với BOM (UTF-8-sig)
- Delimiter trong CSV là semicolon (`;`)
- Script sẽ tự động bỏ qua các dòng comment (bắt đầu bằng `;`)
- `name_vi` sẽ được tự động chuyển sang Sentence case
- Nếu không tìm thấy mapping cho `sample_matrix_group`, record đó sẽ bị bỏ qua và hiển thị cảnh báo

## Xử lý lỗi

Nếu gặp lỗi kết nối database:
- Kiểm tra connection string
- Kiểm tra ODBC Driver đã được cài đặt
- Kiểm tra quyền truy cập database

Nếu gặp lỗi encoding:
- Đảm bảo file CSV được lưu với encoding UTF-8 với BOM


