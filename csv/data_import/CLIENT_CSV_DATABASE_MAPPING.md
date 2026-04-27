# Mapping `Client.csv` → SQL Server (VietLabs)

Tài liệu đối chiếu với script [`import_customer_csv.py`](import_customer_csv.py). Cột DB dùng **snake_case** như `INFORMATION_SCHEMA` / EF (bảng `dbo`).

---

## 1. Định dạng file

| Thuộc tính | Giá trị |
|------------|---------|
| Đường dẫn mặc định | `data/Client.csv` (tính từ root repo Vietlabs) |
| Encoding | UTF-8 với BOM (`utf-8-sig`) |
| Delimiter | `;` |
| Số cột | **51** (`EXPECTED_COLS`) — thiếu được pad rỗng, thừa bị cắt |
| Header | **Một dòng** — cột 44, 49–51 trong `Client.csv` hiện không còn ký tự xuống dòng trong tên cột (khớp `EXPECTED_CLIENT_CSV_HEADERS` trong script). |

**Dòng dữ liệu**

- **Client.csv:** dòng ngay sau header: nếu cột 1 **không** trống → coi là dòng khách hàng đầu tiên.
- **Customer.csv:** nếu cột 1 của dòng đầu sau header **trống** → bỏ thêm 2 dòng mô tả, dữ liệu từ dòng thứ 4.

---

## 2. Ghi đè theo **vị trí cột** (1-based)

Sau khi map theo header, script **ghi đè** giá trị sau bằng ô đúng **chỉ số cột** (tránh lệch khi header template):

| Cột (1-based) | Key nội bộ | Ghi chú |
|---------------|------------|---------|
| 17 | `profession` | catalog |
| 18 | `scale` | |
| 19 | `customerType` | |
| 34 | `contactFullName` | người liên hệ |
| 35 | `contactEmail` | |
| 36 | `contactPhone` | |
| 37 | `contactDepartment` | |
| 38 | `contactTitle` | |
| 39 | `contactIsPrimary` | |
| 40 | `contactIsSampleSender` | |
| 41 | `contactIsResultReceiver` | |
| 42 | `contactIsPayer` | |
| 43 | `contactNotes` | |
| 44 | `__debt_payment` | |
| 45 | `__debt_term` | |
| 46 | `__debt_limit` | |
| 47 | `__contract_start` | |
| 48 | `__contract_end` | |
| 49 | `__fc_from` | |
| 50 | `__fc_to` | |
| 51 | `__fc_amount` | |

---

## 3. Bảng `client`

### 3.1. Cột CSV → cột DB

| # | Khóa CSV (sau `normalize_header`) | Cột SQL `client` | Xử lý |
|---|-----------------------------------|------------------|--------|
| 1 | `companyName` | `company_name` | Bắt buộc; trống → bỏ qua dòng |
| 2 | `companyNameEn` | `company_name_en` | |
| 3 | `internalCode` | `internal_code` | Trùng mã đã có trong DB hoặc trùng trong file → **log + không insert** client |
| 4 | `taxCode` | `tax_code` | |
| 5 | `representativeName` | `representative_name` | Xem **§3.2** |
| 6 | `representativeEmail` | `representative_email` | |
| 7 | `representativePhone` | `representative_phone` | |
| 8 | `representativeTitle` | `representative_title` | |
| 9 | `bankName` | `bank_name` | |
| 10 | `bankAccountNumber` | `bank_account_number` | |
| 11 | `bankAccountName` | `bank_account_name` | |
| 12 | `address` | `address` | |
| 13 | `ward` | `ward` | |
| 14 | `province` | `province` | |
| 15 | `country` | `country` | Trống → `country.full_name_vi` với `alpha_2 = 'VN'`, fallback `"Nước Việt Nam"` |
| 16 | `clientIndustryId` | `client_industry_id` | Chuỗi UUID; không parse được → `NULL` |
| 17–19 | `profession`, `scale`, `customerType` | `profession`, `scale`, `customer_type` | `customer_type` qua **chuẩn hóa** (§5). Vị trí cột **ghi đè** theo §2 |
| 20 | `discountRate` | `discount_rate` | `%` → `Decimal` |
| 21 | `commissionRate` | `commission_rate` | `%` → `Decimal`; mặc định `0` |
| 22 | `Column1` | — | **Không map** |
| 23–25 | `salesOwnerName`, `salesOwnerEmail`, `salesOwnerPhone` | `sales_owner_*` | Tra `employee` theo tên chuẩn hóa; tên lấy DB nếu khớp; email/SĐT **ưu tiên CSV** |
| 26–28 | `csoOwnerName`, `csoOwnerEmail`, `csoOwnerPhone` | `cso_owner_*` | Cùng logic |
| 29 | `agentClientId` | `agent_client_id` | **Mã đại lý** = `internal_code` của client đại lý trong DB (UUID). Chưa có mã → `NULL` rồi **pass 2** UPDATE |
| 30 | `isBlacklisted` | `is_blacklisted` | Bool |
| 31 | `blacklistReason` | `blacklist_reason` | |
| 32 | `notes` | `notes` | |
| 33 | `issueInvoice` | `issue_invoice` | |
| 34–51 | — | — | **Không** vào `client` (contact / debt / forecast) |

### 3.2. Người đại diện trên `client`

- Nguồn chính: cột **5–8** (`representative*`).
- **Backfill:** Nếu cả bốn nhóm (name, email, phone, title từ CSV đại diện) đều trống **nhưng** có dữ liệu khối liên hệ (34–38…), script điền:
  - `representative_name` ← `contactFullName`
  - `representative_email` ← **email đầu tiên** sau khi tách `;`/`,` từ `contactEmail`
  - `representative_phone` ← `contactPhone`
  - `representative_title` ← `contactTitle`

Người đại diện **không** được dùng làm nguồn tạo dòng `contact` — `contact` chỉ từ cột 34–43.

### 3.3. Cột do script gán (không có trong CSV)

| Cột | Giá trị |
|-----|---------|
| `client_id` | UUID mới (uppercase string khi insert) |
| `created_date` | UTC, naive |
| `status` | `"Active"` |

Nếu DB có `city` mà không có `province`: `city` ← giá trị CSV `province`.

Insert chỉ gồm những cột **tồn tại** trên bảng (đọc `INFORMATION_SCHEMA`).

---

## 4. Bảng `contact`

**Điều kiện insert:** có dữ liệu ở bất kỳ trường nào trong cột 34–43 (chuỗi non-empty hoặc cờ bool true).

**Nhiều email trong một ô `contactEmail`:** tách theo `;` hoặc `,` → **mỗi email một dòng** `contact` (cùng tên, SĐT, phòng ban, chức danh, ghi chú, cờ vai trò).

| Khóa CSV | Cột SQL `contact` |
|----------|-------------------|
| `contactFullName` | `full_name` (fallback tạm thời: email hoặc SĐT hoặc `-`) |
| `contactEmail` | `email` (mỗi INSERT một email sau tách) |
| `contactPhone` | `phone` |
| `contactDepartment` | `department` |
| `contactTitle` | `title` |
| `contactNotes` | `notes` |
| `contactIsPrimary` | `is_primary` |
| `contactIsSampleSender` | `is_sample_sender` |
| `contactIsResultReceiver` | `is_result_receiver` |
| `contactIsPayer` | `is_payer` |

`client_id`: FK tới client vừa tạo. `contact_id`: UUID mới. `created_at`: set nếu bảng có cột.

---

## 5. Bảng `client_debt`

Nguồn: cột 44–48 qua khóa `__debt_*` (§2). Có bất kỳ ô nào non-empty → upsert theo `client_id`.

| Key nội bộ | Cột SQL | Ghi chú |
|------------|---------|--------|
| `__debt_payment` | `payment_method` | Chuẩn hóa theo 3 giá trị UI (§6) |
| `__debt_term` | `debt_term_days` | int, mặc định 0 |
| `__debt_limit` | `credit_limit` | `Decimal`, mặc định 0 |
| `__contract_start` | `contract_effective_date` | parse ngày |
| `__contract_end` | `contract_end_date` | parse ngày |

`total_debt`: `0` (placeholder). Đã có dòng: **UPDATE** các field (trừ `client_debt_id`, `client_id`, `created_at`).

---

## 6. Bảng `client_forecast`

Nguồn: `__fc_from`, `__fc_to`, `__fc_amount` (cột 49–51). Cần đủ **from + to** mới insert.

| Key | Cột SQL |
|-----|---------|
| `__fc_from` | `from_date` |
| `__fc_to` | `to_date` |
| `__fc_amount` | `forecast_amount` |

---

## 7. Chuẩn hóa giá trị

### `customer_type` → 5 giá trị form UI

`Cá nhân`, `Doanh nghiệp`, `Nhà nước`, `Đại lý`, `CTV` — NFC + `casefold` để gom hoa/thường; không khớp → giữ chuỗi đã NFC/trim.

### `payment_method` (công nợ)

`Thu tiền khi gửi mẫu`, `Thu tiền trả kết quả`, `Công nợ` — NFC, gom khoảng trắng, `casefold`; không khớp → giữ chuỗi đã chuẩn.

---

## 8. Khác

- **Trùng `internal_code`:** file log CSV (mặc định cạnh file nguồn), cột: line, reason, internalCode, companyName, agentClientCode.
- **Đại lý (pass 2):** sau khi import batch, `UPDATE client SET agent_client_id = …` khi mã đại lý đã có trong map `internal_code → client_id`.
- **Header template:** so khớp 51 tên sau `normalize_header` với `EXPECTED_CLIENT_CSV_HEADERS` trong script (cảnh báo nếu lệch).

---

## 9. Tham chiếu model C#

- Client: [`Models/Client.cs`](../../Models/Client.cs)
- Contact: [`Models/Contact.cs`](../../Models/Contact.cs)  
  (DB có thể có thêm cột so với file model — script chỉ insert cột tồn tại.)

---

*Cập nhật theo `import_customer_csv.py` — khi đổi logic import, nên sửa song song file này.*
