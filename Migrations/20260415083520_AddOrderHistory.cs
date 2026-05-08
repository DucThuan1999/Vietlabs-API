using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "agent_name",
                table: "order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "approval_note",
                table: "order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "customer_address",
                table: "order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "customer_code",
                table: "order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "customer_name",
                table: "order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "customer_tax_code",
                table: "order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_approved",
                table: "order",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "issue_invoice",
                table: "order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payer_email",
                table: "order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payer_name",
                table: "order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payer_phone",
                table: "order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "quotation_id",
                table: "order",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rejection_note",
                table: "order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sample_sender_email",
                table: "order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sample_sender_name",
                table: "order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sample_sender_phone",
                table: "order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "order_history",
                columns: table => new
                {
                    order_history_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    order_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    activity_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    activity = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    performed_by_account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_history", x => x.order_history_id);
                    table.ForeignKey(
                        name: "f_k_order_history_account_performed_by_account_id",
                        column: x => x.performed_by_account_id,
                        principalTable: "account",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_order_history_order_order_id",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0001-0001-0001-000000000001"),
                column: "created_at",
                value: new DateTime(2026, 4, 15, 8, 35, 18, 993, DateTimeKind.Utc).AddTicks(4070));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0002-0002-0002-000000000002"),
                column: "created_at",
                value: new DateTime(2026, 4, 15, 8, 35, 18, 993, DateTimeKind.Utc).AddTicks(4072));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0003-0003-0003-000000000003"),
                column: "created_at",
                value: new DateTime(2026, 4, 15, 8, 35, 18, 993, DateTimeKind.Utc).AddTicks(4074));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0004-0004-0004-000000000004"),
                column: "created_at",
                value: new DateTime(2026, 4, 15, 8, 35, 18, 993, DateTimeKind.Utc).AddTicks(4076));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0005-0005-0005-000000000005"),
                column: "created_at",
                value: new DateTime(2026, 4, 15, 8, 35, 18, 993, DateTimeKind.Utc).AddTicks(4077));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0006-0006-0006-000000000006"),
                column: "created_at",
                value: new DateTime(2026, 4, 15, 8, 35, 18, 993, DateTimeKind.Utc).AddTicks(4079));

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 4, 15, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3677), new DateTime(2026, 4, 10, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3690) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 16, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3703), new DateTime(2026, 4, 13, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3704) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "created_date",
                value: new DateTime(2026, 2, 14, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3710));

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("44444444-1111-1111-1111-111111111111"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 31, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3715), new DateTime(2026, 4, 14, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3715) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("55555555-2222-2222-2222-222222222222"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 1, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3764), new DateTime(2026, 4, 12, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3765) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("66666666-3333-3333-3333-333333333333"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 26, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3769), new DateTime(2026, 4, 8, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3770) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("77777777-4444-4444-4444-444444444444"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 4, 5, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3774), new DateTime(2026, 4, 11, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3775) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("88888888-5555-5555-5555-555555555555"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 21, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3780), new DateTime(2026, 4, 9, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3781) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("99999999-6666-6666-6666-666666666666"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 6, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3785), new DateTime(2026, 4, 13, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3786) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("aaaaaaaa-7777-7777-7777-777777777777"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 11, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3790), new DateTime(2026, 4, 14, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3791) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("bbbbbbbb-8888-8888-8888-888888888888"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 2, 24, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3795), new DateTime(2026, 4, 7, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3796) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("cccccccc-9999-9999-9999-999999999999"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 4, 10, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3800), new DateTime(2026, 4, 14, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3802) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("dddddddd-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 2, 19, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3807), new DateTime(2026, 4, 11, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3807) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("eeeeeeee-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 2, 4, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3811), new DateTime(2026, 4, 5, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3812) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("ffffffff-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 1, 15, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3817), new DateTime(2026, 4, 14, 15, 35, 18, 993, DateTimeKind.Local).AddTicks(3817) });

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0001-0001-0001-000000000001"),
                column: "created_at",
                value: new DateTime(2026, 4, 15, 8, 35, 18, 993, DateTimeKind.Utc).AddTicks(4182));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0002-0002-0002-000000000002"),
                column: "created_at",
                value: new DateTime(2026, 4, 15, 8, 35, 18, 993, DateTimeKind.Utc).AddTicks(4186));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0003-0003-0003-000000000003"),
                column: "created_at",
                value: new DateTime(2026, 4, 15, 8, 35, 18, 993, DateTimeKind.Utc).AddTicks(4231));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0004-0004-0004-000000000004"),
                column: "created_at",
                value: new DateTime(2026, 4, 15, 8, 35, 18, 993, DateTimeKind.Utc).AddTicks(4233));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0005-0005-0005-000000000005"),
                column: "created_at",
                value: new DateTime(2026, 4, 15, 8, 35, 18, 993, DateTimeKind.Utc).AddTicks(4236));

            migrationBuilder.CreateIndex(
                name: "i_x_order_quotation_id",
                table: "order",
                column: "quotation_id");

            migrationBuilder.CreateIndex(
                name: "i_x_order_history_activity_date",
                table: "order_history",
                column: "activity_date");

            migrationBuilder.CreateIndex(
                name: "i_x_order_history_order_id",
                table: "order_history",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "i_x_order_history_performed_by_account_id",
                table: "order_history",
                column: "performed_by_account_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_order_quotation_quotation_id",
                table: "order",
                column: "quotation_id",
                principalTable: "quotation",
                principalColumn: "quotation_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_order_quotation_quotation_id",
                table: "order");

            migrationBuilder.DropTable(
                name: "order_history");

            migrationBuilder.DropIndex(
                name: "i_x_order_quotation_id",
                table: "order");

            migrationBuilder.DropColumn(
                name: "agent_name",
                table: "order");

            migrationBuilder.DropColumn(
                name: "approval_note",
                table: "order");

            migrationBuilder.DropColumn(
                name: "customer_address",
                table: "order");

            migrationBuilder.DropColumn(
                name: "customer_code",
                table: "order");

            migrationBuilder.DropColumn(
                name: "customer_name",
                table: "order");

            migrationBuilder.DropColumn(
                name: "customer_tax_code",
                table: "order");

            migrationBuilder.DropColumn(
                name: "is_approved",
                table: "order");

            migrationBuilder.DropColumn(
                name: "issue_invoice",
                table: "order");

            migrationBuilder.DropColumn(
                name: "payer_email",
                table: "order");

            migrationBuilder.DropColumn(
                name: "payer_name",
                table: "order");

            migrationBuilder.DropColumn(
                name: "payer_phone",
                table: "order");

            migrationBuilder.DropColumn(
                name: "quotation_id",
                table: "order");

            migrationBuilder.DropColumn(
                name: "rejection_note",
                table: "order");

            migrationBuilder.DropColumn(
                name: "sample_sender_email",
                table: "order");

            migrationBuilder.DropColumn(
                name: "sample_sender_name",
                table: "order");

            migrationBuilder.DropColumn(
                name: "sample_sender_phone",
                table: "order");

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0001-0001-0001-000000000001"),
                column: "created_at",
                value: new DateTime(2026, 4, 5, 15, 11, 7, 764, DateTimeKind.Utc).AddTicks(7329));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0002-0002-0002-000000000002"),
                column: "created_at",
                value: new DateTime(2026, 4, 5, 15, 11, 7, 764, DateTimeKind.Utc).AddTicks(7331));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0003-0003-0003-000000000003"),
                column: "created_at",
                value: new DateTime(2026, 4, 5, 15, 11, 7, 764, DateTimeKind.Utc).AddTicks(7333));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0004-0004-0004-000000000004"),
                column: "created_at",
                value: new DateTime(2026, 4, 5, 15, 11, 7, 764, DateTimeKind.Utc).AddTicks(7335));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0005-0005-0005-000000000005"),
                column: "created_at",
                value: new DateTime(2026, 4, 5, 15, 11, 7, 764, DateTimeKind.Utc).AddTicks(7336));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0006-0006-0006-000000000006"),
                column: "created_at",
                value: new DateTime(2026, 4, 5, 15, 11, 7, 764, DateTimeKind.Utc).AddTicks(7339));

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 4, 5, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6858), new DateTime(2026, 3, 31, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6870) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 6, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6882), new DateTime(2026, 4, 3, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6882) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "created_date",
                value: new DateTime(2026, 2, 4, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6888));

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("44444444-1111-1111-1111-111111111111"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 21, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6893), new DateTime(2026, 4, 4, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6894) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("55555555-2222-2222-2222-222222222222"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 2, 19, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6899), new DateTime(2026, 4, 2, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6899) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("66666666-3333-3333-3333-333333333333"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 16, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6904), new DateTime(2026, 3, 29, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6905) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("77777777-4444-4444-4444-444444444444"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 26, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6909), new DateTime(2026, 4, 1, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6910) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("88888888-5555-5555-5555-555555555555"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 11, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6916), new DateTime(2026, 3, 30, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6916) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("99999999-6666-6666-6666-666666666666"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 2, 24, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6921), new DateTime(2026, 4, 3, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6922) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("aaaaaaaa-7777-7777-7777-777777777777"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 1, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6927), new DateTime(2026, 4, 4, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6927) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("bbbbbbbb-8888-8888-8888-888888888888"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 2, 14, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6960), new DateTime(2026, 3, 28, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6961) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("cccccccc-9999-9999-9999-999999999999"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 31, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6966), new DateTime(2026, 4, 4, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6966) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("dddddddd-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 2, 9, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6972), new DateTime(2026, 4, 1, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6973) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("eeeeeeee-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 1, 25, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6978), new DateTime(2026, 3, 26, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6979) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("ffffffff-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 1, 5, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6983), new DateTime(2026, 4, 4, 22, 11, 7, 764, DateTimeKind.Local).AddTicks(6984) });

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0001-0001-0001-000000000001"),
                column: "created_at",
                value: new DateTime(2026, 4, 5, 15, 11, 7, 764, DateTimeKind.Utc).AddTicks(7389));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0002-0002-0002-000000000002"),
                column: "created_at",
                value: new DateTime(2026, 4, 5, 15, 11, 7, 764, DateTimeKind.Utc).AddTicks(7393));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0003-0003-0003-000000000003"),
                column: "created_at",
                value: new DateTime(2026, 4, 5, 15, 11, 7, 764, DateTimeKind.Utc).AddTicks(7395));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0004-0004-0004-000000000004"),
                column: "created_at",
                value: new DateTime(2026, 4, 5, 15, 11, 7, 764, DateTimeKind.Utc).AddTicks(7398));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0005-0005-0005-000000000005"),
                column: "created_at",
                value: new DateTime(2026, 4, 5, 15, 11, 7, 764, DateTimeKind.Utc).AddTicks(7400));
        }
    }
}
