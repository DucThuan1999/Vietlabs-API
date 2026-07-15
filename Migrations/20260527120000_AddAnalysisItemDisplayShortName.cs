using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <summary>
    /// Thêm display_short_name (JSON Tiptap) cho tên viết tắt có format từ Excel.
    /// Chạy Scripts/AddAnalysisItemDisplayShortName.sql trên DB thực tế nếu không dùng EF migrate.
    /// </summary>
    public partial class AddAnalysisItemDisplayShortName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "display_short_name",
                table: "analysis_item",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "display_short_name",
                table: "analysis_item");
        }
    }
}
