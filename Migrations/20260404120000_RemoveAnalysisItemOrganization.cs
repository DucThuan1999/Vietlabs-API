using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <summary>Bỏ cột organization trên analysis_item — không dùng trên frontend.</summary>
    public partial class RemoveAnalysisItemOrganization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "organization",
                table: "analysis_item");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "organization",
                table: "analysis_item",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
