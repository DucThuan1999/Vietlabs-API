-- =============================================
-- Thêm cột updated_by vào store_record
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.store_record') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[store_record] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[store_record] ADD CONSTRAINT [FK_store_record_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'Đã thêm cột updated_by và FK vào store_record.';
END
ELSE
    PRINT N'Cột updated_by đã tồn tại trong store_record.';
GO
