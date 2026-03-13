-- =============================================
-- Thêm các cột thông tin nhà thầu phụ: MST, Ngân hàng, Hợp đồng, Chu kỳ thanh toán
-- =============================================

USE [VietLabs]
GO

-- Mã số thuế
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.subcontractor') AND name = N'tax_code')
BEGIN
    ALTER TABLE [dbo].[subcontractor] ADD [tax_code] NVARCHAR(50) NULL;
    PRINT N'Đã thêm cột tax_code.';
END
ELSE
    PRINT N'Cột tax_code đã tồn tại.';
GO

-- Số tài khoản ngân hàng
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.subcontractor') AND name = N'bank_account_number')
BEGIN
    ALTER TABLE [dbo].[subcontractor] ADD [bank_account_number] NVARCHAR(100) NULL;
    PRINT N'Đã thêm cột bank_account_number.';
END
ELSE
    PRINT N'Cột bank_account_number đã tồn tại.';
GO

-- Tên người nhận (tài khoản)
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.subcontractor') AND name = N'bank_account_holder')
BEGIN
    ALTER TABLE [dbo].[subcontractor] ADD [bank_account_holder] NVARCHAR(200) NULL;
    PRINT N'Đã thêm cột bank_account_holder.';
END
ELSE
    PRINT N'Cột bank_account_holder đã tồn tại.';
GO

-- Tên ngân hàng
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.subcontractor') AND name = N'bank_name')
BEGIN
    ALTER TABLE [dbo].[subcontractor] ADD [bank_name] NVARCHAR(200) NULL;
    PRINT N'Đã thêm cột bank_name.';
END
ELSE
    PRINT N'Cột bank_name đã tồn tại.';
GO

-- Hợp đồng: Yes, No, Overdue
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.subcontractor') AND name = N'contract_status')
BEGIN
    ALTER TABLE [dbo].[subcontractor] ADD [contract_status] NVARCHAR(50) NULL;
    PRINT N'Đã thêm cột contract_status.';
END
ELSE
    PRINT N'Cột contract_status đã tồn tại.';
GO

-- Chu kỳ thanh toán: BeforeAnalysis, BeforeReceivingResult, AfterInvoice
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.subcontractor') AND name = N'payment_cycle')
BEGIN
    ALTER TABLE [dbo].[subcontractor] ADD [payment_cycle] NVARCHAR(50) NULL;
    PRINT N'Đã thêm cột payment_cycle.';
END
ELSE
    PRINT N'Cột payment_cycle đã tồn tại.';
GO

-- Số ngày thanh toán (khi chu kỳ = Sau khi nhận hóa đơn)
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.subcontractor') AND name = N'payment_days')
BEGIN
    ALTER TABLE [dbo].[subcontractor] ADD [payment_days] INT NULL;
    PRINT N'Đã thêm cột payment_days.';
END
ELSE
    PRINT N'Cột payment_days đã tồn tại.';
GO
