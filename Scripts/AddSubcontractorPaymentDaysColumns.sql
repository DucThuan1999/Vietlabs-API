-- =============================================
-- Chu kỳ thanh toán: 3 cột số ngày (thay cho payment_cycle + payment_days)
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.subcontractor') AND name = N'payment_days_before_analysis')
BEGIN
    ALTER TABLE [dbo].[subcontractor] ADD [payment_days_before_analysis] INT NULL;
    PRINT N'Đã thêm cột payment_days_before_analysis.';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.subcontractor') AND name = N'payment_days_before_receiving_result')
BEGIN
    ALTER TABLE [dbo].[subcontractor] ADD [payment_days_before_receiving_result] INT NULL;
    PRINT N'Đã thêm cột payment_days_before_receiving_result.';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.subcontractor') AND name = N'payment_days_after_invoice')
BEGIN
    ALTER TABLE [dbo].[subcontractor] ADD [payment_days_after_invoice] INT NULL;
    PRINT N'Đã thêm cột payment_days_after_invoice.';
END
GO
