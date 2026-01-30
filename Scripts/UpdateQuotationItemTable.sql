-- =============================================
-- Script cập nhật bảng quotation_item
-- Thêm các cột mới: is_standalone, snapshot fields, default_price
-- =============================================

USE [VietLabs]; -- Thay đổi tên database nếu cần
GO

PRINT 'Bắt đầu cập nhật bảng [quotation_item]...';
GO

-- =============================================
-- 1. Thêm cột is_standalone
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[quotation_item]') 
               AND name = 'is_standalone')
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ADD [is_standalone] BIT NULL;
    
    PRINT 'Đã thêm cột [is_standalone]';
END
ELSE
BEGIN
    PRINT 'Cột [is_standalone] đã tồn tại, bỏ qua.';
END
GO

-- =============================================
-- 2. Thêm cột sample_matrix_name
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[quotation_item]') 
               AND name = 'sample_matrix_name')
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ADD [sample_matrix_name] NVARCHAR(500) NULL;
    
    PRINT 'Đã thêm cột [sample_matrix_name]';
END
ELSE
BEGIN
    PRINT 'Cột [sample_matrix_name] đã tồn tại, bỏ qua.';
END
GO

-- =============================================
-- 3. Thêm cột published_group_code
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[quotation_item]') 
               AND name = 'published_group_code')
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ADD [published_group_code] NVARCHAR(255) NULL;
    
    PRINT 'Đã thêm cột [published_group_code]';
END
ELSE
BEGIN
    PRINT 'Cột [published_group_code] đã tồn tại, bỏ qua.';
END
GO

-- =============================================
-- 4. Thêm cột unit
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[quotation_item]') 
               AND name = 'unit')
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ADD [unit] NVARCHAR(50) NULL;
    
    PRINT 'Đã thêm cột [unit]';
END
ELSE
BEGIN
    PRINT 'Cột [unit] đã tồn tại, bỏ qua.';
END
GO

-- =============================================
-- 5. Thêm cột lod
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[quotation_item]') 
               AND name = 'lod')
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ADD [lod] NVARCHAR(50) NULL;
    
    PRINT 'Đã thêm cột [lod]';
END
ELSE
BEGIN
    PRINT 'Cột [lod] đã tồn tại, bỏ qua.';
END
GO

-- =============================================
-- 6. Thêm cột loq
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[quotation_item]') 
               AND name = 'loq')
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ADD [loq] NVARCHAR(50) NULL;
    
    PRINT 'Đã thêm cột [loq]';
END
ELSE
BEGIN
    PRINT 'Cột [loq] đã tồn tại, bỏ qua.';
END
GO

-- =============================================
-- 7. Thêm cột tat
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[quotation_item]') 
               AND name = 'tat')
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ADD [tat] NVARCHAR(100) NULL;
    
    PRINT 'Đã thêm cột [tat]';
END
ELSE
BEGIN
    PRINT 'Cột [tat] đã tồn tại, bỏ qua.';
END
GO

-- =============================================
-- 8. Thêm cột default_price
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[quotation_item]') 
               AND name = 'default_price')
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ADD [default_price] DECIMAL(18,2) NULL;
    
    PRINT 'Đã thêm cột [default_price]';
END
ELSE
BEGIN
    PRINT 'Cột [default_price] đã tồn tại, bỏ qua.';
END
GO

-- =============================================
-- 9. Cập nhật item_code max length nếu cần
-- =============================================
-- Kiểm tra và cập nhật max length của item_code từ 50 lên 255
IF EXISTS (SELECT * FROM sys.columns 
           WHERE object_id = OBJECT_ID(N'[dbo].[quotation_item]') 
           AND name = 'item_code'
           AND max_length < 510) -- 255 * 2 (NVARCHAR)
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ALTER COLUMN [item_code] NVARCHAR(255) NULL;
    
    PRINT 'Đã cập nhật max length của [item_code] lên 255';
END
ELSE
BEGIN
    PRINT 'Cột [item_code] đã có max length >= 255, bỏ qua.';
END
GO

-- =============================================
-- 10. Cập nhật discount_percent precision nếu cần
-- =============================================
-- Kiểm tra và cập nhật discount_percent từ DECIMAL(18,2) sang DECIMAL(5,2)
IF EXISTS (SELECT * FROM sys.columns 
           WHERE object_id = OBJECT_ID(N'[dbo].[quotation_item]') 
           AND name = 'discount_percent'
           AND system_type_id = 106 -- DECIMAL
           AND (SELECT precision FROM sys.columns 
                WHERE object_id = OBJECT_ID(N'[dbo].[quotation_item]') 
                AND name = 'discount_percent') > 5)
BEGIN
    -- SQL Server không cho phép ALTER COLUMN để giảm precision trực tiếp
    -- Cần tạo cột mới, copy data, drop cột cũ, rename cột mới
    PRINT 'Cần cập nhật precision của [discount_percent] từ DECIMAL(18,2) sang DECIMAL(5,2)';
    PRINT 'Lưu ý: Script này chỉ thông báo, cần thực hiện thủ công nếu cần.';
END
ELSE
BEGIN
    PRINT 'Cột [discount_percent] đã có precision phù hợp, bỏ qua.';
END
GO

-- =============================================
-- 11. Cập nhật notes max length nếu cần
-- =============================================
-- Kiểm tra và cập nhật notes từ NVARCHAR(MAX) hoặc nhỏ hơn lên 2000
IF EXISTS (SELECT * FROM sys.columns 
           WHERE object_id = OBJECT_ID(N'[dbo].[quotation_item]') 
           AND name = 'notes'
           AND max_length != -1 -- Không phải MAX
           AND max_length < 4000) -- 2000 * 2 (NVARCHAR)
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ALTER COLUMN [notes] NVARCHAR(2000) NULL;
    
    PRINT 'Đã cập nhật max length của [notes] lên 2000';
END
ELSE IF EXISTS (SELECT * FROM sys.columns 
                WHERE object_id = OBJECT_ID(N'[dbo].[quotation_item]') 
                AND name = 'notes'
                AND max_length = -1) -- NVARCHAR(MAX)
BEGIN
    -- Giữ nguyên MAX hoặc có thể thay đổi nếu muốn
    PRINT 'Cột [notes] đang là NVARCHAR(MAX), giữ nguyên.';
END
ELSE
BEGIN
    PRINT 'Cột [notes] đã có max length >= 2000, bỏ qua.';
END
GO

-- =============================================
-- 12. Cập nhật description max length nếu cần
-- =============================================
IF EXISTS (SELECT * FROM sys.columns 
           WHERE object_id = OBJECT_ID(N'[dbo].[quotation_item]') 
           AND name = 'description'
           AND max_length != -1 -- Không phải MAX
           AND max_length < 4000) -- 2000 * 2 (NVARCHAR)
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ALTER COLUMN [description] NVARCHAR(2000) NULL;
    
    PRINT 'Đã cập nhật max length của [description] lên 2000';
END
ELSE
BEGIN
    PRINT 'Cột [description] đã có max length phù hợp, bỏ qua.';
END
GO

-- =============================================
-- 13. Kiểm tra và hiển thị kết quả
-- =============================================
PRINT '';
PRINT '=== Kết quả cập nhật ===';
PRINT '';

SELECT 
    c.name AS ColumnName,
    t.name AS DataType,
    c.max_length AS MaxLength,
    c.is_nullable AS IsNullable,
    c.precision AS Precision,
    c.scale AS Scale
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID(N'[dbo].[quotation_item]')
ORDER BY c.column_id;

PRINT '';
PRINT 'Hoàn thành cập nhật bảng [quotation_item]!';
GO

