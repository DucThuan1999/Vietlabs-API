/*
  Ma trận quyền (module × hành động) — SQL Server
  - Mục 8 dùng cột account.permission_id: CHỈ chạy khi DB vẫn còn permission_id.
  - Sau khi migrate xong, chạy Scripts/RemoveAccountPermissionId.sql (hoặc migration EF RemoveAccountPermissionFromAccount) để xóa permission_id.

  Thứ tự khuyến nghị:
  1) PermissionMatrix_Full.sql (đủ mục 1–8 nếu còn permission_id)
  2) RemoveAccountPermissionId.sql
*/

SET NOCOUNT ON;

/* ========== 1. Bảng security_module ========== */
IF OBJECT_ID(N'dbo.security_module', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.security_module (
        security_module_id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_security_module PRIMARY KEY,
        code             NVARCHAR(450) NOT NULL,
        name_vi          NVARCHAR(MAX) NOT NULL,
        sort_order       INT NOT NULL,
        status           NVARCHAR(MAX) NOT NULL
    );
    CREATE UNIQUE NONCLUSTERED INDEX IX_security_module_code ON dbo.security_module(code);
END
GO

/* ========== 2. Bảng matrix_action ========== */
IF OBJECT_ID(N'dbo.matrix_action', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.matrix_action (
        matrix_action_id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_matrix_action PRIMARY KEY,
        code             NVARCHAR(450) NOT NULL,
        name_vi          NVARCHAR(MAX) NOT NULL,
        sort_order       INT NOT NULL
    );
    CREATE UNIQUE NONCLUSTERED INDEX IX_matrix_action_code ON dbo.matrix_action(code);
END
GO

/* ========== 3. Bảng security_module_matrix_action ========== */
IF OBJECT_ID(N'dbo.security_module_matrix_action', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.security_module_matrix_action (
        security_module_id UNIQUEIDENTIFIER NOT NULL,
        matrix_action_id   UNIQUEIDENTIFIER NOT NULL,
        CONSTRAINT PK_security_module_matrix_action PRIMARY KEY (security_module_id, matrix_action_id),
        CONSTRAINT FK_smma_security_module FOREIGN KEY (security_module_id)
            REFERENCES dbo.security_module(security_module_id) ON DELETE CASCADE,
        CONSTRAINT FK_smma_matrix_action FOREIGN KEY (matrix_action_id)
            REFERENCES dbo.matrix_action(matrix_action_id) ON DELETE CASCADE
    );
END
GO

/* ========== 4. Bảng account_module_grant ========== */
IF OBJECT_ID(N'dbo.account_module_grant', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.account_module_grant (
        account_module_grant_id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_account_module_grant PRIMARY KEY,
        account_id              UNIQUEIDENTIFIER NOT NULL,
        security_module_id      UNIQUEIDENTIFIER NOT NULL,
        matrix_action_id        UNIQUEIDENTIFIER NOT NULL,
        CONSTRAINT FK_amg_account FOREIGN KEY (account_id)
            REFERENCES dbo.account(account_id) ON DELETE CASCADE,
        CONSTRAINT FK_amg_security_module FOREIGN KEY (security_module_id)
            REFERENCES dbo.security_module(security_module_id),
        CONSTRAINT FK_amg_matrix_action FOREIGN KEY (matrix_action_id)
            REFERENCES dbo.matrix_action(matrix_action_id)
    );
    CREATE UNIQUE NONCLUSTERED INDEX IX_account_module_grant_account_module_action
        ON dbo.account_module_grant(account_id, security_module_id, matrix_action_id);
END
GO

/* ========== GUID cố định (khớp migration EF) ========== */
DECLARE
    @ModAdmin     UNIQUEIDENTIFIER = 'f1111111-1111-1111-1111-111111111101',
    @ModQuotation UNIQUEIDENTIFIER = 'f1111111-1111-1111-1111-111111111102',
    @ModCustomer  UNIQUEIDENTIFIER = 'f1111111-1111-1111-1111-111111111103',
    @ActView      UNIQUEIDENTIFIER = 'e1111111-1111-1111-1111-111111111101',
    @ActCreate    UNIQUEIDENTIFIER = 'e1111111-1111-1111-1111-111111111102',
    @ActEdit      UNIQUEIDENTIFIER = 'e1111111-1111-1111-1111-111111111103',
    @ActDelete    UNIQUEIDENTIFIER = 'e1111111-1111-1111-1111-111111111104',
    @ActApprove   UNIQUEIDENTIFIER = 'e1111111-1111-1111-1111-111111111105',
    @ActExport    UNIQUEIDENTIFIER = 'e1111111-1111-1111-1111-111111111106',
    @PermAdmin    UNIQUEIDENTIFIER = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
    @PermUser     UNIQUEIDENTIFIER = 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
    @PermManager  UNIQUEIDENTIFIER = 'cccccccc-cccc-cccc-cccc-cccccccccccc',
    @PermSales    UNIQUEIDENTIFIER = 'dddddddd-dddd-dddd-dddd-dddddddddddd';

/* ========== 5. Seed security_module ========== */
IF NOT EXISTS (SELECT 1 FROM dbo.security_module WHERE security_module_id = @ModAdmin)
    INSERT INTO dbo.security_module (security_module_id, code, name_vi, sort_order, status)
    VALUES (@ModAdmin, N'Admin', N'Quản trị / Cài đặt', 1, N'Active');
IF NOT EXISTS (SELECT 1 FROM dbo.security_module WHERE security_module_id = @ModQuotation)
    INSERT INTO dbo.security_module (security_module_id, code, name_vi, sort_order, status)
    VALUES (@ModQuotation, N'Quotation', N'Báo giá', 2, N'Active');
IF NOT EXISTS (SELECT 1 FROM dbo.security_module WHERE security_module_id = @ModCustomer)
    INSERT INTO dbo.security_module (security_module_id, code, name_vi, sort_order, status)
    VALUES (@ModCustomer, N'Customer', N'Khách hàng', 3, N'Active');

/* ========== 6. Seed matrix_action ========== */
IF NOT EXISTS (SELECT 1 FROM dbo.matrix_action WHERE matrix_action_id = @ActView)
    INSERT INTO dbo.matrix_action (matrix_action_id, code, name_vi, sort_order) VALUES (@ActView, N'View', N'Xem', 1);
IF NOT EXISTS (SELECT 1 FROM dbo.matrix_action WHERE matrix_action_id = @ActCreate)
    INSERT INTO dbo.matrix_action (matrix_action_id, code, name_vi, sort_order) VALUES (@ActCreate, N'Create', N'Tạo mới', 2);
IF NOT EXISTS (SELECT 1 FROM dbo.matrix_action WHERE matrix_action_id = @ActEdit)
    INSERT INTO dbo.matrix_action (matrix_action_id, code, name_vi, sort_order) VALUES (@ActEdit, N'Edit', N'Sửa', 3);
IF NOT EXISTS (SELECT 1 FROM dbo.matrix_action WHERE matrix_action_id = @ActDelete)
    INSERT INTO dbo.matrix_action (matrix_action_id, code, name_vi, sort_order) VALUES (@ActDelete, N'Delete', N'Xóa', 4);
IF NOT EXISTS (SELECT 1 FROM dbo.matrix_action WHERE matrix_action_id = @ActApprove)
    INSERT INTO dbo.matrix_action (matrix_action_id, code, name_vi, sort_order) VALUES (@ActApprove, N'Approve', N'Phê duyệt', 5);
IF NOT EXISTS (SELECT 1 FROM dbo.matrix_action WHERE matrix_action_id = @ActExport)
    INSERT INTO dbo.matrix_action (matrix_action_id, code, name_vi, sort_order) VALUES (@ActExport, N'Export', N'Xuất', 6);

/* ========== 7. Seed ô ma trận (Admin + Quotation + Customer) ========== */
INSERT INTO dbo.security_module_matrix_action (security_module_id, matrix_action_id)
SELECT v.security_module_id, v.matrix_action_id
FROM (VALUES
    (@ModAdmin, @ActView), (@ModAdmin, @ActCreate), (@ModAdmin, @ActEdit), (@ModAdmin, @ActDelete), (@ModAdmin, @ActApprove), (@ModAdmin, @ActExport),
    (@ModQuotation, @ActView), (@ModQuotation, @ActCreate), (@ModQuotation, @ActEdit), (@ModQuotation, @ActDelete), (@ModQuotation, @ActApprove), (@ModQuotation, @ActExport),
    (@ModCustomer, @ActView), (@ModCustomer, @ActCreate), (@ModCustomer, @ActEdit), (@ModCustomer, @ActDelete), (@ModCustomer, @ActApprove), (@ModCustomer, @ActExport)
) AS v(security_module_id, matrix_action_id)
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.security_module_matrix_action x
    WHERE x.security_module_id = v.security_module_id AND x.matrix_action_id = v.matrix_action_id
);

/* ========== 8. Migrate quyền account (chỉ dòng chưa tồn tại) ========== */

-- PERM-ADMIN: tất cả ô
INSERT INTO dbo.account_module_grant (account_module_grant_id, account_id, security_module_id, matrix_action_id)
SELECT NEWID(), a.account_id, sma.security_module_id, sma.matrix_action_id
FROM dbo.account a
CROSS JOIN dbo.security_module_matrix_action sma
WHERE a.permission_id = @PermAdmin
  AND NOT EXISTS (
      SELECT 1 FROM dbo.account_module_grant g
      WHERE g.account_id = a.account_id
        AND g.security_module_id = sma.security_module_id
        AND g.matrix_action_id = sma.matrix_action_id
    );

-- PERM-USER: View trên mọi module
INSERT INTO dbo.account_module_grant (account_module_grant_id, account_id, security_module_id, matrix_action_id)
SELECT NEWID(), a.account_id, m.security_module_id, @ActView
FROM dbo.account a
CROSS JOIN dbo.security_module m
WHERE a.permission_id = @PermUser
  AND NOT EXISTS (
      SELECT 1 FROM dbo.account_module_grant g
      WHERE g.account_id = a.account_id
        AND g.security_module_id = m.security_module_id
        AND g.matrix_action_id = @ActView
    );

-- PERM-MANAGER: đủ ô trừ Admin + Delete
INSERT INTO dbo.account_module_grant (account_module_grant_id, account_id, security_module_id, matrix_action_id)
SELECT NEWID(), a.account_id, sma.security_module_id, sma.matrix_action_id
FROM dbo.account a
CROSS JOIN dbo.security_module_matrix_action sma
WHERE a.permission_id = @PermManager
  AND NOT (sma.security_module_id = @ModAdmin AND sma.matrix_action_id = @ActDelete)
  AND NOT EXISTS (
      SELECT 1 FROM dbo.account_module_grant g
      WHERE g.account_id = a.account_id
        AND g.security_module_id = sma.security_module_id
        AND g.matrix_action_id = sma.matrix_action_id
    );

-- PERM-SALES: Quotation + Customer (cùng bộ quyền) + Admin View
INSERT INTO dbo.account_module_grant (account_module_grant_id, account_id, security_module_id, matrix_action_id)
SELECT NEWID(), a.account_id, sma.security_module_id, sma.matrix_action_id
FROM dbo.account a
CROSS JOIN dbo.security_module_matrix_action sma
WHERE a.permission_id = @PermSales
  AND (
      sma.security_module_id = @ModQuotation
      OR sma.security_module_id = @ModCustomer
      OR (sma.security_module_id = @ModAdmin AND sma.matrix_action_id = @ActView)
    )
  AND NOT EXISTS (
      SELECT 1 FROM dbo.account_module_grant g
      WHERE g.account_id = a.account_id
        AND g.security_module_id = sma.security_module_id
        AND g.matrix_action_id = sma.matrix_action_id
    );

PRINT N'PermissionMatrix_Full.sql hoàn tất.';
GO

/*
  (Tùy chọn) Ghi nhận migration EF nếu bạn chỉ chạy SQL, không dùng dotnet ef database update:
  Thay MigrationId nếu đã có bản ghi khác.

INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion)
SELECT N'20260319000000_PermissionMatrix', N'8.0.0'
WHERE NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = N'20260319000000_PermissionMatrix');
*/
