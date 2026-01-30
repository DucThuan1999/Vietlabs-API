# BÁO CÁO KIỂM TRA TÍNH HỢP LỆ FILE CSV

## Tổng quan

Đã kiểm tra 3 file CSV trong thư mục `csv/`:
- `country.csv` - Danh sách quốc gia
- `provinces.csv` - Danh sách tỉnh/thành phố
- `ward.csv` - Danh sách phường/xã

## Cấu trúc File

### 1. country.csv
**Cột:** STT, Tên nước (EN), Tên đầy đủ (VI), Tên đầy đủ (EN), Alpha-2, Alpha-3, Trạng Thái, Ghi chú

**Vấn đề phát hiện:**
- ✅ Cấu trúc đúng với 8 cột
- ⚠️ Encoding có thể có vấn đề (hiển thị ký tự lạ)
- ⚠️ Trạng thái là "Actived" (có thể cần chuẩn hóa thành "Active")

### 2. provinces.csv
**Cột:** STT, Tỉnh/Thành phố, Loại, Đầy đủ, Quốc Gia, Trạng Thái, Ghi chú

**Vấn đề phát hiện:**
- ✅ Cấu trúc đúng với 7 cột
- ⚠️ Foreign Key: Sử dụng tên Quốc Gia (text) thay vì ID
- ⚠️ Tất cả provinces đều tham chiếu đến "Việt Nam"
- ⚠️ Trạng thái là "Actived" (có thể cần chuẩn hóa thành "Active")

### 3. ward.csv
**Cột:** STT, Mã, Xã/Phường, Loại, Tỉnh/Thành Phố, Quốc Gia, Trạng Thái, Ghi chú

**Vấn đề phát hiện:**
- ✅ Cấu trúc đúng với 8 cột
- ⚠️ Foreign Key: Sử dụng tên Tỉnh/Thành Phố và Quốc Gia (text) thay vì ID
- ⚠️ Tất cả wards đều tham chiếu đến "Việt Nam"
- ⚠️ Trạng thái là "Actived" (có thể cần chuẩn hóa thành "Active")

## Vấn đề chính cần xử lý

### 1. Foreign Key Mapping
**Vấn đề:** CSV sử dụng tên (text) thay vì ID cho foreign keys:
- `provinces.csv` → `Quốc Gia` = "Việt Nam" (text)
- `ward.csv` → `Tỉnh/Thành Phố` = "Thành phố Hà Nội" (text)
- `ward.csv` → `Quốc Gia` = "Việt Nam" (text)

**Giải pháp:** Cần tạo script import để:
1. Đọc `country.csv` và tạo mapping: `"Việt Nam"` → `CountryId`
2. Đọc `provinces.csv` và tạo mapping: `"Thành phố Hà Nội"` → `ProvinceId`
3. Khi import `provinces.csv`: Map `Quốc Gia` text → `CountryId`
4. Khi import `ward.csv`: Map `Tỉnh/Thành Phố` text → `ProvinceId`, `Quốc Gia` text → `CountryId`

### 2. Trạng thái không chuẩn
**Vấn đề:** Tất cả records có `Trạng Thái = "Actived"` nhưng trong database model là `Status = "Active"`

**Giải pháp:** Chuẩn hóa khi import: `"Actived"` → `"Active"`

### 3. Encoding
**Vấn đề:** File CSV có thể có vấn đề encoding (hiển thị ký tự lạ)

**Giải pháp:** Đảm bảo đọc file với UTF-8 encoding

## Khuyến nghị

### Trước khi Import:
1. ✅ **Tạo script import** để map text → ID
2. ✅ **Chuẩn hóa trạng thái** từ "Actived" → "Active"
3. ✅ **Validate foreign keys** trước khi insert
4. ✅ **Xử lý encoding** UTF-8 đúng cách

### Script Import cần có:
1. Đọc `country.csv` → Insert vào bảng `country` → Lưu mapping `FullNameVi → CountryId`
2. Đọc `provinces.csv` → Map `Quốc Gia` text → `CountryId` → Insert vào bảng `province` → Lưu mapping `Name → ProvinceId`
3. Đọc `ward.csv` → Map `Tỉnh/Thành Phố` text → `ProvinceId`, `Quốc Gia` text → `CountryId` → Insert vào bảng `ward`

## Kết luận

**Tình trạng:** ⚠️ **CẦN XỬ LÝ TRƯỚC KHI INSERT**

**Lý do:**
- Foreign keys sử dụng text thay vì ID → Cần mapping
- Trạng thái không chuẩn → Cần chuẩn hóa
- Cần validate tất cả foreign key relationships

**Hành động tiếp theo:**
1. Tạo script SQL import với mapping logic
2. Hoặc tạo C# console app để import với validation

