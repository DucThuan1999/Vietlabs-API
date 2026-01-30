-- Script thêm cột whole_group_standard_price vào bảng analysis_group
-- Cột này lưu giá group (Whole group standard) - giá mặc định cho cả group

USE [VietLabs]; -- Thay đổi tên database nếu cần
GO

-- =============================================
-- THÊM CỘT whole_group_standard_price VÀO BẢNG analysis_group
-- =============================================

-- Kiểm tra xem cột đã tồn tại chưa
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[analysis_group]') 
    AND name = 'whole_group_standard_price'
)
BEGIN
    -- Thêm cột whole_group_standard_price
    ALTER TABLE [dbo].[analysis_group]
    ADD [whole_group_standard_price] DECIMAL(18,2) NULL;
    
    PRINT N'Đã thêm cột whole_group_standard_price vào bảng analysis_group thành công!';
END
ELSE
BEGIN
    PRINT N'Cột whole_group_standard_price đã tồn tại trong bảng analysis_group.';
END
GO

-- =============================================
-- KIỂM TRA KẾT QUẢ
-- =============================================
-- Xem thông tin cột whole_group_standard_price
SELECT 
    c.name AS ColumnName,
    t.name AS DataType,
    c.max_length AS MaxLength,
    c.precision AS Precision,
    c.scale AS Scale,
    c.is_nullable AS IsNullable,
    dc.definition AS DefaultValue
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
LEFT JOIN sys.default_constraints dc ON c.default_object_id = dc.object_id
WHERE c.object_id = OBJECT_ID(N'[dbo].[analysis_group]')
AND c.name = 'whole_group_standard_price';

PRINT N'Hoàn tất thêm cột whole_group_standard_price vào bảng analysis_group!';
GO

-- =============================================
-- GHI CHÚ VỀ whole_group_standard_price
-- =============================================
/*
WHOLE GROUP STANDARD PRICE:

1. Mục đích:
   - Lưu giá mặc định cho cả AnalysisGroup
   - Có thể dùng làm giá tham chiếu khi tạo QuotationItem với ItemType = "AnalysisGroup"
   - Khác với step pricing: đây là giá cố định cho cả group

2. Sử dụng:
   - Khi tạo QuotationItem với AnalysisGroup, có thể lấy giá này làm UnitPrice
   - Có thể override trong QuotationItem nếu cần
   - Không bắt buộc (nullable) - có thể để NULL nếu không có giá mặc định

3. Ví dụ:
   - AnalysisGroup "Huyết học" có whole_group_standard_price = 2,000,000 VND
   - Khi thêm AnalysisGroup này vào Quotation, giá mặc định sẽ là 2,000,000 VND
   - Có thể thay đổi giá này trong QuotationItem nếu cần
*/

