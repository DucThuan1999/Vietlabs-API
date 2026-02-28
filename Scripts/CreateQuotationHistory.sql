-- =============================================
-- Script tạo bảng QuotationHistory để lưu lịch sử thay đổi của báo giá
-- =============================================

USE [VietLabs] -- Thay đổi tên database nếu cần
GO

-- =============================================
-- 1. Tạo bảng quotation_history
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[quotation_history]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[quotation_history] (
        [quotation_history_id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [quotation_id] UNIQUEIDENTIFIER NOT NULL,
        [changed_date] DATETIME2 NOT NULL,
        [change_description] NVARCHAR(2000) NOT NULL,
        [changed_by_account_id] UNIQUEIDENTIFIER NOT NULL,
        [change_type] NVARCHAR(50) NULL,
        [old_values] NVARCHAR(MAX) NULL,
        [new_values] NVARCHAR(MAX) NULL
    );
    
    PRINT 'Bảng quotation_history đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Bảng quotation_history đã tồn tại.';
END
GO

-- =============================================
-- 2. Thêm Foreign Key Constraints
-- =============================================

-- Foreign key: quotation_id -> quotation.quotation_id
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_quotation_history_quotation')
BEGIN
    ALTER TABLE [dbo].[quotation_history]
    ADD CONSTRAINT [FK_quotation_history_quotation] 
    FOREIGN KEY ([quotation_id]) 
    REFERENCES [dbo].[quotation] ([quotation_id])
    ON DELETE CASCADE
    ON UPDATE NO ACTION;
    
    PRINT 'Foreign key FK_quotation_history_quotation đã được thêm.';
END
ELSE
BEGIN
    PRINT 'Foreign key FK_quotation_history_quotation đã tồn tại.';
END
GO

-- Foreign key: changed_by_account_id -> account.account_id
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_quotation_history_account')
BEGIN
    ALTER TABLE [dbo].[quotation_history]
    ADD CONSTRAINT [FK_quotation_history_account] 
    FOREIGN KEY ([changed_by_account_id]) 
    REFERENCES [dbo].[account] ([account_id])
    ON DELETE NO ACTION
    ON UPDATE NO ACTION;
    
    PRINT 'Foreign key FK_quotation_history_account đã được thêm.';
END
ELSE
BEGIN
    PRINT 'Foreign key FK_quotation_history_account đã tồn tại.';
END
GO

-- =============================================
-- 3. Tạo Indexes để tối ưu hiệu suất truy vấn
-- =============================================

-- Index cho quotation_id
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_quotation_history_quotation_id' AND object_id = OBJECT_ID(N'[dbo].[quotation_history]'))
BEGIN
    CREATE INDEX [IX_quotation_history_quotation_id] 
    ON [dbo].[quotation_history] ([quotation_id]);
    
    PRINT 'Index IX_quotation_history_quotation_id đã được tạo.';
END
ELSE
BEGIN
    PRINT 'Index IX_quotation_history_quotation_id đã tồn tại.';
END
GO

-- Index cho changed_date
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_quotation_history_changed_date' AND object_id = OBJECT_ID(N'[dbo].[quotation_history]'))
BEGIN
    CREATE INDEX [IX_quotation_history_changed_date] 
    ON [dbo].[quotation_history] ([changed_date]);
    
    PRINT 'Index IX_quotation_history_changed_date đã được tạo.';
END
ELSE
BEGIN
    PRINT 'Index IX_quotation_history_changed_date đã tồn tại.';
END
GO

-- Index cho changed_by_account_id
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_quotation_history_changed_by_account_id' AND object_id = OBJECT_ID(N'[dbo].[quotation_history]'))
BEGIN
    CREATE INDEX [IX_quotation_history_changed_by_account_id] 
    ON [dbo].[quotation_history] ([changed_by_account_id]);
    
    PRINT 'Index IX_quotation_history_changed_by_account_id đã được tạo.';
END
ELSE
BEGIN
    PRINT 'Index IX_quotation_history_changed_by_account_id đã tồn tại.';
END
GO

-- Composite index cho truy vấn theo quotation_id và changed_date (thường dùng để lấy lịch sử theo thời gian)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_quotation_history_quotation_id_changed_date' AND object_id = OBJECT_ID(N'[dbo].[quotation_history]'))
BEGIN
    CREATE INDEX [IX_quotation_history_quotation_id_changed_date] 
    ON [dbo].[quotation_history] ([quotation_id], [changed_date] DESC);
    
    PRINT 'Index IX_quotation_history_quotation_id_changed_date đã được tạo.';
END
ELSE
BEGIN
    PRINT 'Index IX_quotation_history_quotation_id_changed_date đã tồn tại.';
END
GO

PRINT '=============================================';
PRINT 'Hoàn tất tạo bảng quotation_history và các ràng buộc.';
PRINT '=============================================';

