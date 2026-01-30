-- =============================================
-- Script tạo 3 bảng: country, province, ward
-- Bảng quản lý địa chỉ: Quốc gia, Tỉnh/Thành phố, Phường/Xã
-- Chạy script này nếu các bảng chưa tồn tại trong database
-- =============================================

USE [VietLabs]; -- Thay đổi tên database nếu cần
GO

-- =============================================
-- TẠO BẢNG country (Quốc gia)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[country]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[country] (
        [country_id] UNIQUEIDENTIFIER NOT NULL,
        [sequence_number] INT NULL, -- STT
        [name_en] NVARCHAR(200) NOT NULL, -- Tên nước (EN)
        [full_name_vi] NVARCHAR(500) NOT NULL, -- Tên đầy đủ (VI)
        [full_name_en] NVARCHAR(500) NOT NULL, -- Tên đầy đủ (EN)
        [alpha_2] NVARCHAR(2) NULL, -- Alpha-2 code (VD: VN, US)
        [alpha_3] NVARCHAR(3) NULL, -- Alpha-3 code (VD: VNM, USA)
        [status] NVARCHAR(50) NOT NULL DEFAULT 'Active', -- Trạng Thái
        [notes] NVARCHAR(2000) NULL, -- Ghi chú
        CONSTRAINT [PK_country] PRIMARY KEY CLUSTERED ([country_id] ASC)
    );
    
    -- Indexes
    CREATE NONCLUSTERED INDEX [IX_country_alpha_2] 
        ON [dbo].[country] ([alpha_2]);
    
    CREATE NONCLUSTERED INDEX [IX_country_alpha_3] 
        ON [dbo].[country] ([alpha_3]);
    
    CREATE NONCLUSTERED INDEX [IX_country_name_en] 
        ON [dbo].[country] ([name_en]);
    
    CREATE NONCLUSTERED INDEX [IX_country_status] 
        ON [dbo].[country] ([status]);
    
    PRINT 'Bảng country đã được tạo thành công!';
END
ELSE
BEGIN
    PRINT 'Bảng country đã tồn tại.';
END
GO

-- =============================================
-- TẠO BẢNG province (Tỉnh/Thành phố)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[province]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[province] (
        [province_id] UNIQUEIDENTIFIER NOT NULL,
        [sequence_number] INT NULL, -- STT
        [name] NVARCHAR(200) NOT NULL, -- Tỉnh/Thành phố
        [type] NVARCHAR(100) NULL, -- Loại (Tỉnh, Thành phố, Thành phố trực thuộc TW)
        [full_name] NVARCHAR(500) NULL, -- Đầy đủ
        [country_id] UNIQUEIDENTIFIER NOT NULL, -- Quốc Gia
        [status] NVARCHAR(50) NOT NULL DEFAULT 'Active', -- Trạng Thái
        [notes] NVARCHAR(2000) NULL, -- Ghi chú
        CONSTRAINT [PK_province] PRIMARY KEY CLUSTERED ([province_id] ASC),
        CONSTRAINT [FK_province_country] FOREIGN KEY ([country_id]) 
            REFERENCES [dbo].[country] ([country_id]) 
            ON DELETE NO ACTION
    );
    
    -- Indexes
    CREATE NONCLUSTERED INDEX [IX_province_country_id] 
        ON [dbo].[province] ([country_id]);
    
    CREATE NONCLUSTERED INDEX [IX_province_name] 
        ON [dbo].[province] ([name]);
    
    CREATE NONCLUSTERED INDEX [IX_province_status] 
        ON [dbo].[province] ([status]);
    
    PRINT 'Bảng province đã được tạo thành công!';
END
ELSE
BEGIN
    PRINT 'Bảng province đã tồn tại.';
END
GO

-- =============================================
-- TẠO BẢNG ward (Phường/Xã)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ward]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ward] (
        [ward_id] UNIQUEIDENTIFIER NOT NULL,
        [sequence_number] INT NULL, -- STT
        [code] NVARCHAR(50) NULL, -- Mã
        [name] NVARCHAR(200) NOT NULL, -- Xã/Phường
        [type] NVARCHAR(100) NULL, -- Loại (Xã, Phường, Thị trấn)
        [province_id] UNIQUEIDENTIFIER NOT NULL, -- Tỉnh/Thành Phố
        [country_id] UNIQUEIDENTIFIER NOT NULL, -- Quốc Gia
        [status] NVARCHAR(50) NOT NULL DEFAULT 'Active', -- Trạng Thái
        [notes] NVARCHAR(2000) NULL, -- Ghi chú
        CONSTRAINT [PK_ward] PRIMARY KEY CLUSTERED ([ward_id] ASC),
        CONSTRAINT [FK_ward_province] FOREIGN KEY ([province_id]) 
            REFERENCES [dbo].[province] ([province_id]) 
            ON DELETE NO ACTION,
        CONSTRAINT [FK_ward_country] FOREIGN KEY ([country_id]) 
            REFERENCES [dbo].[country] ([country_id]) 
            ON DELETE NO ACTION
    );
    
    -- Indexes
    CREATE NONCLUSTERED INDEX [IX_ward_province_id] 
        ON [dbo].[ward] ([province_id]);
    
    CREATE NONCLUSTERED INDEX [IX_ward_country_id] 
        ON [dbo].[ward] ([country_id]);
    
    CREATE NONCLUSTERED INDEX [IX_ward_code] 
        ON [dbo].[ward] ([code]);
    
    CREATE NONCLUSTERED INDEX [IX_ward_name] 
        ON [dbo].[ward] ([name]);
    
    CREATE NONCLUSTERED INDEX [IX_ward_status] 
        ON [dbo].[ward] ([status]);
    
    PRINT 'Bảng ward đã được tạo thành công!';
END
ELSE
BEGIN
    PRINT 'Bảng ward đã tồn tại.';
END
GO

PRINT 'Hoàn tất tạo các bảng country, province và ward!';
GO

