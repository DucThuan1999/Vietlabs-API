-- =============================================
-- Script SQL: Tạo bảng QuotationItem, Package, PackageAnalysisGroup
-- Database: VietLabs
-- =============================================

USE VietLabs;
GO

-- =============================================
-- 1. TẠO BẢNG PACKAGE
-- Bảng gói phân tích (chứa nhiều nhóm chỉ tiêu)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'package')
BEGIN
    CREATE TABLE [dbo].[package] (
        [package_id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [package_code] NVARCHAR(50) NULL,
        [name_vi] NVARCHAR(500) NULL,
        [name_en] NVARCHAR(500) NULL,
        [description] NVARCHAR(MAX) NULL,
        [default_price] DECIMAL(18,2) NULL,
        [status] NVARCHAR(50) NOT NULL DEFAULT 'Active',
        [notes] NVARCHAR(MAX) NULL,
        [created_at] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [updated_at] DATETIME2 NULL
    );
    
    PRINT 'Bảng [package] đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Bảng [package] đã tồn tại.';
END
GO

-- =============================================
-- 2. TẠO BẢNG PACKAGE_ANALYSIS_GROUP
-- Bảng trung gian: Package - AnalysisGroup (many-to-many)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'package_analysis_group')
BEGIN
    CREATE TABLE [dbo].[package_analysis_group] (
        [package_analysis_group_id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [package_id] UNIQUEIDENTIFIER NOT NULL,
        [analysis_group_id] UNIQUEIDENTIFIER NOT NULL,
        [display_order] INT NULL,
        [is_required] BIT NOT NULL DEFAULT 1,
        [notes] NVARCHAR(MAX) NULL,
        [created_at] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        -- Unique constraint: một package không thể có cùng analysis group 2 lần
        CONSTRAINT [UQ_package_analysis_group_package_group] UNIQUE ([package_id], [analysis_group_id])
    );
    
    PRINT 'Bảng [package_analysis_group] đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Bảng [package_analysis_group] đã tồn tại.';
END
GO

-- =============================================
-- 3. TẠO BẢNG QUOTATION_ITEM
-- Bảng chi tiết báo giá (hỗ trợ 3 dạng: AnalysisItem, AnalysisGroup, Package)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'quotation_item')
BEGIN
    CREATE TABLE [dbo].[quotation_item] (
        [quotation_item_id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [quotation_id] UNIQUEIDENTIFIER NOT NULL,
        
        -- Loại item: "AnalysisItem", "AnalysisGroup", "Package"
        [item_type] NVARCHAR(50) NOT NULL,
        
        -- Foreign keys (chỉ một trong 3 có giá trị)
        [analysis_item_id] UNIQUEIDENTIFIER NULL,
        [analysis_group_id] UNIQUEIDENTIFIER NULL,
        [package_id] UNIQUEIDENTIFIER NULL,
        
        -- Thông tin hiển thị (có thể override từ master data)
        [item_code] NVARCHAR(50) NULL,
        [item_name_vi] NVARCHAR(500) NULL,
        [item_name_en] NVARCHAR(500) NULL,
        [description] NVARCHAR(MAX) NULL,
        
        -- Thông tin giá và số lượng
        [quantity] INT NOT NULL DEFAULT 1,
        [unit_price] DECIMAL(18,2) NOT NULL,
        [discount_percent] DECIMAL(5,2) NULL,
        [discount_amount] DECIMAL(18,2) NULL,
        [sub_total] DECIMAL(18,2) NOT NULL,
        
        -- Thông tin bổ sung
        [display_order] INT NULL,
        [notes] NVARCHAR(MAX) NULL,
        
        [created_at] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [updated_at] DATETIME2 NULL
    );
    
    PRINT 'Bảng [quotation_item] đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Bảng [quotation_item] đã tồn tại.';
END
GO

-- =============================================
-- 4. TẠO FOREIGN KEY CONSTRAINTS
-- =============================================

-- Foreign Key: package_analysis_group -> package
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_package_analysis_group_package')
BEGIN
    ALTER TABLE [dbo].[package_analysis_group]
    ADD CONSTRAINT [FK_package_analysis_group_package]
    FOREIGN KEY ([package_id]) REFERENCES [dbo].[package]([package_id])
    ON DELETE CASCADE;
    
    PRINT 'Foreign key FK_package_analysis_group_package đã được tạo.';
END
GO

-- Foreign Key: package_analysis_group -> analysis_group
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_package_analysis_group_analysis_group')
BEGIN
    ALTER TABLE [dbo].[package_analysis_group]
    ADD CONSTRAINT [FK_package_analysis_group_analysis_group]
    FOREIGN KEY ([analysis_group_id]) REFERENCES [dbo].[analysis_group]([analysis_group_id])
    ON DELETE NO ACTION;
    
    PRINT 'Foreign key FK_package_analysis_group_analysis_group đã được tạo.';
END
GO

-- Foreign Key: quotation_item -> quotation
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_quotation_item_quotation')
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ADD CONSTRAINT [FK_quotation_item_quotation]
    FOREIGN KEY ([quotation_id]) REFERENCES [dbo].[quotation]([quotation_id])
    ON DELETE CASCADE;
    
    PRINT 'Foreign key FK_quotation_item_quotation đã được tạo.';
END
GO

-- Foreign Key: quotation_item -> analysis_item (optional)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_quotation_item_analysis_item')
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ADD CONSTRAINT [FK_quotation_item_analysis_item]
    FOREIGN KEY ([analysis_item_id]) REFERENCES [dbo].[analysis_item]([analysis_item_id])
    ON DELETE SET NULL;
    
    PRINT 'Foreign key FK_quotation_item_analysis_item đã được tạo.';
END
GO

-- Foreign Key: quotation_item -> analysis_group (optional)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_quotation_item_analysis_group')
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ADD CONSTRAINT [FK_quotation_item_analysis_group]
    FOREIGN KEY ([analysis_group_id]) REFERENCES [dbo].[analysis_group]([analysis_group_id])
    ON DELETE SET NULL;
    
    PRINT 'Foreign key FK_quotation_item_analysis_group đã được tạo.';
END
GO

-- Foreign Key: quotation_item -> package (optional)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_quotation_item_package')
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ADD CONSTRAINT [FK_quotation_item_package]
    FOREIGN KEY ([package_id]) REFERENCES [dbo].[package]([package_id])
    ON DELETE SET NULL;
    
    PRINT 'Foreign key FK_quotation_item_package đã được tạo.';
END
GO

-- =============================================
-- 5. TẠO CHECK CONSTRAINTS
-- Đảm bảo chỉ một trong 3 foreign keys có giá trị và khớp với ItemType
-- =============================================

-- Check constraint: Chỉ một trong 3 FK có giá trị
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_quotation_item_single_reference')
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ADD CONSTRAINT [CK_quotation_item_single_reference]
    CHECK (
        -- Chỉ một trong 3 có giá trị
        (
            ([analysis_item_id] IS NOT NULL AND [analysis_group_id] IS NULL AND [package_id] IS NULL) OR
            ([analysis_item_id] IS NULL AND [analysis_group_id] IS NOT NULL AND [package_id] IS NULL) OR
            ([analysis_item_id] IS NULL AND [analysis_group_id] IS NULL AND [package_id] IS NOT NULL)
        ) AND
        -- ItemType phải khớp với FK có giá trị
        (
            ([item_type] = 'AnalysisItem' AND [analysis_item_id] IS NOT NULL) OR
            ([item_type] = 'AnalysisGroup' AND [analysis_group_id] IS NOT NULL) OR
            ([item_type] = 'Package' AND [package_id] IS NOT NULL)
        )
    );
    
    PRINT 'Check constraint CK_quotation_item_single_reference đã được tạo.';
END
GO

-- Check constraint: ItemType phải là một trong 3 giá trị hợp lệ
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_quotation_item_item_type')
BEGIN
    ALTER TABLE [dbo].[quotation_item]
    ADD CONSTRAINT [CK_quotation_item_item_type]
    CHECK ([item_type] IN ('AnalysisItem', 'AnalysisGroup', 'Package'));
    
    PRINT 'Check constraint CK_quotation_item_item_type đã được tạo.';
END
GO

-- =============================================
-- 6. TẠO INDEXES ĐỂ TỐI ƯU HIỆU SUẤT
-- =============================================

-- Index cho quotation_item
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_quotation_item_quotation_id')
BEGIN
    CREATE INDEX [IX_quotation_item_quotation_id] ON [dbo].[quotation_item]([quotation_id]);
    PRINT 'Index IX_quotation_item_quotation_id đã được tạo.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_quotation_item_item_type')
BEGIN
    CREATE INDEX [IX_quotation_item_item_type] ON [dbo].[quotation_item]([item_type]);
    PRINT 'Index IX_quotation_item_item_type đã được tạo.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_quotation_item_display_order')
BEGIN
    CREATE INDEX [IX_quotation_item_display_order] ON [dbo].[quotation_item]([quotation_id], [display_order]);
    PRINT 'Index IX_quotation_item_display_order đã được tạo.';
END
GO

-- Index cho package_analysis_group
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_package_analysis_group_package_id')
BEGIN
    CREATE INDEX [IX_package_analysis_group_package_id] ON [dbo].[package_analysis_group]([package_id]);
    PRINT 'Index IX_package_analysis_group_package_id đã được tạo.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_package_analysis_group_analysis_group_id')
BEGIN
    CREATE INDEX [IX_package_analysis_group_analysis_group_id] ON [dbo].[package_analysis_group]([analysis_group_id]);
    PRINT 'Index IX_package_analysis_group_analysis_group_id đã được tạo.';
END
GO

-- =============================================
-- HOÀN TẤT
-- =============================================
PRINT '';
PRINT '========================================';
PRINT 'Script SQL đã được thực thi thành công!';
PRINT '========================================';
PRINT '';
PRINT 'Đã tạo:';
PRINT '  - Bảng [package]';
PRINT '  - Bảng [package_analysis_group]';
PRINT '  - Bảng [quotation_item]';
PRINT '  - Foreign keys và check constraints';
PRINT '  - Indexes';
PRINT '';

