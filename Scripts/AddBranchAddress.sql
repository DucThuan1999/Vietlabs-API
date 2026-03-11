-- =============================================
-- Thêm cột Địa chỉ (address) vào bảng branch (Chi nhánh)
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.branch') AND name = N'address'
)
BEGIN
    ALTER TABLE [dbo].[branch]
    ADD [address] NVARCHAR(500) NULL;
    PRINT N'Đã thêm cột address vào bảng branch.';
END
ELSE
    PRINT N'Cột address đã tồn tại trong bảng branch.';
GO
