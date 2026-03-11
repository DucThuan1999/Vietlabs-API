-- =============================================
-- Thêm cột created_at, updated_at, updated_by vào contact
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.contact') AND name = N'created_at')
BEGIN
    ALTER TABLE [dbo].[contact] ADD [created_at] DATETIME2 NULL;
    PRINT N'Đã thêm cột created_at vào contact.';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.contact') AND name = N'updated_at')
BEGIN
    ALTER TABLE [dbo].[contact] ADD [updated_at] DATETIME2 NULL;
    PRINT N'Đã thêm cột updated_at vào contact.';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.contact') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[contact] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[contact] ADD CONSTRAINT [FK_contact_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'Đã thêm cột updated_by và FK vào contact.';
END
GO
