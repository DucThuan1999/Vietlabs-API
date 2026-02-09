# Kế hoạch Import Analysis Item từ CSV

## 1. Phân tích cấu trúc CSV

### Các cột trong file `analysis_item.csv`:

| Cột CSV | Mô tả | Mapping Database | Ghi chú |
|---------|-------|------------------|---------|
| `analysis_group` | Tên nhóm chỉ tiêu | `analysis_group_id` (FK) | Cần mapping từ tên → ID |
| `analysis_item_name_vi` | Tên chỉ tiêu (Tiếng Việt) | `name_vi` | Sentence case |
| `analysis_item_name_en` | Tên chỉ tiêu (Tiếng Anh) | `name_en` | Giữ nguyên |
| `public_group_code` | Mã nhóm công bố | `published_group_code` | Giữ nguyên |
| `Equipment_type` | Loại thiết bị | `equipment_type_id` (FK) | Cần mapping từ tên → ID |
| `LOD` | Limit of Detection | `lod` | Parse decimal, xử lý đơn vị |
| `LOQ` | Limit of Quantification | `loq` | Parse decimal, xử lý đơn vị |
| `TAT_Normal` | Turnaround Time Normal | `analysis_item_tat` | Lưu vào bảng riêng |
| `TAT_Fast` | Turnaround Time Fast | `analysis_item_tat` | Lưu vào bảng riêng |
| `TAT_Urgent` | Turnaround Time Urgent | `analysis_item_tat` | Lưu vào bảng riêng |
| `Analysis Group Whole group standard` | Giá nhóm chuẩn | `analysis_group.whole_group_standard_price` | Cập nhật AnalysisGroup |

## 2. Cấu trúc Database

### Bảng `analysis_item` (Required fields):

**Primary Key:**
- `analysis_item_id` (Guid) - Tự generate

**Foreign Keys (Required):**
- `equipment_type_id` (Guid) - Từ mapping `Equipment_type`
- `analysis_group_id` (Guid) - Từ mapping `analysis_group`
- `sample_matrix_id` (Guid) - **THIẾU trong CSV** - Cần xử lý
- `sample_matrix_group_id` (Guid) - **THIẾU trong CSV** - Cần xử lý

**Fields từ CSV:**
- `name_vi` (string) - Từ `analysis_item_name_vi`
- `name_en` (string) - Từ `analysis_item_name_en`
- `published_group_code` (string) - Từ `public_group_code`
- `lod` (decimal?) - Parse từ `LOD`
- `loq` (decimal?) - Parse từ `LOQ`
- `unit_price` (decimal) - **THIẾU trong CSV** - Default = 0
- `unit` (string) - **THIẾU trong CSV** - Extract từ LOD/LOQ hoặc NULL

**Fields mặc định:**
- `status` = "Active"
- `nd_107`, `iso`, `cuc_bvtv`, `bo_cong_thuong`, `nafi`, `cuc_chan_nuoi` = false
- `created_at` = DateTime.UtcNow
- `updated_at` = NULL

## 3. Các vấn đề cần xử lý

### 3.1. Mapping Foreign Keys

#### a) AnalysisGroup Mapping
- **Nguồn:** Cột `analysis_group` trong CSV
- **Đích:** `analysis_group_id` trong database
- **Cách làm:**
  1. Đọc tất cả `analysis_group` từ database
  2. Tạo mapping: `name_vi` (normalized) → `analysis_group_id`
  3. Nếu không tìm thấy → tạo mới AnalysisGroup hoặc bỏ qua record

#### b) EquipmentType Mapping
- **Nguồn:** Cột `Equipment_type` trong CSV
- **Đích:** `equipment_type_id` trong database
- **Cách làm:**
  1. Đọc tất cả `equipment_type` từ database
  2. Tạo mapping: `name_vi` hoặc `name_en` (normalized) → `equipment_type_id`
  3. Nếu không tìm thấy → tạo mới EquipmentType hoặc bỏ qua record

#### c) SampleMatrix & SampleMatrixGroup
- **Vấn đề:** CSV không có thông tin về sample matrix
- **Giải pháp:**
  - Option 1: Yêu cầu thêm cột vào CSV
  - Option 2: Sử dụng default/mẫu chung
  - Option 3: Bỏ qua nếu không bắt buộc (nhưng theo schema là required)

### 3.2. Parse LOD/LOQ

- **Format:** Có thể chứa số + đơn vị (ví dụ: "0.05 µg/mL", "0,25 µg/g")
- **Xử lý:**
  1. Extract số (xử lý dấu phẩy/thập phân)
  2. Extract đơn vị (lưu vào `unit` nếu chưa có)
  3. Parse thành decimal

### 3.3. TAT (Turnaround Time)

- **Vấn đề:** TAT không có trong model `AnalysisItem`
- **Giải pháp:** Lưu vào bảng `analysis_item_tat` (nếu có)
- **Cấu trúc:**
  - `analysis_item_id` (FK)
  - `tat_type` ("Normal", "Fast", "Urgent")
  - `tat_value` (int - số giờ)

### 3.4. Analysis Group Whole Group Standard

- **Vấn đề:** Giá trị này thuộc về `AnalysisGroup`, không phải `AnalysisItem`
- **Giải pháp:** 
  - Cập nhật `whole_group_standard_price` trong bảng `analysis_group`
  - Hoặc lưu vào `notes` của AnalysisItem

## 4. Kế hoạch thực hiện

### Bước 1: Chuẩn bị dữ liệu mapping

1. **Load AnalysisGroup từ database:**
   - Tạo mapping: `name_vi` (normalized) → `analysis_group_id`
   - Tạo mapping: `name_en` (normalized) → `analysis_group_id`

2. **Load EquipmentType từ database:**
   - Tạo mapping: `name_vi` (normalized) → `equipment_type_id`
   - Tạo mapping: `name_en` (normalized) → `equipment_type_id`

3. **Load SampleMatrix từ database:**
   - Tạo mapping: `name_vi` (normalized) → `sample_matrix_id` + `sample_matrix_group_id`
   - Hoặc sử dụng default sample matrix

### Bước 2: Đọc và xử lý CSV

1. **Đọc file CSV:**
   - Encoding: UTF-8
   - Delimiter: `;` (semicolon)
   - Bỏ qua dòng header

2. **Xử lý từng record:**
   - Parse các cột
   - Convert `name_vi` sang Sentence case
   - Parse LOD/LOQ (extract số và đơn vị)
   - Mapping foreign keys

### Bước 3: Validation

1. **Kiểm tra required fields:**
   - `analysis_group_id` - phải có
   - `equipment_type_id` - phải có
   - `sample_matrix_id` - phải có (cần xử lý)
   - `sample_matrix_group_id` - phải có (cần xử lý)

2. **Kiểm tra format:**
   - LOD/LOQ phải parse được thành decimal
   - TAT phải là số nguyên

### Bước 4: Insert vào database

1. **Insert/Update AnalysisItem:**
   - Nếu `analysis_item_id` đã tồn tại → Update
   - Nếu chưa → Insert mới (generate Guid)

2. **Insert AnalysisItemTat (nếu có TAT):**
   - Insert 3 records cho Normal, Fast, Urgent (nếu có giá trị)

3. **Update AnalysisGroup (nếu có Whole Group Standard):**
   - Update `whole_group_standard_price` nếu có giá trị

## 5. Script Python cần tạo

### File: `import_analysis_item.py`

**Chức năng:**
1. Load mappings từ database
2. Đọc và parse CSV
3. Validate dữ liệu
4. Insert/Update vào database
5. Xử lý TAT (nếu có bảng analysis_item_tat)

**Dependencies:**
- `pyodbc` - Kết nối SQL Server
- `csv` - Đọc CSV
- `re` - Parse LOD/LOQ
- `uuid` - Generate Guid

## 6. Các vấn đề cần quyết định

### 6.1. Sample Matrix
- **Câu hỏi:** CSV không có thông tin sample matrix, làm thế nào?
- **Đề xuất:** 
  - Thêm cột vào CSV, hoặc
  - Sử dụng một sample matrix mặc định, hoặc
  - Yêu cầu user cung cấp mapping

### 6.2. Unit Price
- **Câu hỏi:** CSV không có unit_price, dùng giá trị nào?
- **Đề xuất:** Default = 0, hoặc tính từ AnalysisGroup

### 6.3. Analysis Item Code
- **Câu hỏi:** CSV không có analysis_item_code, có cần generate không?
- **Đề xuất:** Generate tự động (ví dụ: "AI-0001", "AI-0002", ...)

### 6.4. TAT
- **Câu hỏi:** Có bảng `analysis_item_tat` không? Cấu trúc như thế nào?
- **Cần kiểm tra:** Model `AnalysisItemTat`

## 7. Checklist trước khi import

- [ ] Kiểm tra file CSV có đầy đủ dữ liệu
- [ ] Load và verify mappings (AnalysisGroup, EquipmentType)
- [ ] Xử lý vấn đề SampleMatrix (thêm cột hoặc default)
- [ ] Test parse LOD/LOQ với các format khác nhau
- [ ] Kiểm tra bảng `analysis_item_tat` (nếu có)
- [ ] Backup database trước khi import
- [ ] Test với một vài records trước
- [ ] Xử lý encoding UTF-8 cho tiếng Việt

## 8. Ước tính thời gian

- **Phân tích và thiết kế:** ✅ Hoàn thành
- **Viết script:** ~2-3 giờ
- **Testing và debug:** ~1-2 giờ
- **Import thực tế:** Tùy số lượng records

## 9. Rủi ro và giải pháp

| Rủi ro | Giải pháp |
|--------|-----------|
| CSV thiếu sample_matrix | Thêm cột hoặc dùng default |
| Mapping không tìm thấy | Log lỗi, bỏ qua record |
| Parse LOD/LOQ sai | Validate và log warning |
| Duplicate records | Check trước khi insert |
| Encoding issues | Sử dụng UTF-8-sig |


