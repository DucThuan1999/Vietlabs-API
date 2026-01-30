# Hướng dẫn Test POST QuotationItem

## Cách 1: Test bằng Postman

1. Mở Postman
2. Tạo request mới:
   - Method: `POST`
   - URL: `https://localhost:5001/odata/QuotationItems`
   - Headers:
     - `Content-Type: application/json`
     - `Authorization: Bearer YOUR_TOKEN` (nếu cần)
3. Body (raw JSON): Copy nội dung từ file `test-quotation-item.json`
4. Click Send

## Cách 2: Test bằng curl

```bash
curl -X POST "https://localhost:5001/odata/QuotationItems" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d @Scripts/test-quotation-item.json \
  -k
```

## Cách 3: Test bằng PowerShell

```powershell
$body = Get-Content Scripts\test-quotation-item.json -Raw
Invoke-RestMethod -Uri "https://localhost:5001/odata/QuotationItems" `
  -Method Post `
  -Body $body `
  -ContentType "application/json"
```

## Cách 4: Test trực tiếp với Database (C#)

Chạy script test:
```bash
dotnet run --project . -- TestQuotationItem
```

Hoặc tạo một console app riêng để test.

## Lưu ý

1. **Thay đổi GUIDs**: Các GUID trong file JSON là placeholder. Bạn cần thay bằng:
   - `quotationId`: ID của một Quotation có sẵn trong database
   - `analysisItemId`: ID của một AnalysisItem có sẵn trong database
   - `packageId`: ID của một Package có sẵn trong database (nếu test Package)

2. **Lấy dữ liệu thực tế**:
   ```bash
   # Lấy QuotationId
   curl "https://localhost:5001/odata/Quotations?$top=1"
   
   # Lấy AnalysisItemId
   curl "https://localhost:5001/odata/AnalysisItems?$top=1"
   
   # Lấy PackageId
   curl "https://localhost:5001/odata/Packages?$top=1"
   ```

3. **Kiểm tra kết quả**: Sau khi POST thành công, bạn sẽ nhận được response với:
   - `quotationItemId`: ID mới được tạo
   - Các fields đã được snapshot tự động từ master data
   - `createdAt`: Thời gian tạo

## Ví dụ Response thành công

```json
{
  "quotationItemId": "33333333-3333-3333-3333-333333333333",
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
  "subTotal": 150000,
  "createdAt": "2024-01-15T10:30:00Z"
}
```

