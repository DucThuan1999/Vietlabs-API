-- =============================================
-- Di chuyển Nd107, Nd107ExpiredDate, Designations từ AnalysisItem
-- sang DepartmentAnalysisCapability và SubcontractorCapability
-- =============================================

USE [VietLabs]
GO

-- ========== 1. Thêm cột vào department_analysis_capability ==========
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.department_analysis_capability') AND name = N'nd_107')
BEGIN
    ALTER TABLE [dbo].[department_analysis_capability]
        ADD [nd_107] BIT NOT NULL DEFAULT 0,
            [nd_107_expired_date] DATE NULL;
    PRINT N'Đã thêm nd_107, nd_107_expired_date vào department_analysis_capability.';
END
GO

-- ========== 2. Thêm cột vào subcontractor_capability ==========
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.subcontractor_capability') AND name = N'nd_107')
BEGIN
    ALTER TABLE [dbo].[subcontractor_capability]
        ADD [nd_107] BIT NOT NULL DEFAULT 0,
            [nd_107_expired_date] DATE NULL;
    PRINT N'Đã thêm nd_107, nd_107_expired_date vào subcontractor_capability.';
END
GO

-- ========== 3. Tạo bảng department_analysis_capability_designation ==========
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'department_analysis_capability_designation')
BEGIN
    CREATE TABLE [dbo].[department_analysis_capability_designation] (
        [department_analysis_capability_designation_id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [department_analysis_capability_id] UNIQUEIDENTIFIER NOT NULL,
        [designation_id] UNIQUEIDENTIFIER NOT NULL,
        [expired_date] DATE NULL,
        CONSTRAINT [FK_department_analysis_capability_designation_capability]
            FOREIGN KEY ([department_analysis_capability_id])
            REFERENCES [dbo].[department_analysis_capability] ([department_analysis_capability_id]) ON DELETE CASCADE,
        CONSTRAINT [FK_department_analysis_capability_designation_designation]
            FOREIGN KEY ([designation_id])
            REFERENCES [dbo].[designation] ([designation_id]) ON DELETE NO ACTION,
        CONSTRAINT [UQ_department_analysis_capability_designation]
            UNIQUE ([department_analysis_capability_id], [designation_id])
    );
    CREATE UNIQUE INDEX [IX_department_analysis_capability_designation_unique]
        ON [dbo].[department_analysis_capability_designation] ([department_analysis_capability_id], [designation_id]);
    PRINT N'Đã tạo bảng department_analysis_capability_designation.';
END
ELSE
    PRINT N'Bảng department_analysis_capability_designation đã tồn tại.';
GO

-- ========== 4. Migrate dữ liệu nd_107, nd_107_expired_date từ analysis_item ==========
-- Chỉ copy các cột tồn tại trên analysis_item (có DB chỉ có nd_107, không có nd_107_expired_date)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.analysis_item') AND name = N'nd_107')
BEGIN
    -- Copy nd_107 sang department_analysis_capability
    UPDATE dac
    SET dac.[nd_107] = ai.[nd_107]
    FROM [dbo].[department_analysis_capability] dac
    INNER JOIN [dbo].[analysis_item] ai ON ai.[analysis_item_id] = dac.[analysis_item_id];

    -- Copy nd_107 sang subcontractor_capability
    UPDATE sc
    SET sc.[nd_107] = ai.[nd_107]
    FROM [dbo].[subcontractor_capability] sc
    INNER JOIN [dbo].[analysis_item] ai ON ai.[analysis_item_id] = sc.[analysis_item_id];

    PRINT N'Đã copy nd_107 từ analysis_item sang capabilities.';
END
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.analysis_item') AND name = N'nd_107_expired_date')
BEGIN
    -- Copy nd_107_expired_date sang department_analysis_capability
    UPDATE dac
    SET dac.[nd_107_expired_date] = ai.[nd_107_expired_date]
    FROM [dbo].[department_analysis_capability] dac
    INNER JOIN [dbo].[analysis_item] ai ON ai.[analysis_item_id] = dac.[analysis_item_id];

    -- Copy nd_107_expired_date sang subcontractor_capability
    UPDATE sc
    SET sc.[nd_107_expired_date] = ai.[nd_107_expired_date]
    FROM [dbo].[subcontractor_capability] sc
    INNER JOIN [dbo].[analysis_item] ai ON ai.[analysis_item_id] = sc.[analysis_item_id];

    PRINT N'Đã copy nd_107_expired_date từ analysis_item sang capabilities.';
END
GO

-- ========== 5. Migrate designation từ analysis_item_designation sang department_analysis_capability_designation ==========
-- Với mỗi (analysis_item_id, designation_id) trong analysis_item_designation,
-- tạo bản ghi department_analysis_capability_designation cho từng department_analysis_capability có cùng analysis_item_id
IF EXISTS (SELECT * FROM sys.tables WHERE name = N'analysis_item_designation')
BEGIN
    INSERT INTO [dbo].[department_analysis_capability_designation] (
        [department_analysis_capability_designation_id],
        [department_analysis_capability_id],
        [designation_id],
        [expired_date]
    )
    SELECT
        NEWID(),
        dac.[department_analysis_capability_id],
        aid.[designation_id],
        aid.[expired_date]
    FROM [dbo].[analysis_item_designation] aid
    INNER JOIN [dbo].[department_analysis_capability] dac ON dac.[analysis_item_id] = aid.[analysis_item_id]
    WHERE NOT EXISTS (
        SELECT 1
        FROM [dbo].[department_analysis_capability_designation] dacd
        WHERE dacd.[department_analysis_capability_id] = dac.[department_analysis_capability_id]
          AND dacd.[designation_id] = aid.[designation_id]
    );

    PRINT N'Đã copy designation từ analysis_item_designation sang department_analysis_capability_designation.';
END
GO

-- ========== 6. Xóa cột nd_107, nd_107_expired_date khỏi analysis_item ==========
-- Phải xóa default constraint trước khi DROP COLUMN (SQL Server)
DECLARE @ConstraintName NVARCHAR(200);

-- Xóa default constraint và cột nd_107
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.analysis_item') AND name = N'nd_107')
BEGIN
    SELECT @ConstraintName = dc.name
    FROM sys.default_constraints dc
    INNER JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
    WHERE dc.parent_object_id = OBJECT_ID(N'dbo.analysis_item') AND c.name = N'nd_107';

    IF @ConstraintName IS NOT NULL
    BEGIN
        EXEC(N'ALTER TABLE [dbo].[analysis_item] DROP CONSTRAINT [' + @ConstraintName + N']');
        PRINT N'Đã xóa default constraint trên nd_107.';
    END

    ALTER TABLE [dbo].[analysis_item] DROP COLUMN [nd_107];
    PRINT N'Đã xóa cột nd_107 khỏi analysis_item.';
END
GO

-- Xóa default constraint (nếu có) và cột nd_107_expired_date
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.analysis_item') AND name = N'nd_107_expired_date')
BEGIN
    DECLARE @ConstraintName2 NVARCHAR(200);
    SELECT @ConstraintName2 = dc.name
    FROM sys.default_constraints dc
    INNER JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
    WHERE dc.parent_object_id = OBJECT_ID(N'dbo.analysis_item') AND c.name = N'nd_107_expired_date';

    IF @ConstraintName2 IS NOT NULL
    BEGIN
        EXEC(N'ALTER TABLE [dbo].[analysis_item] DROP CONSTRAINT [' + @ConstraintName2 + N']');
        PRINT N'Đã xóa default constraint trên nd_107_expired_date.';
    END

    ALTER TABLE [dbo].[analysis_item] DROP COLUMN [nd_107_expired_date];
    PRINT N'Đã xóa cột nd_107_expired_date khỏi analysis_item.';
END
GO

-- ========== Ghi chú ==========
-- Bảng analysis_item_designation được giữ lại (legacy). Ứng dụng không còn dùng Designations trên AnalysisItem.
-- Nếu muốn xóa bảng analysis_item_designation sau khi đã migrate xong, chạy riêng:
--   DROP TABLE [dbo].[analysis_item_designation];
-- (Chỉ làm khi chắc chắn không cần dữ liệu cũ.)
