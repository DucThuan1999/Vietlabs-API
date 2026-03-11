-- =============================================
-- Cập nhật giá trị updated_by, updated_at cho bản ghi hiện có trong employee_title
-- =============================================

USE [VietLabs]
GO

UPDATE [dbo].[employee_title]
SET [updated_by] = '94eab415-1624-49de-85a6-a80916db3ab2',
    [updated_at] = SYSUTCDATETIME()
WHERE [updated_by] IS NULL OR [updated_at] IS NULL;

PRINT N'Hoàn tất cập nhật người cập nhật cho employee_title.';
