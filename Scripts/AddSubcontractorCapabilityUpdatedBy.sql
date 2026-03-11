-- =============================================
-- Thêm cột updated_by vào subcontractor_capability
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.subcontractor_capability') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[subcontractor_capability] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[subcontractor_capability] ADD CONSTRAINT [FK_subcontractor_capability_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'Đã thêm cột updated_by và FK vào subcontractor_capability.';
END
ELSE
    PRINT N'Cột updated_by đã tồn tại trong subcontractor_capability.';
GO
