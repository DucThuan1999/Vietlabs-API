-- =============================================
-- Thêm cột mobile và chuyển SĐT từ notes sang mobile
-- =============================================

USE [VietLabs]
GO

-- 1. Thêm cột mobile nếu chưa có
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[employee]') AND name = N'mobile'
)
BEGIN
    ALTER TABLE [dbo].[employee]
    ADD [mobile] NVARCHAR(50) NULL;
    PRINT 'Đã thêm cột mobile vào bảng employee.';
END
GO

-- 2. Di chuyển SĐT từ notes sang mobile (notes có dạng "SĐT: 0123456789")
-- Cột notes không cho NULL nên gán N'' thay vì NULL
UPDATE [dbo].[employee]
SET [mobile] = LTRIM(RTRIM(SUBSTRING([notes], 6, 50))),
    [notes] = N''
WHERE [notes] IS NOT NULL
  AND LTRIM(RTRIM([notes])) LIKE N'SĐT:%';
GO

PRINT 'Đã chuyển SĐT từ notes sang mobile (các dòng có format SĐT: xxx).';
