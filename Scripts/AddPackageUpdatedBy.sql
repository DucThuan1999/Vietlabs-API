-- =============================================
-- Thêm cột updated_by vào package
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.package') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[package] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[package] ADD CONSTRAINT [FK_package_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'Đã thêm cột updated_by và FK vào package.';
END
ELSE
    PRINT N'Cột updated_by đã tồn tại trong package.';
GO
