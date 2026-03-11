-- =============================================
-- Thêm cột updated_by vào quotation_analysis_group
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.quotation_analysis_group') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[quotation_analysis_group] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[quotation_analysis_group] ADD CONSTRAINT [FK_quotation_analysis_group_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'Đã thêm cột updated_by và FK vào quotation_analysis_group.';
END
ELSE
    PRINT N'Cột updated_by đã tồn tại trong quotation_analysis_group.';
GO
