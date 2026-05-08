using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <inheritdoc />
    public partial class _20260420_AddQuotationToOrderSample : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "quotation_id",
                table: "order_sample",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "i_x_order_sample_quotation_id",
                table: "order_sample",
                column: "quotation_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_order_sample_quotation_quotation_id",
                table: "order_sample",
                column: "quotation_id",
                principalTable: "quotation",
                principalColumn: "quotation_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_order_sample_quotation_quotation_id",
                table: "order_sample");

            migrationBuilder.DropIndex(
                name: "i_x_order_sample_quotation_id",
                table: "order_sample");

            migrationBuilder.DropColumn(
                name: "quotation_id",
                table: "order_sample");
        }
    }
}
