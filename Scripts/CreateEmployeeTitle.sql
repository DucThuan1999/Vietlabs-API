-- =============================================
-- Script tạo bảng employee_title (chức vụ nhân viên)
-- và thêm cột employee_title_id vào bảng employee
-- =============================================

USE [VietLabs] -- Thay đổi tên database nếu cần
GO

-- =============================================
-- 1. Tạo bảng employee_title
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[employee_title]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[employee_title] (
        [employee_title_id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [sequence_number] INT NULL,
        [title_code] NVARCHAR(50) NOT NULL,
        [name_vi] NVARCHAR(200) NOT NULL,
        [name_en] NVARCHAR(200) NULL,
        [status] NVARCHAR(50) NOT NULL DEFAULT N'Active',
        [notes] NVARCHAR(2000) NULL,
        [created_at] DATETIME2 NULL,
        [updated_at] DATETIME2 NULL,
        [created_by] UNIQUEIDENTIFIER NULL,
        [updated_by] UNIQUEIDENTIFIER NULL
    );

    PRINT 'Bảng employee_title đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Bảng employee_title đã tồn tại.';
END
GO

-- =============================================
-- 2. Tạo Index trên bảng employee_title
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'i_x_employee_title_title_code' AND object_id = OBJECT_ID(N'[dbo].[employee_title]'))
BEGIN
    CREATE INDEX [i_x_employee_title_title_code]
    ON [dbo].[employee_title] ([title_code]);

    PRINT 'Index i_x_employee_title_title_code đã được tạo.';
END
ELSE
BEGIN
    PRINT 'Index i_x_employee_title_title_code đã tồn tại.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'i_x_employee_title_status' AND object_id = OBJECT_ID(N'[dbo].[employee_title]'))
BEGIN
    CREATE INDEX [i_x_employee_title_status]
    ON [dbo].[employee_title] ([status]);

    PRINT 'Index i_x_employee_title_status đã được tạo.';
END
ELSE
BEGIN
    PRINT 'Index i_x_employee_title_status đã tồn tại.';
END
GO

-- =============================================
-- 3. Thêm cột employee_title_id vào bảng employee (nếu chưa có)
-- =============================================
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[employee]') AND name = N'employee_title_id'
)
BEGIN
    ALTER TABLE [dbo].[employee]
    ADD [employee_title_id] UNIQUEIDENTIFIER NULL;

    PRINT 'Cột employee_title_id đã được thêm vào bảng employee.';
END
ELSE
BEGIN
    PRINT 'Cột employee_title_id đã tồn tại trong bảng employee.';
END
GO

-- =============================================
-- 4. Thêm Foreign Key: employee.employee_title_id -> employee_title.employee_title_id
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'f_k_employee_employee_title')
BEGIN
    ALTER TABLE [dbo].[employee]
    ADD CONSTRAINT [f_k_employee_employee_title]
    FOREIGN KEY ([employee_title_id])
    REFERENCES [dbo].[employee_title] ([employee_title_id])
    ON DELETE SET NULL
    ON UPDATE NO ACTION;

    PRINT 'Foreign key f_k_employee_employee_title đã được thêm.';
END
ELSE
BEGIN
    PRINT 'Foreign key f_k_employee_employee_title đã tồn tại.';
END
GO

-- =============================================
-- 5. Index trên employee.employee_title_id
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'i_x_employee_employee_title_id' AND object_id = OBJECT_ID(N'[dbo].[employee]'))
BEGIN
    CREATE INDEX [i_x_employee_employee_title_id]
    ON [dbo].[employee] ([employee_title_id]);

    PRINT 'Index i_x_employee_employee_title_id đã được tạo.';
END
ELSE
BEGIN
    PRINT 'Index i_x_employee_employee_title_id đã tồn tại.';
END
GO

-- =============================================
-- 6. Thêm cột người tạo / người cập nhật (nếu bảng đã tồn tại từ trước)
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[employee_title]') AND type in (N'U'))
   AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[employee_title]') AND name = N'created_at')
BEGIN
    ALTER TABLE [dbo].[employee_title]
    ADD [created_at] DATETIME2 NULL,
        [updated_at] DATETIME2 NULL,
        [created_by] UNIQUEIDENTIFIER NULL,
        [updated_by] UNIQUEIDENTIFIER NULL;
    PRINT 'Đã thêm cột created_at, updated_at, created_by, updated_by vào employee_title.';
END
GO

-- Foreign key: employee_title.created_by -> account.account_id
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[employee_title]') AND type in (N'U'))
   AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'f_k_employee_title_created_by')
BEGIN
    ALTER TABLE [dbo].[employee_title]
    ADD CONSTRAINT [f_k_employee_title_created_by]
    FOREIGN KEY ([created_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT 'FK f_k_employee_title_created_by đã được thêm.';
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[employee_title]') AND type in (N'U'))
   AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'f_k_employee_title_updated_by')
BEGIN
    ALTER TABLE [dbo].[employee_title]
    ADD CONSTRAINT [f_k_employee_title_updated_by]
    FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT 'FK f_k_employee_title_updated_by đã được thêm.';
END
GO

PRINT '=============================================';
PRINT 'Hoàn tất tạo bảng employee_title và cập nhật bảng employee.';
PRINT '=============================================';
