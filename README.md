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

## Deploy lên IIS Subapplication

### Yêu cầu

- Windows Server với IIS đã được cài đặt
- .NET 8.0 Runtime (ASP.NET Core Runtime) đã được cài đặt
- ASP.NET Core Module V2 (tự động cài với .NET Runtime)
- Quyền Administrator trên server

### Các bước deploy

#### 1. Build và Publish ứng dụng

```bash
dotnet publish -c Release -o publish
```

#### 2. Sử dụng script tự động (Khuyến nghị)

Mở PowerShell với quyền Administrator và chạy:

```powershell
.\deploy-iis.ps1
```

Script sẽ tự động:
- Kiểm tra IIS và .NET Runtime
- Tạo Application Pool `crm-api`
- Copy files vào `C:\inetpub\wwwroot\crm-api`
- Tạo Application `crm-api` trong IIS Site
- Cấu hình quyền truy cập
- Restart Application Pool

**Tham số tùy chọn:**
```powershell
.\deploy-iis.ps1 -SiteName "Default Web Site" -AppName "crm-api" -PhysicalPath "C:\inetpub\wwwroot\crm-api"
```

#### 3. Deploy thủ công

**Bước 1: Tạo Application Pool**

1. Mở IIS Manager
2. Chọn **Application Pools** → Click phải → **Add Application Pool**
3. Đặt tên: `crm-api`
4. .NET CLR Version: **No Managed Code**
5. Managed Pipeline Mode: **Integrated**
6. Click **OK**

**Bước 2: Copy files**

Copy toàn bộ nội dung từ thư mục `publish` vào thư mục IIS, ví dụ: `C:\inetpub\wwwroot\crm-api`

**Bước 3: Tạo Application trong IIS**

1. Mở IIS Manager
2. Chọn Site (ví dụ: **Default Web Site**)
3. Click phải → **Add Application**
4. Alias: `crm-api`
5. Application Pool: `crm-api`
6. Physical Path: `C:\inetpub\wwwroot\crm-api`
7. Click **OK**

**Bước 4: Cấu hình quyền**

Cấp quyền cho Application Pool Identity:
- Click phải vào thư mục `C:\inetpub\wwwroot\crm-api` → **Properties** → **Security**
- Click **Edit** → **Add**
- Nhập: `IIS AppPool\crm-api`
- Chọn quyền: **Read & Execute**, **List folder contents**, **Read**
- Click **OK**

**Bước 5: Tạo thư mục logs**

Tạo thư mục `logs` trong `C:\inetpub\wwwroot\crm-api` và cấp quyền tương tự như trên.

### Kiểm tra sau khi deploy

1. **Truy cập Swagger UI:**
   ```
   http://your-server/crm-api/swagger
   ```

2. **Kiểm tra API endpoint:**
   ```
   http://your-server/crm-api/odata/Clients
   ```

3. **Kiểm tra logs:**
   - Logs được lưu tại: `C:\inetpub\wwwroot\crm-api\logs\stdout_*.log`
   - Nếu có lỗi, kiểm tra Event Viewer → Windows Logs → Application

### Cấu hình Base Path

Ứng dụng đã được cấu hình sẵn để chạy như subapplication với base path `/crm-api`:

- `web.config` đã có `ASPNETCORE_BASEPATH=/crm-api`
- `appsettings.json` có `BasePath: "/crm-api"`
- `Program.cs` tự động xử lý base path

### Troubleshooting

**Lỗi 500.31 - Failed to load ASP.NET Core runtime (QUAN TRỌNG):**

Đây là lỗi phổ biến nhất khi deploy. Nguyên nhân: **.NET 8.0 Runtime chưa được cài đặt trên server IIS**.

**Cách khắc phục:**

1. **Cài đặt .NET 8.0 Hosting Bundle:**
   - Truy cập: https://dotnet.microsoft.com/download/dotnet/8.0
   - Tải: **ASP.NET Core Runtime 8.0.x - Windows Hosting Bundle**
   - (Bao gồm .NET Runtime + ASP.NET Core Runtime + ASP.NET Core Module V2)
   - Chạy file installer và làm theo hướng dẫn

2. **Sau khi cài đặt, restart IIS:**
   ```powershell
   iisreset
   ```

3. **Sử dụng script tự động để kiểm tra:**
   ```powershell
   .\fix-500-31-error.ps1
   ```

4. **Kiểm tra Application Pool:**
   - Mở IIS Manager → Application Pools → `crm-api`
   - Đảm bảo:
     - .NET CLR Version: **No Managed Code**
     - Managed Pipeline Mode: **Integrated**
     - Status: **Started**

5. **Kiểm tra Event Viewer:**
   - Windows Logs → Application
   - Tìm các lỗi liên quan đến ASP.NET Core

**Lỗi 500.30 - In-Process Start Failure:**
- Kiểm tra .NET 8.0 Runtime đã được cài đặt
- Kiểm tra Application Pool đang chạy
- Xem logs trong `logs\stdout_*.log`

**Lỗi 500.0 - ANCM In-Process Handler Load Failure:**
- Kiểm tra ASP.NET Core Module V2 đã được cài đặt
- Restart IIS: `iisreset`

**Lỗi 503 - Service Unavailable:**
- Kiểm tra Application Pool đang chạy
- Kiểm tra .NET Runtime đã được cài đặt (xem lỗi 500.31)
- Xem logs trong `logs\stdout_*.log`

**Lỗi kết nối Database:**
- Kiểm tra connection string trong `appsettings.json`
- Đảm bảo SQL Server có thể truy cập từ server IIS
- Kiểm tra firewall và network

**Swagger không hiển thị đúng:**
- Kiểm tra base path trong `web.config` và `appsettings.json`
- Đảm bảo URL truy cập có đúng path: `http://server/crm-api/swagger`

