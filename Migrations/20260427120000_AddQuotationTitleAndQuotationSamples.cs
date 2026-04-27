using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <summary>
    /// Thêm tiêu đề báo giá (quotation.quotation_title) và bảng quotation_sample (mẫu + khối lượng mẫu).
    /// </summary>
    public partial class AddQuotationTitleAndQuotationSamples : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "quotation_title",
                table: "quotation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "quotation_sample",
                columns: table => new
                {
                    quotation_sample_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    quotation_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sample_name = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    sample_volume = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    display_order = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quotation_sample", x => x.quotation_sample_id);
                    table.ForeignKey(
                        name: "f_k_quotation_sample_quotation_quotation_id",
                        column: x => x.quotation_id,
                        principalTable: "quotation",
                        principalColumn: "quotation_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_quotation_sample_quotation_id",
                table: "quotation_sample",
                column: "quotation_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quotation_sample");

            migrationBuilder.DropColumn(
                name: "quotation_title",
                table: "quotation");
        }
    }
}
