-- =============================================
-- Tạo bảng Chỉ định (designation)
-- Các cột: STT, Mã chỉ định, Tên chỉ định, Trạng thái, Người cập nhật
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'designation')
BEGIN
    CREATE TABLE [dbo].[designation] (
        [designation_id]   UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [sequence_number] INT NULL,                    -- STT
        [designation_code] NVARCHAR(100) NULL,         -- Mã chỉ định
        [name]            NVARCHAR(500) NULL,          -- Tên chỉ định
        [status]          NVARCHAR(50) NOT NULL DEFAULT N'Active',  -- Trạng thái
        [created_at]      DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        [updated_at]      DATETIME2 NULL,
        [updated_by]      UNIQUEIDENTIFIER NULL,      -- Người cập nhật (FK -> account)
        CONSTRAINT [FK_designation_updated_by]
            FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION
    );
    PRINT N'Đã tạo bảng designation.';
END
ELSE
    PRINT N'Bảng designation đã tồn tại.';
GO
