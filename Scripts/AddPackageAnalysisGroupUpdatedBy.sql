-- =============================================
-- Thêm cột updated_at, updated_by vào package_analysis_group
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.package_analysis_group') AND name = N'updated_at')
BEGIN
    ALTER TABLE [dbo].[package_analysis_group] ADD [updated_at] DATETIME2 NULL;
    PRINT N'Đã thêm cột updated_at vào package_analysis_group.';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.package_analysis_group') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[package_analysis_group] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[package_analysis_group] ADD CONSTRAINT [FK_package_analysis_group_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'Đã thêm cột updated_by và FK vào package_analysis_group.';
END
GO
