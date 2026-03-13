-- =============================================
-- Thêm cột Năng lực (CapacityType + FK capability) vào bảng quotation_item
-- Chi tiết báo giá ghi nhận thuộc Năng lực Vietlabs hay Năng lực nhà thầu phụ và tham chiếu bản ghi năng lực cụ thể.
-- =============================================

-- USE [VietLabs]  -- Bỏ comment nếu cần chỉ định database
-- GO

-- 1. Thêm cột capacity_type
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.quotation_item') AND name = N'capacity_type'
)
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ADD [capacity_type] NVARCHAR(50) NULL;
    PRINT N'Đã thêm cột capacity_type vào bảng quotation_item.';
END
ELSE
    PRINT N'Cột capacity_type đã tồn tại.';
GO

-- 2. Thêm cột department_analysis_capability_id
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.quotation_item') AND name = N'department_analysis_capability_id'
)
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ADD [department_analysis_capability_id] UNIQUEIDENTIFIER NULL;
    PRINT N'Đã thêm cột department_analysis_capability_id vào bảng quotation_item.';
END
ELSE
    PRINT N'Cột department_analysis_capability_id đã tồn tại.';
GO

-- 3. Thêm cột subcontractor_capability_id
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.quotation_item') AND name = N'subcontractor_capability_id'
)
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ADD [subcontractor_capability_id] UNIQUEIDENTIFIER NULL;
    PRINT N'Đã thêm cột subcontractor_capability_id vào bảng quotation_item.';
END
ELSE
    PRINT N'Cột subcontractor_capability_id đã tồn tại.';
GO

-- 4. Tạo index cho department_analysis_capability_id
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.quotation_item') AND name = N'i_x_quotation_item_department_analysis_capability_id'
)
BEGIN
    CREATE INDEX [i_x_quotation_item_department_analysis_capability_id]
    ON [dbo].[quotation_item] ([department_analysis_capability_id]);
    PRINT N'Đã tạo index i_x_quotation_item_department_analysis_capability_id.';
END
ELSE
    PRINT N'Index i_x_quotation_item_department_analysis_capability_id đã tồn tại.';
GO

-- 5. Tạo index cho subcontractor_capability_id
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.quotation_item') AND name = N'i_x_quotation_item_subcontractor_capability_id'
)
BEGIN
    CREATE INDEX [i_x_quotation_item_subcontractor_capability_id]
    ON [dbo].[quotation_item] ([subcontractor_capability_id]);
    PRINT N'Đã tạo index i_x_quotation_item_subcontractor_capability_id.';
END
ELSE
    PRINT N'Index i_x_quotation_item_subcontractor_capability_id đã tồn tại.';
GO

-- 6. FK: quotation_item -> department_analysis_capability
IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'f_k_quotation_item_department_analysis_capability_department_analysis_capability_id'
)
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ADD CONSTRAINT [f_k_quotation_item_department_analysis_capability_department_analysis_capability_id]
    FOREIGN KEY ([department_analysis_capability_id])
    REFERENCES [dbo].[department_analysis_capability] ([department_analysis_capability_id])
    ON DELETE NO ACTION;
    PRINT N'Đã thêm FK tới department_analysis_capability.';
END
ELSE
    PRINT N'FK f_k_quotation_item_department_analysis_capability_department_analysis_capability_id đã tồn tại.';
GO

-- 7. FK: quotation_item -> subcontractor_capability
IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'f_k_quotation_item_subcontractor_capability_subcontractor_capability_id'
)
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ADD CONSTRAINT [f_k_quotation_item_subcontractor_capability_subcontractor_capability_id]
    FOREIGN KEY ([subcontractor_capability_id])
    REFERENCES [dbo].[subcontractor_capability] ([subcontractor_capability_id])
    ON DELETE NO ACTION;
    PRINT N'Đã thêm FK tới subcontractor_capability.';
END
ELSE
    PRINT N'FK f_k_quotation_item_subcontractor_capability_subcontractor_capability_id đã tồn tại.';
GO
