-- =============================================
-- Script SQL: Tạo bảng Quotation, ClientDebt, ClientForecast
-- và cập nhật bảng Contacts, Clients
-- Database: VietLabs
-- =============================================

USE VietLabs;
GO

-- =============================================
-- 1. CẬP NHẬT BẢNG CONTACTS
-- Thêm các cột mới cho người liên hệ
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[contact]') AND name = 'notes')
BEGIN
    ALTER TABLE [dbo].[contact]
    ADD [notes] NVARCHAR(MAX) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[contacts]') AND name = 'is_sample_sender')
BEGIN
    ALTER TABLE [dbo].[contact]
    ADD [is_sample_sender] BIT NOT NULL DEFAULT 0;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[contacts]') AND name = 'is_result_receiver')
BEGIN
    ALTER TABLE [dbo].[contact]
    ADD [is_result_receiver] BIT NOT NULL DEFAULT 0;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[contacts]') AND name = 'is_payer')
BEGIN
    ALTER TABLE [dbo].[contact]
    ADD [is_payer] BIT NOT NULL DEFAULT 0;
END
GO

-- =============================================
-- 2. CẬP NHẬT BẢNG CLIENTS
-- Thêm các cột mới cho thông tin bổ sung
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[client]') AND name = 'agent_name')
BEGIN
    ALTER TABLE [dbo].[client]
    ADD [agent_name] NVARCHAR(255) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[clients]') AND name = 'forecast')
BEGIN
    ALTER TABLE [dbo].[client]
    ADD [forecast] DECIMAL(18,2) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[clients]') AND name = 'revenue')
BEGIN
    ALTER TABLE [dbo].[client]
    ADD [revenue] DECIMAL(18,2) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[clients]') AND name = 'debt_contact_name')
BEGIN
    ALTER TABLE [dbo].[client]
    ADD [debt_contact_name] NVARCHAR(255) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[clients]') AND name = 'debt_contact_phone')
BEGIN
    ALTER TABLE [dbo].[client]
    ADD [debt_contact_phone] NVARCHAR(50) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[clients]') AND name = 'debt_contact_email')
BEGIN
    ALTER TABLE [dbo].[client]
    ADD [debt_contact_email] NVARCHAR(255) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[clients]') AND name = 'payment_method')
BEGIN
    ALTER TABLE [dbo].[client]
    ADD [payment_method] NVARCHAR(255) NULL;
END
GO

-- =============================================
-- 3. TẠO BẢNG QUOTATION
-- Bảng báo giá
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'quotation')
BEGIN
    CREATE TABLE [dbo].[quotation] (
        [quotation_id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [quotation_code] NVARCHAR(50) NULL,
        
        -- THÔNG TIN NHÂN VIÊN KINH DOANH
        [employee_id] UNIQUEIDENTIFIER NULL,
        [sales_person_name] NVARCHAR(255) NULL,
        [sales_person_email] NVARCHAR(255) NULL,
        [sales_person_phone] NVARCHAR(50) NULL,
        
        -- THÔNG TIN KHÁCH HÀNG
        [client_id] UNIQUEIDENTIFIER NOT NULL,
        [agent_name] NVARCHAR(255) NULL,
        [company_name] NVARCHAR(255) NULL,
        [contact_id] UNIQUEIDENTIFIER NULL,
        [contact_name] NVARCHAR(255) NULL,
        [tax_code] NVARCHAR(50) NULL,
        [contact_email] NVARCHAR(255) NULL,
        [forecast] DECIMAL(18,2) NULL,
        [contact_phone] NVARCHAR(50) NULL,
        [revenue] DECIMAL(18,2) NULL,
        [address] NVARCHAR(500) NULL,
        
        -- THÔNG TIN CÔNG NỢ
        [debt_contact_name] NVARCHAR(255) NULL,
        [debt_contact_phone] NVARCHAR(50) NULL,
        [debt_contact_email] NVARCHAR(255) NULL,
        [payment_method] NVARCHAR(255) NULL,
        
        -- HIỆU LỰC BÁO GIÁ
        [valid_from] DATETIME2 NULL,
        [valid_to] DATETIME2 NULL,
        
        -- GIẢM GIÁ
        [discount_percent] DECIMAL(5,2) NULL,
        
        -- TÓM TẮT
        [sub_total] DECIMAL(18,2) NULL,
        [total_discount_percent] DECIMAL(5,2) NULL,
        [total_discount] DECIMAL(18,2) NULL,
        [vat_percent] DECIMAL(5,2) NULL DEFAULT 8,
        [vat_amount] DECIMAL(18,2) NULL,
        [total_amount] DECIMAL(18,2) NULL,
        
        -- CHIẾT KHẤU
        [quotation_discount_percent] DECIMAL(5,2) NULL,
        [client_discount_percent] DECIMAL(5,2) NULL,
        
        -- THÔNG TIN CHUNG
        [status] NVARCHAR(50) NOT NULL DEFAULT 'Draft',
        [notes] NVARCHAR(MAX) NULL,
        [created_at] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [updated_at] DATETIME2 NULL,
        [created_by] UNIQUEIDENTIFIER NULL,
        [updated_by] UNIQUEIDENTIFIER NULL
    );
    
    PRINT 'Bảng [quotation] đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Bảng [quotation] đã tồn tại.';
END
GO

-- =============================================
-- 4. TẠO BẢNG CLIENT_DEBT
-- Bảng công nợ của khách hàng (1-1 với Client)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'client_debt')
BEGIN
    CREATE TABLE [dbo].[client_debt] (
        [client_debt_id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [client_id] UNIQUEIDENTIFIER NOT NULL UNIQUE, -- UNIQUE để đảm bảo 1-1 relationship
        
        -- THÔNG TIN CÔNG NỢ
        [payment_method] NVARCHAR(255) NULL,
        [total_debt] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [debt_term_days] INT NOT NULL DEFAULT 0,
        [credit_limit] DECIMAL(18,2) NOT NULL DEFAULT 0,
        
        -- TÌNH TRẠNG HỢP ĐỒNG
        [contract_effective_date] DATETIME2 NULL,
        [contract_end_date] DATETIME2 NULL,
        
        -- ATTACHMENTS
        [attachments] NVARCHAR(MAX) NULL, -- JSON array hoặc comma-separated paths
        
        -- THÔNG TIN SYNC TỪ MISA
        [last_synced_at] DATETIME2 NULL,
        [misa_reference_id] NVARCHAR(100) NULL,
        
        [created_at] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [updated_at] DATETIME2 NULL
    );
    
    PRINT 'Bảng [client_debt] đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Bảng [client_debt] đã tồn tại.';
END
GO

-- =============================================
-- 5. TẠO BẢNG CLIENT_FORECAST
-- Bảng forecast của khách hàng (1-n với Client)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'client_forecast')
BEGIN
    CREATE TABLE [dbo].[client_forecast] (
        [client_forecast_id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [client_id] UNIQUEIDENTIFIER NOT NULL,
        
        -- THÔNG TIN FORECAST
        [from_date] DATETIME2 NOT NULL,
        [to_date] DATETIME2 NOT NULL,
        [forecast_amount] DECIMAL(18,2) NOT NULL,
        [notes] NVARCHAR(MAX) NULL,
        
        [created_at] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [updated_at] DATETIME2 NULL,
        [created_by] UNIQUEIDENTIFIER NULL,
        [updated_by] UNIQUEIDENTIFIER NULL
    );
    
    PRINT 'Bảng [client_forecast] đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Bảng [client_forecast] đã tồn tại.';
END
GO

-- =============================================
-- 6. TẠO FOREIGN KEY CONSTRAINTS
-- =============================================

-- Foreign Key: quotation -> employee
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_quotation_employee')
BEGIN
    ALTER TABLE [dbo].[quotation]
    ADD CONSTRAINT [FK_quotation_employee]
    FOREIGN KEY ([employee_id]) REFERENCES [dbo].[employee]([employee_id])
    ON DELETE SET NULL;
    
    PRINT 'Foreign key FK_quotation_employee đã được tạo.';
END
GO

-- Foreign Key: quotation -> client
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_quotation_client')
BEGIN
    ALTER TABLE [dbo].[quotation]
    ADD CONSTRAINT [FK_quotation_client]
    FOREIGN KEY ([client_id]) REFERENCES [dbo].[client]([client_id])
    ON DELETE CASCADE;
    
    PRINT 'Foreign key FK_quotation_client đã được tạo.';
END
GO

-- Foreign Key: quotation -> contact
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_quotation_contact')
BEGIN
    ALTER TABLE [dbo].[quotation]
    ADD CONSTRAINT [FK_quotation_contact]
    FOREIGN KEY ([contact_id]) REFERENCES [dbo].[contact]([contact_id])
    ON DELETE SET NULL;
    
    PRINT 'Foreign key FK_quotation_contact đã được tạo.';
END
GO

-- Foreign Key: client_debt -> client (1-1 relationship)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_client_debt_client')
BEGIN
    ALTER TABLE [dbo].[client_debt]
    ADD CONSTRAINT [FK_client_debt_client]
    FOREIGN KEY ([client_id]) REFERENCES [dbo].[client]([client_id])
    ON DELETE CASCADE;
    
    PRINT 'Foreign key FK_client_debt_client đã được tạo.';
END
GO

-- Foreign Key: client_forecast -> client
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_client_forecast_client')
BEGIN
    ALTER TABLE [dbo].[client_forecast]
    ADD CONSTRAINT [FK_client_forecast_client]
    FOREIGN KEY ([client_id]) REFERENCES [dbo].[client]([client_id])
    ON DELETE CASCADE;
    
    PRINT 'Foreign key FK_client_forecast_client đã được tạo.';
END
GO

-- =============================================
-- 7. TẠO INDEXES ĐỂ TỐI ƯU HIỆU SUẤT
-- =============================================

-- Index cho quotation
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_quotation_client_id')
BEGIN
    CREATE INDEX [IX_quotation_client_id] ON [dbo].[quotation]([client_id]);
    PRINT 'Index IX_quotation_client_id đã được tạo.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_quotation_employee_id')
BEGIN
    CREATE INDEX [IX_quotation_employee_id] ON [dbo].[quotation]([employee_id]);
    PRINT 'Index IX_quotation_employee_id đã được tạo.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_quotation_status')
BEGIN
    CREATE INDEX [IX_quotation_status] ON [dbo].[quotation]([status]);
    PRINT 'Index IX_quotation_status đã được tạo.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_quotation_created_at')
BEGIN
    CREATE INDEX [IX_quotation_created_at] ON [dbo].[quotation]([created_at]);
    PRINT 'Index IX_quotation_created_at đã được tạo.';
END
GO

-- Index cho client_forecast
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_client_forecast_client_id')
BEGIN
    CREATE INDEX [IX_client_forecast_client_id] ON [dbo].[client_forecast]([client_id]);
    PRINT 'Index IX_client_forecast_client_id đã được tạo.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_client_forecast_from_date')
BEGIN
    CREATE INDEX [IX_client_forecast_from_date] ON [dbo].[client_forecast]([from_date]);
    PRINT 'Index IX_client_forecast_from_date đã được tạo.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_client_forecast_to_date')
BEGIN
    CREATE INDEX [IX_client_forecast_to_date] ON [dbo].[client_forecast]([to_date]);
    PRINT 'Index IX_client_forecast_to_date đã được tạo.';
END
GO

-- =============================================
-- HOÀN TẤT
-- =============================================
PRINT '';
PRINT '========================================';
PRINT 'Script SQL đã được thực thi thành công!';
PRINT '========================================';
PRINT '';
PRINT 'Đã tạo/cập nhật:';
PRINT '  - Bảng [quotation]';
PRINT '  - Bảng [client_debt]';
PRINT '  - Bảng [client_forecast]';
PRINT '  - Cập nhật bảng [contact]';
PRINT '  - Cập nhật bảng [client]';
PRINT '  - Foreign keys và indexes';
PRINT '';

