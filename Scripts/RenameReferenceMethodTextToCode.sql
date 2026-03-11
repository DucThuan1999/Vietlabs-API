-- =============================================
-- Đổi tên cột reference_method_text -> reference_method_code
-- Chạy script này nếu DB đang dùng cột reference_method_text (tạo từ script cũ)
-- =============================================

USE [VietLabs]
GO

IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.reference_method') AND name = N'reference_method_text'
)
AND NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.reference_method') AND name = N'reference_method_code'
)
BEGIN
    EXEC sp_rename N'dbo.reference_method.reference_method_text', N'reference_method_code', N'COLUMN';
    PRINT N'Đã đổi tên cột reference_method_text thành reference_method_code.';
END
ELSE IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.reference_method') AND name = N'reference_method_text')
AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.reference_method') AND name = N'reference_method_code')
    PRINT N'Bảng reference_method chưa có cột reference_method_text/reference_method_code.';
ELSE
    PRINT N'Cột reference_method_code đã tồn tại hoặc reference_method_text không tồn tại, bỏ qua.';
GO
