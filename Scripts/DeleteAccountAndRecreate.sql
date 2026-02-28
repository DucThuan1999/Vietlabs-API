-- =============================================
-- Xóa toàn bộ dữ liệu account (và refresh_token do CASCADE)
-- Sau đó chạy import_employee.py để tạo lại account (username = email, quyền PERM-USER)
-- =============================================

USE [VietLabs]
GO

-- 1. Xóa bảng history (changed_by_account_id NOT NULL, không thể set NULL)
DELETE FROM [dbo].[quotation_history];
DELETE FROM [dbo].[client_history];
GO

-- 2. Set NULL các cột created_by / updated_by (cho phép NULL)
UPDATE [dbo].[client_forecast]  SET [created_by] = NULL, [updated_by] = NULL WHERE [created_by] IS NOT NULL OR [updated_by] IS NOT NULL;
UPDATE [dbo].[quotation]       SET [created_by] = NULL, [updated_by] = NULL WHERE [created_by] IS NOT NULL OR [updated_by] IS NOT NULL;
UPDATE [dbo].[client_industry] SET [created_by] = NULL, [updated_by] = NULL WHERE [created_by] IS NOT NULL OR [updated_by] IS NOT NULL;
UPDATE [dbo].[employee_title]  SET [created_by] = NULL, [updated_by] = NULL WHERE [created_by] IS NOT NULL OR [updated_by] IS NOT NULL;

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[module_approver]') AND type in (N'U'))
BEGIN
    UPDATE [dbo].[module_approver] SET [created_by] = NULL, [updated_by] = NULL WHERE [created_by] IS NOT NULL OR [updated_by] IS NOT NULL;
END
GO

-- 3. Xóa refresh_token rồi xóa account
DELETE FROM [dbo].[refresh_token];
DELETE FROM [dbo].[account];
GO

PRINT 'Đã xóa: quotation_history, client_history, refresh_token, account.';
PRINT 'Chạy: python csv\data_import\import_employee.py để tạo lại account (username = email, quyền PERM-USER).';
