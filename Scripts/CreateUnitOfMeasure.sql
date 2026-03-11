-- =============================================
-- Tạo bảng Đơn vị tính (unit_of_measure)
-- Dùng cho field ĐVT của analysis_item (unit / unit_of_measure_id)
-- Cột: STT, Mã ĐVT, Tên (VIE), Tên (ENG), Trạng thái, Ghi chú, Người cập nhật
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'unit_of_measure')
BEGIN
    CREATE TABLE [dbo].[unit_of_measure] (
        [unit_of_measure_id]   UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [sequence_number]      INT NULL,                             -- STT
        [unit_of_measure_code] NVARCHAR(100) NULL,                   -- Mã đơn vị tính
        [name_vi]              NVARCHAR(500) NULL,                   -- Tên (VIE)
        [name_en]              NVARCHAR(500) NULL,                   -- Tên (ENG)
        [status]               NVARCHAR(50) NOT NULL DEFAULT N'Active',
        [notes]                NVARCHAR(2000) NULL,
        [created_at]           DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        [updated_at]           DATETIME2 NULL,
        [updated_by]           UNIQUEIDENTIFIER NULL,
        CONSTRAINT [FK_unit_of_measure_updated_by]
            FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION
    );
    PRINT N'Đã tạo bảng unit_of_measure (Đơn vị tính).';
END
ELSE
    PRINT N'Bảng unit_of_measure đã tồn tại.';
GO
