-- =============================================
-- Thêm cột updated_by vào client_debt
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.client_debt') AND name = N'updated_by')
BEGIN
    ALTER TABLE [dbo].[client_debt] ADD [updated_by] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[client_debt] ADD CONSTRAINT [FK_client_debt_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;
    PRINT N'Đã thêm cột updated_by và FK vào client_debt.';
END
ELSE
    PRINT N'Cột updated_by đã tồn tại trong client_debt.';
GO
