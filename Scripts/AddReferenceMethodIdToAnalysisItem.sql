-- =============================================
-- Thêm cột reference_method_id vào analysis_item (liên kết với reference_method)
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.analysis_item') AND name = N'reference_method_id'
)
BEGIN
    ALTER TABLE [dbo].[analysis_item]
    ADD [reference_method_id] UNIQUEIDENTIFIER NULL;

    ALTER TABLE [dbo].[analysis_item]
    ADD CONSTRAINT [FK_analysis_item_reference_method]
        FOREIGN KEY ([reference_method_id]) REFERENCES [dbo].[reference_method] ([reference_method_id]) ON DELETE NO ACTION;

    PRINT N'Đã thêm cột reference_method_id và FK vào analysis_item.';
END
ELSE
    PRINT N'Cột reference_method_id đã tồn tại trong analysis_item.';
GO
