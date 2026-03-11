-- =============================================
-- Bảng năng lực nhân viên (employee_analysis_capability)
-- Gắn nhân viên với chỉ tiêu phân tích mà nhân viên được chỉ định thực hiện
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'employee_analysis_capability')
BEGIN
    CREATE TABLE [dbo].[employee_analysis_capability] (
        [employee_analysis_capability_id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [employee_id]                     UNIQUEIDENTIFIER NOT NULL,
        [analysis_item_id]                UNIQUEIDENTIFIER NOT NULL,
        [status]                          NVARCHAR(50)    NULL,
        [notes]                           NVARCHAR(MAX)   NULL,
        [created_at]                      DATETIME2(7)    NULL,
        [updated_at]                      DATETIME2(7)    NULL,
        [updated_by]                      UNIQUEIDENTIFIER NULL,
        CONSTRAINT [PK_employee_analysis_capability] PRIMARY KEY ([employee_analysis_capability_id]),
        CONSTRAINT [FK_employee_analysis_capability_employee]
            FOREIGN KEY ([employee_id])
            REFERENCES [dbo].[employee] ([employee_id]) ON DELETE CASCADE,
        CONSTRAINT [FK_employee_analysis_capability_analysis_item]
            FOREIGN KEY ([analysis_item_id])
            REFERENCES [dbo].[analysis_item] ([analysis_item_id]) ON DELETE CASCADE,
        CONSTRAINT [FK_employee_analysis_capability_updated_by]
            FOREIGN KEY ([updated_by])
            REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION,
        CONSTRAINT [UQ_employee_analysis_capability_employee_item]
            UNIQUE ([employee_id], [analysis_item_id])
    );

    PRINT N'Đã tạo bảng employee_analysis_capability.';
END
ELSE
    PRINT N'Bảng employee_analysis_capability đã tồn tại.';
GO
