# VietLab CRM - ASP.NET Core với OData và SQL Server

Project CRM ASP.NET Core với kết nối SQL Server và hỗ trợ OData endpoints để quản lý khách hàng.

## Yêu cầu

- .NET 8.0 SDK
- SQL Server (LocalDB hoặc SQL Server Express/Full)

## Cài đặt

1. Restore các packages:
```bash
dotnet restore
```

2. Cấu hình connection string trong `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=VietLabDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

3. Chạy ứng dụng:
```bash
dotnet run
```

4. Truy cập Swagger UI tại: `https://localhost:5001/swagger` (hoặc port được cấu hình)

## OData Endpoints

### Lấy danh sách khách hàng
```
GET /odata/Clients
```

### Lấy khách hàng theo ID
```
GET /odata/Clients(1)
```

### Tìm kiếm và lọc (OData Query Options)
```
GET /odata/Clients?$filter=Status eq 'Active'
GET /odata/Clients?$filter=City eq 'Hà Nội'
GET /odata/Clients?$orderby=CreatedDate desc
GET /odata/Clients?$select=CompanyName,Email,Phone
GET /odata/Clients?$top=10&$skip=0
GET /odata/Clients?$count=true
GET /odata/Clients?$filter=Industry eq 'Công nghệ thông tin'&$orderby=CompanyName
```

### Tạo khách hàng mới
```
POST /odata/Clients
Content-Type: application/json

{
  "companyName": "Công ty mới",
  "internalCode": "CLI-NEW-001",
  "taxCode": "0123456789",
  "bankAccountNumber": "1234567890",
  "address": "123 Đường ABC",
  "city": "Hà Nội",
  "country": "Việt Nam",
  "industry": "Công nghệ",
  "scale": "50 nhân sự",
  "customerType": "SMB",
  "discountRate": 5,
  "representativeName": "Nguyễn Văn X",
  "representativeEmail": "contact@newcompany.com",
  "representativePhone": "0123456789",
  "representativeTitle": "Giám đốc",
  "salesOwnerName": "Admin",
  "salesOwnerEmail": "admin@viet-labs.com",
  "salesOwnerPhone": "0900000000",
  "isBlacklisted": false,
  "blacklistReason": "",
  "status": "Prospect",
  "notes": "Khách hàng tiềm năng"
}
```

### Cập nhật khách hàng
```
PUT /odata/Clients(1)
Content-Type: application/json

{
  "id": 1,
  "companyName": "Công ty ABC",
  "internalCode": "CLI-ABC-001",
  "taxCode": "0101234567",
  "bankAccountNumber": "1234567890",
  "address": "123 Đường XYZ",
  "city": "Hà Nội",
  "country": "Việt Nam",
  "industry": "Công nghệ thông tin",
  "scale": "200 nhân sự",
  "customerType": "Enterprise",
  "discountRate": 5,
  "representativeName": "Nguyễn Văn A",
  "representativeEmail": "contact@abc.com",
  "representativePhone": "0123456789",
  "representativeTitle": "Giám đốc",
  "salesOwnerName": "Admin",
  "salesOwnerEmail": "admin@viet-labs.com",
  "salesOwnerPhone": "0900000001",
  "isBlacklisted": false,
  "blacklistReason": "",
  "createdDate": "2024-01-01T00:00:00",
  "lastContactDate": "2024-01-15T00:00:00",
  "status": "Active",
  "notes": "Đã liên hệ thành công"
}
```

### Xóa khách hàng
```
DELETE /odata/Clients(1)
```

## Cấu trúc Project

```
VietLab/
├── Controllers/
│   └── ClientsController.cs    # OData Controller cho CRM
├── Data/
│   └── ApplicationDbContext.cs  # DbContext với SQL Server
├── Models/
│   └── Client.cs               # Model khách hàng cho CRM
├── Program.cs                   # Cấu hình OData và EF Core
├── appsettings.json            # Connection string
└── VietLab.csproj              # Project file
```

## Model Client

Model `Client` bao gồm các trường:
- `Id` - ID khách hàng
- `CompanyName` - Tên công ty
- `InternalCode` - Mã khách hàng nội bộ
- `TaxCode` - Mã số thuế
- `BankAccountNumber` - Số tài khoản ngân hàng
- `Address` - Địa chỉ
- `City` - Thành phố
- `Country` - Quốc gia
- `Industry` - Ngành nghề
- `Scale` - Quy mô
- `CustomerType` - Loại khách hàng
- `DiscountRate` - Mức chiết khấu (%)
- `RepresentativeName` - Người đại diện
- `RepresentativeEmail` - Email người đại diện
- `RepresentativePhone` - Số điện thoại người đại diện
- `RepresentativeTitle` - Chức vụ người đại diện
- `SalesOwnerName` - Nhân viên kinh doanh phụ trách
- `SalesOwnerEmail` - Email kinh doanh
- `SalesOwnerPhone` - Số điện thoại kinh doanh
- `IsBlacklisted` - Có trong blacklist hay không
- `BlacklistReason` - Lý do blacklist
- `CreatedDate` - Ngày tạo
- `LastContactDate` - Ngày liên hệ cuối
- `Status` - Trạng thái (Active, Inactive, Prospect)
- `Notes` - Ghi chú

## Tính năng OData được hỗ trợ

- `$select` - Chọn các trường cụ thể
- `$filter` - Lọc dữ liệu
- `$orderby` - Sắp xếp
- `$top` - Giới hạn số lượng kết quả
- `$skip` - Bỏ qua số lượng kết quả
- `$count` - Đếm tổng số bản ghi

## Database

Database sẽ được tự động tạo khi chạy ứng dụng lần đầu (nếu chưa tồn tại) nhờ `EnsureCreated()`.

Để sử dụng Migrations (khuyến nghị cho production):
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

