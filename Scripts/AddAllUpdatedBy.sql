-- =============================================
-- GỘP TẤT CẢ: Thêm cột updated_at / updated_by (và FK) vào các bảng
-- Chạy 1 lần trên database VietLabs. Account mặc định: 94eab415-1624-49de-85a6-a80916db3ab2
-- =============================================

USE [VietLabs]
GO

-- ---------- client ----------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.client') AND name = N'updated_at')
BEGIN
    ALTER TABLE [dbo].[client] ADD [updated_at] DATETIME2 NULL;
    PRINT N'[client] Đã thêm updated_at.';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.client') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[client] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[client] ADD CONSTRAINT [FK_client_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'[client] Đã thêm updated_by + FK.';
END
GO

-- ---------- contact ----------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.contact') AND name = N'created_at')
BEGIN
    ALTER TABLE [dbo].[contact] ADD [created_at] DATETIME2 NULL;
    PRINT N'[contact] Đã thêm created_at.';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.contact') AND name = N'updated_at')
BEGIN
    ALTER TABLE [dbo].[contact] ADD [updated_at] DATETIME2 NULL;
    PRINT N'[contact] Đã thêm updated_at.';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.contact') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[contact] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[contact] ADD CONSTRAINT [FK_contact_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'[contact] Đã thêm updated_by + FK.';
END
GO

-- ---------- equipment_type ----------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.equipment_type') AND name = N'created_at')
BEGIN
    ALTER TABLE [dbo].[equipment_type] ADD [created_at] DATETIME2 NULL;
    PRINT N'[equipment_type] Đã thêm created_at.';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.equipment_type') AND name = N'updated_at')
BEGIN
    ALTER TABLE [dbo].[equipment_type] ADD [updated_at] DATETIME2 NULL;
    PRINT N'[equipment_type] Đã thêm updated_at.';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.equipment_type') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[equipment_type] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[equipment_type] ADD CONSTRAINT [FK_equipment_type_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'[equipment_type] Đã thêm updated_by + FK.';
END
GO

-- ---------- employee ----------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.employee') AND name = N'updated_at')
BEGIN
    ALTER TABLE [dbo].[employee] ADD [updated_at] DATETIME2 NULL;
    PRINT N'[employee] Đã thêm updated_at.';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.employee') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[employee] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    PRINT N'[employee] Đã thêm updated_by.';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_employee_updated_by')
BEGIN
    ALTER TABLE [dbo].[employee] ADD CONSTRAINT [FK_employee_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'[employee] Đã tạo FK_employee_updated_by.';
END
GO

-- ---------- branch ----------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.branch') AND name = N'updated_at')
BEGIN
    ALTER TABLE [dbo].[branch] ADD [updated_at] DATETIME2 NULL;
    PRINT N'[branch] Đã thêm updated_at.';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.branch') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[branch] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    PRINT N'[branch] Đã thêm updated_by.';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_branch_updated_by')
BEGIN
    ALTER TABLE [dbo].[branch] ADD CONSTRAINT [FK_branch_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'[branch] Đã tạo FK_branch_updated_by.';
END
GO

-- ---------- department ----------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.department') AND name = N'updated_at')
BEGIN
    ALTER TABLE [dbo].[department] ADD [updated_at] DATETIME2 NULL;
    PRINT N'[department] Đã thêm updated_at.';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.department') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[department] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    PRINT N'[department] Đã thêm updated_by.';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_department_updated_by')
BEGIN
    ALTER TABLE [dbo].[department] ADD CONSTRAINT [FK_department_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'[department] Đã tạo FK_department_updated_by.';
END
GO

-- ---------- sample_matrix_group ----------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.sample_matrix_group') AND name = N'updated_at')
BEGIN
    ALTER TABLE [dbo].[sample_matrix_group] ADD [updated_at] DATETIME2 NULL;
    PRINT N'[sample_matrix_group] Đã thêm updated_at.';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.sample_matrix_group') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[sample_matrix_group] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    PRINT N'[sample_matrix_group] Đã thêm updated_by.';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_sample_matrix_group_updated_by')
BEGIN
    ALTER TABLE [dbo].[sample_matrix_group] ADD CONSTRAINT [FK_sample_matrix_group_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'[sample_matrix_group] Đã tạo FK.';
END
GO

-- ---------- sample_matrix ----------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.sample_matrix') AND name = N'updated_at')
BEGIN
    ALTER TABLE [dbo].[sample_matrix] ADD [updated_at] DATETIME2 NULL;
    PRINT N'[sample_matrix] Đã thêm updated_at.';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.sample_matrix') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[sample_matrix] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    PRINT N'[sample_matrix] Đã thêm updated_by.';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_sample_matrix_updated_by')
BEGIN
    ALTER TABLE [dbo].[sample_matrix] ADD CONSTRAINT [FK_sample_matrix_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'[sample_matrix] Đã tạo FK.';
END
GO

-- ---------- analysis_group ----------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.analysis_group') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[analysis_group] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[analysis_group] ADD CONSTRAINT [FK_analysis_group_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'[analysis_group] Đã thêm updated_by + FK.';
END
GO

-- ---------- analysis_item ----------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.analysis_item') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[analysis_item] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[analysis_item] ADD CONSTRAINT [FK_analysis_item_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'[analysis_item] Đã thêm updated_by + FK.';
END
GO

-- ---------- department_analysis_capability ----------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.department_analysis_capability') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[department_analysis_capability] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[department_analysis_capability] ADD CONSTRAINT [FK_department_analysis_capability_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'[department_analysis_capability] Đã thêm updated_by + FK.';
END
GO

-- ---------- quotation_item ----------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.quotation_item') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[quotation_item] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[quotation_item] ADD CONSTRAINT [FK_quotation_item_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'[quotation_item] Đã thêm updated_by + FK.';
END
GO

-- ---------- quotation_analysis_group ----------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.quotation_analysis_group') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[quotation_analysis_group] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[quotation_analysis_group] ADD CONSTRAINT [FK_quotation_analysis_group_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'[quotation_analysis_group] Đã thêm updated_by + FK.';
END
GO

-- ---------- package ----------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.package') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[package] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[package] ADD CONSTRAINT [FK_package_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'[package] Đã thêm updated_by + FK.';
END
GO

-- ---------- package_analysis_group ----------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.package_analysis_group') AND name = N'updated_at')
BEGIN
    ALTER TABLE [dbo].[package_analysis_group] ADD [updated_at] DATETIME2 NULL;
    PRINT N'[package_analysis_group] Đã thêm updated_at.';
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.package_analysis_group') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[package_analysis_group] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[package_analysis_group] ADD CONSTRAINT [FK_package_analysis_group_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'[package_analysis_group] Đã thêm updated_by + FK.';
END
GO

-- ---------- client_debt ----------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.client_debt') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[client_debt] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[client_debt] ADD CONSTRAINT [FK_client_debt_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'[client_debt] Đã thêm updated_by + FK.';
END
GO

-- ---------- store_record ----------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.store_record') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[store_record] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[store_record] ADD CONSTRAINT [FK_store_record_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'[store_record] Đã thêm updated_by + FK.';
END
GO

-- ---------- analysis_item_tat ----------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.analysis_item_tat') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[analysis_item_tat] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[analysis_item_tat] ADD CONSTRAINT [FK_analysis_item_tat_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'[analysis_item_tat] Đã thêm updated_by + FK.';
END
GO

-- ---------- subcontractor ----------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.subcontractor') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[subcontractor] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[subcontractor] ADD CONSTRAINT [FK_subcontractor_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'[subcontractor] Đã thêm updated_by + FK.';
END
GO

-- ---------- subcontractor_capability ----------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.subcontractor_capability') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[subcontractor_capability] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[subcontractor_capability] ADD CONSTRAINT [FK_subcontractor_capability_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'[subcontractor_capability] Đã thêm updated_by + FK.';
END
GO

-- ========== Gán giá trị mặc định cho bản ghi hiện có (updated_by = NULL) ==========
DECLARE @AccountId UNIQUEIDENTIFIER = '94eab415-1624-49de-85a6-a80916db3ab2';

UPDATE [dbo].[client] SET [updated_by] = @AccountId, [updated_at] = SYSUTCDATETIME() WHERE [updated_by] IS NULL;
UPDATE [dbo].[employee] SET [updated_by] = @AccountId, [updated_at] = SYSUTCDATETIME() WHERE [updated_by] IS NULL OR [updated_at] IS NULL;
UPDATE [dbo].[branch] SET [updated_by] = @AccountId, [updated_at] = SYSUTCDATETIME() WHERE [updated_by] IS NULL OR [updated_at] IS NULL;
UPDATE [dbo].[department] SET [updated_by] = @AccountId, [updated_at] = SYSUTCDATETIME() WHERE [updated_by] IS NULL OR [updated_at] IS NULL;
UPDATE [dbo].[sample_matrix_group] SET [updated_by] = @AccountId, [updated_at] = SYSUTCDATETIME() WHERE [updated_by] IS NULL OR [updated_at] IS NULL;
UPDATE [dbo].[sample_matrix] SET [updated_by] = @AccountId, [updated_at] = SYSUTCDATETIME() WHERE [updated_by] IS NULL OR [updated_at] IS NULL;
UPDATE [dbo].[analysis_item] SET [updated_by] = @AccountId, [updated_at] = SYSUTCDATETIME() WHERE [updated_by] IS NULL;

PRINT N'Hoàn tất: Đã thêm updated_at/updated_by và gán account mặc định cho các bảng.';
GO
