using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <summary>Module ma trận Khách hàng (Customer) + backfill quyền (mirror Quotation + Admin.View).</summary>
    public partial class AddCustomerSecurityModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
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

IF NOT EXISTS (SELECT 1 FROM security_module WHERE security_module_id = @ModCustomer)
    INSERT INTO security_module (security_module_id, code, name_vi, sort_order, status)
    VALUES (@ModCustomer, N'Customer', N'Khách hàng', 3, N'Active');

INSERT INTO security_module_matrix_action (security_module_id, matrix_action_id)
SELECT v.security_module_id, v.matrix_action_id
FROM (VALUES
    (@ModCustomer, @ActView), (@ModCustomer, @ActCreate), (@ModCustomer, @ActEdit),
    (@ModCustomer, @ActDelete), (@ModCustomer, @ActApprove), (@ModCustomer, @ActExport)
) AS v(security_module_id, matrix_action_id)
WHERE NOT EXISTS (
    SELECT 1 FROM security_module_matrix_action x
    WHERE x.security_module_id = v.security_module_id AND x.matrix_action_id = v.matrix_action_id
);

INSERT INTO account_module_grant (account_module_grant_id, account_id, security_module_id, matrix_action_id)
SELECT NEWID(), g.account_id, @ModCustomer, g.matrix_action_id
FROM account_module_grant g
WHERE g.security_module_id = @ModQuotation
  AND NOT EXISTS (
      SELECT 1 FROM account_module_grant g2
      WHERE g2.account_id = g.account_id AND g2.security_module_id = @ModCustomer AND g2.matrix_action_id = g.matrix_action_id
    );

INSERT INTO account_module_grant (account_module_grant_id, account_id, security_module_id, matrix_action_id)
SELECT NEWID(), g.account_id, @ModCustomer, @ActView
FROM account_module_grant g
WHERE g.security_module_id = @ModAdmin AND g.matrix_action_id = @ActView
  AND NOT EXISTS (
      SELECT 1 FROM account_module_grant g2
      WHERE g2.account_id = g.account_id AND g2.security_module_id = @ModCustomer AND g2.matrix_action_id = @ActView
    );
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @ModCustomer UNIQUEIDENTIFIER = 'f1111111-1111-1111-1111-111111111103';
DELETE FROM account_module_grant WHERE security_module_id = @ModCustomer;
DELETE FROM security_module_matrix_action WHERE security_module_id = @ModCustomer;
DELETE FROM security_module WHERE security_module_id = @ModCustomer;
");
        }
    }
}
