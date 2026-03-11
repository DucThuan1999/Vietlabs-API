-- =============================================
-- Thêm cột Note vào bảng designation
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.designation') AND name = N'note'
)
BEGIN
    ALTER TABLE [dbo].[designation]
    ADD [note] NVARCHAR(2000) NULL;
    PRINT N'Đã thêm cột note vào bảng designation.';
END
ELSE
    PRINT N'Cột note đã tồn tại trong bảng designation.';
GO
