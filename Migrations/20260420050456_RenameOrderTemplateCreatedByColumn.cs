using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrderTemplateCreatedByColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_order_template_account_created_by_account_id",
                table: "order_template");

            migrationBuilder.RenameColumn(
                name: "created_by_account_id",
                table: "order_template",
                newName: "created_by");

            migrationBuilder.RenameIndex(
                name: "i_x_order_template_created_by_account_id",
                table: "order_template",
                newName: "i_x_order_template_created_by");

            migrationBuilder.AddForeignKey(
                name: "f_k_order_template_account_created_by",
                table: "order_template",
                column: "created_by",
                principalTable: "account",
                principalColumn: "account_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_order_template_account_created_by",
                table: "order_template");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "order_template",
                newName: "created_by_account_id");

            migrationBuilder.RenameIndex(
                name: "i_x_order_template_created_by",
                table: "order_template",
                newName: "i_x_order_template_created_by_account_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_order_template_account_created_by_account_id",
                table: "order_template",
                column: "created_by_account_id",
                principalTable: "account",
                principalColumn: "account_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
