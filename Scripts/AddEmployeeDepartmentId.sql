-- =============================================
-- Thêm cột department_id (phòng ban) vào bảng employee
-- Dùng cho expand Department khi load Employee
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[employee]') AND name = N'department_id')
BEGIN
    ALTER TABLE [dbo].[employee]
    ADD [department_id] UNIQUEIDENTIFIER NULL;
    PRINT N'Đã thêm cột department_id vào bảng employee.';
END
ELSE
    PRINT N'Cột department_id đã tồn tại trong bảng employee.';
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_employee_department')
BEGIN
    ALTER TABLE [dbo].[employee]
    ADD CONSTRAINT [FK_employee_department]
    FOREIGN KEY ([department_id]) REFERENCES [dbo].[department] ([department_id]) ON DELETE NO ACTION;
    PRINT N'FK_employee_department đã được tạo.';
END
ELSE
    PRINT N'FK_employee_department đã tồn tại.';
GO

PRINT N'Hoàn tất thêm department_id cho employee.';
