/*
  Thêm module Khách hàng (Customer) — chạy khi DB đã có ma trận (Admin + Quotation) nhưng chưa có Customer.

  - Module Customer + 6 ô ma trận.
  - Copy mọi quyền đang có trên module Quotation sang Customer (cùng hành động).
  - Ai chỉ có Admin.View (không có Quotation) được thêm Customer.View.
*/
SET NOCOUNT ON;

DECLARE
    @ModCustomer UNIQUEIDENTIFIER = 'f1111111-1111-1111-1111-111111111103',
    @ModQuotation UNIQUEIDENTIFIER = 'f1111111-1111-1111-1111-111111111102',
    @ModAdmin     UNIQUEIDENTIFIER = 'f1111111-1111-1111-1111-111111111101',
    @ActView      UNIQUEIDENTIFIER = 'e1111111-1111-1111-1111-111111111101',
    @ActCreate    UNIQUEIDENTIFIER = 'e1111111-1111-1111-1111-111111111102',
    @ActEdit      UNIQUEIDENTIFIER = 'e1111111-1111-1111-1111-111111111103',
    @ActDelete    UNIQUEIDENTIFIER = 'e1111111-1111-1111-1111-111111111104',
    @ActApprove   UNIQUEIDENTIFIER = 'e1111111-1111-1111-1111-111111111105',
    @ActExport    UNIQUEIDENTIFIER = 'e1111111-1111-1111-1111-111111111106';

IF NOT EXISTS (SELECT 1 FROM dbo.security_module WHERE security_module_id = @ModCustomer)
    INSERT INTO dbo.security_module (security_module_id, code, name_vi, sort_order, status)
    VALUES (@ModCustomer, N'Customer', N'Khách hàng', 3, N'Active');

INSERT INTO dbo.security_module_matrix_action (security_module_id, matrix_action_id)
SELECT v.security_module_id, v.matrix_action_id
FROM (VALUES
    (@ModCustomer, @ActView), (@ModCustomer, @ActCreate), (@ModCustomer, @ActEdit),
    (@ModCustomer, @ActDelete), (@ModCustomer, @ActApprove), (@ModCustomer, @ActExport)
) AS v(security_module_id, matrix_action_id)
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.security_module_matrix_action x
    WHERE x.security_module_id = v.security_module_id AND x.matrix_action_id = v.matrix_action_id
);

INSERT INTO dbo.account_module_grant (account_module_grant_id, account_id, security_module_id, matrix_action_id)
SELECT NEWID(), g.account_id, @ModCustomer, g.matrix_action_id
FROM dbo.account_module_grant g
WHERE g.security_module_id = @ModQuotation
  AND NOT EXISTS (
      SELECT 1 FROM dbo.account_module_grant g2
      WHERE g2.account_id = g.account_id AND g2.security_module_id = @ModCustomer AND g2.matrix_action_id = g.matrix_action_id
    );

INSERT INTO dbo.account_module_grant (account_module_grant_id, account_id, security_module_id, matrix_action_id)
SELECT NEWID(), g.account_id, @ModCustomer, @ActView
FROM dbo.account_module_grant g
WHERE g.security_module_id = @ModAdmin AND g.matrix_action_id = @ActView
  AND NOT EXISTS (
      SELECT 1 FROM dbo.account_module_grant g2
      WHERE g2.account_id = g.account_id AND g2.security_module_id = @ModCustomer AND g2.matrix_action_id = @ActView
    );

PRINT N'AddCustomerSecurityModule.sql hoàn tất.';
GO
