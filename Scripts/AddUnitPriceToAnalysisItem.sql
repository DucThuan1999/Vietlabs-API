-- Script thêm cột unit_price vào bảng analysis_item
-- Cột này lưu đơn giá của AnalysisItem

USE [VietLabs]; -- Thay đổi tên database nếu cần
GO

-- =============================================
-- THÊM CỘT unit_price VÀO BẢNG analysis_item
-- =============================================

-- Kiểm tra xem cột đã tồn tại chưa
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[analysis_item]') 
    AND name = 'unit_price'
)
BEGIN
    -- Thêm cột unit_price
    ALTER TABLE [dbo].[analysis_item]
    ADD [unit_price] DECIMAL(18,2) NOT NULL DEFAULT 0;
    
    PRINT N'Đã thêm cột unit_price vào bảng analysis_item thành công!';
END
ELSE
BEGIN
    PRINT N'Cột unit_price đã tồn tại trong bảng analysis_item.';
END
GO

-- =============================================
-- CẬP NHẬT DỮ LIỆU MẪU (TÙY CHỌN)
-- =============================================
-- Nếu bạn muốn cập nhật giá trị mặc định cho các record hiện có
-- Bỏ comment các dòng dưới đây và điều chỉnh logic cập nhật theo nhu cầu

-- UPDATE [dbo].[analysis_item]
-- SET [unit_price] = 100000 -- Giá mặc định (ví dụ: 100,000 VND)
-- WHERE [unit_price] = 0 OR [unit_price] IS NULL;

-- PRINT N'Đã cập nhật giá trị unit_price cho các record hiện có.';
-- GO

-- =============================================
-- KIỂM TRA KẾT QUẢ
-- =============================================
-- Xem thông tin cột unit_price
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
WHERE c.object_id = OBJECT_ID(N'[dbo].[analysis_item]')
AND c.name = 'unit_price';

PRINT N'Hoàn tất thêm cột unit_price vào bảng analysis_item!';
GO

