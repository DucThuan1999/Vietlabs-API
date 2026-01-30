-- Script tạo bảng refresh_token
-- Chạy script này nếu bảng refresh_token chưa tồn tại trong database

USE [VietLabDb]; -- Thay đổi tên database nếu cần
GO

-- Kiểm tra xem bảng đã tồn tại chưa
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[refresh_token]') AND type in (N'U'))
BEGIN
    -- Tạo bảng refresh_token
    CREATE TABLE [dbo].[refresh_token] (
        [refresh_token_id] UNIQUEIDENTIFIER NOT NULL,
        [account_id] UNIQUEIDENTIFIER NOT NULL,
        [token] NVARCHAR(450) NOT NULL,
        [expires_at] DATETIME2 NOT NULL,
        [created_at] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [is_revoked] BIT NOT NULL DEFAULT 0,
        [revoked_reason] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_refresh_token] PRIMARY KEY CLUSTERED ([refresh_token_id] ASC),
        CONSTRAINT [FK_refresh_token_account] FOREIGN KEY ([account_id]) 
            REFERENCES [dbo].[account] ([account_id]) 
            ON DELETE CASCADE
    );
    
    -- Tạo index cho account_id để tăng hiệu suất query
    CREATE NONCLUSTERED INDEX [IX_refresh_token_account_id] 
        ON [dbo].[refresh_token] ([account_id]);
    
    -- Tạo index cho token để tăng hiệu suất tìm kiếm token
    CREATE NONCLUSTERED INDEX [IX_refresh_token_token] 
        ON [dbo].[refresh_token] ([token]);
    
    -- Tạo index cho expires_at để tăng hiệu suất query token hết hạn
    CREATE NONCLUSTERED INDEX [IX_refresh_token_expires_at] 
        ON [dbo].[refresh_token] ([expires_at]);
    
    PRINT 'Bảng refresh_token đã được tạo thành công!';
END
ELSE
BEGIN
    PRINT 'Bảng refresh_token đã tồn tại.';
END
GO

