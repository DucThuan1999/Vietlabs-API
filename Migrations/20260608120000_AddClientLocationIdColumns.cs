using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <summary>
    /// Thêm country_id, province_id, ward_id cho bảng client.
    /// Chạy Scripts/AddClientLocationIdColumns.sql trên DB thực tế nếu không dùng EF migrate.
    /// </summary>
    public partial class AddClientLocationIdColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "country_id",
                table: "client",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "province_id",
                table: "client",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ward_id",
                table: "client",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_client_country_id",
                table: "client",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "IX_client_province_id",
                table: "client",
                column: "province_id");

            migrationBuilder.CreateIndex(
                name: "IX_client_ward_id",
                table: "client",
                column: "ward_id");

            migrationBuilder.AddForeignKey(
                name: "FK_client_country_country_id",
                table: "client",
                column: "country_id",
                principalTable: "country",
                principalColumn: "country_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_client_province_province_id",
                table: "client",
                column: "province_id",
                principalTable: "province",
                principalColumn: "province_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_client_ward_ward_id",
                table: "client",
                column: "ward_id",
                principalTable: "ward",
                principalColumn: "ward_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_client_country_country_id",
                table: "client");

            migrationBuilder.DropForeignKey(
                name: "FK_client_province_province_id",
                table: "client");

            migrationBuilder.DropForeignKey(
                name: "FK_client_ward_ward_id",
                table: "client");

            migrationBuilder.DropIndex(
                name: "IX_client_country_id",
                table: "client");

            migrationBuilder.DropIndex(
                name: "IX_client_province_id",
                table: "client");

            migrationBuilder.DropIndex(
                name: "IX_client_ward_id",
                table: "client");

            migrationBuilder.DropColumn(
                name: "country_id",
                table: "client");

            migrationBuilder.DropColumn(
                name: "province_id",
                table: "client");

            migrationBuilder.DropColumn(
                name: "ward_id",
                table: "client");
        }
    }
}
