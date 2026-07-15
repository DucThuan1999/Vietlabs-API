# Hướng dẫn tích hợp AMIS — Khách hàng & Công nợ

VietLab **chủ động gọi** API AMIS Kế toán (MISA ACT Open). Phạm vi hiện tại:

| # | Nghiệp vụ | AMIS API | API VietLab |
|---|-----------|----------|-------------|
| 1 | Tạo khách hàng | `POST {ApiUrl}/apir/sync/actopen/save_dictionary` | `POST /api/AmisCustomers` |
| 2 | Get khách hàng | `POST {ApiUrl}/apir/sync/actopen/get_dictionary` (`data_type=1`) | `GET /api/AmisCustomers` |
| 3 | Get công nợ | `POST {ApiUrl}/apir/sync/actopen/get_list_acc_obj_debt` | `GET /api/AmisCustomers/debts` |

Tài liệu gốc: [ACT Open API Help](https://actdocs.misa.vn/g2/graph/ACTOpenAPIHelp/index.html#5-1)

---

## 1. Cấu hình

Trong `appsettings.json` / User Secrets:

```json
{
  "Amis": {
    "ApiUrl": "https://actapp.misa.vn",
    "AppId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
    "AccessCode": "mã-kết-nối-từ-tab-API-trên-AMIS",
    "OrgCompanyCode": "domain-cong-ty-doi-tac",
    "DefaultTake": 100,
    "TokenRefreshBeforeExpiryMinutes": 30
  }
}
```

| Tham số | Mô tả |
|---------|--------|
| `ApiUrl` | URL gốc AMIS (không có `/` cuối) |
| `AppId` | Mã ứng dụng MISA cấp |
| `AccessCode` | Mã kết nối công ty (màn hình thiết lập kết nối API) |
| `OrgCompanyCode` | Domain khách hàng phía đối tác |

---

## 2. Luồng xác thực

```text
VietLab API
    → POST /api/oauth/actopen/connect (app_id, access_code, org_company_code)
    ← access_token (TTL ~12h, cache trong memory)
    → Các API nghiệp vụ kèm header X-MISA-AccessToken
```

Service: `AmisAccountingService` — tự refresh token khi `ExpiredToken`.

---

## 3. API VietLab

### 3.1 Tạo khách hàng

`POST /api/AmisCustomers`

**Cách 1 — từ CRM Client:**

```json
{
  "clientId": "11111111-1111-1111-1111-111111111111"
}
```

Map: `InternalCode` → `account_object_code`, `CompanyName` → `account_object_name`, `TaxCode` → `company_tax_code`, ...

**Cách 2 — payload trực tiếp:**

```json
{
  "accountObjectCode": "KH-001",
  "accountObjectName": "Công ty ABC",
  "companyTaxCode": "0101234567",
  "address": "123 Đường XYZ",
  "country": "Việt Nam",
  "isCustomer": true
}
```

**Response thành công:**

```json
{
  "success": true,
  "data": [ { "accountObjectId": "...", "accountObjectCode": "KH-001", ... } ],
  "customData": null
}
```

### 3.2 Get khách hàng

`GET /api/AmisCustomers?skip=0&take=20&lastSyncTime=2024-01-01%2014:15:02&branchId=`

| Query | Mô tả |
|-------|--------|
| `skip` | Bỏ qua N bản ghi |
| `take` | Tối đa 100/lần |
| `lastSyncTime` | Lấy thay đổi sau thời điểm (format AMIS, VD `yyyy-MM-dd HH:mm:ss`) |
| `branchId` | Chi nhánh (null = tất cả) |

### 3.3 Get công nợ

`GET /api/AmisCustomers/debts?dataType=0&skip=0&take=20`

| Query | Mô tả |
|-------|--------|
| `dataType` | `0` = công nợ **phải thu** (mặc định), `1` = phải trả |
| `skip`, `take`, `lastSyncTime`, `branchId` | Giống get khách hàng |

**Trường trong `data`:**

- `accountObjectId`, `accountObjectCode`, `accountObjectName`
- `debtAmount`, `invoiceDebtAmount`
- `organizationUnitId`, `organizationUnitCode`, `organizationUnitName`

`customData` có thể chứa `LastSyncTime` cho lần sync sau.

---

## 4. Ví dụ cURL

```bash
# Get khách hàng (cần cấu hình Amis đầy đủ trên server)
curl -s "http://localhost:5000/api/AmisCustomers?skip=0&take=10"

# Get công nợ phải thu
curl -s "http://localhost:5000/api/AmisCustomers/debts?dataType=0&skip=0&take=10"

# Tạo khách hàng
curl -s -X POST "http://localhost:5000/api/AmisCustomers" \
  -H "Content-Type: application/json" \
  -d '{"accountObjectCode":"TEST-001","accountObjectName":"Khách hàng test"}'
```

Production (SBT): `https://www.sbt-software.com:444/crm-api/api/AmisCustomers`  
Frontend CRM: `https://www.sbt-software.com:444/vietlabs-quotation/`

---

## 5. Mã nguồn liên quan

| Thành phần | File |
|------------|------|
| Cấu hình | `Configuration/AmisOptions.cs` |
| Service | `Services/AmisAccountingService.cs` |
| Controller (outbound) | `Controllers/AmisCustomersController.cs` |
| Controller (callback) | `Controllers/AmisCallbacksController.cs` |
| Service callback | `Services/AmisCallbackService.cs` |
| DTO | `Models/DTOs/AmisApiDtos.cs`, `Models/DTOs/AmisCallbackDtos.cs` |
| Entity log | `Models/AmisCallbackLog.cs` |

---

## 6. Callback inbound (AMIS gọi về VietLab)

AMIS/MISA gọi callback sau khi xử lý chứng từ (save/delete). VietLab expose endpoint production:

| | |
|---|---|
| **Method** | `POST` |
| **URL** | `/api/AmisCallbacks/call_back_data` |
| **Auth** | Không Bearer — `AllowAnonymous` (xác thực bằng chữ ký HMAC) |

**URL callback đăng ký trên AMIS (Production):**

```
https://www.sbt-software.com:444/crm-api/api/AmisCallbacks/call_back_data
```

| Môi trường | URL |
|------------|-----|
| Production API (`BasePath` = `/crm-api`) | `https://www.sbt-software.com:444/crm-api/api/AmisCallbacks/call_back_data` |
| Frontend (tham khảo, AMIS **không** gọi vào đây) | `https://www.sbt-software.com:444/vietlabs-quotation/` |
| Development | `http://localhost:5000/api/AmisCallbacks/call_back_data` |

> AMIS phải gọi **API backend** (`/crm-api/...`), không phải URL React (`/vietlabs-quotation/`).

Cấu hình URL callback trên tab API kết nối AMIS.

**Tài liệu gửi riêng cho MISA/AMIS:** [amis-callback-api-for-misa.md](./amis-callback-api-for-misa.md)

### 6.1 Request body (snake_case)

```json
{
  "success": true,
  "error_code": null,
  "error_message": "",
  "signature": "hex-hmac-sha256-cua-truong-data",
  "data_type": 1,
  "data": "[{\"org_refid\":\"...\",\"success\":true}]",
  "org_company_code": "domain-cong-ty",
  "app_id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
}
```

| Trường | Mô tả |
|--------|--------|
| `signature` | `HMAC-SHA256(data, AppId)` — hex lowercase, key là `Amis:AppId` |
| `data_type` | `1` = SaveVoucher, `2` = DeleteVoucher (xem enum `AmisCallbackDataType`) |
| `data` | JSON string (mảng chi tiết chứng từ) |

### 6.2 Response (PascalCase theo mẫu MISA)

```json
{
  "Success": true,
  "ErrorCode": null,
  "ErrorMessage": "",
  "Data": null
}
```

Chữ ký sai:

```json
{
  "Success": false,
  "ErrorCode": "InvalidParam",
  "ErrorMessage": "Signature invalid",
  "Data": null
}
```

### 6.3 Lưu trữ

Mọi callback (kể cả chữ ký invalid) được ghi vào bảng `amis_callback_log` để audit/tra cứu. **Không** xử lý nghiệp vụ theo `data_type` (SaveVoucher, DeleteVoucher, …) — chỉ lưu log SQL.

Migration: `20260519120000_AddAmisCallbackLog.cs`

### 6.4 Ví dụ cURL (test chữ ký)

```bash
# Tính signature: HMAC-SHA256 của chuỗi data, key = Amis:AppId
DATA='[{"org_refid":"test-001","success":true}]'
# signature = (tính bằng script hoặc tool — phải khớp Amis:AppId trong appsettings)

curl -s -X POST "http://localhost:5000/api/AmisCallbacks/call_back_data" \
  -H "Content-Type: application/json" \
  -d "{\"success\":true,\"signature\":\"<hex>\",\"data_type\":1,\"data\":\"$DATA\",\"org_company_code\":\"your-org\",\"app_id\":\"your-app-id\"}"
```

---

## 7. Phạm vi chưa triển khai

- Save/delete chứng từ **outbound** (VietLab gọi AMIS)
- Xử lý nghiệp vụ callback theo `data_type` / `org_refid` (hiện chỉ lưu `amis_callback_log`)
- Đồng bộ tồn kho, danh mục khác
