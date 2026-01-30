# AnalysisItem API Documentation

Tài liệu này mô tả cách Frontend có thể load và làm việc với dữ liệu AnalysisItem từ API.

## 📋 Tổng quan

**AnalysisItem** (Chỉ tiêu phân tích) là một entity quan trọng trong hệ thống, đại diện cho các chỉ tiêu xét nghiệm có thể được thực hiện trong phòng lab.

**Base URL:** `/odata/AnalysisItems`

**EntitySet:** `AnalysisItems`

---

## 📊 Cấu trúc dữ liệu (Model)

### AnalysisItem

```typescript
interface AnalysisItem {
  // Primary Key
  analysisItemId: string; // GUID
  
  // Thông tin cơ bản
  analysisItemCode?: string; // Mã chỉ tiêu
  nameVi?: string; // Tên tiếng Việt
  nameEn?: string; // Tên tiếng Anh
  organization?: string; // Tổ chức
  
  // Foreign Keys
  equipmentTypeId: string; // GUID - Loại thiết bị
  analysisGroupId: string; // GUID - Nhóm phân tích
  sampleMatrixId: string; // GUID - Mẫu vật
  sampleMatrixGroupId: string; // GUID - Nhóm mẫu vật
  
  // Thông tin kỹ thuật
  publishedGroupCode?: string; // Mã nhóm công bố
  lod?: number; // Limit of Detection (Giới hạn phát hiện)
  loq?: number; // Limit of Quantification (Giới hạn định lượng)
  unitPrice: number; // Đơn giá (decimal, mặc định: 0)
  unit?: string; // Đơn vị tính
  
  // Boolean flags - Các tiêu chuẩn áp dụng
  nd107: boolean; // ND 107
  iso: boolean; // ISO
  cucBvtv: boolean; // Cục BVTV
  boCongThuong: boolean; // Bộ Công Thương
  nafi: boolean; // NAFI
  cucChanNuoi: boolean; // Cục Chăn Nuôi
  
  // Trạng thái
  status: string; // "Active" | "Inactive" (mặc định: "Active")
  notes?: string; // Ghi chú
  
  // Timestamps
  createdAt: string; // ISO 8601 DateTime (UTC)
  updatedAt?: string; // ISO 8601 DateTime (UTC)
  
  // Navigation Properties (chỉ có khi expand)
  equipmentType?: EquipmentType;
  analysisGroup?: AnalysisGroup;
  sampleMatrix?: SampleMatrix;
  sampleMatrixGroup?: SampleMatrixGroup;
  analysisItemTats?: AnalysisItemTat[]; // Danh sách TAT (Turn Around Time)
}
```

### AnalysisItemTat (Navigation Property)

```typescript
interface AnalysisItemTat {
  analysisItemTatId: string; // GUID
  analysisItemId: string; // GUID - Foreign Key
  tatType: string; // "Normal" | "Fast" | "Urgent" | "Thường" | "Nhanh" | "Khẩn"
  tatValue: number; // Giá trị TAT (số ngày hoặc giờ)
  tatUnit: string; // "Days" | "Hours" | "Ngày" | "Giờ" (mặc định: "Days")
  notes?: string; // Ghi chú
  createdAt: string; // ISO 8601 DateTime (UTC)
  updatedAt?: string; // ISO 8601 DateTime (UTC)
}
```

---

## 🔌 API Endpoints

### 1. Lấy danh sách AnalysisItems

**GET** `/odata/AnalysisItems`

**Query Parameters (OData):**
- `$filter` - Lọc dữ liệu
- `$select` - Chọn các trường cần lấy
- `$expand` - Mở rộng navigation properties
- `$orderby` - Sắp xếp
- `$top` - Giới hạn số lượng (max: 2000)
- `$skip` - Bỏ qua số lượng
- `$count` - Đếm tổng số bản ghi

**Response:** `200 OK`
```json
{
  "@odata.context": "https://api.example.com/odata/$metadata#AnalysisItems",
  "value": [
    {
      "analysisItemId": "guid-here",
      "analysisItemCode": "AI-001",
      "nameVi": "Xét nghiệm A",
      "nameEn": "Test A",
      "unitPrice": 100000,
      "status": "Active",
      // ... các trường khác
    }
  ],
  "@odata.count": 100 // Nếu có $count=true
}
```

---

### 2. Lấy một AnalysisItem theo ID

**GET** `/odata/AnalysisItems({key})`

**Parameters:**
- `key` (path) - GUID của AnalysisItem

**Query Parameters (OData):**
- `$select` - Chọn các trường cần lấy
- `$expand` - Mở rộng navigation properties

**Response:** `200 OK`
```json
{
  "analysisItemId": "guid-here",
  "analysisItemCode": "AI-001",
  "nameVi": "Xét nghiệm A",
  // ... các trường khác
}
```

**Response:** `404 Not Found` - Nếu không tìm thấy

---

### 3. Tạo mới AnalysisItem

**POST** `/odata/AnalysisItems`

**Request Body:**
```json
{
  "analysisItemCode": "AI-001",
  "nameVi": "Xét nghiệm mới",
  "nameEn": "New Test",
  "equipmentTypeId": "guid-here",
  "analysisGroupId": "guid-here",
  "sampleMatrixId": "guid-here",
  "sampleMatrixGroupId": "guid-here",
  "unitPrice": 150000,
  "status": "Active"
  // ... các trường khác
}
```

**Response:** `201 Created`
```json
{
  "analysisItemId": "new-guid-here",
  // ... dữ liệu đã tạo
}
```

**Response:** `400 Bad Request` - Nếu dữ liệu không hợp lệ

---

### 4. Cập nhật AnalysisItem

**PUT** `/odata/AnalysisItems({key})`

**Parameters:**
- `key` (path) - GUID của AnalysisItem

**Request Body:**
```json
{
  "analysisItemId": "guid-here",
  "nameVi": "Xét nghiệm đã cập nhật",
  "unitPrice": 200000,
  // ... các trường cần cập nhật
}
```

**Response:** `200 OK` - Cập nhật thành công

**Response:** `404 Not Found` - Nếu không tìm thấy

---

### 5. Xóa AnalysisItem

**DELETE** `/odata/AnalysisItems({key})`

**Parameters:**
- `key` (path) - GUID của AnalysisItem

**Response:** `204 No Content` - Xóa thành công

**Response:** `404 Not Found` - Nếu không tìm thấy

---

## 🔍 OData Query Examples

### 1. Lọc dữ liệu ($filter)

```http
GET /odata/AnalysisItems?$filter=status eq 'Active'
```

Lấy tất cả AnalysisItem có status = "Active"

```http
GET /odata/AnalysisItems?$filter=unitPrice gt 100000
```

Lấy các AnalysisItem có đơn giá > 100,000

```http
GET /odata/AnalysisItems?$filter=contains(nameVi, 'Huyết')
```

Tìm các AnalysisItem có tên tiếng Việt chứa "Huyết"

```http
GET /odata/AnalysisItems?$filter=analysisGroupId eq guid'xxx-xxx-xxx' and status eq 'Active'
```

Lọc theo analysisGroupId và status

---

### 2. Chọn trường ($select)

```http
GET /odata/AnalysisItems?$select=analysisItemId,nameVi,nameEn,unitPrice
```

Chỉ lấy các trường: analysisItemId, nameVi, nameEn, unitPrice

---

### 3. Mở rộng Navigation Properties ($expand)

```http
GET /odata/AnalysisItems?$expand=analysisGroup
```

Lấy AnalysisItem kèm thông tin AnalysisGroup

```http
GET /odata/AnalysisItems?$expand=equipmentType,analysisGroup,sampleMatrix
```

Lấy AnalysisItem kèm nhiều navigation properties

```http
GET /odata/AnalysisItems?$expand=analysisItemTats
```

Lấy AnalysisItem kèm danh sách TAT (Turn Around Time)

```http
GET /odata/AnalysisItems?$expand=analysisGroup,analysisItemTats
```

Lấy AnalysisItem kèm AnalysisGroup và danh sách TAT

---

### 4. Sắp xếp ($orderby)

```http
GET /odata/AnalysisItems?$orderby=nameVi asc
```

Sắp xếp theo tên tiếng Việt tăng dần

```http
GET /odata/AnalysisItems?$orderby=unitPrice desc,createdAt desc
```

Sắp xếp theo đơn giá giảm dần, sau đó theo ngày tạo giảm dần

---

### 5. Phân trang ($top, $skip)

```http
GET /odata/AnalysisItems?$top=10&$skip=0
```

Lấy 10 bản ghi đầu tiên

```http
GET /odata/AnalysisItems?$top=20&$skip=40
```

Lấy 20 bản ghi, bỏ qua 40 bản ghi đầu (trang 3, mỗi trang 20)

---

### 6. Đếm tổng số ($count)

```http
GET /odata/AnalysisItems?$count=true
```

Lấy danh sách kèm tổng số bản ghi

---

### 7. Kết hợp nhiều query options

```http
GET /odata/AnalysisItems?$filter=status eq 'Active'&$expand=analysisGroup,analysisItemTats&$select=analysisItemId,nameVi,unitPrice,analysisGroup,analysisItemTats&$orderby=nameVi asc&$top=50
```

Lấy 50 AnalysisItem đang active, kèm AnalysisGroup và TAT, sắp xếp theo tên, chỉ lấy các trường cần thiết

---

## 📦 Navigation Properties

### EquipmentType
```http
GET /odata/AnalysisItems?$expand=equipmentType
```

### AnalysisGroup
```http
GET /odata/AnalysisItems?$expand=analysisGroup
```

### SampleMatrix
```http
GET /odata/AnalysisItems?$expand=sampleMatrix
```

### SampleMatrixGroup
```http
GET /odata/AnalysisItems?$expand=sampleMatrixGroup
```

### AnalysisItemTats (Collection)
```http
GET /odata/AnalysisItems?$expand=analysisItemTats
```

**Lưu ý:** Mỗi AnalysisItem có thể có nhiều TAT với các loại khác nhau (Thường, Nhanh, Khẩn)

---

## 💡 Use Cases phổ biến

### 1. Load danh sách AnalysisItem cho dropdown/select

```http
GET /odata/AnalysisItems?$filter=status eq 'Active'&$select=analysisItemId,nameVi,nameEn&$orderby=nameVi asc
```

### 2. Load AnalysisItem với đầy đủ thông tin liên quan

```http
GET /odata/AnalysisItems?$expand=equipmentType,analysisGroup,sampleMatrix,sampleMatrixGroup,analysisItemTats
```

### 3. Tìm kiếm AnalysisItem theo tên

```http
GET /odata/AnalysisItems?$filter=contains(nameVi, 'Huyết') or contains(nameEn, 'Blood')&$expand=analysisGroup
```

### 4. Load AnalysisItem theo nhóm phân tích

```http
GET /odata/AnalysisItems?$filter=analysisGroupId eq guid'xxx-xxx-xxx'&$expand=analysisItemTats
```

### 5. Load AnalysisItem với phân trang

```http
GET /odata/AnalysisItems?$filter=status eq 'Active'&$orderby=createdAt desc&$top=20&$skip=0&$count=true
```

### 6. Load một AnalysisItem cụ thể với TAT

```http
GET /odata/AnalysisItems(guid-here)?$expand=analysisItemTats,analysisGroup
```

---

## ⚠️ Lưu ý quan trọng

1. **GUID Format:** Tất cả GUID phải được format đúng trong OData query:
   ```
   guid'12345678-1234-1234-1234-123456789012'
   ```

2. **CamelCase:** API trả về dữ liệu dưới dạng camelCase (analysisItemId, not AnalysisItemId)

3. **DateTime Format:** Tất cả DateTime được trả về dưới dạng ISO 8601 (UTC)

4. **Default Values:**
   - `status`: "Active"
   - `unitPrice`: 0
   - `tatUnit`: "Days"

5. **Required Fields khi tạo mới:**
   - `equipmentTypeId`
   - `analysisGroupId`
   - `sampleMatrixId`
   - `sampleMatrixGroupId`

6. **Max Top:** Giới hạn tối đa cho `$top` là 2000

7. **Expand Multiple:** Có thể expand nhiều navigation properties cùng lúc, cách nhau bởi dấu phẩy:
   ```
   $expand=analysisGroup,equipmentType,analysisItemTats
   ```

---

## 🔗 Related APIs

- **AnalysisGroups:** `/odata/AnalysisGroups`
- **EquipmentTypes:** `/odata/EquipmentTypes`
- **SampleMatrices:** `/odata/SampleMatrices`
- **SampleMatrixGroups:** `/odata/SampleMatrixGroups`
- **AnalysisItemTats:** `/odata/AnalysisItemTats`

---

## 📝 Ví dụ JavaScript/TypeScript

### Sử dụng fetch API

```typescript
// Lấy danh sách AnalysisItem với expand
async function getAnalysisItems() {
  const response = await fetch(
    '/odata/AnalysisItems?$expand=analysisGroup,analysisItemTats&$filter=status eq \'Active\'',
    {
      headers: {
        'Authorization': 'Bearer YOUR_TOKEN',
        'Content-Type': 'application/json'
      }
  });
  
  const data = await response.json();
  return data.value;
}

// Lấy một AnalysisItem theo ID
async function getAnalysisItemById(id: string) {
  const response = await fetch(
    `/odata/AnalysisItems(${id})?$expand=analysisItemTats`,
    {
      headers: {
        'Authorization': 'Bearer YOUR_TOKEN',
        'Content-Type': 'application/json'
      }
  });
  
  return await response.json();
}

// Tạo mới AnalysisItem
async function createAnalysisItem(item: Partial<AnalysisItem>) {
  const response = await fetch('/odata/AnalysisItems', {
    method: 'POST',
    headers: {
      'Authorization': 'Bearer YOUR_TOKEN',
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(item)
  });
  
  return await response.json();
}
```

### Sử dụng axios

```typescript
import axios from 'axios';

// Lấy danh sách với filter và expand
const getAnalysisItems = async () => {
  const response = await axios.get('/odata/AnalysisItems', {
    params: {
      '$expand': 'analysisGroup,analysisItemTats',
      '$filter': "status eq 'Active'",
      '$orderby': 'nameVi asc',
      '$top': 50
    }
  });
  return response.data.value;
};
```

---

## 🎯 Best Practices

1. **Luôn filter theo status** khi load danh sách để chỉ lấy dữ liệu active
2. **Sử dụng $select** để chỉ lấy các trường cần thiết, giảm băng thông
3. **Sử dụng $expand** một cách hợp lý, không expand quá nhiều navigation properties cùng lúc
4. **Sử dụng phân trang** ($top, $skip) cho danh sách lớn
5. **Cache dữ liệu** ở Frontend cho các dữ liệu ít thay đổi (như AnalysisGroup, EquipmentType)

---

**Cập nhật lần cuối:** 2024
**Version:** 1.0

