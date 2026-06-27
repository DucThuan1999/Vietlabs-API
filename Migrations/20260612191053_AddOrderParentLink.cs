using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderParentLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "quotation_id",
                table: "order_sample",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "linked_order_index",
                table: "order",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "parent_order_id",
                table: "order",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0001-0001-0001-000000000001"),
                column: "created_at",
                value: new DateTime(2026, 6, 12, 19, 10, 51, 775, DateTimeKind.Utc).AddTicks(8049));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0002-0002-0002-000000000002"),
                column: "created_at",
                value: new DateTime(2026, 6, 12, 19, 10, 51, 775, DateTimeKind.Utc).AddTicks(8052));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0003-0003-0003-000000000003"),
                column: "created_at",
                value: new DateTime(2026, 6, 12, 19, 10, 51, 775, DateTimeKind.Utc).AddTicks(8054));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0004-0004-0004-000000000004"),
                column: "created_at",
                value: new DateTime(2026, 6, 12, 19, 10, 51, 775, DateTimeKind.Utc).AddTicks(8055));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0005-0005-0005-000000000005"),
                column: "created_at",
                value: new DateTime(2026, 6, 12, 19, 10, 51, 775, DateTimeKind.Utc).AddTicks(8057));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0006-0006-0006-000000000006"),
                column: "created_at",
                value: new DateTime(2026, 6, 12, 19, 10, 51, 775, DateTimeKind.Utc).AddTicks(8058));

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 6, 13, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7670), new DateTime(2026, 6, 8, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7681) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 5, 14, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7694), new DateTime(2026, 6, 11, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7695) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "created_date",
                value: new DateTime(2026, 4, 14, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7701));

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("44444444-1111-1111-1111-111111111111"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 5, 29, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7706), new DateTime(2026, 6, 12, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7706) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("55555555-2222-2222-2222-222222222222"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 4, 29, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7711), new DateTime(2026, 6, 10, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7712) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("66666666-3333-3333-3333-333333333333"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 5, 24, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7716), new DateTime(2026, 6, 6, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7717) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("77777777-4444-4444-4444-444444444444"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 6, 3, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7781), new DateTime(2026, 6, 9, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7781) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("88888888-5555-5555-5555-555555555555"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 5, 19, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7786), new DateTime(2026, 6, 7, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7787) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("99999999-6666-6666-6666-666666666666"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 5, 4, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7791), new DateTime(2026, 6, 11, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7792) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("aaaaaaaa-7777-7777-7777-777777777777"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 5, 9, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7797), new DateTime(2026, 6, 12, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7797) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("bbbbbbbb-8888-8888-8888-888888888888"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 4, 24, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7802), new DateTime(2026, 6, 5, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7802) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("cccccccc-9999-9999-9999-999999999999"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 6, 8, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7807), new DateTime(2026, 6, 12, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7807) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("dddddddd-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 4, 19, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7813), new DateTime(2026, 6, 9, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7814) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("eeeeeeee-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 4, 4, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7818), new DateTime(2026, 6, 3, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7819) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("ffffffff-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 15, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7823), new DateTime(2026, 6, 12, 2, 10, 51, 775, DateTimeKind.Local).AddTicks(7824) });

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0001-0001-0001-000000000001"),
                column: "created_at",
                value: new DateTime(2026, 6, 12, 19, 10, 51, 775, DateTimeKind.Utc).AddTicks(8099));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0002-0002-0002-000000000002"),
                column: "created_at",
                value: new DateTime(2026, 6, 12, 19, 10, 51, 775, DateTimeKind.Utc).AddTicks(8103));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0003-0003-0003-000000000003"),
                column: "created_at",
                value: new DateTime(2026, 6, 12, 19, 10, 51, 775, DateTimeKind.Utc).AddTicks(8105));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0004-0004-0004-000000000004"),
                column: "created_at",
                value: new DateTime(2026, 6, 12, 19, 10, 51, 775, DateTimeKind.Utc).AddTicks(8108));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0005-0005-0005-000000000005"),
                column: "created_at",
                value: new DateTime(2026, 6, 12, 19, 10, 51, 775, DateTimeKind.Utc).AddTicks(8110));

            migrationBuilder.CreateIndex(
                name: "i_x_order_sample_quotation_id",
                table: "order_sample",
                column: "quotation_id");

            migrationBuilder.CreateIndex(
                name: "i_x_order_parent_order_id",
                table: "order",
                column: "parent_order_id");

            migrationBuilder.CreateIndex(
                name: "u_q_order_parent_linked_index",
                table: "order",
                columns: new[] { "parent_order_id", "linked_order_index" },
                unique: true,
                filter: "[parent_order_id] IS NOT NULL AND [linked_order_index] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "f_k_order_order_parent_order_id",
                table: "order",
                column: "parent_order_id",
                principalTable: "order",
                principalColumn: "order_id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "f_k_order_sample_quotation_quotation_id",
                table: "order_sample",
                column: "quotation_id",
                principalTable: "quotation",
                principalColumn: "quotation_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_order_order_parent_order_id",
                table: "order");

            migrationBuilder.DropForeignKey(
                name: "f_k_order_sample_quotation_quotation_id",
                table: "order_sample");

            migrationBuilder.DropIndex(
                name: "i_x_order_sample_quotation_id",
                table: "order_sample");

            migrationBuilder.DropIndex(
                name: "i_x_order_parent_order_id",
                table: "order");

            migrationBuilder.DropIndex(
                name: "u_q_order_parent_linked_index",
                table: "order");

            migrationBuilder.DropColumn(
                name: "quotation_id",
                table: "order_sample");

            migrationBuilder.DropColumn(
                name: "linked_order_index",
                table: "order");

            migrationBuilder.DropColumn(
                name: "parent_order_id",
                table: "order");

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0001-0001-0001-000000000001"),
                column: "created_at",
                value: new DateTime(2026, 4, 20, 9, 3, 13, 941, DateTimeKind.Utc).AddTicks(1111));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0002-0002-0002-000000000002"),
                column: "created_at",
                value: new DateTime(2026, 4, 20, 9, 3, 13, 941, DateTimeKind.Utc).AddTicks(1114));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0003-0003-0003-000000000003"),
                column: "created_at",
                value: new DateTime(2026, 4, 20, 9, 3, 13, 941, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0004-0004-0004-000000000004"),
                column: "created_at",
                value: new DateTime(2026, 4, 20, 9, 3, 13, 941, DateTimeKind.Utc).AddTicks(1118));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0005-0005-0005-000000000005"),
                column: "created_at",
                value: new DateTime(2026, 4, 20, 9, 3, 13, 941, DateTimeKind.Utc).AddTicks(1120));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0006-0006-0006-000000000006"),
                column: "created_at",
                value: new DateTime(2026, 4, 20, 9, 3, 13, 941, DateTimeKind.Utc).AddTicks(1123));

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 4, 20, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(349), new DateTime(2026, 4, 15, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(454) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 21, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(478), new DateTime(2026, 4, 18, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(479) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "created_date",
                value: new DateTime(2026, 2, 19, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(486));

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("44444444-1111-1111-1111-111111111111"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 4, 5, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(494), new DateTime(2026, 4, 19, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(495) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("55555555-2222-2222-2222-222222222222"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 6, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(501), new DateTime(2026, 4, 17, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(502) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("66666666-3333-3333-3333-333333333333"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 31, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(508), new DateTime(2026, 4, 13, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(509) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("77777777-4444-4444-4444-444444444444"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 4, 10, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(515), new DateTime(2026, 4, 16, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(516) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("88888888-5555-5555-5555-555555555555"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 26, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(522), new DateTime(2026, 4, 14, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(526) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("99999999-6666-6666-6666-666666666666"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 11, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(533), new DateTime(2026, 4, 18, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(534) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("aaaaaaaa-7777-7777-7777-777777777777"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 16, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(584), new DateTime(2026, 4, 19, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(586) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("bbbbbbbb-8888-8888-8888-888888888888"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 1, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(592), new DateTime(2026, 4, 12, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(594) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("cccccccc-9999-9999-9999-999999999999"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(600), new DateTime(2026, 4, 19, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(601) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("dddddddd-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 2, 24, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(608), new DateTime(2026, 4, 16, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(609) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("eeeeeeee-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 2, 9, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(617), new DateTime(2026, 4, 10, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(618) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("ffffffff-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 1, 20, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(625), new DateTime(2026, 4, 19, 16, 3, 13, 941, DateTimeKind.Local).AddTicks(626) });

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0001-0001-0001-000000000001"),
                column: "created_at",
                value: new DateTime(2026, 4, 20, 9, 3, 13, 941, DateTimeKind.Utc).AddTicks(1191));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0002-0002-0002-000000000002"),
                column: "created_at",
                value: new DateTime(2026, 4, 20, 9, 3, 13, 941, DateTimeKind.Utc).AddTicks(1197));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0003-0003-0003-000000000003"),
                column: "created_at",
                value: new DateTime(2026, 4, 20, 9, 3, 13, 941, DateTimeKind.Utc).AddTicks(1200));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0004-0004-0004-000000000004"),
                column: "created_at",
                value: new DateTime(2026, 4, 20, 9, 3, 13, 941, DateTimeKind.Utc).AddTicks(1203));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0005-0005-0005-000000000005"),
                column: "created_at",
                value: new DateTime(2026, 4, 20, 9, 3, 13, 941, DateTimeKind.Utc).AddTicks(1206));
        }
    }
}
