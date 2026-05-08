using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrderHistoriesAndCreatedByAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_order_history_account_performed_by_account_id",
                table: "order_history");

            migrationBuilder.DropForeignKey(
                name: "f_k_order_history_order_order_id",
                table: "order_history");

            migrationBuilder.DropPrimaryKey(
                name: "PK_order_history",
                table: "order_history");

            migrationBuilder.RenameTable(
                name: "order_history",
                newName: "order_histories");

            migrationBuilder.RenameColumn(
                name: "performed_by_account_id",
                table: "order_histories",
                newName: "created_by_account_id");

            migrationBuilder.RenameIndex(
                name: "i_x_order_history_performed_by_account_id",
                table: "order_histories",
                newName: "i_x_order_histories_created_by_account_id");

            migrationBuilder.RenameIndex(
                name: "i_x_order_history_order_id",
                table: "order_histories",
                newName: "i_x_order_histories_order_id");

            migrationBuilder.RenameIndex(
                name: "i_x_order_history_activity_date",
                table: "order_histories",
                newName: "i_x_order_histories_activity_date");

            migrationBuilder.AddPrimaryKey(
                name: "PK_order_histories",
                table: "order_histories",
                column: "order_history_id");

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0001-0001-0001-000000000001"),
                column: "created_at",
                value: new DateTime(2026, 4, 16, 9, 52, 49, 694, DateTimeKind.Utc).AddTicks(9817));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0002-0002-0002-000000000002"),
                column: "created_at",
                value: new DateTime(2026, 4, 16, 9, 52, 49, 694, DateTimeKind.Utc).AddTicks(9820));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0003-0003-0003-000000000003"),
                column: "created_at",
                value: new DateTime(2026, 4, 16, 9, 52, 49, 694, DateTimeKind.Utc).AddTicks(9822));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0004-0004-0004-000000000004"),
                column: "created_at",
                value: new DateTime(2026, 4, 16, 9, 52, 49, 694, DateTimeKind.Utc).AddTicks(9824));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0005-0005-0005-000000000005"),
                column: "created_at",
                value: new DateTime(2026, 4, 16, 9, 52, 49, 694, DateTimeKind.Utc).AddTicks(9826));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0006-0006-0006-000000000006"),
                column: "created_at",
                value: new DateTime(2026, 4, 16, 9, 52, 49, 694, DateTimeKind.Utc).AddTicks(9827));

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 4, 16, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9434), new DateTime(2026, 4, 11, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9450) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 17, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9464), new DateTime(2026, 4, 14, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9465) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "created_date",
                value: new DateTime(2026, 2, 15, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9470));

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("44444444-1111-1111-1111-111111111111"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 4, 1, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9475), new DateTime(2026, 4, 15, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9475) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("55555555-2222-2222-2222-222222222222"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 2, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9480), new DateTime(2026, 4, 13, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9481) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("66666666-3333-3333-3333-333333333333"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 27, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9549), new DateTime(2026, 4, 9, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9550) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("77777777-4444-4444-4444-444444444444"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 4, 6, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9555), new DateTime(2026, 4, 12, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9555) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("88888888-5555-5555-5555-555555555555"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 22, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9560), new DateTime(2026, 4, 10, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9561) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("99999999-6666-6666-6666-666666666666"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 7, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9565), new DateTime(2026, 4, 14, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9567) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("aaaaaaaa-7777-7777-7777-777777777777"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 12, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9571), new DateTime(2026, 4, 15, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9572) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("bbbbbbbb-8888-8888-8888-888888888888"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 2, 25, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9576), new DateTime(2026, 4, 8, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9577) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("cccccccc-9999-9999-9999-999999999999"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9581), new DateTime(2026, 4, 15, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9582) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("dddddddd-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 2, 20, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9586), new DateTime(2026, 4, 12, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9587) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("eeeeeeee-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 2, 5, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9592), new DateTime(2026, 4, 6, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9593) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("ffffffff-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 1, 16, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9597), new DateTime(2026, 4, 15, 16, 52, 49, 694, DateTimeKind.Local).AddTicks(9598) });

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0001-0001-0001-000000000001"),
                column: "created_at",
                value: new DateTime(2026, 4, 16, 9, 52, 49, 694, DateTimeKind.Utc).AddTicks(9932));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0002-0002-0002-000000000002"),
                column: "created_at",
                value: new DateTime(2026, 4, 16, 9, 52, 49, 694, DateTimeKind.Utc).AddTicks(9936));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0003-0003-0003-000000000003"),
                column: "created_at",
                value: new DateTime(2026, 4, 16, 9, 52, 49, 694, DateTimeKind.Utc).AddTicks(9938));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0004-0004-0004-000000000004"),
                column: "created_at",
                value: new DateTime(2026, 4, 16, 9, 52, 49, 694, DateTimeKind.Utc).AddTicks(9941));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0005-0005-0005-000000000005"),
                column: "created_at",
                value: new DateTime(2026, 4, 16, 9, 52, 49, 694, DateTimeKind.Utc).AddTicks(9943));

            migrationBuilder.AddForeignKey(
                name: "f_k_order_histories_account_created_by_account_id",
                table: "order_histories",
                column: "created_by_account_id",
                principalTable: "account",
                principalColumn: "account_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_order_histories_order_order_id",
                table: "order_histories",
                column: "order_id",
                principalTable: "order",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_order_histories_account_created_by_account_id",
                table: "order_histories");

            migrationBuilder.DropForeignKey(
                name: "f_k_order_histories_order_order_id",
                table: "order_histories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_order_histories",
                table: "order_histories");

            migrationBuilder.RenameTable(
                name: "order_histories",
                newName: "order_history");

            migrationBuilder.RenameColumn(
                name: "created_by_account_id",
                table: "order_history",
                newName: "performed_by_account_id");

            migrationBuilder.RenameIndex(
                name: "i_x_order_histories_order_id",
                table: "order_history",
                newName: "i_x_order_history_order_id");

            migrationBuilder.RenameIndex(
                name: "i_x_order_histories_created_by_account_id",
                table: "order_history",
                newName: "i_x_order_history_performed_by_account_id");

            migrationBuilder.RenameIndex(
                name: "i_x_order_histories_activity_date",
                table: "order_history",
                newName: "i_x_order_history_activity_date");

            migrationBuilder.AddPrimaryKey(
                name: "PK_order_history",
                table: "order_history",
                column: "order_history_id");

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

            migrationBuilder.AddForeignKey(
                name: "f_k_order_history_account_performed_by_account_id",
                table: "order_history",
                column: "performed_by_account_id",
                principalTable: "account",
                principalColumn: "account_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_order_history_order_order_id",
                table: "order_history",
                column: "order_id",
                principalTable: "order",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
