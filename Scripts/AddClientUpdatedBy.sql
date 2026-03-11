-- =============================================
-- Thêm cột updated_at, updated_by vào client
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.client') AND name = N'updated_at')
BEGIN
    ALTER TABLE [dbo].[client] ADD [updated_at] DATETIME2 NULL;
    PRINT N'Đã thêm cột updated_at vào client.';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.client') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[client] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[client] ADD CONSTRAINT [FK_client_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'Đã thêm cột updated_by và FK vào client.';
END
ELSE
    PRINT N'Cột updated_by đã tồn tại trong client.';
GO

UPDATE [dbo].[client] SET [updated_by] = '94eab415-1624-49de-85a6-a80916db3ab2', [updated_at] = SYSUTCDATETIME() WHERE [updated_by] IS NULL;
GO
