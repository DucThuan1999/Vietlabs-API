using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <inheritdoc />
    public partial class _20260420_AddQuotationToOrderTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "quotation_id",
                table: "order_template",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "i_x_order_template_quotation_id",
                table: "order_template",
                column: "quotation_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_order_template_quotation_quotation_id",
                table: "order_template",
                column: "quotation_id",
                principalTable: "quotation",
                principalColumn: "quotation_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_order_template_quotation_quotation_id",
                table: "order_template");

            migrationBuilder.DropIndex(
                name: "i_x_order_template_quotation_id",
                table: "order_template");

            migrationBuilder.DropColumn(
                name: "quotation_id",
                table: "order_template");
        }
    }
}
