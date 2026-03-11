-- =============================================
-- Thêm cột short_name (Tên viết tắt) vào analysis_item
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.analysis_item') AND name = N'short_name'
)
BEGIN
    ALTER TABLE [dbo].[analysis_item]
    ADD [short_name] NVARCHAR(255) NULL;

    PRINT N'Đã thêm cột short_name (Tên viết tắt) vào analysis_item.';
END
ELSE
    PRINT N'Cột short_name đã tồn tại trong analysis_item.';
GO
