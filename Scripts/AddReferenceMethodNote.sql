-- =============================================
-- Thêm cột notes (ghi chú) vào reference_method
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.reference_method') AND name = N'notes'
)
BEGIN
    ALTER TABLE [dbo].[reference_method]
    ADD [notes] NVARCHAR(2000) NULL;

    PRINT N'Đã thêm cột notes vào reference_method.';
END
ELSE
    PRINT N'Cột notes đã tồn tại trong reference_method.';
GO
