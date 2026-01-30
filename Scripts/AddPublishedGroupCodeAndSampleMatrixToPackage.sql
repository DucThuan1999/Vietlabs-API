-- Migration: Thêm PublishedGroupCode và SampleMatrixId vào bảng Package
-- Chạy script này để cập nhật database schema

USE [YourDatabaseName]; -- Thay thế bằng tên database của bạn
GO

-- Kiểm tra và thêm cột published_group_code nếu chưa tồn tại
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[package]') 
    AND name = 'published_group_code'
)
BEGIN
    ALTER TABLE [dbo].[package]
    ADD [published_group_code] NVARCHAR(100) NULL;
    
    PRINT 'Đã thêm cột published_group_code vào bảng package';
END
ELSE
BEGIN
    PRINT 'Cột published_group_code đã tồn tại';
END
GO

-- Kiểm tra và thêm cột sample_matrix_id nếu chưa tồn tại
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[package]') 
    AND name = 'sample_matrix_id'
)
BEGIN
    ALTER TABLE [dbo].[package]
    ADD [sample_matrix_id] UNIQUEIDENTIFIER NULL;
    
    PRINT 'Đã thêm cột sample_matrix_id vào bảng package';
END
ELSE
BEGIN
    PRINT 'Cột sample_matrix_id đã tồn tại';
END
GO

-- Thêm foreign key constraint cho sample_matrix_id nếu chưa tồn tại
IF NOT EXISTS (
    SELECT 1 
    FROM sys.foreign_keys 
    WHERE name = 'FK_package_sample_matrix_sample_matrix_id'
)
BEGIN
    ALTER TABLE [dbo].[package]
    ADD CONSTRAINT [FK_package_sample_matrix_sample_matrix_id]
    FOREIGN KEY ([sample_matrix_id])
    REFERENCES [dbo].[sample_matrix] ([sample_matrix_id])
    ON DELETE SET NULL;
    
    PRINT 'Đã thêm foreign key constraint FK_package_sample_matrix_sample_matrix_id';
END
ELSE
BEGIN
    PRINT 'Foreign key constraint FK_package_sample_matrix_sample_matrix_id đã tồn tại';
END
GO

-- Kiểm tra kết quả
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'package'
AND COLUMN_NAME IN ('published_group_code', 'sample_matrix_id')
ORDER BY COLUMN_NAME;

PRINT 'Migration hoàn tất!';
GO

