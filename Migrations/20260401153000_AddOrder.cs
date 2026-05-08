using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <inheritdoc />
    public partial class AddOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "order",
                columns: table => new
                {
                    order_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    client_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    testing_purpose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    test_method = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    result_turnaround_time_requirement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    result_delivery_channel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    technique = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    comparison_standard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    total = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    subtotal_before_vat = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    vat = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    payment_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    sample_receipt_method = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    laboratory_sample_retention = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    additional_information = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    test_request_confirmation = table.Column<bool>(type: "bit", nullable: true),
                    mail_document_confirmation = table.Column<bool>(type: "bit", nullable: true),
                    payment_method = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order", x => x.order_id);
                    table.ForeignKey(
                        name: "f_k_order_client_client_id",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "client_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_order_client_id",
                table: "order",
                column: "client_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order");
        }
    }
}
