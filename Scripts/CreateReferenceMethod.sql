-- =============================================
-- Tạo bảng Phương pháp tham chiếu (reference_method)
-- Cột: STT, Tên (VIE), Tên (ENG), Mã phương pháp tham chiếu, Trạng thái, Người cập nhật
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'reference_method')
BEGIN
    CREATE TABLE [dbo].[reference_method] (
        [reference_method_id]   UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [sequence_number]       INT NULL,                            -- STT
        [name_vi]               NVARCHAR(500) NULL,                   -- Tên phương pháp (VIE)
        [name_en]               NVARCHAR(500) NULL,                   -- Tên phương pháp (ENG)
        [reference_method_code] NVARCHAR(1000) NULL,                   -- Mã phương pháp tham chiếu (quốc tế)
        [status]                NVARCHAR(50) NOT NULL DEFAULT N'Active',  -- Trạng thái
        [created_at]            DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        [updated_at]            DATETIME2 NULL,
        [updated_by]            UNIQUEIDENTIFIER NULL,                -- Người cập nhật (FK -> account)
        CONSTRAINT [FK_reference_method_updated_by]
            FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION
    );
    PRINT N'Đã tạo bảng reference_method.';
END
ELSE
    PRINT N'Bảng reference_method đã tồn tại.';
GO
