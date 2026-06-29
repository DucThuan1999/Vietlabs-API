using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrderApprovalColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "rejection_note",
                table: "order",
                newName: "approval_reason");

            migrationBuilder.RenameColumn(
                name: "approval_note",
                table: "order",
                newName: "approval_notes");

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0001-0001-0001-000000000001"),
                column: "created_at",
                value: new DateTime(2026, 6, 27, 16, 23, 47, 600, DateTimeKind.Utc).AddTicks(2940));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0002-0002-0002-000000000002"),
                column: "created_at",
                value: new DateTime(2026, 6, 27, 16, 23, 47, 600, DateTimeKind.Utc).AddTicks(2943));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0003-0003-0003-000000000003"),
                column: "created_at",
                value: new DateTime(2026, 6, 27, 16, 23, 47, 600, DateTimeKind.Utc).AddTicks(2946));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0004-0004-0004-000000000004"),
                column: "created_at",
                value: new DateTime(2026, 6, 27, 16, 23, 47, 600, DateTimeKind.Utc).AddTicks(2950));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0005-0005-0005-000000000005"),
                column: "created_at",
                value: new DateTime(2026, 6, 27, 16, 23, 47, 600, DateTimeKind.Utc).AddTicks(2953));

            migrationBuilder.UpdateData(
                table: "analysis_group",
                keyColumn: "analysis_group_id",
                keyValue: new Guid("aaaaaaaa-0006-0006-0006-000000000006"),
                column: "created_at",
                value: new DateTime(2026, 6, 27, 16, 23, 47, 600, DateTimeKind.Utc).AddTicks(2956));

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 6, 27, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2221), new DateTime(2026, 6, 22, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2237) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 5, 28, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2327), new DateTime(2026, 6, 25, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2329) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "created_date",
                value: new DateTime(2026, 4, 28, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2337));

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("44444444-1111-1111-1111-111111111111"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 6, 12, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2344), new DateTime(2026, 6, 26, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2345) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("55555555-2222-2222-2222-222222222222"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 5, 13, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2352), new DateTime(2026, 6, 24, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2354) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("66666666-3333-3333-3333-333333333333"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 6, 7, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2361), new DateTime(2026, 6, 20, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2362) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("77777777-4444-4444-4444-444444444444"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 6, 17, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2370), new DateTime(2026, 6, 23, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2371) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("88888888-5555-5555-5555-555555555555"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 6, 2, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2380), new DateTime(2026, 6, 21, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2381) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("99999999-6666-6666-6666-666666666666"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 5, 18, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2500), new DateTime(2026, 6, 25, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2501) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("aaaaaaaa-7777-7777-7777-777777777777"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 5, 23, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2509), new DateTime(2026, 6, 26, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2510) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("bbbbbbbb-8888-8888-8888-888888888888"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 5, 8, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2518), new DateTime(2026, 6, 19, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2519) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("cccccccc-9999-9999-9999-999999999999"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 6, 22, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2527), new DateTime(2026, 6, 26, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2528) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("dddddddd-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 5, 3, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2537), new DateTime(2026, 6, 23, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2538) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("eeeeeeee-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 4, 18, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2546), new DateTime(2026, 6, 17, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2548) });

            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "client_id",
                keyValue: new Guid("ffffffff-cccc-cccc-cccc-cccccccccccc"),
                columns: new[] { "created_date", "last_contact_date" },
                values: new object[] { new DateTime(2026, 3, 29, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2555), new DateTime(2026, 6, 26, 23, 23, 47, 600, DateTimeKind.Local).AddTicks(2556) });

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0001-0001-0001-000000000001"),
                column: "created_at",
                value: new DateTime(2026, 6, 27, 16, 23, 47, 600, DateTimeKind.Utc).AddTicks(3029));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0002-0002-0002-000000000002"),
                column: "created_at",
                value: new DateTime(2026, 6, 27, 16, 23, 47, 600, DateTimeKind.Utc).AddTicks(3035));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0003-0003-0003-000000000003"),
                column: "created_at",
                value: new DateTime(2026, 6, 27, 16, 23, 47, 600, DateTimeKind.Utc).AddTicks(3040));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0004-0004-0004-000000000004"),
                column: "created_at",
                value: new DateTime(2026, 6, 27, 16, 23, 47, 600, DateTimeKind.Utc).AddTicks(3044));

            migrationBuilder.UpdateData(
                table: "package",
                keyColumn: "package_id",
                keyValue: new Guid("bbbbbbbb-0005-0005-0005-000000000005"),
                column: "created_at",
                value: new DateTime(2026, 6, 27, 16, 23, 47, 600, DateTimeKind.Utc).AddTicks(3048));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "approval_reason",
                table: "order",
                newName: "rejection_note");

            migrationBuilder.RenameColumn(
                name: "approval_notes",
                table: "order",
                newName: "approval_note");

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
        }
    }
}
