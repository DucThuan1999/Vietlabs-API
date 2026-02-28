-- =============================================
-- Thêm cột department_id (phòng ban nhà thầu phụ) vào bảng subcontractor
-- Chạy script này khi bảng subcontractor đã tồn tại
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[subcontractor]') AND name = N'department_id')
BEGIN
    ALTER TABLE [dbo].[subcontractor]
    ADD [department_id] UNIQUEIDENTIFIER NULL;
    PRINT N'Đã thêm cột department_id vào bảng subcontractor.';
END
ELSE
BEGIN
    PRINT N'Cột department_id đã tồn tại trong bảng subcontractor.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_subcontractor_department')
BEGIN
    ALTER TABLE [dbo].[subcontractor]
    ADD CONSTRAINT [FK_subcontractor_department]
    FOREIGN KEY ([department_id]) REFERENCES [dbo].[department] ([department_id]) ON DELETE NO ACTION;
    PRINT N'FK_subcontractor_department đã được tạo.';
END
ELSE
BEGIN
    PRINT N'FK_subcontractor_department đã tồn tại.';
END
GO

PRINT N'Hoàn tất thêm department nhà thầu phụ.';
