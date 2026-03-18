using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <summary>Ma trận quyền module × hành động + migrate từ permission_id cũ.</summary>
    public partial class PermissionMatrix : Migration
    {
        private static readonly Guid ModAdmin = Guid.Parse("f1111111-1111-1111-1111-111111111101");
        private static readonly Guid ModQuotation = Guid.Parse("f1111111-1111-1111-1111-111111111102");
        private static readonly Guid ActView = Guid.Parse("e1111111-1111-1111-1111-111111111101");
        private static readonly Guid ActCreate = Guid.Parse("e1111111-1111-1111-1111-111111111102");
        private static readonly Guid ActEdit = Guid.Parse("e1111111-1111-1111-1111-111111111103");
        private static readonly Guid ActDelete = Guid.Parse("e1111111-1111-1111-1111-111111111104");
        private static readonly Guid ActApprove = Guid.Parse("e1111111-1111-1111-1111-111111111105");
        private static readonly Guid ActExport = Guid.Parse("e1111111-1111-1111-1111-111111111106");

        private static readonly Guid PermAdmin = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        private static readonly Guid PermUser = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        private static readonly Guid PermManager = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        private static readonly Guid PermSales = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "security_module",
                columns: table => new
                {
                    security_module_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    code = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    name_vi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_security_module", x => x.security_module_id);
                });

            migrationBuilder.CreateTable(
                name: "matrix_action",
                columns: table => new
                {
                    matrix_action_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    code = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    name_vi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sort_order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matrix_action", x => x.matrix_action_id);
                });

            migrationBuilder.CreateTable(
                name: "security_module_matrix_action",
                columns: table => new
                {
                    security_module_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    matrix_action_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_security_module_matrix_action", x => new { x.security_module_id, x.matrix_action_id });
                    table.ForeignKey(
                        name: "FK_security_module_matrix_action_security_module_security_module_id",
                        column: x => x.security_module_id,
                        principalTable: "security_module",
                        principalColumn: "security_module_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_security_module_matrix_action_matrix_action_matrix_action_id",
                        column: x => x.matrix_action_id,
                        principalTable: "matrix_action",
                        principalColumn: "matrix_action_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_module_grant",
                columns: table => new
                {
                    account_module_grant_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    security_module_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    matrix_action_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_module_grant", x => x.account_module_grant_id);
                    table.ForeignKey(
                        name: "FK_account_module_grant_account_account_id",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_account_module_grant_security_module_security_module_id",
                        column: x => x.security_module_id,
                        principalTable: "security_module",
                        principalColumn: "security_module_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_account_module_grant_matrix_action_matrix_action_id",
                        column: x => x.matrix_action_id,
                        principalTable: "matrix_action",
                        principalColumn: "matrix_action_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_security_module_code",
                table: "security_module",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_matrix_action_code",
                table: "matrix_action",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_account_module_grant_account_module_action",
                table: "account_module_grant",
                columns: new[] { "account_id", "security_module_id", "matrix_action_id" },
                unique: true);

            migrationBuilder.InsertData(
                table: "security_module",
                columns: new[] { "security_module_id", "code", "name_vi", "sort_order", "status" },
                values: new object[,]
                {
                    { ModAdmin, "Admin", "Quản trị / Cài đặt", 1, "Active" },
                    { ModQuotation, "Quotation", "Báo giá", 2, "Active" }
                });

            migrationBuilder.InsertData(
                table: "matrix_action",
                columns: new[] { "matrix_action_id", "code", "name_vi", "sort_order" },
                values: new object[,]
                {
                    { ActView, "View", "Xem", 1 },
                    { ActCreate, "Create", "Tạo mới", 2 },
                    { ActEdit, "Edit", "Sửa", 3 },
                    { ActDelete, "Delete", "Xóa", 4 },
                    { ActApprove, "Approve", "Phê duyệt", 5 },
                    { ActExport, "Export", "Xuất", 6 }
                });

            var cells = new (Guid Mod, Guid Act)[]
            {
                (ModAdmin, ActView), (ModAdmin, ActCreate), (ModAdmin, ActEdit), (ModAdmin, ActDelete), (ModAdmin, ActApprove), (ModAdmin, ActExport),
                (ModQuotation, ActView), (ModQuotation, ActCreate), (ModQuotation, ActEdit), (ModQuotation, ActDelete), (ModQuotation, ActApprove), (ModQuotation, ActExport)
            };
            foreach (var (mod, act) in cells)
            {
                migrationBuilder.InsertData(
                    table: "security_module_matrix_action",
                    columns: new[] { "security_module_id", "matrix_action_id" },
                    values: new object[] { mod, act });
            }

            // PERM-ADMIN: toàn bộ ô
            migrationBuilder.Sql($@"
INSERT INTO account_module_grant (account_module_grant_id, account_id, security_module_id, matrix_action_id)
SELECT NEWID(), a.account_id, sma.security_module_id, sma.matrix_action_id
FROM account a
CROSS JOIN security_module_matrix_action sma
WHERE a.permission_id = '{PermAdmin}'");

            // PERM-USER: chỉ View
            migrationBuilder.Sql($@"
INSERT INTO account_module_grant (account_module_grant_id, account_id, security_module_id, matrix_action_id)
SELECT NEWID(), a.account_id, m.security_module_id, '{ActView}'
FROM account a
CROSS JOIN security_module m
WHERE a.permission_id = '{PermUser}'");

            // PERM-MANAGER: đủ quyền hai module (trừ Delete Admin — hoặc full; dùng full Quotation + Admin trừ Delete)
            migrationBuilder.Sql($@"
INSERT INTO account_module_grant (account_module_grant_id, account_id, security_module_id, matrix_action_id)
SELECT NEWID(), a.account_id, sma.security_module_id, sma.matrix_action_id
FROM account a
CROSS JOIN security_module_matrix_action sma
WHERE a.permission_id = '{PermManager}'
  AND NOT (sma.security_module_id = '{ModAdmin}' AND sma.matrix_action_id = '{ActDelete}')");

            // PERM-SALES: Báo giá đủ; Admin chỉ View
            migrationBuilder.Sql($@"
INSERT INTO account_module_grant (account_module_grant_id, account_id, security_module_id, matrix_action_id)
SELECT NEWID(), a.account_id, sma.security_module_id, sma.matrix_action_id
FROM account a
CROSS JOIN security_module_matrix_action sma
WHERE a.permission_id = '{PermSales}'
  AND (sma.security_module_id = '{ModQuotation}' OR (sma.security_module_id = '{ModAdmin}' AND sma.matrix_action_id = '{ActView}'))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "account_module_grant");
            migrationBuilder.DropTable(name: "security_module_matrix_action");
            migrationBuilder.DropTable(name: "matrix_action");
            migrationBuilder.DropTable(name: "security_module");
        }
    }
}
