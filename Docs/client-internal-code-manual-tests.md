# Kiểm thử thủ công — Mã khách hàng nội bộ

## API

- `GET /odata/Clients/NextInternalCode?customerType=...&country=...&province=...` — không chọn đại lý.
- `GET /odata/Clients/NextInternalCode?customerType=...&agentClientId={guid}` — khách có đại lý; **đếm số hậu tố** theo mọi bản ghi có mã bắt đầu bằng `{InternalCode đại lý}.` (không chỉ các dòng có `AgentClientId` khớp — tránh sót dữ liệu cũ).

Yêu cầu: header `Authorization: Bearer ...` (endpoint có `[Authorize]`).

## Case có đại lý (khách con)

1. Đại lý có `InternalCode` = `N/CTH0001`, chưa có khách con → next = `N/CTH0001.1`.
2. Đã có `N/CTH0001.1`, `N/CTH0001.2` → next = `N/CTH0001.3`.
3. Đổi Tỉnh/Quốc gia trên form **không** đổi prefix khi đã chọn đại lý (API chỉ dựa vào mã đại lý).
4. Đại lý chưa có `InternalCode` → API trả 400 kèm message.

## Case không đại lý

1. **Cá nhân / Doanh nghiệp / Nhà nước**: mã dạng `{ProvinceCode hoặc Alpha2/Alpha3}{5 số}`; số kế tiếp lấy **max + 1** trên toàn DB (không phụ thuộc `top:1000` client).
2. **Đại lý / CTV** (tạo bản ghi đại lý): mã dạng `N/{area}{4 số}`; max + 1 trên DB.
3. Tạo nhiều hơn 1000 khách toàn hệ thống: mã mới vẫn đúng theo prefix/loại (không bị “đếm lại từ đầu” như Redux cũ).

## Trùng mã (backend)

1. `POST /odata/Clients` với `InternalCode` đã tồn tại → **409 Conflict**, `{ "message": "Mã khách hàng nội bộ đã tồn tại." }`.
2. `PUT` đổi sang mã đã dùng bởi khách khác → 409.
3. Hai phiên cùng lưu mã mới trùng: một request 409 (hoặc lỗi DB tùy race); nên hiển thị message và tải lại mã (gọi lại NextInternalCode).

## UI (tạo mới)

1. Chọn Đại lý/CTV → `AgentClientId` được set → mã tự điền theo rule đại lý.
2. Xóa đại lý (Clear) → mã gọi lại API theo quốc gia/tỉnh.
3. Đổi loại KH sang **Đại lý** hoặc **CTV** → ô đại lý ẩn và FK agent được xóa; mã sinh theo N/{area}...
