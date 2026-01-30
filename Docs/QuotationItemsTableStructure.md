# Mô tả Cấu trúc QuotationItemsTable Components cho Backend

## Tổng quan

Hệ thống quản lý báo giá có 3 chế độ hiển thị items khác nhau, mỗi chế độ được hiển thị bằng một component table riêng. Tất cả các items đều được lưu trong mảng `quotationItems` với cấu trúc thống nhất, nhưng được nhóm và hiển thị khác nhau tùy theo chế độ.

## 1. CriterionViewTable (Giá theo Chỉ tiêu)

### Mục đích
Hiển thị tất cả các chỉ tiêu phân tích (AnalysisItem) dưới dạng danh sách phẳng, không nhóm. Chỉ hiển thị các items có `ItemType === "AnalysisItem"` và `isStandalone === true`.

### Cấu trúc dữ liệu hiển thị
- **dataSource**: Mảng các items từ `quotationItems` được lọc theo `isStandalone === true`
- Mỗi item là một AnalysisItem độc lập

### Các cột hiển thị
1. **STT**: Số thứ tự (tự động)
2. **Nền Mẫu** (SampleMatrixName): Tên nền mẫu
3. **Chỉ tiêu** (ItemNameVi): Tên chỉ tiêu tiếng Việt
4. **Phương pháp** (PublishedGroupCode): Mã phương pháp
5. **Đơn vị tính** (Unit): Đơn vị đo lường
6. **GHĐL/ LOQ**: Giới hạn phát hiện / Giới hạn định lượng
7. **TAT**: Thời gian quay vòng
8. **Đơn giá chuẩn** (DefaultPrice): Giá chuẩn từ master data (read-only, hiển thị)
9. **Đơn giá bán** (UnitPrice): Giá bán có thể chỉnh sửa (input number với format currency)
10. **Ghi chú** (Notes): Ghi chú có thể chỉnh sửa
11. **Thao tác**: Nút xóa item

### Tính năng đặc biệt
- **Row Selection**: Cho phép chọn nhiều items để áp dụng giảm giá hàng loạt
- **Filtering**: Có thể lọc theo Nền Mẫu, Chỉ tiêu, Phương pháp, Đơn vị tính
- **Sorting**: Có thể sắp xếp theo các cột
- **Duplicate Detection**: Highlight các items trùng lặp (cùng AnalysisItemId)

### Cấu trúc Item trong quotationItems (AnalysisItem standalone)

```javascript
{
    key: string,                    // Unique key: "{AnalysisItemId}-standalone-{timestamp}"
    ItemType: "AnalysisItem",       // Loại item
    AnalysisItemId: number,         // ID của AnalysisItem từ master data
    AnalysisGroupId: number | null, // ID nhóm (có thể null nếu là standalone)
    ItemCode: string,               // Mã chỉ tiêu
    ItemNameVi: string,             // Tên tiếng Việt
    ItemNameEn: string,             // Tên tiếng Anh
    SampleMatrixName: string,       // Tên nền mẫu
    PublishedGroupCode: string,     // Mã phương pháp
    Unit: string,                   // Đơn vị tính
    LOD: string,                   // Giới hạn phát hiện
    LOQ: string,                   // Giới hạn định lượng
    DefaultPrice: number,           // Đơn giá chuẩn (từ master data)
    TAT: string,                   // Thời gian quay vòng
    Notes: string,                  // Ghi chú
    Quantity: number,               // Số lượng (mặc định 1)
    UnitPrice: number,             // Đơn giá bán (có thể chỉnh sửa)
    DiscountPercent: number,       // % giảm giá
    DiscountAmount: number,         // Số tiền giảm giá
    SubTotal: number,              // Thành tiền sau giảm giá
    DisplayOrder: number,          // Thứ tự hiển thị
    isStandalone: true              // Đánh dấu là chỉ tiêu lẻ
}
```

### API Operations
- **onUpdateItem(key, field, value)**: Cập nhật field của item (UnitPrice, Notes, Quantity, DiscountPercent)
- **onRemoveItem(key)**: Xóa item khỏi quotationItems
- **onRowSelectionChange(selectedKeys)**: Chọn/bỏ chọn items để áp dụng giảm giá

---

## 2. GroupViewTable (Giá theo Nhóm)

### Mục đích
Hiển thị các chỉ tiêu được nhóm theo AnalysisGroup. Mỗi nhóm có thể expand để xem các chỉ tiêu bên trong. Chỉ hiển thị các items có `ItemType === "AnalysisItem"` và `isStandalone !== true` (tức là được thêm qua nhóm).

### Cấu trúc dữ liệu hiển thị
- **dataSource**: Mảng các group objects, mỗi group chứa:
  - Thông tin nhóm (groupId, groupName, groupCode)
  - Mảng `items`: Các AnalysisItem thuộc nhóm này
  - `WholeGroupStandardPrice`: Tổng giá chuẩn của nhóm (từ AnalysisGroup.stepPrice)
  - `stepPrice`: Bước nhảy giá (từ AnalysisGroup.stepPrice)

### Các cột hiển thị (Group Level)
1. **STT**: Số thứ tự nhóm
2. **Nhóm chỉ tiêu** (groupName): Tên nhóm
3. **Mã nhóm** (groupCode): Mã nhóm
4. **Bước nhảy** (stepPrice): Input number có thể chỉnh sửa, format currency
   - Mặc định load từ `AnalysisGroup.stepPrice`
   - Logic: 
     - Nếu stepPrice > 0: Từ item thứ 2 trở đi (index >= 1) sẽ có UnitPrice = stepPrice
     - Nếu stepPrice = 0: Tất cả items giữ nguyên UnitPrice = DefaultPrice
5. **Giá nhóm chuẩn** (WholeGroupStandardPrice): Tổng giá chuẩn của nhóm (read-only, từ AnalysisGroup.WholeGroupStandardPrice)
6. **Tổng đơn giá bán**: Tổng UnitPrice của tất cả items trong nhóm (read-only, tính toán)
7. **Thao tác**: 
   - Nút "Giảm giá": Áp dụng giảm giá cho toàn bộ nhóm
   - Nút "Xóa nhóm": Xóa tất cả items trong nhóm

### Các cột hiển thị (Item Level - trong nested table)
Khi expand một nhóm, hiển thị bảng con với các cột:
1. **STT**: Số thứ tự trong nhóm
2. **Nền Mẫu** (SampleMatrixName)
3. **Chỉ tiêu** (ItemNameVi)
4. **Phương pháp** (PublishedGroupCode)
5. **Đơn vị tính** (Unit)
6. **GHĐL/ LOQ**
7. **TAT**
8. **Đơn giá chuẩn** (DefaultPrice): Read-only
9. **Đơn giá bán** (UnitPrice): Input number có thể chỉnh sửa, format currency
10. **Ghi chú** (Notes): Input có thể chỉnh sửa
11. **Thao tác**: Nút xóa item

### Logic đặc biệt: StepPrice (Bước nhảy)
Khi user thay đổi `stepPrice` của một nhóm:
1. Cập nhật `stepPrice` trong state (groupStepPrices)
2. Tìm tất cả items trong nhóm đó
3. Với các items từ index >= 1 (item thứ 2 trở đi):
   - Nếu stepPrice > 0: Set `UnitPrice = stepPrice`
   - Nếu stepPrice = 0: Set `UnitPrice = DefaultPrice`
4. Item đầu tiên (index 0) không bị ảnh hưởng
5. Recalculate `DiscountAmount` và `SubTotal` cho các items bị ảnh hưởng

### Cấu trúc Item trong quotationItems (AnalysisItem trong nhóm)

```javascript
{
    key: string,                    // Unique key: "{AnalysisItemId}-group-{timestamp}-{index}"
    ItemType: "AnalysisItem",       // Loại item
    AnalysisItemId: number,         // ID của AnalysisItem
    AnalysisGroupId: number,        // ID nhóm (bắt buộc)
    ItemCode: string,              // Mã chỉ tiêu
    ItemNameVi: string,            // Tên tiếng Việt
    ItemNameEn: string,            // Tên tiếng Anh
    SampleMatrixName: string,      // Tên nền mẫu
    PublishedGroupCode: string,    // Mã phương pháp
    Unit: string,                  // Đơn vị tính
    LOD: string,                   // Giới hạn phát hiện
    LOQ: string,                   // Giới hạn định lượng
    DefaultPrice: number,          // Đơn giá chuẩn
    TAT: string,                  // Thời gian quay vòng
    Notes: string,                // Ghi chú
    Quantity: number,             // Số lượng (mặc định 1)
    UnitPrice: number,            // Đơn giá bán (có thể bị ảnh hưởng bởi stepPrice)
    DiscountPercent: number,      // % giảm giá
    DiscountAmount: number,        // Số tiền giảm giá
    SubTotal: number,             // Thành tiền sau giảm giá
    DisplayOrder: number,         // Thứ tự hiển thị
    isStandalone: false            // Đánh dấu là thêm qua nhóm
}
```

### API Operations
- **onUpdateItem(key, field, value)**: Cập nhật field của item
- **onRemoveItem(key)**: Xóa một item khỏi nhóm
- **onRemoveGroup(itemKeys, groupName, count)**: Xóa toàn bộ nhóm (xóa tất cả items có key trong itemKeys)
- **onGroupDiscount(record)**: Mở modal giảm giá cho nhóm
- **onGroupStepPriceChange(groupId, stepPriceValue)**: Cập nhật stepPrice và áp dụng logic cho items trong nhóm

---

## 3. PackageViewTable (Giá theo Gói)

### Mục đích
Hiển thị các gói phân tích (Package) với các nhóm chỉ tiêu bên trong. Mỗi package có thể chứa nhiều AnalysisGroup, mỗi group có thể expand để xem danh sách chỉ tiêu.

### Cấu trúc dữ liệu hiển thị
- **dataSource**: Mảng các row objects, mỗi row đại diện cho một AnalysisGroup trong một Package
- Mỗi row chứa:
  - Thông tin package (packageId, packageName, packageCode)
  - Thông tin group (groupId, groupName)
  - `packageItem`: Item trong quotationItems có `ItemType === "Package"`
  - `analysisItems`: Danh sách AnalysisItem trong group (chỉ để hiển thị, không edit)
  - `packageInfo`: Thông tin đầy đủ của Package từ master data

### Các cột hiển thị
1. **STT**: Số thứ tự package (rowSpan cho các groups cùng package)
2. **Gói phân tích** (packageName): 
   - Tên gói
   - Nền mẫu (từ Package.SampleMatrix)
   - Phương pháp (từ Package.PublishedGroupCode)
   - RowSpan cho các groups cùng package
3. **Nhóm chỉ tiêu** (groupName): Tên nhóm chỉ tiêu trong gói
4. **Chỉ tiêu** (analysisList): 
   - Danh sách có thể expand/collapse
   - Hiển thị số lượng chỉ tiêu
   - Khi expand, hiển thị danh sách các AnalysisItem (chỉ để xem, không edit)
   - Highlight các items trùng với items ở các view khác
5. **Đơn giá chuẩn** (DefaultPrice): Giá chuẩn của package (rowSpan, read-only)
6. **Đơn giá bán** (UnitPrice): Input number có thể chỉnh sửa, format currency (rowSpan)
7. **Thao tác**: 
   - Nút "Giảm giá": Áp dụng giảm giá cho package
   - Nút "Xóa gói": Xóa package khỏi quotationItems
   - RowSpan cho các groups cùng package

### Cấu trúc Item trong quotationItems (Package)

```javascript
{
    key: string,                   // Unique key: "{PackageId}-standalone-{timestamp}"
    ItemType: "Package",            // Loại item
    PackageId: number,              // ID của Package từ master data
    ItemCode: string,               // Mã gói (PackageCode)
    ItemNameVi: string,             // Tên gói tiếng Việt
    ItemNameEn: string,             // Tên gói tiếng Anh
    DefaultPrice: number,           // Đơn giá chuẩn của gói
    Quantity: number,              // Số lượng (mặc định 1)
    UnitPrice: number,            // Đơn giá bán (có thể chỉnh sửa)
    DiscountPercent: number,      // % giảm giá
    DiscountAmount: number,       // Số tiền giảm giá
    SubTotal: number,             // Thành tiền sau giảm giá
    DisplayOrder: number          // Thứ tự hiển thị
}
```

**Lưu ý**: Package item KHÔNG chứa thông tin về các AnalysisItem bên trong. Các AnalysisItem chỉ được hiển thị từ master data (Package.PackageAnalysisGroups) để tham khảo, không được lưu trong quotationItems.

### API Operations
- **onUpdateItem(key, field, value)**: Cập nhật field của package item (chủ yếu là UnitPrice)
- **onRemovePackage(key, packageName)**: Xóa package khỏi quotationItems
- **onPackageDiscount(record)**: Mở modal giảm giá cho package
- **onToggleAllAnalysisLists(expanded)**: Expand/collapse tất cả danh sách chỉ tiêu
- **onToggleAnalysisList(listKey, expanded)**: Expand/collapse một danh sách chỉ tiêu cụ thể

---

## Cấu trúc tổng quát của quotationItems Array

Mảng `quotationItems` là một mảng phẳng chứa tất cả các items, không phân biệt loại:

```javascript
quotationItems: [
    // AnalysisItem standalone
    {
        key: "123-standalone-1234567890",
        ItemType: "AnalysisItem",
        isStandalone: true,
        AnalysisItemId: 123,
        // ... các fields khác
    },
    // AnalysisItem trong nhóm
    {
        key: "456-group-1234567890-0",
        ItemType: "AnalysisItem",
        isStandalone: false,
        AnalysisItemId: 456,
        AnalysisGroupId: 10,
        // ... các fields khác
    },
    {
        key: "789-group-1234567890-1",
        ItemType: "AnalysisItem",
        isStandalone: false,
        AnalysisItemId: 789,
        AnalysisGroupId: 10,
        // ... các fields khác
    },
    // Package
    {
        key: "999-standalone-1234567890",
        ItemType: "Package",
        PackageId: 999,
        // ... các fields khác
    }
]
```

## Các trường dữ liệu quan trọng cần lưu vào Database

### Bắt buộc cho tất cả items:
- `key`: Unique identifier (có thể dùng làm primary key hoặc tạo ID riêng)
- `ItemType`: "AnalysisItem" hoặc "Package"
- `DisplayOrder`: Thứ tự hiển thị
- `Quantity`: Số lượng
- `UnitPrice`: Đơn giá bán
- `DiscountPercent`: % giảm giá
- `DiscountAmount`: Số tiền giảm giá
- `SubTotal`: Thành tiền sau giảm giá
- `Notes`: Ghi chú

### Cho AnalysisItem:
- `AnalysisItemId`: Reference đến AnalysisItem master
- `AnalysisGroupId`: Reference đến AnalysisGroup (có thể null nếu standalone)
- `isStandalone`: Boolean để phân biệt standalone hay trong nhóm
- `ItemCode`, `ItemNameVi`, `ItemNameEn`: Thông tin từ master (có thể lưu để backup hoặc không)
- `SampleMatrixName`, `PublishedGroupCode`, `Unit`, `LOD`, `LOQ`, `TAT`: Thông tin từ master (có thể lưu để backup hoặc không)
- `DefaultPrice`: Giá chuẩn từ master (có thể lưu để backup hoặc không)

### Cho Package:
- `PackageId`: Reference đến Package master
- `ItemCode`, `ItemNameVi`, `ItemNameEn`: Thông tin từ master (có thể lưu để backup hoặc không)
- `DefaultPrice`: Giá chuẩn từ master (có thể lưu để backup hoặc không)

### Metadata cần lưu:
- `QuotationId`: Reference đến Quotation
- `CreatedDate`, `CreatedBy`: Thông tin tạo
- `ModifiedDate`, `ModifiedBy`: Thông tin sửa

## Logic tính toán

### SubTotal calculation:
```javascript
SubTotal = (Quantity * UnitPrice) - DiscountAmount
DiscountAmount = (Quantity * UnitPrice) * (DiscountPercent / 100)
```

### StepPrice logic (chỉ áp dụng cho GroupView):
- Khi stepPrice > 0: Items từ index >= 1 có UnitPrice = stepPrice
- Khi stepPrice = 0: Items từ index >= 1 có UnitPrice = DefaultPrice
- Item đầu tiên (index 0) không bị ảnh hưởng

## Validation Rules

1. **UnitPrice**: Phải >= 0
2. **Quantity**: Phải >= 1
3. **DiscountPercent**: Phải trong khoảng 0-100
4. **AnalysisItemId**: Bắt buộc nếu ItemType === "AnalysisItem"
5. **PackageId**: Bắt buộc nếu ItemType === "Package"
6. **AnalysisGroupId**: Bắt buộc nếu ItemType === "AnalysisItem" và isStandalone === false

## API Endpoints đề xuất

### GET /api/quotations/{quotationId}/items
Trả về mảng quotationItems

### POST /api/quotations/{quotationId}/items
Thêm item mới

### PUT /api/quotations/{quotationId}/items/{itemKey}
Cập nhật item

### DELETE /api/quotations/{quotationId}/items/{itemKey}
Xóa item

### PUT /api/quotations/{quotationId}/items/batch
Cập nhật nhiều items cùng lúc (cho giảm giá hàng loạt, stepPrice, etc.)

