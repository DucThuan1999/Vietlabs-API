using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <inheritdoc />
    public partial class AddQuotationItemCapacity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "capacity_type",
                table: "quotation_item",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "department_analysis_capability_id",
                table: "quotation_item",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "subcontractor_capability_id",
                table: "quotation_item",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "i_x_quotation_item_department_analysis_capability_id",
                table: "quotation_item",
                column: "department_analysis_capability_id");

            migrationBuilder.CreateIndex(
                name: "i_x_quotation_item_subcontractor_capability_id",
                table: "quotation_item",
                column: "subcontractor_capability_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_quotation_item_department_analysis_capability_department_analysis_capability_id",
                table: "quotation_item",
                column: "department_analysis_capability_id",
                principalTable: "department_analysis_capability",
                principalColumn: "department_analysis_capability_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_quotation_item_subcontractor_capability_subcontractor_capability_id",
                table: "quotation_item",
                column: "subcontractor_capability_id",
                principalTable: "subcontractor_capability",
                principalColumn: "subcontractor_capability_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_quotation_item_department_analysis_capability_department_analysis_capability_id",
                table: "quotation_item");

            migrationBuilder.DropForeignKey(
                name: "f_k_quotation_item_subcontractor_capability_subcontractor_capability_id",
                table: "quotation_item");

            migrationBuilder.DropIndex(
                name: "i_x_quotation_item_department_analysis_capability_id",
                table: "quotation_item");

            migrationBuilder.DropIndex(
                name: "i_x_quotation_item_subcontractor_capability_id",
                table: "quotation_item");

            migrationBuilder.DropColumn(
                name: "capacity_type",
                table: "quotation_item");

            migrationBuilder.DropColumn(
                name: "department_analysis_capability_id",
                table: "quotation_item");

            migrationBuilder.DropColumn(
                name: "subcontractor_capability_id",
                table: "quotation_item");
        }
    }
}
