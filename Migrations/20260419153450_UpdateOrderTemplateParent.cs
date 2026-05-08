using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderTemplateParent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_order_template_analysis_group_order_sample_order_sample_id",
                table: "order_template_analysis_group");

            migrationBuilder.DropForeignKey(
                name: "f_k_order_template_item_analysis_group_analysis_group_id",
                table: "order_template_item");

            migrationBuilder.DropForeignKey(
                name: "f_k_order_template_item_analysis_item_analysis_item_id",
                table: "order_template_item");

            migrationBuilder.DropForeignKey(
                name: "f_k_order_template_item_order_sample_order_sample_id",
                table: "order_template_item");

            migrationBuilder.DropForeignKey(
                name: "f_k_order_template_item_package_package_id",
                table: "order_template_item");

            migrationBuilder.DropForeignKey(
                name: "f_k_order_template_package_order_sample_order_sample_id",
                table: "order_template_package");

            migrationBuilder.RenameColumn(
                name: "order_sample_id",
                table: "order_template_package",
                newName: "template_id");

            migrationBuilder.RenameIndex(
                name: "i_x_order_template_package_order_sample_id",
                table: "order_template_package",
                newName: "i_x_order_template_package_template_id");

            migrationBuilder.RenameColumn(
                name: "order_sample_id",
                table: "order_template_item",
                newName: "template_id");

            migrationBuilder.RenameIndex(
                name: "i_x_order_template_item_order_sample_id",
                table: "order_template_item",
                newName: "i_x_order_template_item_template_id");

            migrationBuilder.RenameColumn(
                name: "order_sample_id",
                table: "order_template_analysis_group",
                newName: "template_id");

            migrationBuilder.RenameIndex(
                name: "u_q_order_template_analysis_group_sample_group",
                table: "order_template_analysis_group",
                newName: "u_q_order_template_analysis_group_template_group");

            migrationBuilder.CreateTable(
                name: "order_template",
                columns: table => new
                {
                    template_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    order_sample_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    template_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_by_account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_template", x => x.template_id);
                    table.ForeignKey(
                        name: "f_k_order_template_account_created_by_account_id",
                        column: x => x.created_by_account_id,
                        principalTable: "account",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_order_template_order_sample_order_sample_id",
                        column: x => x.order_sample_id,
                        principalTable: "order_sample",
                        principalColumn: "order_sample_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_order_template_created_by_account_id",
                table: "order_template",
                column: "created_by_account_id");

            migrationBuilder.CreateIndex(
                name: "i_x_order_template_sample_name",
                table: "order_template",
                columns: new[] { "order_sample_id", "template_name" });

            migrationBuilder.AddForeignKey(
                name: "f_k_order_template_analysis_group_order_template_template_id",
                table: "order_template_analysis_group",
                column: "template_id",
                principalTable: "order_template",
                principalColumn: "template_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_order_template_item_analysis_group_analysis_group_id",
                table: "order_template_item",
                column: "analysis_group_id",
                principalTable: "analysis_group",
                principalColumn: "analysis_group_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_order_template_item_analysis_item_analysis_item_id",
                table: "order_template_item",
                column: "analysis_item_id",
                principalTable: "analysis_item",
                principalColumn: "analysis_item_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_order_template_item_order_template_template_id",
                table: "order_template_item",
                column: "template_id",
                principalTable: "order_template",
                principalColumn: "template_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_order_template_item_package_package_id",
                table: "order_template_item",
                column: "package_id",
                principalTable: "package",
                principalColumn: "package_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_order_template_package_order_template_template_id",
                table: "order_template_package",
                column: "template_id",
                principalTable: "order_template",
                principalColumn: "template_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_order_template_analysis_group_order_template_template_id",
                table: "order_template_analysis_group");

            migrationBuilder.DropForeignKey(
                name: "f_k_order_template_item_analysis_group_analysis_group_id",
                table: "order_template_item");

            migrationBuilder.DropForeignKey(
                name: "f_k_order_template_item_analysis_item_analysis_item_id",
                table: "order_template_item");

            migrationBuilder.DropForeignKey(
                name: "f_k_order_template_item_order_template_template_id",
                table: "order_template_item");

            migrationBuilder.DropForeignKey(
                name: "f_k_order_template_item_package_package_id",
                table: "order_template_item");

            migrationBuilder.DropForeignKey(
                name: "f_k_order_template_package_order_template_template_id",
                table: "order_template_package");

            migrationBuilder.DropTable(
                name: "order_template");

            migrationBuilder.RenameColumn(
                name: "template_id",
                table: "order_template_package",
                newName: "order_sample_id");

            migrationBuilder.RenameIndex(
                name: "i_x_order_template_package_template_id",
                table: "order_template_package",
                newName: "i_x_order_template_package_order_sample_id");

            migrationBuilder.RenameColumn(
                name: "template_id",
                table: "order_template_item",
                newName: "order_sample_id");

            migrationBuilder.RenameIndex(
                name: "i_x_order_template_item_template_id",
                table: "order_template_item",
                newName: "i_x_order_template_item_order_sample_id");

            migrationBuilder.RenameColumn(
                name: "template_id",
                table: "order_template_analysis_group",
                newName: "order_sample_id");

            migrationBuilder.RenameIndex(
                name: "u_q_order_template_analysis_group_template_group",
                table: "order_template_analysis_group",
                newName: "u_q_order_template_analysis_group_sample_group");

            migrationBuilder.AddForeignKey(
                name: "f_k_order_template_analysis_group_order_sample_order_sample_id",
                table: "order_template_analysis_group",
                column: "order_sample_id",
                principalTable: "order_sample",
                principalColumn: "order_sample_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_order_template_item_analysis_group_analysis_group_id",
                table: "order_template_item",
                column: "analysis_group_id",
                principalTable: "analysis_group",
                principalColumn: "analysis_group_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_order_template_item_analysis_item_analysis_item_id",
                table: "order_template_item",
                column: "analysis_item_id",
                principalTable: "analysis_item",
                principalColumn: "analysis_item_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_order_template_item_order_sample_order_sample_id",
                table: "order_template_item",
                column: "order_sample_id",
                principalTable: "order_sample",
                principalColumn: "order_sample_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_order_template_item_package_package_id",
                table: "order_template_item",
                column: "package_id",
                principalTable: "package",
                principalColumn: "package_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_order_template_package_order_sample_order_sample_id",
                table: "order_template_package",
                column: "order_sample_id",
                principalTable: "order_sample",
                principalColumn: "order_sample_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
