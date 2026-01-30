-- Script thêm cột step_price vào bảng analysis_group
-- Cột này lưu giá bước nhảy (Step Price) - giá cho các item từ thứ 2 trở đi trong nhóm
--
USE [VietLabs]; -- Thay đổi tên database nếu cần
GO

-- =============================================
-- THÊM CỘT step_price VÀO BẢNG analysis_group
-- =============================================

-- Kiểm tra xem cột đã tồn tại chưa
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[analysis_group]') 
    AND name = 'step_price'
)
BEGIN
    -- Thêm cột step_price
    ALTER TABLE [dbo].[analysis_group]
    ADD [step_price] DECIMAL(18,2) NULL;
    
    PRINT N'Đã thêm cột step_price vào bảng analysis_group thành công!';
END
ELSE
BEGIN
    PRINT N'Cột step_price đã tồn tại trong bảng analysis_group.';
END
GO

-- =============================================
-- KIỂM TRA KẾT QUẢ
-- =============================================
-- Xem thông tin cột step_price
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
AND c.name = 'step_price';

PRINT N'Hoàn tất thêm cột step_price vào bảng analysis_group!';
GO

-- =============================================
-- GHI CHÚ VỀ step_price
-- =============================================
/*
STEP PRICE (Giá bước nhảy):

1. Mục đích:
   - Lưu giá bước nhảy cho các item từ thứ 2 trở đi trong nhóm
   - Khi step_price > 0: Các item từ index >= 1 sẽ có UnitPrice = step_price
   - Khi step_price = 0 hoặc NULL: Tất cả items giữ nguyên UnitPrice = DefaultPrice

2. Logic sử dụng:
   - Nếu stepPrice > 0: Items từ index >= 1 có UnitPrice = stepPrice
   - Nếu stepPrice = 0: Items từ index >= 1 có UnitPrice = DefaultPrice
   - Item đầu tiên (index 0) không bị ảnh hưởng bởi stepPrice

3. Ví dụ:
   - Nhóm có 5 items, step_price = 100000
   - Item 1 (index 0): UnitPrice = DefaultPrice (ví dụ: 200000)
   - Item 2-5 (index 1-4): UnitPrice = step_price = 100000

4. Kiểu dữ liệu:
   - DECIMAL(18,2): Cho phép giá trị từ -999,999,999,999,999.99 đến 999,999,999,999,999.99
   - NULL: Cho phép giá trị null (chưa set giá bước nhảy)
*/

