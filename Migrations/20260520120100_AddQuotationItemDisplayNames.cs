using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    public partial class AddQuotationItemDisplayNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "item_display_name_vi",
                table: "quotation_item",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "item_display_name_en",
                table: "quotation_item",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "item_display_name_vi",
                table: "quotation_item");

            migrationBuilder.DropColumn(
                name: "item_display_name_en",
                table: "quotation_item");
        }
    }
}
