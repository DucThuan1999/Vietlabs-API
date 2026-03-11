-- =============================================================================
-- Thay PackageAnalysisGroup bằng PackageAnalysisItem
-- 1. Tạo bảng package_analysis_item
-- 2. Migrate dữ liệu: mỗi (Package + AnalysisGroup) cũ -> gắn tất cả AnalysisItem thuộc nhóm đó vào Package
-- 3. Xóa bảng package_analysis_group
-- =============================================================================

BEGIN TRANSACTION;

-- -----------------------------------------------------------------------------
-- Bước 1: Tạo bảng package_analysis_item
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'package_analysis_item')
BEGIN
    CREATE TABLE [dbo].[package_analysis_item] (
        [package_analysis_item_id] UNIQUEIDENTIFIER NOT NULL,
        [package_id]               UNIQUEIDENTIFIER NOT NULL,
        [analysis_item_id]         UNIQUEIDENTIFIER NOT NULL,
        [display_order]            INT              NULL,
        [is_required]              BIT              NOT NULL DEFAULT 1,
        [notes]                    NVARCHAR(MAX)    NULL,
        [created_at]               DATETIME2(7)      NOT NULL,
        [updated_at]               DATETIME2(7)     NULL,
        [updated_by]               UNIQUEIDENTIFIER NULL,
        CONSTRAINT [PK_package_analysis_item] PRIMARY KEY ([package_analysis_item_id]),
        CONSTRAINT [FK_package_analysis_item_package] FOREIGN KEY ([package_id]) REFERENCES [dbo].[package] ([package_id]) ON DELETE CASCADE,
        CONSTRAINT [FK_package_analysis_item_analysis_item] FOREIGN KEY ([analysis_item_id]) REFERENCES [dbo].[analysis_item] ([analysis_item_id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_package_analysis_item_updated_by] FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION
    );

    CREATE UNIQUE INDEX [UQ_package_analysis_item_package_item]
        ON [dbo].[package_analysis_item] ([package_id], [analysis_item_id]);

    PRINT N'Đã tạo bảng package_analysis_item.';
END
ELSE
    PRINT N'Bảng package_analysis_item đã tồn tại.';

-- -----------------------------------------------------------------------------
-- Bước 2: Migrate dữ liệu từ package_analysis_group (nếu bảng cũ còn tồn tại)
-- Với mỗi cặp (package_id, analysis_group_id), thêm tất cả analysis_item thuộc nhóm đó vào gói
-- -----------------------------------------------------------------------------
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = N'package_analysis_group')
BEGIN
    INSERT INTO [dbo].[package_analysis_item] (
        [package_analysis_item_id],
        [package_id],
        [analysis_item_id],
        [display_order],
        [is_required],
        [notes],
        [created_at],
        [updated_at],
        [updated_by]
    )
    SELECT
        NEWID(),
        pag.[package_id],
        ai.[analysis_item_id],
        pag.[display_order],
        ISNULL(pag.[is_required], 1),
        pag.[notes],
        ISNULL(pag.[created_at], GETUTCDATE()),
        pag.[updated_at],
        pag.[updated_by]
    FROM [dbo].[package_analysis_group] pag
    INNER JOIN [dbo].[analysis_item] ai ON ai.[analysis_group_id] = pag.[analysis_group_id]
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[package_analysis_item] pai
        WHERE pai.[package_id] = pag.[package_id] AND pai.[analysis_item_id] = ai.[analysis_item_id]
    );

    PRINT N'Đã migrate dữ liệu từ package_analysis_group sang package_analysis_item.';
END

-- -----------------------------------------------------------------------------
-- Bước 3: Xóa bảng package_analysis_group
-- -----------------------------------------------------------------------------
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = N'package_analysis_group')
BEGIN
    DROP TABLE [dbo].[package_analysis_group];
    PRINT N'Đã xóa bảng package_analysis_group.';
END
ELSE
    PRINT N'Bảng package_analysis_group không tồn tại (đã xóa trước đó).';

COMMIT TRANSACTION;
PRINT N'Hoàn tất.';
