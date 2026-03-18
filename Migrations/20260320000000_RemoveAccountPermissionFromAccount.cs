using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <summary>Bỏ permission_id trên account — quyền chỉ còn ma trận account_module_grant.</summary>
    public partial class RemoveAccountPermissionFromAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_account_permission_permission_id",
                table: "account");

            migrationBuilder.DropIndex(
                name: "i_x_account_permission_id",
                table: "account");

            migrationBuilder.DropColumn(
                name: "permission_id",
                table: "account");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "permission_id",
                table: "account",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.CreateIndex(
                name: "i_x_account_permission_id",
                table: "account",
                column: "permission_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_account_permission_permission_id",
                table: "account",
                column: "permission_id",
                principalTable: "permission",
                principalColumn: "permission_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
