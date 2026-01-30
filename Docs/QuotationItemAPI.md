# QuotationItem API Schema - Hướng dẫn cho Frontend

## 📋 Tổng quan

**QuotationItem** (Chi tiết báo giá) là entity lưu trữ các items trong một báo giá. Hỗ trợ 3 loại items:
1. **AnalysisItem standalone** - Chỉ tiêu phân tích đứng riêng
2. **AnalysisItem trong nhóm** - Chỉ tiêu phân tích thuộc một nhóm
3. **Package** - Gói phân tích

**Base URL:** `/odata/QuotationItems`

**EntitySet:** `QuotationItems`

---

## 📊 Cấu trúc dữ liệu (TypeScript Interface)

```typescript
interface QuotationItem {
  // ============================================
  // PRIMARY KEY & FOREIGN KEYS
  // ============================================
  
  quotationItemId?: string; // GUID - Optional khi POST (backend sẽ tự tạo)
  quotationId: string; // GUID - BẮT BUỘC - ID của Quotation chứa item này
  
  // ============================================
  // ITEM TYPE & IDENTIFICATION
  // ============================================
  
  itemType: "AnalysisItem" | "AnalysisGroup" | "Package"; // BẮT BUỘC
  
  // Foreign keys (chỉ một trong 3 có giá trị tùy theo itemType)
  analysisItemId?: string; // GUID - BẮT BUỘC nếu itemType = "AnalysisItem"
  analysisGroupId?: string; // GUID - BẮT BUỘC nếu itemType = "AnalysisItem" và isStandalone = false
  packageId?: string; // GUID - BẮT BUỘC nếu itemType = "Package"
  
  // Phân biệt AnalysisItem standalone hay trong nhóm
  isStandalone?: boolean; // BẮT BUỘC nếu itemType = "AnalysisItem"
  // - true: AnalysisItem đứng riêng (standalone) - dùng cho CriterionViewTable
  // - false: AnalysisItem trong nhóm (thêm qua nhóm) - dùng cho GroupViewTable
  // - null/undefined: Không phải AnalysisItem (Package)
  
  // ============================================
  // THÔNG TIN HIỂN THỊ (Optional - Backend sẽ tự động snapshot nếu null)
  // ============================================
  
  itemCode?: string; // Mã item (AnalysisItemCode hoặc PackageCode)
  itemNameVi?: string; // Tên tiếng Việt
  itemNameEn?: string; // Tên tiếng Anh
  description?: string; // Mô tả
  
  // ============================================
  // SNAPSHOT DỮ LIỆU (Optional - Backend sẽ tự động snapshot khi tạo)
  // Chỉ có giá trị khi itemType = "AnalysisItem"
  // ============================================
  
  sampleMatrixName?: string; // Tên nền mẫu (snapshot từ SampleMatrix.NameVi)
  publishedGroupCode?: string; // Mã phương pháp (snapshot từ AnalysisItem.PublishedGroupCode)
  unit?: string; // Đơn vị tính (snapshot từ AnalysisItem.Unit)
  lod?: string; // Giới hạn phát hiện (snapshot từ AnalysisItem.Lod, format string)
  loq?: string; // Giới hạn định lượng (snapshot từ AnalysisItem.Loq, format string)
  tat?: string; // Thời gian quay vòng (snapshot từ AnalysisItemTat, format string)
  
  // ============================================
  // THÔNG TIN GIÁ VÀ SỐ LƯỢNG
  // ============================================
  
  quantity: number; // BẮT BUỘC - Số lượng (mặc định: 1, phải >= 1)
  defaultPrice?: number; // Đơn giá chuẩn (snapshot từ AnalysisItem.UnitPrice hoặc Package.DefaultPrice)
  unitPrice: number; // BẮT BUỘC - Đơn giá bán (có thể chỉnh sửa, phải >= 0)
  discountPercent?: number; // % giảm giá (0-100)
  discountAmount?: number; // Số tiền giảm giá (>= 0)
  subTotal: number; // BẮT BUỘC - Thành tiền (Quantity * UnitPrice - DiscountAmount)
  
  // ============================================
  // THÔNG TIN BỔ SUNG
  // ============================================
  
  displayOrder?: number; // Thứ tự hiển thị
  notes?: string; // Ghi chú
  
  // ============================================
  // METADATA (Tự động set bởi backend - KHÔNG GỬI KHI POST/PUT)
  // ============================================
  
  createdAt?: string; // ISO 8601 DateTime (UTC) - Tự động set bởi backend
  updatedAt?: string; // ISO 8601 DateTime (UTC) - Tự động set bởi backend
  
  // ============================================
  // NAVIGATION PROPERTIES (Chỉ có khi expand - KHÔNG GỬI KHI POST/PUT)
  // ============================================
  
  quotation?: Quotation;
  analysisItem?: AnalysisItem;
  analysisGroup?: AnalysisGroup;
  package?: Package;
}
```

---

## 🎯 Các trường bắt buộc theo loại Item

### 1. AnalysisItem Standalone (CriterionViewTable)

**Bắt buộc:**
- `quotationId` ✅
- `itemType: "AnalysisItem"` ✅
- `analysisItemId` ✅
- `isStandalone: true` ✅
- `quantity` ✅ (mặc định: 1)
- `unitPrice` ✅

**Optional (Backend sẽ tự snapshot nếu null):**
- `itemCode`, `itemNameVi`, `itemNameEn`
- `sampleMatrixName`, `publishedGroupCode`, `unit`, `lod`, `loq`, `tat`
- `defaultPrice`
- `discountPercent`, `discountAmount`
- `displayOrder`, `notes`

### 2. AnalysisItem trong nhóm (GroupViewTable)

**Bắt buộc:**
- `quotationId` ✅
- `itemType: "AnalysisItem"` ✅
- `analysisItemId` ✅
- `analysisGroupId` ✅
- `isStandalone: false` ✅
- `quantity` ✅ (mặc định: 1)
- `unitPrice` ✅

**Optional (Backend sẽ tự snapshot nếu null):**
- `itemCode`, `itemNameVi`, `itemNameEn`
- `sampleMatrixName`, `publishedGroupCode`, `unit`, `lod`, `loq`, `tat`
- `defaultPrice`
- `discountPercent`, `discountAmount`
- `displayOrder`, `notes`

### 3. Package (PackageViewTable)

**Bắt buộc:**
- `quotationId` ✅
- `itemType: "Package"` ✅
- `packageId` ✅
- `quantity` ✅ (mặc định: 1)
- `unitPrice` ✅

**Optional (Backend sẽ tự snapshot nếu null):**
- `itemCode`, `itemNameVi`, `itemNameEn`
- `defaultPrice`
- `discountPercent`, `discountAmount`
- `displayOrder`, `notes`

**Lưu ý:** Package KHÔNG có các fields snapshot từ AnalysisItem (sampleMatrixName, publishedGroupCode, unit, lod, loq, tat)

---

## 📤 Ví dụ Request Body

### 1. Tạo AnalysisItem Standalone (Tối thiểu)

```json
{
  "quotationId": "12345678-1234-1234-1234-123456789012",
  "itemType": "AnalysisItem",
  "analysisItemId": "87654321-4321-4321-4321-210987654321",
  "isStandalone": true,
  "quantity": 1,
  "unitPrice": 150000,
  "subTotal": 150000
}
```

**Backend sẽ tự động snapshot:**
- `itemCode` từ `AnalysisItem.AnalysisItemCode`
- `itemNameVi` từ `AnalysisItem.NameVi`
- `itemNameEn` từ `AnalysisItem.NameEn`
- `sampleMatrixName` từ `AnalysisItem.SampleMatrix.NameVi`
- `publishedGroupCode` từ `AnalysisItem.PublishedGroupCode`
- `unit` từ `AnalysisItem.Unit`
- `lod` từ `AnalysisItem.Lod` (format string)
- `loq` từ `AnalysisItem.Loq` (format string)
- `tat` từ `AnalysisItem.AnalysisItemTats` (format string)
- `defaultPrice` từ `AnalysisItem.UnitPrice`

### 2. Tạo AnalysisItem Standalone (Đầy đủ)

```json
{
  "quotationId": "12345678-1234-1234-1234-123456789012",
  "itemType": "AnalysisItem",
  "analysisItemId": "87654321-4321-4321-4321-210987654321",
  "isStandalone": true,
  "itemCode": "AI-001",
  "itemNameVi": "Xét nghiệm A",
  "itemNameEn": "Test A",
  "sampleMatrixName": "Mẫu nước",
  "publishedGroupCode": "TCVN-123",
  "unit": "mg/L",
  "lod": "0.001",
  "loq": "0.005",
  "tat": "5 Days",
  "quantity": 1,
  "defaultPrice": 150000,
  "unitPrice": 150000,
  "discountPercent": 10,
  "discountAmount": 15000,
  "subTotal": 135000,
  "displayOrder": 1,
  "notes": "Ghi chú cho item này"
}
```

### 3. Tạo AnalysisItem trong nhóm

```json
{
  "quotationId": "12345678-1234-1234-1234-123456789012",
  "itemType": "AnalysisItem",
  "analysisItemId": "87654321-4321-4321-4321-210987654321",
  "analysisGroupId": "11111111-1111-1111-1111-111111111111",
  "isStandalone": false,
  "quantity": 1,
  "unitPrice": 150000,
  "subTotal": 150000
}
```

### 4. Tạo Package

```json
{
  "quotationId": "12345678-1234-1234-1234-123456789012",
  "itemType": "Package",
  "packageId": "22222222-2222-2222-2222-222222222222",
  "quantity": 1,
  "unitPrice": 500000,
  "subTotal": 500000
}
```

**Backend sẽ tự động snapshot:**
- `itemCode` từ `Package.PackageCode`
- `itemNameVi` từ `Package.NameVi`
- `itemNameEn` từ `Package.NameEn`
- `defaultPrice` từ `Package.DefaultPrice`

---

## 📥 Ví dụ Response

### Success Response (201 Created)

```json
{
  "quotationItemId": "33333333-3333-3333-3333-333333333333",
  "quotationId": "12345678-1234-1234-1234-123456789012",
  "itemType": "AnalysisItem",
  "analysisItemId": "87654321-4321-4321-4321-210987654321",
  "analysisGroupId": null,
  "packageId": null,
  "isStandalone": true,
  "itemCode": "AI-001",
  "itemNameVi": "Xét nghiệm A",
  "itemNameEn": "Test A",
  "description": null,
  "sampleMatrixName": "Mẫu nước",
  "publishedGroupCode": "TCVN-123",
  "unit": "mg/L",
  "lod": "0.001",
  "loq": "0.005",
  "tat": "5 Days",
  "quantity": 1,
  "defaultPrice": 150000,
  "unitPrice": 150000,
  "discountPercent": 10,
  "discountAmount": 15000,
  "subTotal": 135000,
  "displayOrder": 1,
  "notes": "Ghi chú cho item này",
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": null
}
```

### Error Response (400 Bad Request)

```json
{
  "error": {
    "code": "ValidationError",
    "message": "Validation failed",
    "details": [
      {
        "field": "quotationId",
        "message": "QuotationId là bắt buộc"
      },
      {
        "field": "itemType",
        "message": "ItemType là bắt buộc"
      },
      {
        "field": "analysisItemId",
        "message": "AnalysisItemId là bắt buộc khi ItemType = 'AnalysisItem'"
      }
    ]
  }
}
```

---

## ✅ Validation Rules

### 1. Required Fields

| Field | Required When |
|-------|---------------|
| `quotationId` | Luôn luôn |
| `itemType` | Luôn luôn |
| `analysisItemId` | `itemType = "AnalysisItem"` |
| `analysisGroupId` | `itemType = "AnalysisItem"` và `isStandalone = false` |
| `packageId` | `itemType = "Package"` |
| `isStandalone` | `itemType = "AnalysisItem"` |
| `quantity` | Luôn luôn (mặc định: 1) |
| `unitPrice` | Luôn luôn |
| `subTotal` | Luôn luôn |

### 2. Value Constraints

- `quantity`: Phải >= 1
- `unitPrice`: Phải >= 0
- `defaultPrice`: Phải >= 0 (nếu có)
- `discountPercent`: Phải trong khoảng 0-100 (nếu có)
- `discountAmount`: Phải >= 0 (nếu có)
- `subTotal`: Phải >= 0
- `itemType`: Chỉ nhận giá trị: "AnalysisItem", "AnalysisGroup", "Package"
- `isStandalone`: Chỉ có giá trị khi `itemType = "AnalysisItem"`

### 3. Business Rules

1. **Chỉ một trong 3 foreign keys có giá trị:**
   - Nếu `itemType = "AnalysisItem"`: `analysisItemId` phải có giá trị
   - Nếu `itemType = "Package"`: `packageId` phải có giá trị
   - `analysisGroupId` chỉ có giá trị khi `itemType = "AnalysisItem"` và `isStandalone = false`

2. **Tính toán SubTotal:**
   ```javascript
   SubTotal = (Quantity * UnitPrice) - DiscountAmount
   DiscountAmount = (Quantity * UnitPrice) * (DiscountPercent / 100)
   ```
   Frontend có thể tính trước hoặc để backend tính (khuyến nghị để backend tính)

---

## 🔌 API Endpoints

### 1. Lấy danh sách QuotationItems

**GET** `/odata/QuotationItems`

**Query Parameters:**
- `$filter`: Lọc dữ liệu
- `$orderby`: Sắp xếp
- `$top`: Giới hạn số lượng
- `$skip`: Bỏ qua số lượng
- `$expand`: Expand navigation properties

**Ví dụ:**
```http
GET /odata/QuotationItems?$filter=quotationId eq guid'12345678-1234-1234-1234-123456789012'&$expand=analysisItem,package
```

**Response:** `200 OK`
```json
{
  "@odata.context": "...",
  "value": [
    {
      "quotationItemId": "...",
      // ... dữ liệu
    }
  ]
}
```

### 2. Lấy một QuotationItem theo ID

**GET** `/odata/QuotationItems({key})`

**Parameters:**
- `key` (path) - GUID của QuotationItem

**Ví dụ:**
```http
GET /odata/QuotationItems(33333333-3333-3333-3333-333333333333)?$expand=analysisItem,package
```

**Response:** `200 OK` hoặc `404 Not Found`

### 3. Tạo mới QuotationItem

**POST** `/odata/QuotationItems`

**Request Body:** Xem ví dụ ở trên

**Response:** `201 Created` - Trả về QuotationItem đã tạo

**Response:** `400 Bad Request` - Nếu dữ liệu không hợp lệ

### 4. Cập nhật QuotationItem

**PUT** `/odata/QuotationItems({key})`

**Parameters:**
- `key` (path) - GUID của QuotationItem

**Request Body:** Tương tự POST, nhưng phải có `quotationItemId`

**Response:** `200 OK` - Cập nhật thành công

**Response:** `404 Not Found` - Nếu không tìm thấy

### 5. Xóa QuotationItem

**DELETE** `/odata/QuotationItems({key})`

**Parameters:**
- `key` (path) - GUID của QuotationItem

**Response:** `204 No Content` - Xóa thành công

**Response:** `404 Not Found` - Nếu không tìm thấy

---

## 💡 Ví dụ Code JavaScript/TypeScript

### 1. Tạo AnalysisItem Standalone

```typescript
async function createAnalysisItemStandalone(
  quotationId: string,
  analysisItemId: string,
  unitPrice: number,
  quantity: number = 1
) {
  const response = await fetch('/odata/QuotationItems', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer YOUR_TOKEN'
    },
    body: JSON.stringify({
      quotationId: quotationId,
      itemType: 'AnalysisItem',
      analysisItemId: analysisItemId,
      isStandalone: true,
      quantity: quantity,
      unitPrice: unitPrice,
      subTotal: quantity * unitPrice
    })
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Failed to create quotation item');
  }

  return await response.json();
}
```

### 2. Tạo AnalysisItem trong nhóm

```typescript
async function createAnalysisItemInGroup(
  quotationId: string,
  analysisItemId: string,
  analysisGroupId: string,
  unitPrice: number,
  quantity: number = 1
) {
  const response = await fetch('/odata/QuotationItems', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer YOUR_TOKEN'
    },
    body: JSON.stringify({
      quotationId: quotationId,
      itemType: 'AnalysisItem',
      analysisItemId: analysisItemId,
      analysisGroupId: analysisGroupId,
      isStandalone: false,
      quantity: quantity,
      unitPrice: unitPrice,
      subTotal: quantity * unitPrice
    })
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Failed to create quotation item');
  }

  return await response.json();
}
```

### 3. Tạo Package

```typescript
async function createPackageItem(
  quotationId: string,
  packageId: string,
  unitPrice: number,
  quantity: number = 1
) {
  const response = await fetch('/odata/QuotationItems', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer YOUR_TOKEN'
    },
    body: JSON.stringify({
      quotationId: quotationId,
      itemType: 'Package',
      packageId: packageId,
      quantity: quantity,
      unitPrice: unitPrice,
      subTotal: quantity * unitPrice
    })
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Failed to create quotation item');
  }

  return await response.json();
}
```

### 4. Lấy danh sách QuotationItems của một Quotation

```typescript
async function getQuotationItems(quotationId: string) {
  const guid = `guid'${quotationId}'`;
  const response = await fetch(
    `/odata/QuotationItems?$filter=quotationId eq ${guid}&$expand=analysisItem,package,analysisGroup&$orderby=displayOrder`,
    {
      headers: {
        'Authorization': 'Bearer YOUR_TOKEN'
      }
    }
  );

  if (!response.ok) {
    throw new Error('Failed to fetch quotation items');
  }

  const data = await response.json();
  return data.value;
}
```

### 5. Cập nhật UnitPrice và tính lại SubTotal

```typescript
async function updateQuotationItemPrice(
  quotationItemId: string,
  unitPrice: number,
  discountPercent?: number
) {
  // Lấy item hiện tại
  const currentItem = await fetch(
    `/odata/QuotationItems(${quotationItemId})`,
    {
      headers: {
        'Authorization': 'Bearer YOUR_TOKEN'
      }
    }
  ).then(res => res.json());

  // Tính lại discountAmount và subTotal
  const discountAmount = discountPercent
    ? (currentItem.quantity * unitPrice * discountPercent) / 100
    : currentItem.discountAmount || 0;

  const subTotal = (currentItem.quantity * unitPrice) - discountAmount;

  // Cập nhật
  const response = await fetch(`/odata/QuotationItems(${quotationItemId})`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': 'Bearer YOUR_TOKEN'
    },
    body: JSON.stringify({
      ...currentItem,
      unitPrice: unitPrice,
      discountPercent: discountPercent,
      discountAmount: discountAmount,
      subTotal: subTotal
    })
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Failed to update quotation item');
  }

  return await response.json();
}
```

---

## ⚠️ Lưu ý quan trọng

### 1. Snapshot tự động

Backend sẽ **tự động snapshot** các fields sau từ master data khi tạo QuotationItem (nếu các fields này null):

**Cho AnalysisItem:**
- `itemCode` ← `AnalysisItem.AnalysisItemCode`
- `itemNameVi` ← `AnalysisItem.NameVi`
- `itemNameEn` ← `AnalysisItem.NameEn`
- `sampleMatrixName` ← `AnalysisItem.SampleMatrix.NameVi`
- `publishedGroupCode` ← `AnalysisItem.PublishedGroupCode`
- `unit` ← `AnalysisItem.Unit`
- `lod` ← `AnalysisItem.Lod` (format string)
- `loq` ← `AnalysisItem.Loq` (format string)
- `tat` ← `AnalysisItem.AnalysisItemTats` (format string: "5 Days, 3 Hours")
- `defaultPrice` ← `AnalysisItem.UnitPrice`

**Cho Package:**
- `itemCode` ← `Package.PackageCode`
- `itemNameVi` ← `Package.NameVi`
- `itemNameEn` ← `Package.NameEn`
- `defaultPrice` ← `Package.DefaultPrice`

**Frontend có thể:**
- ✅ Gửi các fields này nếu muốn override giá trị từ master data
- ✅ Để null/undefined và để backend tự snapshot

### 2. Navigation Properties

**KHÔNG gửi** các navigation properties khi POST/PUT:
- `quotation`
- `analysisItem`
- `analysisGroup`
- `package`

Các properties này chỉ có khi expand trong GET request.

### 3. Metadata Fields

**KHÔNG gửi** các metadata fields khi POST/PUT:
- `quotationItemId` (có thể để null hoặc Guid.Empty, backend sẽ tự tạo)
- `createdAt` (backend tự set)
- `updatedAt` (backend tự set khi update)

### 4. GUID Format trong OData Query

Khi filter bằng GUID trong OData query, phải format:
```
guid'12345678-1234-1234-1234-123456789012'
```

### 5. CamelCase

API trả về dữ liệu dưới dạng **camelCase**:
- `quotationItemId` (không phải `QuotationItemId`)
- `itemType` (không phải `ItemType`)
- `isStandalone` (không phải `IsStandalone`)

### 6. DateTime Format

Tất cả DateTime được trả về dưới dạng **ISO 8601 (UTC)**:
```
"2024-01-15T10:30:00Z"
```

---

## 🔗 Related APIs

- **Quotations:** `/odata/Quotations`
- **AnalysisItems:** `/odata/AnalysisItems`
- **AnalysisGroups:** `/odata/AnalysisGroups`
- **Packages:** `/odata/Packages`

---

## 📝 Checklist khi tạo QuotationItem

### AnalysisItem Standalone
- [ ] `quotationId` đã có giá trị
- [ ] `itemType = "AnalysisItem"`
- [ ] `analysisItemId` đã có giá trị
- [ ] `isStandalone = true`
- [ ] `quantity >= 1`
- [ ] `unitPrice >= 0`
- [ ] `subTotal` đã tính đúng

### AnalysisItem trong nhóm
- [ ] `quotationId` đã có giá trị
- [ ] `itemType = "AnalysisItem"`
- [ ] `analysisItemId` đã có giá trị
- [ ] `analysisGroupId` đã có giá trị
- [ ] `isStandalone = false`
- [ ] `quantity >= 1`
- [ ] `unitPrice >= 0`
- [ ] `subTotal` đã tính đúng

### Package
- [ ] `quotationId` đã có giá trị
- [ ] `itemType = "Package"`
- [ ] `packageId` đã có giá trị
- [ ] `quantity >= 1`
- [ ] `unitPrice >= 0`
- [ ] `subTotal` đã tính đúng

---

## 🆘 Troubleshooting

### Lỗi: "AnalysisItemId là bắt buộc khi ItemType = 'AnalysisItem'"
**Giải pháp:** Đảm bảo `analysisItemId` có giá trị khi `itemType = "AnalysisItem"`

### Lỗi: "AnalysisGroupId là bắt buộc khi ItemType = 'AnalysisItem' và IsStandalone = false"
**Giải pháp:** Đảm bảo `analysisGroupId` có giá trị khi `itemType = "AnalysisItem"` và `isStandalone = false`

### Lỗi: "PackageId là bắt buộc khi ItemType = 'Package'"
**Giải pháp:** Đảm bảo `packageId` có giá trị khi `itemType = "Package"`

### Lỗi: "Quantity phải >= 1"
**Giải pháp:** Đảm bảo `quantity >= 1`

### Lỗi: "UnitPrice phải >= 0"
**Giải pháp:** Đảm bảo `unitPrice >= 0`

---

## 📞 Hỗ trợ

Nếu gặp vấn đề, vui lòng liên hệ team backend hoặc xem thêm tài liệu:
- `Docs/QuotationItemsTableStructure.md` - Cấu trúc chi tiết của QuotationItems
- `Docs/AnalysisItemAPI.md` - API của AnalysisItem

