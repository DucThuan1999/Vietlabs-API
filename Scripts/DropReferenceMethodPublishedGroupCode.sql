-- =============================================
-- Xóa cột published_group_code khỏi reference_method
-- Chạy script này nếu bảng reference_method đang có cột published_group_code
-- =============================================

USE [VietLabs]
GO

IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.reference_method') AND name = N'published_group_code'
)
BEGIN
    ALTER TABLE [dbo].[reference_method] DROP COLUMN [published_group_code];
    PRINT N'Đã xóa cột published_group_code khỏi reference_method.';
END
ELSE
    PRINT N'Cột published_group_code không tồn tại trong reference_method, bỏ qua.';
GO
