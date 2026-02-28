-- =============================================
-- Script tạo bảng client_industry (ngành nghề khách hàng)
-- và thêm cột client_industry_id vào bảng client
-- =============================================

USE [VietLabs] -- Thay đổi tên database nếu cần
GO

-- =============================================
-- 1. Tạo bảng client_industry
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[client_industry]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[client_industry] (
        [client_industry_id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [sequence_number] INT NULL,
        [industry_code] NVARCHAR(50) NOT NULL,
        [name_vi] NVARCHAR(200) NOT NULL,
        [name_en] NVARCHAR(200) NULL,
        [status] NVARCHAR(50) NOT NULL DEFAULT N'Active',
        [notes] NVARCHAR(2000) NULL,
        [created_at] DATETIME2 NULL,
        [updated_at] DATETIME2 NULL,
        [created_by] UNIQUEIDENTIFIER NULL,
        [updated_by] UNIQUEIDENTIFIER NULL
    );

    PRINT 'Bảng client_industry đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Bảng client_industry đã tồn tại.';
END
GO

-- =============================================
-- 2. Tạo Index trên bảng client_industry
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'i_x_client_industry_industry_code' AND object_id = OBJECT_ID(N'[dbo].[client_industry]'))
BEGIN
    CREATE INDEX [i_x_client_industry_industry_code]
    ON [dbo].[client_industry] ([industry_code]);

    PRINT 'Index i_x_client_industry_industry_code đã được tạo.';
END
ELSE
BEGIN
    PRINT 'Index i_x_client_industry_industry_code đã tồn tại.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'i_x_client_industry_status' AND object_id = OBJECT_ID(N'[dbo].[client_industry]'))
BEGIN
    CREATE INDEX [i_x_client_industry_status]
    ON [dbo].[client_industry] ([status]);

    PRINT 'Index i_x_client_industry_status đã được tạo.';
END
ELSE
BEGIN
    PRINT 'Index i_x_client_industry_status đã tồn tại.';
END
GO

-- =============================================
-- 3. Thêm cột client_industry_id vào bảng client (nếu chưa có)
-- =============================================
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[client]') AND name = N'client_industry_id'
)
BEGIN
    ALTER TABLE [dbo].[client]
    ADD [client_industry_id] UNIQUEIDENTIFIER NULL;

    PRINT 'Cột client_industry_id đã được thêm vào bảng client.';
END
ELSE
BEGIN
    PRINT 'Cột client_industry_id đã tồn tại trong bảng client.';
END
GO

-- =============================================
-- 4. Thêm Foreign Key: client.client_industry_id -> client_industry.client_industry_id
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'f_k_client_client_industry')
BEGIN
    ALTER TABLE [dbo].[client]
    ADD CONSTRAINT [f_k_client_client_industry]
    FOREIGN KEY ([client_industry_id])
    REFERENCES [dbo].[client_industry] ([client_industry_id])
    ON DELETE SET NULL
    ON UPDATE NO ACTION;

    PRINT 'Foreign key f_k_client_client_industry đã được thêm.';
END
ELSE
BEGIN
    PRINT 'Foreign key f_k_client_client_industry đã tồn tại.';
END
GO

-- =============================================
-- 5. Index trên client.client_industry_id (để join/ filter nhanh)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'i_x_client_client_industry_id' AND object_id = OBJECT_ID(N'[dbo].[client]'))
BEGIN
    CREATE INDEX [i_x_client_client_industry_id]
    ON [dbo].[client] ([client_industry_id]);

    PRINT 'Index i_x_client_client_industry_id đã được tạo.';
END
ELSE
BEGIN
    PRINT 'Index i_x_client_client_industry_id đã tồn tại.';
END
GO

-- =============================================
-- 6. Thêm cột người tạo / người cập nhật (nếu bảng đã tồn tại từ trước)
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[client_industry]') AND type in (N'U'))
   AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[client_industry]') AND name = N'created_at')
BEGIN
    ALTER TABLE [dbo].[client_industry]
    ADD [created_at] DATETIME2 NULL,
        [updated_at] DATETIME2 NULL,
        [created_by] UNIQUEIDENTIFIER NULL,
        [updated_by] UNIQUEIDENTIFIER NULL;
    PRINT 'Đã thêm cột created_at, updated_at, created_by, updated_by vào client_industry.';
END
GO

-- Foreign key: client_industry.created_by -> account.account_id
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[client_industry]') AND type in (N'U'))
   AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'f_k_client_industry_created_by')
BEGIN
    ALTER TABLE [dbo].[client_industry]
    ADD CONSTRAINT [f_k_client_industry_created_by]
    FOREIGN KEY ([created_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT 'FK f_k_client_industry_created_by đã được thêm.';
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[client_industry]') AND type in (N'U'))
   AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'f_k_client_industry_updated_by')
BEGIN
    ALTER TABLE [dbo].[client_industry]
    ADD CONSTRAINT [f_k_client_industry_updated_by]
    FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT 'FK f_k_client_industry_updated_by đã được thêm.';
END
GO

PRINT '=============================================';
PRINT 'Hoàn tất tạo bảng client_industry và cập nhật bảng client.';
PRINT '=============================================';
