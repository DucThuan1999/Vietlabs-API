-- =============================================
-- Thêm cột người cập nhật (updated_at, updated_by) vào bảng department
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[department]') AND name = N'updated_at')
BEGIN
    ALTER TABLE [dbo].[department]
    ADD [updated_at] DATETIME2 NULL;
    PRINT N'Đã thêm cột updated_at vào bảng department.';
END
ELSE
    PRINT N'Cột updated_at đã tồn tại trong bảng department.';
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[department]') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[department]
    ADD [updated_by] UNIQUEIDENTIFIER NULL;
    PRINT N'Đã thêm cột updated_by vào bảng department.';
END
ELSE
    PRINT N'Cột updated_by đã tồn tại trong bảng department.';
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_department_updated_by')
BEGIN
    ALTER TABLE [dbo].[department]
    ADD CONSTRAINT [FK_department_updated_by]
    FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'FK_department_updated_by đã được tạo.';
END
ELSE
    PRINT N'FK_department_updated_by đã tồn tại.';
GO

-- Gán giá trị mặc định cho bản ghi hiện có
UPDATE [dbo].[department]
SET [updated_by] = '94eab415-1624-49de-85a6-a80916db3ab2',
    [updated_at] = SYSUTCDATETIME()
WHERE [updated_by] IS NULL OR [updated_at] IS NULL;

PRINT N'Hoàn tất thêm người cập nhật cho department.';
