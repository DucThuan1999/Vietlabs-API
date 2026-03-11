-- =============================================
-- Tạo bảng Tiêu chuẩn/Qui chuẩn (standard)
-- Các cột: STT, Mã, Tên (VIE), Tên (ENG), Trạng thái, Ghi chú, Người cập nhật
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'standard')
BEGIN
    CREATE TABLE [dbo].[standard] (
        [standard_id]     UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [sequence_number] INT NULL,                            -- STT
        [standard_code]   NVARCHAR(100) NULL,                   -- Mã tiêu chuẩn/qui chuẩn
        [name_vi]         NVARCHAR(500) NULL,                  -- Tiêu chuẩn/Qui chuẩn (VIE)
        [name_en]         NVARCHAR(500) NULL,                  -- Tên tiêu chuẩn/Qui chuẩn (ENG)
        [status]          NVARCHAR(50) NOT NULL DEFAULT N'Active',  -- Trạng thái
        [notes]           NVARCHAR(2000) NULL,                 -- Ghi chú
        [created_at]      DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        [updated_at]      DATETIME2 NULL,
        [updated_by]      UNIQUEIDENTIFIER NULL,               -- Người cập nhật (FK -> account)
        CONSTRAINT [FK_standard_updated_by]
            FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION
    );
    PRINT N'Đã tạo bảng standard.';
END
ELSE
    PRINT N'Bảng standard đã tồn tại.';
GO
