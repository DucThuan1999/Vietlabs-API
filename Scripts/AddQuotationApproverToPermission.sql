-- =============================================
-- Script thêm cột quotation_approver_level2_id vào bảng permission
-- Để lưu người phê duyệt cấp 2 cho module báo giá
-- Cấp 1 là manager của employee (đã có sẵn trong employee.manager_id)
-- =============================================

USE [VietLabs] -- Thay đổi tên database nếu cần
GO

-- =============================================
-- Thêm cột quotation_approver_level2_id vào bảng permission
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[permission]') AND name = 'quotation_approver_level2_id')
BEGIN
    ALTER TABLE [dbo].[permission]
    ADD [quotation_approver_level2_id] UNIQUEIDENTIFIER NULL;
    
    PRINT 'Cột quotation_approver_level2_id đã được thêm vào bảng permission.';
END
ELSE
BEGIN
    PRINT 'Cột quotation_approver_level2_id đã tồn tại trong bảng permission.';
END
GO

-- =============================================
-- Thêm Foreign Key Constraint
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_permission_quotation_approver_level2')
BEGIN
    ALTER TABLE [dbo].[permission]
    ADD CONSTRAINT [FK_permission_quotation_approver_level2] 
    FOREIGN KEY ([quotation_approver_level2_id]) 
    REFERENCES [dbo].[employee] ([employee_id])
    ON DELETE SET NULL
    ON UPDATE NO ACTION;
    
    PRINT 'Foreign key FK_permission_quotation_approver_level2 đã được thêm.';
END
ELSE
BEGIN
    PRINT 'Foreign key FK_permission_quotation_approver_level2 đã tồn tại.';
END
GO

PRINT 'Hoàn thành: Đã thêm cột quotation_approver_level2_id vào bảng permission.';
GO

