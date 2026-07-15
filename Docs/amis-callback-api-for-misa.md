# Tài liệu API Callback — VietLab CRM ↔ AMIS Kế toán (ACT Open)

Tài liệu này mô tả endpoint callback do **VietLab** cung cấp để **AMIS/MISA** gọi về sau khi xử lý dữ liệu (cất/xóa chứng từ, …). Cấu trúc request/response và thuật toán chữ ký **tuân theo mẫu tham khảo ACT Open** (`call_back_data`).

---

## 1. Thông tin endpoint

| Hạng mục | Giá trị |
|----------|---------|
| **Phương thức** | `POST` |
| **Content-Type** | `application/json; charset=utf-8` |
| **Xác thực** | Không dùng Bearer Token. Xác thực bằng trường `signature` (HMAC-SHA256). |
| **Đường dẫn tương đối** | `/api/AmisCallbacks/call_back_data` |

### 1.1 URL Production (đăng ký trên AMIS)

```
https://www.sbt-software.com:444/crm-api/api/AmisCallbacks/call_back_data
```

### 1.2 HTTP status code

| Tình huống | HTTP |
|------------|------|
| Nhận request, xử lý (kể cả chữ ký sai) | `200 OK` |
| Lỗi server nội bộ nghiêm trọng | `500` *(hiếm)* |

Kết quả nghiệp vụ (thành công / chữ ký invalid) được trả trong **body JSON** (trường `Success`), không chỉ dựa vào HTTP status.

---

## 2. Request — AMIS gửi tới VietLab

### 2.1 Quy ước đặt tên

- Tên trường request: **snake_case** (ví dụ: `error_code`, `data_type`).
- Body là một object JSON.

### 2.2 Schema

| Trường | Kiểu | Bắt buộc | Mô tả |
|--------|------|----------|--------|
| `success` | `boolean` | Có | `true` = AMIS xử lý thành công ở tầng gọi callback; `false` = thất bại |
| `error_code` | `string` \| `null` | Không | Mã lỗi từ AMIS (nếu có) |
| `error_message` | `string` | Không | Chi tiết lỗi; mặc định `""` |
| `signature` | `string` | Có | `HMAC-SHA256(data, app_id)` — hex lowercase (theo ACT Open) |
| `data_type` | `integer` | Có | Mã loại callback ACT Open (0, 1, 2, …) |
| `data` | `string` | Không | Chuỗi JSON (thường là mảng object), **không** phải object lồng trực tiếp |
| `org_company_code` | `string` | Không | Mã công ty phía dữ liệu nguồn |
| `app_id` | `string` | Không | ID ứng dụng MISA đã cấp cho VietLab |

### 2.3 Ví dụ request — `data_type = 1` (SaveVoucher)

```json
{
  "success": true,
  "error_code": null,
  "error_message": "",
  "signature": "a1b2c3d4e5f6789012345678901234567890abcdef1234567890abcdef123456",
  "data_type": 1,
  "data": "[{\"org_refid\":\"550e8400-e29b-41d4-a716-446655440000\",\"success\":true,\"error_code\":null,\"error_message\":\"\",\"session_id\":\"6ba7b810-9dad-11d1-80b4-00c04fd430c8\",\"error_call_back_message\":null,\"voucher_type\":13}]",
  "org_company_code": "vietlab-demo",
  "app_id": "0e0a14cf-9e4b-4af9-875b-c490f34a581b"
}
```

### 2.4 Ví dụ request — callback thất bại phía AMIS

```json
{
  "success": false,
  "error_code": "BusinessError",
  "error_message": "Không tìm thấy chứng từ",
  "signature": "...",
  "data_type": 1,
  "data": "[]",
  "org_company_code": "vietlab-demo",
  "app_id": "0e0a14cf-9e4b-4af9-875b-c490f34a581b"
}
```

---

## 3. Trường `data` — định dạng AMIS gửi (VietLab lưu nguyên chuỗi)

Trong request, `data` là **`string`** (JSON đã stringify).

AMIS vẫn có thể gửi theo cấu trúc ACT Open (thường khi `data_type` = `1` hoặc `2`): chuỗi chứa **mảng** object. Tham chiếu schema MISA:

| Trường (trong JSON) | Kiểu | Mô tả (theo MISA) |
|---------------------|------|-------------------|
| `org_refid` | `string` | ID gốc chứng từ phía đối tác |
| `success` | `boolean` | Kết quả từng chứng từ |
| `error_code` | `string` \| `null` | Mã lỗi |
| `error_message` | `string` | Chi tiết lỗi |
| `session_id` | `string` (UUID) \| `null` | Phiên làm việc |
| `error_call_back_message` | `string` \| `null` | Lỗi callback lần trước |
| `voucher_type` | `integer` \| `null` | Loại chứng từ |

### 3.1 Ví dụ `data` AMIS có thể gửi (`data_type` = 1)

Giá trị trường `data` trong body POST (một chuỗi, có escape `\"`):

```text
[{"org_refid":"550e8400-e29b-41d4-a716-446655440000","success":true,"error_code":null,"error_message":"","session_id":"6ba7b810-9dad-11d1-80b4-00c04fd430c8","error_call_back_message":null,"voucher_type":13}]
```

Nội dung tương đương sau khi parse (để đọc tài liệu):

```json
[
  {
    "org_refid": "550e8400-e29b-41d4-a716-446655440000",
    "success": true,
    "error_code": null,
    "error_message": "",
    "session_id": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
    "error_call_back_message": null,
    "voucher_type": 13
  }
]
```

---

## 4. Response — VietLab trả về AMIS

### 4.1 Quy ước đặt tên

- Tên trường response: **PascalCase** (`Success`, `ErrorCode`, …) theo mẫu ACT Open callback.

### 4.2 Schema

| Trường | Kiểu | Mô tả |
|--------|------|--------|
| `Success` | `boolean` | `true` = VietLab đã nhận và chấp nhận callback (chữ ký hợp lệ) |
| `ErrorCode` | `string` \| `null` | Mã lỗi khi `Success = false` |
| `ErrorMessage` | `string` | Thông báo chi tiết |
| `Data` | `string` \| `null` | Dữ liệu bổ sung (hiện tại thường `null`) |

### 4.3 Ví dụ — thành công

```json
{
  "Success": true,
  "ErrorCode": null,
  "ErrorMessage": "",
  "Data": null
}
```

> VietLab trả `Success: true` **ngay** sau khi validate chữ ký và ghi log SQL. Không có xử lý nghiệp vụ bổ sung theo `data_type`.

### 4.4 Ví dụ — lỗi chữ ký

```json
{
  "Success": false,
  "ErrorCode": "InvalidParam",
  "ErrorMessage": "Signature invalid",
  "Data": null
}
```

### 4.5 Ví dụ — lỗi hệ thống

```json
{
  "Success": false,
  "ErrorCode": "Exception",
  "ErrorMessage": "Mô tả lỗi",
  "Data": null
}
```

### 4.6 Bảng mã lỗi `ErrorCode`

| ErrorCode | Ý nghĩa |
|-----------|---------|
| `InvalidParam` | Chữ ký `signature` không khớp |
| `Configuration` | Phía VietLab chưa cấu hình `AppId` |
| `Exception` | Lỗi ngoại lệ khi xử lý request |

---

## 5. Luồng gọi (sequence)

```text
AMIS Kế toán                    VietLab API
     |                                |
     |  POST /api/AmisCallbacks/      |
     |       call_back_data           |
     |  (body + signature)            |
     |------------------------------->|
     |                                | Verify HMAC-SHA256(data, app_id)
     |                                | Lưu amis_callback_log
     |                                | Trả Success=true (đồng bộ)
     |<-------------------------------|
     |  { Success, ErrorCode, ... }   |
```

---

## 6. Kiểm thử (cURL)

Thay `<SIGNATURE>`, `<APP_ID>`, `<ORG_CODE>` bằng giá trị thực tế:

```bash
curl -X POST "https://www.sbt-software.com:444/crm-api/api/AmisCallbacks/call_back_data" \
  -H "Content-Type: application/json" \
  -d '{
    "success": true,
    "error_code": null,
    "error_message": "",
    "signature": "<SIGNATURE>",
    "data_type": 1,
    "data": "[{\"org_refid\":\"test-ref-001\",\"success\":true,\"error_message\":\"\"}]",
    "org_company_code": "<ORG_CODE>",
    "app_id": "<APP_ID>"
  }'
```

Kỳ vọng khi chữ ký đúng: `"Success": true`.

---
