-- =============================================
-- Script SQL: Đổi tên các bảng từ số nhiều sang số ít
-- Database: VietLabs
-- Giai đoạn: Development
-- =============================================

USE VietLabs;
GO

-- =============================================
-- LƯU Ý: Script này sẽ đổi tên bảng và tự động cập nhật
-- tất cả foreign keys, indexes, constraints liên quan
-- =============================================

-- =============================================
-- 1. ĐỔI TÊN BẢNG CLIENTS → CLIENT
-- =============================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'clients' AND name != 'client')
BEGIN
    EXEC sp_rename 'dbo.clients', 'client';
    PRINT 'Đã đổi tên bảng [clients] → [client]';
END
ELSE IF EXISTS (SELECT * FROM sys.tables WHERE name = 'client')
BEGIN
    PRINT 'Bảng [client] đã tồn tại, bỏ qua.';
END
GO

-- =============================================
-- 2. ĐỔI TÊN BẢNG CONTACTS → CONTACT
-- =============================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'contacts' AND name != 'contact')
BEGIN
    EXEC sp_rename 'dbo.contacts', 'contact';
    PRINT 'Đã đổi tên bảng [contacts] → [contact]';
END
ELSE IF EXISTS (SELECT * FROM sys.tables WHERE name = 'contact')
BEGIN
    PRINT 'Bảng [contact] đã tồn tại, bỏ qua.';
END
GO

-- =============================================
-- 3. ĐỔI TÊN BẢNG EMPLOYEES → EMPLOYEE
-- =============================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'employees' AND name != 'employee')
BEGIN
    EXEC sp_rename 'dbo.employees', 'employee';
    PRINT 'Đã đổi tên bảng [employees] → [employee]';
END
ELSE IF EXISTS (SELECT * FROM sys.tables WHERE name = 'employee')
BEGIN
    PRINT 'Bảng [employee] đã tồn tại, bỏ qua.';
END
GO

-- =============================================
-- 4. ĐỔI TÊN BẢNG BRANCHES → BRANCH
-- =============================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'branches' AND name != 'branch')
BEGIN
    EXEC sp_rename 'dbo.branches', 'branch';
    PRINT 'Đã đổi tên bảng [branches] → [branch]';
END
ELSE IF EXISTS (SELECT * FROM sys.tables WHERE name = 'branch')
BEGIN
    PRINT 'Bảng [branch] đã tồn tại, bỏ qua.';
END
GO

-- =============================================
-- 5. ĐỔI TÊN BẢNG DEPARTMENTS → DEPARTMENT
-- =============================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'departments' AND name != 'department')
BEGIN
    EXEC sp_rename 'dbo.departments', 'department';
    PRINT 'Đã đổi tên bảng [departments] → [department]';
END
ELSE IF EXISTS (SELECT * FROM sys.tables WHERE name = 'department')
BEGIN
    PRINT 'Bảng [department] đã tồn tại, bỏ qua.';
END
GO

-- =============================================
-- 6. ĐỔI TÊN BẢNG ACCOUNTS → ACCOUNT
-- =============================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'accounts' AND name != 'account')
BEGIN
    EXEC sp_rename 'dbo.accounts', 'account';
    PRINT 'Đã đổi tên bảng [accounts] → [account]';
END
ELSE IF EXISTS (SELECT * FROM sys.tables WHERE name = 'account')
BEGIN
    PRINT 'Bảng [account] đã tồn tại, bỏ qua.';
END
GO

-- =============================================
-- 7. ĐỔI TÊN BẢNG PERMISSIONS → PERMISSION
-- =============================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'permissions' AND name != 'permission')
BEGIN
    EXEC sp_rename 'dbo.permissions', 'permission';
    PRINT 'Đã đổi tên bảng [permissions] → [permission]';
END
ELSE IF EXISTS (SELECT * FROM sys.tables WHERE name = 'permission')
BEGIN
    PRINT 'Bảng [permission] đã tồn tại, bỏ qua.';
END
GO

-- =============================================
-- 8. ĐỔI TÊN BẢNG REFRESH_TOKENS → REFRESH_TOKEN
-- =============================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'refresh_tokens' AND name != 'refresh_token')
BEGIN
    EXEC sp_rename 'dbo.refresh_tokens', 'refresh_token';
    PRINT 'Đã đổi tên bảng [refresh_tokens] → [refresh_token]';
END
ELSE IF EXISTS (SELECT * FROM sys.tables WHERE name = 'refresh_token')
BEGIN
    PRINT 'Bảng [refresh_token] đã tồn tại, bỏ qua.';
END
GO

-- =============================================
-- 9. ĐỔI TÊN BẢNG QUOTATIONS → QUOTATION
-- =============================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'quotations' AND name != 'quotation')
BEGIN
    EXEC sp_rename 'dbo.quotations', 'quotation';
    PRINT 'Đã đổi tên bảng [quotations] → [quotation]';
END
ELSE IF EXISTS (SELECT * FROM sys.tables WHERE name = 'quotation')
BEGIN
    PRINT 'Bảng [quotation] đã tồn tại, bỏ qua.';
END
GO

-- =============================================
-- 10. ĐỔI TÊN BẢNG CLIENT_DEBTS → CLIENT_DEBT
-- =============================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'client_debts' AND name != 'client_debt')
BEGIN
    EXEC sp_rename 'dbo.client_debts', 'client_debt';
    PRINT 'Đã đổi tên bảng [client_debts] → [client_debt]';
END
ELSE IF EXISTS (SELECT * FROM sys.tables WHERE name = 'client_debt')
BEGIN
    PRINT 'Bảng [client_debt] đã tồn tại, bỏ qua.';
END
GO

-- =============================================
-- 11. ĐỔI TÊN BẢNG CLIENT_FORECASTS → CLIENT_FORECAST
-- =============================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'client_forecasts' AND name != 'client_forecast')
BEGIN
    EXEC sp_rename 'dbo.client_forecasts', 'client_forecast';
    PRINT 'Đã đổi tên bảng [client_forecasts] → [client_forecast]';
END
ELSE IF EXISTS (SELECT * FROM sys.tables WHERE name = 'client_forecast')
BEGIN
    PRINT 'Bảng [client_forecast] đã tồn tại, bỏ qua.';
END
GO

-- =============================================
-- LƯU Ý QUAN TRỌNG:
-- SQL Server sẽ tự động cập nhật:
-- - Foreign key constraints (tên constraint giữ nguyên)
-- - Indexes (tên index giữ nguyên)
-- - Check constraints (tên constraint giữ nguyên)
-- 
-- NHƯNG cần kiểm tra và cập nhật thủ công:
-- - Stored procedures
-- - Views
-- - Functions
-- - Triggers
-- - Application code references
-- =============================================

PRINT '';
PRINT '========================================';
PRINT 'Đã hoàn tất đổi tên các bảng sang số ít!';
PRINT '========================================';
PRINT '';
PRINT 'Các bảng đã được đổi tên:';
PRINT '  - clients → client';
PRINT '  - contacts → contact';
PRINT '  - employees → employee';
PRINT '  - branches → branch';
PRINT '  - departments → department';
PRINT '  - accounts → account';
PRINT '  - permissions → permission';
PRINT '  - refresh_tokens → refresh_token';
PRINT '  - quotations → quotation';
PRINT '  - client_debts → client_debt';
PRINT '  - client_forecasts → client_forecast';
PRINT '';
PRINT 'LƯU Ý: Cần cập nhật ApplicationDbContext và Configuration classes!';
PRINT '';

