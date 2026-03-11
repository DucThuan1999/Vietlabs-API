-- =============================================
-- Thêm cột updated_by vào analysis_item_tat
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.analysis_item_tat') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[analysis_item_tat] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[analysis_item_tat] ADD CONSTRAINT [FK_analysis_item_tat_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'Đã thêm cột updated_by và FK vào analysis_item_tat.';
END
ELSE
    PRINT N'Cột updated_by đã tồn tại trong analysis_item_tat.';
GO
