-- =============================================
-- Script tạo bảng ModuleApprover để quản lý người phê duyệt theo module
-- Hỗ trợ phân quyền theo User chỉ định hoặc theo Title (chức vụ)
-- =============================================

USE [VietLabs] -- Thay đổi tên database nếu cần
GO

-- =============================================
-- 1. Tạo bảng module_approver
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[module_approver]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[module_approver] (
        [module_approver_id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [module_code] NVARCHAR(100) NOT NULL, -- Quotation, Client, Package, etc.
        [approval_level] INT NOT NULL, -- 1, 2, 3...
        [approver_type] NVARCHAR(50) NOT NULL DEFAULT 'User', -- User hoặc Title
        [approver_employee_id] UNIQUEIDENTIFIER NULL, -- Nếu ApproverType = 'User'
        [approver_title] NVARCHAR(200) NULL, -- Nếu ApproverType = 'Title'
        [permission_id] UNIQUEIDENTIFIER NULL, -- Liên kết với Permission (optional)
        [notes] NVARCHAR(2000) NULL,
        [status] NVARCHAR(50) NOT NULL DEFAULT 'Active',
        [created_at] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [updated_at] DATETIME2 NULL,
        [created_by] UNIQUEIDENTIFIER NULL,
        [updated_by] UNIQUEIDENTIFIER NULL
    );
    
    PRINT 'Bảng module_approver đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Bảng module_approver đã tồn tại.';
END
GO

-- =============================================
-- 2. Tạo Indexes để tối ưu truy vấn
-- =============================================

-- Index cho module_code, approval_level, permission_id
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_module_approver_module_level_permission' AND object_id = OBJECT_ID(N'[dbo].[module_approver]'))
BEGIN
    CREATE INDEX [IX_module_approver_module_level_permission]
    ON [dbo].[module_approver] ([module_code], [approval_level], [permission_id]);
    
    PRINT 'Index IX_module_approver_module_level_permission đã được tạo.';
END
ELSE
BEGIN
    PRINT 'Index IX_module_approver_module_level_permission đã tồn tại.';
END
GO

-- Index cho approver_employee_id
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_module_approver_employee' AND object_id = OBJECT_ID(N'[dbo].[module_approver]'))
BEGIN
    CREATE INDEX [IX_module_approver_employee]
    ON [dbo].[module_approver] ([approver_employee_id]);
    
    PRINT 'Index IX_module_approver_employee đã được tạo.';
END
ELSE
BEGIN
    PRINT 'Index IX_module_approver_employee đã tồn tại.';
END
GO

-- Index cho permission_id
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_module_approver_permission' AND object_id = OBJECT_ID(N'[dbo].[module_approver]'))
BEGIN
    CREATE INDEX [IX_module_approver_permission]
    ON [dbo].[module_approver] ([permission_id]);
    
    PRINT 'Index IX_module_approver_permission đã được tạo.';
END
ELSE
BEGIN
    PRINT 'Index IX_module_approver_permission đã tồn tại.';
END
GO

-- =============================================
-- 3. Thêm Foreign Key Constraints
-- =============================================

-- Foreign key: approver_employee_id -> employee.employee_id
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_module_approver_employee')
BEGIN
    ALTER TABLE [dbo].[module_approver]
    ADD CONSTRAINT [FK_module_approver_employee] 
    FOREIGN KEY ([approver_employee_id]) 
    REFERENCES [dbo].[employee] ([employee_id])
    ON DELETE SET NULL
    ON UPDATE NO ACTION;
    
    PRINT 'Foreign key FK_module_approver_employee đã được thêm.';
END
ELSE
BEGIN
    PRINT 'Foreign key FK_module_approver_employee đã tồn tại.';
END
GO

-- Foreign key: permission_id -> permission.permission_id
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_module_approver_permission')
BEGIN
    ALTER TABLE [dbo].[module_approver]
    ADD CONSTRAINT [FK_module_approver_permission] 
    FOREIGN KEY ([permission_id]) 
    REFERENCES [dbo].[permission] ([permission_id])
    ON DELETE SET NULL
    ON UPDATE NO ACTION;
    
    PRINT 'Foreign key FK_module_approver_permission đã được thêm.';
END
ELSE
BEGIN
    PRINT 'Foreign key FK_module_approver_permission đã tồn tại.';
END
GO

-- =============================================
-- 4. Thêm Check Constraints
-- =============================================

-- Check constraint: ApproverType phải là 'User' hoặc 'Title'
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_module_approver_approver_type')
BEGIN
    ALTER TABLE [dbo].[module_approver]
    ADD CONSTRAINT [CK_module_approver_approver_type]
    CHECK ([approver_type] IN ('User', 'Title'));
    
    PRINT 'Check constraint CK_module_approver_approver_type đã được thêm.';
END
ELSE
BEGIN
    PRINT 'Check constraint CK_module_approver_approver_type đã tồn tại.';
END
GO

-- Check constraint: Nếu ApproverType = 'User' thì ApproverEmployeeId phải có giá trị
-- Nếu ApproverType = 'Title' thì ApproverTitle phải có giá trị
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_module_approver_approver_data')
BEGIN
    ALTER TABLE [dbo].[module_approver]
    ADD CONSTRAINT [CK_module_approver_approver_data]
    CHECK (
        ([approver_type] = 'User' AND [approver_employee_id] IS NOT NULL) OR
        ([approver_type] = 'Title' AND [approver_title] IS NOT NULL AND [approver_title] <> '')
    );
    
    PRINT 'Check constraint CK_module_approver_approver_data đã được thêm.';
END
ELSE
BEGIN
    PRINT 'Check constraint CK_module_approver_approver_data đã tồn tại.';
END
GO

PRINT 'Hoàn thành: Đã tạo bảng module_approver và các constraints.';
GO

