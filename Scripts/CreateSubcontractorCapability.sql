-- =============================================
-- Script tạo bảng subcontractor_capability (Năng lực nhà thầu phụ)
-- Mapping giữa nhà thầu phụ (subcontractor) và chỉ tiêu (analysis_item)
-- =============================================
-- Cột: Code nhà thầu (FK), Code chỉ tiêu (FK), Mô tả/Ghi chú, Trạng thái
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[subcontractor_capability]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[subcontractor_capability] (
        [subcontractor_capability_id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [subcontractor_id] UNIQUEIDENTIFIER NOT NULL,
        [analysis_item_id] UNIQUEIDENTIFIER NOT NULL,
        [notes] NVARCHAR(2000) NULL,
        [status] NVARCHAR(50) NOT NULL DEFAULT N'Active',
        [created_at] DATETIME2 NULL,
        [updated_at] DATETIME2 NULL,

        CONSTRAINT [FK_subcontractor_capability_subcontractor]
            FOREIGN KEY ([subcontractor_id]) REFERENCES [dbo].[subcontractor] ([subcontractor_id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_subcontractor_capability_analysis_item]
            FOREIGN KEY ([analysis_item_id]) REFERENCES [dbo].[analysis_item] ([analysis_item_id]) ON DELETE NO ACTION
    );

    PRINT N'Bảng subcontractor_capability (Năng lực nhà thầu phụ) đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT N'Bảng subcontractor_capability đã tồn tại.';
END
GO

-- Unique: mỗi cặp (nhà thầu phụ, chỉ tiêu) chỉ xuất hiện một lần
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_subcontractor_capability_subcontractor_analysis_item' AND object_id = OBJECT_ID(N'[dbo].[subcontractor_capability]'))
BEGIN
    CREATE UNIQUE INDEX [IX_subcontractor_capability_subcontractor_analysis_item]
    ON [dbo].[subcontractor_capability] ([subcontractor_id], [analysis_item_id]);
    PRINT N'Unique index IX_subcontractor_capability_subcontractor_analysis_item đã được tạo.';
END
GO

-- Index: trạng thái
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'i_x_subcontractor_capability_status' AND object_id = OBJECT_ID(N'[dbo].[subcontractor_capability]'))
BEGIN
    CREATE INDEX [i_x_subcontractor_capability_status]
    ON [dbo].[subcontractor_capability] ([status]);
    PRINT N'Index i_x_subcontractor_capability_status đã được tạo.';
END
GO

PRINT N'=============================================';
PRINT N'Hoàn tất tạo bảng subcontractor_capability (Năng lực nhà thầu phụ).';
PRINT N'=============================================';