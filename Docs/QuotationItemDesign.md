# Thiết Kế Bảng Chi Tiết Báo Giá (QuotationItem)

## Tổng Quan

Bảng `QuotationItem` được thiết kế để hỗ trợ **3 dạng chi tiết báo giá**:
1. **Theo Chỉ tiêu** (AnalysisItem) - Từng chỉ tiêu phân tích riêng lẻ
2. **Theo Nhóm chỉ tiêu** (AnalysisGroup) - Một nhóm các chỉ tiêu
3. **Theo Gói** (Package) - Nhiều nhóm chỉ tiêu được đóng gói

## Kiến Trúc Thiết Kế

### 1. Bảng QuotationItem (Chi tiết báo giá)

**Cấu trúc:**
- `ItemType`: Xác định loại item ("AnalysisItem", "AnalysisGroup", "Package")
- 3 Foreign keys nullable:
  - `AnalysisItemId`: Nếu ItemType = "AnalysisItem"
  - `AnalysisGroupId`: Nếu ItemType = "AnalysisGroup"  
  - `PackageId`: Nếu ItemType = "Package"
- **Chỉ một trong 3 FK có giá trị** (được đảm bảo bởi Check Constraint)

**Ưu điểm:**
- ✅ Type-safe với foreign keys
- ✅ Dễ query và join với master data
- ✅ Dễ validate và maintain
- ✅ Hỗ trợ tốt cho Entity Framework

**Thông tin lưu trữ:**
- Thông tin hiển thị (có thể override từ master data)
- Số lượng, đơn giá, giảm giá
- Thành tiền (tự động tính)
- Thứ tự hiển thị

### 2. Bảng Package (Gói phân tích)

**Mục đích:** Định nghĩa các gói phân tích chứa nhiều nhóm chỉ tiêu

**Cấu trúc:**
- Thông tin cơ bản: Mã, Tên (Vi/En), Mô tả
- Giá mặc định của gói
- Status, Notes

### 3. Bảng PackageAnalysisGroup (Many-to-Many)

**Mục đích:** Liên kết Package với AnalysisGroup (many-to-many)

**Cấu trúc:**
- `PackageId` + `AnalysisGroupId`
- `DisplayOrder`: Thứ tự hiển thị trong gói
- `IsRequired`: Bắt buộc hay không (có thể bỏ trong gói tùy chỉnh)
- `Notes`: Ghi chú

## Quan Hệ Dữ Liệu

```
Quotation (1) ──→ (N) QuotationItem
                         │
                         ├──→ AnalysisItem (nếu ItemType = "AnalysisItem")
                         ├──→ AnalysisGroup (nếu ItemType = "AnalysisGroup")
                         └──→ Package (nếu ItemType = "Package")

Package (1) ──→ (N) PackageAnalysisGroup (N) ──→ (1) AnalysisGroup
```

## Các Trường Hợp Sử Dụng

### 1. Báo giá theo Chỉ tiêu
```csharp
var item = new QuotationItem
{
    QuotationId = quotationId,
    ItemType = "AnalysisItem",
    AnalysisItemId = analysisItemId,
    Quantity = 1,
    UnitPrice = 500000,
    // ...
};
```

### 2. Báo giá theo Nhóm chỉ tiêu
```csharp
var item = new QuotationItem
{
    QuotationId = quotationId,
    ItemType = "AnalysisGroup",
    AnalysisGroupId = analysisGroupId,
    Quantity = 1,
    UnitPrice = 2000000, // Giá của cả nhóm
    // ...
};
```

### 3. Báo giá theo Gói
```csharp
var item = new QuotationItem
{
    QuotationId = quotationId,
    ItemType = "Package",
    PackageId = packageId,
    Quantity = 1,
    UnitPrice = 5000000, // Giá của cả gói
    // ...
};
```

## Validation Rules

1. **Check Constraint:** Chỉ một trong 3 FK (AnalysisItemId, AnalysisGroupId, PackageId) có giá trị
2. **ItemType phải khớp với FK có giá trị:**
   - ItemType = "AnalysisItem" → AnalysisItemId != null
   - ItemType = "AnalysisGroup" → AnalysisGroupId != null
   - ItemType = "Package" → PackageId != null

## Tính Toán Tự Động

**SubTotal** được tính tự động:
```csharp
SubTotal = (Quantity * UnitPrice) - (DiscountAmount ?? 0)
```

Hoặc:
```csharp
SubTotal = Quantity * UnitPrice * (1 - (DiscountPercent ?? 0) / 100)
```

## Query Examples

### Lấy tất cả items của một báo giá
```csharp
var items = context.QuotationItems
    .Where(qi => qi.QuotationId == quotationId)
    .OrderBy(qi => qi.DisplayOrder)
    .ToList();
```

### Lấy items theo loại
```csharp
var analysisItems = context.QuotationItems
    .Where(qi => qi.QuotationId == quotationId && qi.ItemType == "AnalysisItem")
    .Include(qi => qi.AnalysisItem)
    .ToList();
```

### Tính tổng tiền
```csharp
var total = context.QuotationItems
    .Where(qi => qi.QuotationId == quotationId)
    .Sum(qi => qi.SubTotal);
```

## Lưu Ý

1. **Snapshot Data:** Các trường `ItemCode`, `ItemNameVi`, `ItemNameEn` cho phép lưu snapshot để không bị ảnh hưởng khi master data thay đổi
2. **Flexibility:** Có thể override giá và thông tin từ master data
3. **Audit Trail:** `CreatedAt`, `UpdatedAt` để tracking
4. **Display Order:** `DisplayOrder` để sắp xếp items trong báo giá

## Mở Rộng Tương Lai

- Có thể thêm `SampleMatrixId` nếu cần filter theo loại mẫu
- Có thể thêm `EquipmentTypeId` nếu cần filter theo thiết bị
- Có thể thêm `DepartmentId` nếu cần assign phòng ban thực hiện

