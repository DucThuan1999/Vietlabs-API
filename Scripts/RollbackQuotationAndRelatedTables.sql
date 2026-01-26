-- =============================================
-- Script SQL: Rollback - Xóa bảng Quotations, ClientDebts, ClientForecasts
-- và revert các thay đổi trên bảng Contacts, Clients
-- Database: VietLabs
-- =============================================

USE VietLabs;
GO

-- =============================================
-- 1. XÓA FOREIGN KEY CONSTRAINTS
-- =============================================
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_quotations_employees')
BEGIN
    ALTER TABLE [dbo].[quotations] DROP CONSTRAINT [FK_quotations_employees];
    PRINT 'Đã xóa foreign key FK_quotations_employees.';
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_quotations_clients')
BEGIN
    ALTER TABLE [dbo].[quotations] DROP CONSTRAINT [FK_quotations_clients];
    PRINT 'Đã xóa foreign key FK_quotations_clients.';
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_quotations_contacts')
BEGIN
    ALTER TABLE [dbo].[quotations] DROP CONSTRAINT [FK_quotations_contacts];
    PRINT 'Đã xóa foreign key FK_quotations_contacts.';
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_client_debts_clients')
BEGIN
    ALTER TABLE [dbo].[client_debts] DROP CONSTRAINT [FK_client_debts_clients];
    PRINT 'Đã xóa foreign key FK_client_debts_clients.';
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_client_forecasts_clients')
BEGIN
    ALTER TABLE [dbo].[client_forecasts] DROP CONSTRAINT [FK_client_forecasts_clients];
    PRINT 'Đã xóa foreign key FK_client_forecasts_clients.';
END
GO

-- =============================================
-- 2. XÓA INDEXES
-- =============================================
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_quotations_client_id')
BEGIN
    DROP INDEX [IX_quotations_client_id] ON [dbo].[quotations];
    PRINT 'Đã xóa index IX_quotations_client_id.';
END
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_quotations_employee_id')
BEGIN
    DROP INDEX [IX_quotations_employee_id] ON [dbo].[quotations];
    PRINT 'Đã xóa index IX_quotations_employee_id.';
END
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_quotations_status')
BEGIN
    DROP INDEX [IX_quotations_status] ON [dbo].[quotations];
    PRINT 'Đã xóa index IX_quotations_status.';
END
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_quotations_created_at')
BEGIN
    DROP INDEX [IX_quotations_created_at] ON [dbo].[quotations];
    PRINT 'Đã xóa index IX_quotations_created_at.';
END
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_client_forecasts_client_id')
BEGIN
    DROP INDEX [IX_client_forecasts_client_id] ON [dbo].[client_forecasts];
    PRINT 'Đã xóa index IX_client_forecasts_client_id.';
END
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_client_forecasts_from_date')
BEGIN
    DROP INDEX [IX_client_forecasts_from_date] ON [dbo].[client_forecasts];
    PRINT 'Đã xóa index IX_client_forecasts_from_date.';
END
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_client_forecasts_to_date')
BEGIN
    DROP INDEX [IX_client_forecasts_to_date] ON [dbo].[client_forecasts];
    PRINT 'Đã xóa index IX_client_forecasts_to_date.';
END
GO

-- =============================================
-- 3. XÓA CÁC BẢNG
-- =============================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'quotations')
BEGIN
    DROP TABLE [dbo].[quotations];
    PRINT 'Đã xóa bảng [quotations].';
END
GO

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'client_debts')
BEGIN
    DROP TABLE [dbo].[client_debts];
    PRINT 'Đã xóa bảng [client_debts].';
END
GO

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'client_forecasts')
BEGIN
    DROP TABLE [dbo].[client_forecasts];
    PRINT 'Đã xóa bảng [client_forecasts].';
END
GO

-- =============================================
-- 4. REVERT CÁC CỘT ĐÃ THÊM VÀO CONTACTS
-- =============================================
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[contacts]') AND name = 'is_payer')
BEGIN
    ALTER TABLE [dbo].[contacts] DROP COLUMN [is_payer];
    PRINT 'Đã xóa cột [is_payer] từ bảng [contacts].';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[contacts]') AND name = 'is_result_receiver')
BEGIN
    ALTER TABLE [dbo].[contacts] DROP COLUMN [is_result_receiver];
    PRINT 'Đã xóa cột [is_result_receiver] từ bảng [contacts].';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[contacts]') AND name = 'is_sample_sender')
BEGIN
    ALTER TABLE [dbo].[contacts] DROP COLUMN [is_sample_sender];
    PRINT 'Đã xóa cột [is_sample_sender] từ bảng [contacts].';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[contacts]') AND name = 'notes')
BEGIN
    ALTER TABLE [dbo].[contacts] DROP COLUMN [notes];
    PRINT 'Đã xóa cột [notes] từ bảng [contacts].';
END
GO

-- =============================================
-- 5. REVERT CÁC CỘT ĐÃ THÊM VÀO CLIENTS
-- =============================================
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[clients]') AND name = 'payment_method')
BEGIN
    ALTER TABLE [dbo].[clients] DROP COLUMN [payment_method];
    PRINT 'Đã xóa cột [payment_method] từ bảng [clients].';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[clients]') AND name = 'debt_contact_email')
BEGIN
    ALTER TABLE [dbo].[clients] DROP COLUMN [debt_contact_email];
    PRINT 'Đã xóa cột [debt_contact_email] từ bảng [clients].';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[clients]') AND name = 'debt_contact_phone')
BEGIN
    ALTER TABLE [dbo].[clients] DROP COLUMN [debt_contact_phone];
    PRINT 'Đã xóa cột [debt_contact_phone] từ bảng [clients].';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[clients]') AND name = 'debt_contact_name')
BEGIN
    ALTER TABLE [dbo].[clients] DROP COLUMN [debt_contact_name];
    PRINT 'Đã xóa cột [debt_contact_name] từ bảng [clients].';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[clients]') AND name = 'revenue')
BEGIN
    ALTER TABLE [dbo].[clients] DROP COLUMN [revenue];
    PRINT 'Đã xóa cột [revenue] từ bảng [clients].';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[clients]') AND name = 'forecast')
BEGIN
    ALTER TABLE [dbo].[clients] DROP COLUMN [forecast];
    PRINT 'Đã xóa cột [forecast] từ bảng [clients].';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[clients]') AND name = 'agent_name')
BEGIN
    ALTER TABLE [dbo].[clients] DROP COLUMN [agent_name];
    PRINT 'Đã xóa cột [agent_name] từ bảng [clients].';
END
GO

-- =============================================
-- HOÀN TẤT
-- =============================================
PRINT '';
PRINT '========================================';
PRINT 'Script Rollback đã được thực thi thành công!';
PRINT '========================================';
PRINT '';

