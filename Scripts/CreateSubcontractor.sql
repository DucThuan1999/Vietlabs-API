-- =============================================
-- Script tạo bảng subcontractor (Nhà thầu phụ)
-- =============================================
-- Cột: Code nhà thầu, Tên nhà thầu, Người liên hệ, SĐT, Email, Địa chỉ, Mô tả/Ghi chú, Trạng thái
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[subcontractor]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[subcontractor] (
        [subcontractor_id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [code] NVARCHAR(50) NOT NULL,
        [name] NVARCHAR(200) NOT NULL,
        [contact_person] NVARCHAR(200) NULL,
        [phone] NVARCHAR(50) NULL,
        [email] NVARCHAR(200) NULL,
        [address] NVARCHAR(500) NULL,
        [department_id] UNIQUEIDENTIFIER NULL,
        [notes] NVARCHAR(2000) NULL,
        [status] NVARCHAR(50) NOT NULL DEFAULT N'Active',
        [created_at] DATETIME2 NULL,
        [updated_at] DATETIME2 NULL
    );

    PRINT N'Bảng subcontractor (Nhà thầu phụ) đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT N'Bảng subcontractor đã tồn tại.';
END
GO

-- FK: department_id -> department (chạy sau khi có cột)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[subcontractor]') AND type in (N'U'))
   AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[subcontractor]') AND name = N'department_id')
   AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_subcontractor_department')
BEGIN
    ALTER TABLE [dbo].[subcontractor]
    ADD CONSTRAINT [FK_subcontractor_department]
    FOREIGN KEY ([department_id]) REFERENCES [dbo].[department] ([department_id]) ON DELETE NO ACTION;
    PRINT N'FK_subcontractor_department đã được tạo.';
END
GO

-- Index: mã nhà thầu (tra cứu, unique có thể thêm sau nếu cần)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'i_x_subcontractor_code' AND object_id = OBJECT_ID(N'[dbo].[subcontractor]'))
BEGIN
    CREATE INDEX [i_x_subcontractor_code]
    ON [dbo].[subcontractor] ([code]);
    PRINT N'Index i_x_subcontractor_code đã được tạo.';
END
GO

-- Index: trạng thái (lọc Active/Inactive)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'i_x_subcontractor_status' AND object_id = OBJECT_ID(N'[dbo].[subcontractor]'))
BEGIN
    CREATE INDEX [i_x_subcontractor_status]
    ON [dbo].[subcontractor] ([status]);
    PRINT N'Index i_x_subcontractor_status đã được tạo.';
END
GO

PRINT N'=============================================';
PRINT N'Hoàn tất tạo bảng subcontractor (Nhà thầu phụ).';
PRINT N'=============================================';
