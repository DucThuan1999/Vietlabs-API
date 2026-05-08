using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderSampleChildTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "order_sample_analysis_group",
                columns: table => new
                {
                    order_sample_analysis_group_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    order_sample_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    analysis_group_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    step_price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    group_sale_price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    discount_rate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_sample_analysis_group", x => x.order_sample_analysis_group_id);
                    table.ForeignKey(
                        name: "f_k_order_sample_analysis_group_account_updated_by",
                        column: x => x.updated_by,
                        principalTable: "account",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_order_sample_analysis_group_analysis_group_analysis_group_id",
                        column: x => x.analysis_group_id,
                        principalTable: "analysis_group",
                        principalColumn: "analysis_group_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_order_sample_analysis_group_order_sample_order_sample_id",
                        column: x => x.order_sample_id,
                        principalTable: "order_sample",
                        principalColumn: "order_sample_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_sample_package",
                columns: table => new
                {
                    order_sample_package_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    order_sample_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    package_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    name_vi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    name_en = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    default_price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    published_group_code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    sample_matrix_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Active"),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_sample_package", x => x.order_sample_package_id);
                    table.ForeignKey(
                        name: "f_k_order_sample_package_account_updated_by",
                        column: x => x.updated_by,
                        principalTable: "account",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_order_sample_package_order_sample_order_sample_id",
                        column: x => x.order_sample_id,
                        principalTable: "order_sample",
                        principalColumn: "order_sample_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_order_sample_package_sample_matrix_sample_matrix_id",
                        column: x => x.sample_matrix_id,
                        principalTable: "sample_matrix",
                        principalColumn: "sample_matrix_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "order_sample_item",
                columns: table => new
                {
                    order_sample_item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    order_sample_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    item_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    analysis_item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    analysis_group_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    package_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    is_standalone = table.Column<bool>(type: "bit", nullable: true),
                    capacity_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    department_analysis_capability_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    subcontractor_capability_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    item_code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    item_name_vi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    item_name_en = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    sample_matrix_name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    published_group_code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    lod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    loq = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    tat = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    default_price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    unit_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    discount_percent = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    discount_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    sub_total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    display_order = table.Column<int>(type: "int", nullable: true),
                    notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_sample_item", x => x.order_sample_item_id);
                    table.ForeignKey(
                        name: "f_k_order_sample_item_account_updated_by",
                        column: x => x.updated_by,
                        principalTable: "account",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_order_sample_item_analysis_group_analysis_group_id",
                        column: x => x.analysis_group_id,
                        principalTable: "analysis_group",
                        principalColumn: "analysis_group_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "f_k_order_sample_item_analysis_item_analysis_item_id",
                        column: x => x.analysis_item_id,
                        principalTable: "analysis_item",
                        principalColumn: "analysis_item_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "f_k_order_sample_item_department_analysis_capability_department_analysis_capability_id",
                        column: x => x.department_analysis_capability_id,
                        principalTable: "department_analysis_capability",
                        principalColumn: "department_analysis_capability_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_order_sample_item_order_sample_order_sample_id",
                        column: x => x.order_sample_id,
                        principalTable: "order_sample",
                        principalColumn: "order_sample_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_order_sample_item_package_package_id",
                        column: x => x.package_id,
                        principalTable: "package",
                        principalColumn: "package_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "f_k_order_sample_item_subcontractor_capability_subcontractor_capability_id",
                        column: x => x.subcontractor_capability_id,
                        principalTable: "subcontractor_capability",
                        principalColumn: "subcontractor_capability_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "order_sample_package_analysis_item",
                columns: table => new
                {
                    order_sample_package_analysis_item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    order_sample_package_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    analysis_item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    display_order = table.Column<int>(type: "int", nullable: true),
                    is_required = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_sample_package_analysis_item", x => x.order_sample_package_analysis_item_id);
                    table.ForeignKey(
                        name: "f_k_order_sample_package_analysis_item_account_updated_by",
                        column: x => x.updated_by,
                        principalTable: "account",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_order_sample_package_analysis_item_analysis_item_analysis_item_id",
                        column: x => x.analysis_item_id,
                        principalTable: "analysis_item",
                        principalColumn: "analysis_item_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_order_sample_package_analysis_item_order_sample_package_order_sample_package_id",
                        column: x => x.order_sample_package_id,
                        principalTable: "order_sample_package",
                        principalColumn: "order_sample_package_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_order_sample_analysis_group_analysis_group_id",
                table: "order_sample_analysis_group",
                column: "analysis_group_id");

            migrationBuilder.CreateIndex(
                name: "i_x_order_sample_analysis_group_updated_by",
                table: "order_sample_analysis_group",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "u_q_order_sample_analysis_group_sample_group",
                table: "order_sample_analysis_group",
                columns: new[] { "order_sample_id", "analysis_group_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_order_sample_package_order_sample_id",
                table: "order_sample_package",
                column: "order_sample_id");

            migrationBuilder.CreateIndex(
                name: "i_x_order_sample_package_sample_matrix_id",
                table: "order_sample_package",
                column: "sample_matrix_id");

            migrationBuilder.CreateIndex(
                name: "i_x_order_sample_package_updated_by",
                table: "order_sample_package",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "i_x_order_sample_package_analysis_item_analysis_item_id",
                table: "order_sample_package_analysis_item",
                column: "analysis_item_id");

            migrationBuilder.CreateIndex(
                name: "i_x_order_sample_package_analysis_item_updated_by",
                table: "order_sample_package_analysis_item",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "u_q_order_sample_package_analysis_item_pkg_item",
                table: "order_sample_package_analysis_item",
                columns: new[] { "order_sample_package_id", "analysis_item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_order_sample_item_analysis_group_id",
                table: "order_sample_item",
                column: "analysis_group_id");

            migrationBuilder.CreateIndex(
                name: "i_x_order_sample_item_analysis_item_id",
                table: "order_sample_item",
                column: "analysis_item_id");

            migrationBuilder.CreateIndex(
                name: "i_x_order_sample_item_department_analysis_capability_id",
                table: "order_sample_item",
                column: "department_analysis_capability_id");

            migrationBuilder.CreateIndex(
                name: "i_x_order_sample_item_order_sample_id",
                table: "order_sample_item",
                column: "order_sample_id");

            migrationBuilder.CreateIndex(
                name: "i_x_order_sample_item_package_id",
                table: "order_sample_item",
                column: "package_id");

            migrationBuilder.CreateIndex(
                name: "i_x_order_sample_item_subcontractor_capability_id",
                table: "order_sample_item",
                column: "subcontractor_capability_id");

            migrationBuilder.CreateIndex(
                name: "i_x_order_sample_item_updated_by",
                table: "order_sample_item",
                column: "updated_by");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_sample_analysis_group");

            migrationBuilder.DropTable(
                name: "order_sample_package_analysis_item");

            migrationBuilder.DropTable(
                name: "order_sample_item");

            migrationBuilder.DropTable(
                name: "order_sample_package");
        }
    }
}
