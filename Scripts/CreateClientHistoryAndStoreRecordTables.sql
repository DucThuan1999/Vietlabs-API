-- Script tạo bảng client_history và store_record
-- Chạy script này nếu các bảng chưa tồn tại trong database

USE [VietLabs]; -- Thay đổi tên database nếu cần
GO

-- =============================================
-- TẠO BẢNG client_history
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[client_history]') AND type in (N'U'))
BEGIN
    -- Tạo bảng client_history
    CREATE TABLE [dbo].[client_history] (
        [client_history_id] UNIQUEIDENTIFIER NOT NULL,
        [client_id] UNIQUEIDENTIFIER NOT NULL,
        [changed_date] DATETIME2 NOT NULL,
        [change_description] NVARCHAR(2000) NOT NULL,
        [changed_by_account_id] UNIQUEIDENTIFIER NOT NULL,
        [change_type] NVARCHAR(50) NULL,
        CONSTRAINT [PK_client_history] PRIMARY KEY CLUSTERED ([client_history_id] ASC),
        CONSTRAINT [FK_client_history_client] FOREIGN KEY ([client_id]) 
            REFERENCES [dbo].[client] ([client_id]) 
            ON DELETE CASCADE,
        CONSTRAINT [FK_client_history_account] FOREIGN KEY ([changed_by_account_id]) 
            REFERENCES [dbo].[account] ([account_id]) 
            ON DELETE NO ACTION
    );
    
    -- Tạo index cho client_id để tăng hiệu suất query
    CREATE NONCLUSTERED INDEX [IX_client_history_client_id] 
        ON [dbo].[client_history] ([client_id]);
    
    -- Tạo index cho changed_date để tăng hiệu suất query theo ngày
    CREATE NONCLUSTERED INDEX [IX_client_history_changed_date] 
        ON [dbo].[client_history] ([changed_date]);
    
    -- Tạo index cho changed_by_account_id để tăng hiệu suất query theo user
    CREATE NONCLUSTERED INDEX [IX_client_history_changed_by_account_id] 
        ON [dbo].[client_history] ([changed_by_account_id]);
    
    PRINT 'Bảng client_history đã được tạo thành công!';
END
ELSE
BEGIN
    PRINT 'Bảng client_history đã tồn tại.';
END
GO

-- =============================================
-- TẠO BẢNG store_record
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[store_record]') AND type in (N'U'))
BEGIN
    -- Tạo bảng store_record
    CREATE TABLE [dbo].[store_record] (
        [store_record_id] UNIQUEIDENTIFIER NOT NULL,
        [client_id] UNIQUEIDENTIFIER NOT NULL,
        [attachment_name] NVARCHAR(500) NULL,
        [attachment_path] NVARCHAR(1000) NOT NULL,
        [file_name] NVARCHAR(500) NULL,
        [file_size] BIGINT NULL,
        [content_type] NVARCHAR(100) NULL,
        [created_date] DATETIME2 NOT NULL,
        [updated_date] DATETIME2 NULL,
        CONSTRAINT [PK_store_record] PRIMARY KEY CLUSTERED ([store_record_id] ASC),
        CONSTRAINT [FK_store_record_client] FOREIGN KEY ([client_id]) 
            REFERENCES [dbo].[client] ([client_id]) 
            ON DELETE CASCADE
    );
    
    -- Tạo index cho client_id để tăng hiệu suất query
    CREATE NONCLUSTERED INDEX [IX_store_record_client_id] 
        ON [dbo].[store_record] ([client_id]);
    
    -- Tạo index cho created_date để tăng hiệu suất query theo ngày
    CREATE NONCLUSTERED INDEX [IX_store_record_created_date] 
        ON [dbo].[store_record] ([created_date]);
    
    PRINT 'Bảng store_record đã được tạo thành công!';
END
ELSE
BEGIN
    PRINT 'Bảng store_record đã tồn tại.';
END
GO

PRINT 'Hoàn tất tạo các bảng client_history và store_record!';
GO

