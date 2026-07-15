using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <summary>
    /// Thêm display_name_vi/en cho tên chỉ tiêu có format; name_vi/en giữ text thuần để search.
    /// Chạy script Python migrate_analysis_item_display_names.py sau migration schema nếu có dữ liệu JSON trong name_vi/name_en.
    /// </summary>
    public partial class AddAnalysisItemDisplayNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "display_name_vi",
                table: "analysis_item",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "display_name_en",
                table: "analysis_item",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "display_name_vi",
                table: "analysis_item");

            migrationBuilder.DropColumn(
                name: "display_name_en",
                table: "analysis_item");
        }
    }
}
