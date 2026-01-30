# Database Schema - VietLabs CRM

## Tổng quan

Database **VietLabs** là hệ thống quản lý CRM và Báo giá cho phòng thí nghiệm VietLabs. Database này quản lý thông tin khách hàng, nhân viên, báo giá, và các chỉ tiêu phân tích.

## Danh sách các bảng

### 1. account (Tài khoản)
Quản lý tài khoản đăng nhập của nhân viên.

**Các cột chính:**
- `account_id` (PK): ID tài khoản
- `employee_id` (FK): Liên kết đến nhân viên
- `permission_id` (FK): Liên kết đến quyền
- `user_name`: Tên đăng nhập
- `password_hash`: Mật khẩu đã hash
- `status`: Trạng thái (Active, Inactive)

**Quan hệ:**
- 1-1 với `employee`
- Many-to-1 với `permission`

---

### 2. client (Khách hàng)
Quản lý thông tin khách hàng.

**Các cột chính:**
- `client_id` (PK): ID khách hàng
- `company_name`: Tên công ty
- `tax_code`: Mã số thuế
- `address`, `city`, `country`: Địa chỉ
- `customer_type`: Loại khách hàng (Enterprise, SMB, Prospect)
- `discount_rate`: Mức chiết khấu (%)
- `status`: Trạng thái (Active, Inactive, Prospect)

**Quan hệ:**
- 1-n với `contact`
- 1-n với `quotation`
- 1-1 với `client_debt`
- 1-n với `client_forecast`

---

### 3. contact (Người liên hệ)
Quản lý thông tin người liên hệ của khách hàng.

**Các cột chính:**
- `contact_id` (PK): ID người liên hệ
- `client_id` (FK): Liên kết đến khách hàng
- `full_name`: Họ và tên
- `email`, `phone`: Thông tin liên lạc
- `is_primary`: Là người liên hệ chính
- `is_sample_sender`: Người gửi mẫu
- `is_result_receiver`: Người nhận kết quả
- `is_payer`: Người thanh toán

**Quan hệ:**
- Many-to-1 với `client`

---

### 4. employee (Nhân viên)
Quản lý thông tin nhân viên.

**Các cột chính:**
- `employee_id` (PK): ID nhân viên
- `employee_code`: Mã nhân viên
- `full_name`: Họ và tên
- `department`: Phòng ban
- `role`: Vai trò
- `email`: Email
- `status`: Trạng thái

**Quan hệ:**
- 1-1 với `account`
- 1-n với `quotation`

---

### 5. branch (Chi nhánh)
Quản lý thông tin chi nhánh.

**Các cột chính:**
- `branch_id` (PK): ID chi nhánh
- `branch_code`: Mã chi nhánh
- `name_vi`, `name_en`: Tên chi nhánh (Tiếng Việt, Tiếng Anh)
- `license`: Chứng nhận hoạt động
- `status`: Trạng thái

**Quan hệ:**
- 1-n với `department`

---

### 6. department (Phòng ban)
Quản lý thông tin phòng ban.

**Các cột chính:**
- `department_id` (PK): ID phòng ban
- `branch_id` (FK): Liên kết đến chi nhánh
- `department_code`: Mã phòng ban
- `name_vi`, `name_en`: Tên phòng ban
- `status`: Trạng thái

**Quan hệ:**
- Many-to-1 với `branch`
- 1-n với `department_analysis_capability`

---

### 7. permission (Quyền)
Quản lý quyền truy cập.

**Các cột chính:**
- `permission_id` (PK): ID quyền
- `permission_code`: Mã quyền
- `name`: Tên quyền
- `status`: Trạng thái

**Quan hệ:**
- 1-n với `account`

---

### 8. quotation (Báo giá)
Quản lý báo giá cho khách hàng.

**Các cột chính:**
- `quotation_id` (PK): ID báo giá
- `quotation_code`: Mã báo giá
- `client_id` (FK): Liên kết đến khách hàng
- `employee_id` (FK): Nhân viên tạo báo giá
- `contact_id` (FK): Người liên hệ
- `sub_total`: Tạm tính
- `total_discount`: Tổng giảm
- `vat_amount`: VAT
- `total_amount`: Tổng đơn giá
- `status`: Trạng thái (Draft, Sent, Approved, Rejected, Expired)
- `valid_from`, `valid_to`: Hiệu lực báo giá

**Quan hệ:**
- Many-to-1 với `client`
- Many-to-1 với `employee`
- Many-to-1 với `contact`
- 1-n với `quotation_item`

---

### 9. quotation_item (Chi tiết báo giá)
Chi tiết các item trong báo giá.

**Các cột chính:**
- `quotation_item_id` (PK): ID chi tiết
- `quotation_id` (FK): Liên kết đến báo giá
- `item_type`: Loại item (AnalysisItem, AnalysisGroup, Package)
- `analysis_item_id` (FK): Nếu là chỉ tiêu
- `analysis_group_id` (FK): Nếu là nhóm chỉ tiêu
- `package_id` (FK): Nếu là gói
- `quantity`: Số lượng
- `unit_price`: Đơn giá
- `discount_percent`: Giảm giá (%)
- `sub_total`: Thành tiền

**Quan hệ:**
- Many-to-1 với `quotation`
- Many-to-1 với `analysis_item` (optional)
- Many-to-1 với `analysis_group` (optional)
- Many-to-1 với `package` (optional)

**Lưu ý:** Chỉ một trong 3 foreign keys (analysis_item_id, analysis_group_id, package_id) có giá trị tùy theo item_type.

---

### 10. analysis_group (Nhóm chỉ tiêu)
Quản lý nhóm chỉ tiêu phân tích.

**Các cột chính:**
- `analysis_group_id` (PK): ID nhóm chỉ tiêu
- `analysis_group_code`: Mã nhóm
- `name_vi`, `name_en`: Tên nhóm
- `status`: Trạng thái

**Quan hệ:**
- 1-n với `analysis_item`
- Many-to-many với `package` (qua `package_analysis_group`)

---

### 11. analysis_item (Chỉ tiêu phân tích)
Quản lý các chỉ tiêu phân tích cụ thể.

**Các cột chính:**
- `analysis_item_id` (PK): ID chỉ tiêu
- `analysis_item_code`: Mã chỉ tiêu
- `name_vi`, `name_en`: Tên chỉ tiêu
- `equipment_type_id` (FK): Loại thiết bị
- `analysis_group_id` (FK): Nhóm chỉ tiêu
- `sample_matrix_id` (FK): Mẫu
- `sample_matrix_group_id` (FK): Nhóm mẫu
- `lod`, `loq`: Giới hạn phát hiện/định lượng
- `unit`: Đơn vị
- Các flag: `nd107`, `iso`, `cuc_bvtv`, `bo_cong_thuong`, `nafi`, `cuc_chan_nuoi`

**Quan hệ:**
- Many-to-1 với `equipment_type`
- Many-to-1 với `analysis_group`
- Many-to-1 với `sample_matrix`
- Many-to-1 với `sample_matrix_group`
- 1-n với `department_analysis_capability`

---

### 12. package (Gói)
Quản lý các gói phân tích.

**Các cột chính:**
- `package_id` (PK): ID gói
- `package_code`: Mã gói
- `name_vi`, `name_en`: Tên gói
- `default_price`: Giá mặc định
- `status`: Trạng thái

**Quan hệ:**
- Many-to-many với `analysis_group` (qua `package_analysis_group`)

---

### 13. package_analysis_group (Gói - Nhóm chỉ tiêu)
Bảng trung gian cho quan hệ many-to-many.

**Các cột chính:**
- `package_analysis_group_id` (PK): ID
- `package_id` (FK): Liên kết đến gói
- `analysis_group_id` (FK): Liên kết đến nhóm chỉ tiêu
- `display_order`: Thứ tự hiển thị
- `is_required`: Bắt buộc hay không

---

### 14. client_debt (Công nợ khách hàng)
Quản lý công nợ của khách hàng (1-1 với Client).

**Các cột chính:**
- `client_debt_id` (PK): ID công nợ
- `client_id` (FK): Liên kết đến khách hàng
- `total_debt`: Tổng công nợ
- `debt_term_days`: Thời hạn công nợ (ngày)
- `credit_limit`: Hạn mức dư nợ
- `contract_effective_date`, `contract_end_date`: Thời hạn hợp đồng
- `last_synced_at`: Thời gian sync từ MISA
- `misa_reference_id`: ID tham chiếu từ MISA

**Quan hệ:**
- 1-1 với `client`

---

### 15. client_forecast (Forecast khách hàng)
Quản lý forecast của khách hàng theo thời gian.

**Các cột chính:**
- `client_forecast_id` (PK): ID forecast
- `client_id` (FK): Liên kết đến khách hàng
- `from_date`, `to_date`: Khoảng thời gian
- `forecast_amount`: Forecast (số tiền)

**Quan hệ:**
- Many-to-1 với `client`

---

### 16. sample_matrix_group (Nhóm mẫu)
Quản lý nhóm mẫu.

**Các cột chính:**
- `sample_matrix_group_id` (PK): ID nhóm mẫu
- `sample_matrix_group_code`: Mã nhóm
- `name_vi`, `name_en`: Tên nhóm mẫu
- `status`: Trạng thái

**Quan hệ:**
- 1-n với `sample_matrix`

---

### 17. sample_matrix (Mẫu)
Quản lý các mẫu phân tích.

**Các cột chính:**
- `sample_matrix_id` (PK): ID mẫu
- `sample_matrix_code`: Mã mẫu
- `name_vi`, `name_en`: Tên mẫu
- `sample_matrix_group_id` (FK): Liên kết đến nhóm mẫu
- `registered_matrix`: Mẫu đã đăng ký
- `status`: Trạng thái

**Quan hệ:**
- Many-to-1 với `sample_matrix_group`

---

### 18. equipment_type (Loại thiết bị)
Quản lý loại thiết bị phân tích.

**Các cột chính:**
- `equipment_type_id` (PK): ID loại thiết bị
- `equipment_type_code`: Mã loại thiết bị
- `name_vi`, `name_en`: Tên loại thiết bị
- `status`: Trạng thái

**Quan hệ:**
- 1-n với `analysis_item`

---

### 19. department_analysis_capability (Khả năng phân tích của phòng ban)
Quản lý khả năng phân tích các chỉ tiêu của từng phòng ban.

**Các cột chính:**
- `department_analysis_capability_id` (PK): ID
- `department_id` (FK): Liên kết đến phòng ban
- `branch_id`: ID chi nhánh (derived)
- `analysis_item_id` (FK): Liên kết đến chỉ tiêu
- `status`: Trạng thái

**Quan hệ:**
- Many-to-1 với `department`
- Many-to-1 với `analysis_item`

---

### 20. refresh_token (Refresh Token)
Quản lý refresh token cho authentication.

**Các cột chính:**
- `refresh_token_id` (PK): ID token
- `account_id` (FK): Liên kết đến tài khoản
- `token`: Token string
- `expires_at`: Thời gian hết hạn
- `is_revoked`: Đã bị thu hồi

**Quan hệ:**
- Many-to-1 với `account`

---

## Business Rules

### Quotation
- Báo giá có thể chứa 3 loại item: `AnalysisItem`, `AnalysisGroup`, hoặc `Package`
- Trạng thái: `Draft`, `Sent`, `Approved`, `Rejected`, `Expired`

### QuotationItem
- Chỉ một trong 3 foreign keys (`analysis_item_id`, `analysis_group_id`, `package_id`) có giá trị tùy theo `item_type`

### Client
- Trạng thái: `Active`, `Inactive`, `Prospect`
- Loại khách hàng: `Enterprise`, `SMB`, `Prospect`

---

## UI Recommendations

### Client Form
**Các section:**
1. Thông tin cơ bản: company_name, tax_code, address
2. Thông tin người đại diện: representative_name, representative_email
3. Thông tin nhân viên phụ trách: sales_owner_name, sales_owner_email
4. Thông tin bổ sung: profession, scale, customer_type, discount_rate

**ListView:**
- Default columns: company_name, tax_code, city, customer_type, status
- Sortable: company_name, created_date, status
- Filterable: status, customer_type, city

### Quotation Form
**Các section:**
1. Thông tin nhân viên: employee_id, sales_person_name
2. Thông tin khách hàng: client_id, contact_id, company_name
3. Chi tiết báo giá: quotation_items (dạng grid)
4. Tổng kết: sub_total, total_discount, vat_amount, total_amount
5. Hiệu lực: valid_from, valid_to, status

**ListView:**
- Default columns: quotation_code, company_name, status, total_amount, created_at
- Sortable: quotation_code, created_at, total_amount
- Filterable: status, client_id, employee_id

---

## File Schema JSON

File `DatabaseSchema.json` chứa đầy đủ thông tin chi tiết về:
- Tất cả các bảng và cột
- Kiểu dữ liệu và constraints
- Foreign keys và relationships
- Business rules
- UI recommendations

File này có thể được sử dụng bởi các AI models để tự động tạo UI forms và views.

