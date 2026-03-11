-- =============================================
-- Thêm cột standard_id vào analysis_item (liên kết Chỉ tiêu với Tiêu chuẩn/qui chuẩn)
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.analysis_item') AND name = N'standard_id'
)
BEGIN
    ALTER TABLE [dbo].[analysis_item]
    ADD [standard_id] UNIQUEIDENTIFIER NULL;

    ALTER TABLE [dbo].[analysis_item]
    ADD CONSTRAINT [FK_analysis_item_standard]
        FOREIGN KEY ([standard_id]) REFERENCES [dbo].[standard] ([standard_id]) ON DELETE NO ACTION;

    PRINT N'Đã thêm cột standard_id và FK vào analysis_item.';
END
ELSE
    PRINT N'Cột standard_id đã tồn tại trong analysis_item.';
GO
