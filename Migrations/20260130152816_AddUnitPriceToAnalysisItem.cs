using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VietLab.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitPriceToAnalysisItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "analysis_group",
                columns: table => new
                {
                    analysis_group_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    analysis_group_code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    name_vi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    name_en = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Active"),
                    notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_analysis_group", x => x.analysis_group_id);
                });

            migrationBuilder.CreateTable(
                name: "branch",
                columns: table => new
                {
                    branch_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    branch_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    name_vi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    name_en = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    license = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_branch", x => x.branch_id);
                });

            migrationBuilder.CreateTable(
                name: "client",
                columns: table => new
                {
                    client_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    company_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    internal_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tax_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bank_account_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    city = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    profession = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    scale = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    customer_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    discount_rate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    representative_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    representative_email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    representative_phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    representative_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sales_owner_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sales_owner_email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sales_owner_phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_blacklisted = table.Column<bool>(type: "bit", nullable: false),
                    blacklist_reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    agent_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    forecast = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    revenue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    debt_contact_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    debt_contact_phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    debt_contact_email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    payment_method = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    last_contact_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client", x => x.client_id);
                });

            migrationBuilder.CreateTable(
                name: "country",
                columns: table => new
                {
                    country_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sequence_number = table.Column<int>(type: "int", nullable: true),
                    name_en = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    full_name_vi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    full_name_en = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    alpha_2 = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    alpha_3 = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Active"),
                    notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_country", x => x.country_id);
                });

            migrationBuilder.CreateTable(
                name: "employee",
                columns: table => new
                {
                    employee_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    employee_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    full_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee", x => x.employee_id);
                });

            migrationBuilder.CreateTable(
                name: "equipment_type",
                columns: table => new
                {
                    equipment_type_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    equipment_type_code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    name_vi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    name_en = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipment_type", x => x.equipment_type_id);
                });

            migrationBuilder.CreateTable(
                name: "permission",
                columns: table => new
                {
                    permission_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    permission_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission", x => x.permission_id);
                });

            migrationBuilder.CreateTable(
                name: "sample_matrix_group",
                columns: table => new
                {
                    sample_matrix_group_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sample_matrix_group_code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    name_vi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    name_en = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Active"),
                    notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sample_matrix_group", x => x.sample_matrix_group_id);
                });

            migrationBuilder.CreateTable(
                name: "department",
                columns: table => new
                {
                    department_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    department_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    branch_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name_vi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    name_en = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_department", x => x.department_id);
                    table.ForeignKey(
                        name: "f_k_department_branch_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branch",
                        principalColumn: "branch_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "client_debt",
                columns: table => new
                {
                    client_debt_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    client_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    payment_method = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    total_debt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    debt_term_days = table.Column<int>(type: "int", nullable: false),
                    credit_limit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    contract_effective_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    contract_end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    attachments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    last_synced_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    misa_reference_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client_debt", x => x.client_debt_id);
                    table.ForeignKey(
                        name: "f_k_client_debt_client_client_id",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "client_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "client_forecast",
                columns: table => new
                {
                    client_forecast_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    client_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    from_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    to_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    forecast_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    updated_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client_forecast", x => x.client_forecast_id);
                    table.ForeignKey(
                        name: "f_k_client_forecast_client_client_id",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "client_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contact",
                columns: table => new
                {
                    contact_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    client_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_primary = table.Column<bool>(type: "bit", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_sample_sender = table.Column<bool>(type: "bit", nullable: false),
                    is_result_receiver = table.Column<bool>(type: "bit", nullable: false),
                    is_payer = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact", x => x.contact_id);
                    table.ForeignKey(
                        name: "f_k_contact_client_client_id",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "client_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "store_record",
                columns: table => new
                {
                    store_record_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    client_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    attachment_name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    attachment_path = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    file_name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    file_size = table.Column<long>(type: "bigint", nullable: true),
                    content_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_record", x => x.store_record_id);
                    table.ForeignKey(
                        name: "f_k_store_record_client_client_id",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "client_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "province",
                columns: table => new
                {
                    province_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sequence_number = table.Column<int>(type: "int", nullable: true),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    full_name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    country_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Active"),
                    notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_province", x => x.province_id);
                    table.ForeignKey(
                        name: "f_k_province_country_country_id",
                        column: x => x.country_id,
                        principalTable: "country",
                        principalColumn: "country_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    employee_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    permission_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account", x => x.account_id);
                    table.ForeignKey(
                        name: "f_k_account_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_account_permission_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permission",
                        principalColumn: "permission_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sample_matrix",
                columns: table => new
                {
                    sample_matrix_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sample_matrix_code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    name_vi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    name_en = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    sample_matrix_group_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    registered_matrix = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Active"),
                    notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sample_matrix", x => x.sample_matrix_id);
                    table.ForeignKey(
                        name: "f_k_sample_matrix_sample_matrix_group_sample_matrix_group_id",
                        column: x => x.sample_matrix_group_id,
                        principalTable: "sample_matrix_group",
                        principalColumn: "sample_matrix_group_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quotation",
                columns: table => new
                {
                    quotation_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    quotation_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    employee_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    sales_person_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sales_person_email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sales_person_phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    client_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    agent_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    company_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    contact_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    contact_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tax_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    contact_email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    forecast = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    contact_phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    revenue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    debt_contact_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    debt_contact_phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    debt_contact_email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    payment_method = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    valid_from = table.Column<DateTime>(type: "datetime2", nullable: true),
                    valid_to = table.Column<DateTime>(type: "datetime2", nullable: true),
                    discount_percent = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    sub_total = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    total_discount_percent = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    total_discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    vat_percent = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    vat_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    total_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    quotation_discount_percent = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    client_discount_percent = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    updated_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quotation", x => x.quotation_id);
                    table.ForeignKey(
                        name: "f_k_quotation_client_client_id",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "client_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_quotation_contact_contact_id",
                        column: x => x.contact_id,
                        principalTable: "contact",
                        principalColumn: "contact_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "f_k_quotation_employee_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employee",
                        principalColumn: "employee_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ward",
                columns: table => new
                {
                    ward_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sequence_number = table.Column<int>(type: "int", nullable: true),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    province_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    country_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Active"),
                    notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ward", x => x.ward_id);
                    table.ForeignKey(
                        name: "f_k_ward_country_country_id",
                        column: x => x.country_id,
                        principalTable: "country",
                        principalColumn: "country_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_ward_province_province_id",
                        column: x => x.province_id,
                        principalTable: "province",
                        principalColumn: "province_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "client_history",
                columns: table => new
                {
                    client_history_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    client_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    changed_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    change_description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    changed_by_account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    change_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client_history", x => x.client_history_id);
                    table.ForeignKey(
                        name: "f_k_client_history_account_changed_by_account_id",
                        column: x => x.changed_by_account_id,
                        principalTable: "account",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_client_history_client_client_id",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "client_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_token",
                columns: table => new
                {
                    refresh_token_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_revoked = table.Column<bool>(type: "bit", nullable: false),
                    revoked_reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_token", x => x.refresh_token_id);
                    table.ForeignKey(
                        name: "f_k_refresh_token_account_account_id",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "analysis_item",
                columns: table => new
                {
                    analysis_item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    analysis_item_code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    name_vi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    name_en = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    organization = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    equipment_type_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    analysis_group_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sample_matrix_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sample_matrix_group_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    published_group_code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    lod = table.Column<decimal>(type: "decimal(10,3)", nullable: true),
                    loq = table.Column<decimal>(type: "decimal(10,3)", nullable: true),
                    unit_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    nd_107 = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    iso = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    cuc_bvtv = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    bo_cong_thuong = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    nafi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    cuc_chan_nuoi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Active"),
                    notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_analysis_item", x => x.analysis_item_id);
                    table.ForeignKey(
                        name: "f_k_analysis_item_analysis_group_analysis_group_id",
                        column: x => x.analysis_group_id,
                        principalTable: "analysis_group",
                        principalColumn: "analysis_group_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_analysis_item_equipment_type_equipment_type_id",
                        column: x => x.equipment_type_id,
                        principalTable: "equipment_type",
                        principalColumn: "equipment_type_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_analysis_item_sample_matrix_group_sample_matrix_group_id",
                        column: x => x.sample_matrix_group_id,
                        principalTable: "sample_matrix_group",
                        principalColumn: "sample_matrix_group_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_analysis_item_sample_matrix_sample_matrix_id",
                        column: x => x.sample_matrix_id,
                        principalTable: "sample_matrix",
                        principalColumn: "sample_matrix_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "package",
                columns: table => new
                {
                    package_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_package", x => x.package_id);
                    table.ForeignKey(
                        name: "f_k_package_sample_matrix_sample_matrix_id",
                        column: x => x.sample_matrix_id,
                        principalTable: "sample_matrix",
                        principalColumn: "sample_matrix_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "analysis_item_tat",
                columns: table => new
                {
                    analysis_item_tat_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    analysis_item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    tat_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    tat_value = table.Column<int>(type: "int", nullable: false),
                    tat_unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Days"),
                    notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_analysis_item_tat", x => x.analysis_item_tat_id);
                    table.ForeignKey(
                        name: "f_k_analysis_item_tat_analysis_item_analysis_item_id",
                        column: x => x.analysis_item_id,
                        principalTable: "analysis_item",
                        principalColumn: "analysis_item_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "department_analysis_capability",
                columns: table => new
                {
                    department_analysis_capability_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    department_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    branch_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    analysis_item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Active"),
                    notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_department_analysis_capability", x => x.department_analysis_capability_id);
                    table.ForeignKey(
                        name: "f_k_department_analysis_capability_analysis_item_analysis_item_id",
                        column: x => x.analysis_item_id,
                        principalTable: "analysis_item",
                        principalColumn: "analysis_item_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_department_analysis_capability_department_department_id",
                        column: x => x.department_id,
                        principalTable: "department",
                        principalColumn: "department_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "package_analysis_group",
                columns: table => new
                {
                    package_analysis_group_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    package_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    analysis_group_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    display_order = table.Column<int>(type: "int", nullable: true),
                    is_required = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_package_analysis_group", x => x.package_analysis_group_id);
                    table.ForeignKey(
                        name: "f_k_package_analysis_group_analysis_group_analysis_group_id",
                        column: x => x.analysis_group_id,
                        principalTable: "analysis_group",
                        principalColumn: "analysis_group_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_package_analysis_group_package_package_id",
                        column: x => x.package_id,
                        principalTable: "package",
                        principalColumn: "package_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quotation_item",
                columns: table => new
                {
                    quotation_item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    quotation_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    item_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    analysis_item_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    analysis_group_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    package_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    item_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    item_name_vi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    item_name_en = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    discount_percent = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    discount_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    sub_total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    display_order = table.Column<int>(type: "int", nullable: true),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quotation_item", x => x.quotation_item_id);
                    table.ForeignKey(
                        name: "f_k_quotation_item_analysis_group_analysis_group_id",
                        column: x => x.analysis_group_id,
                        principalTable: "analysis_group",
                        principalColumn: "analysis_group_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "f_k_quotation_item_analysis_item_analysis_item_id",
                        column: x => x.analysis_item_id,
                        principalTable: "analysis_item",
                        principalColumn: "analysis_item_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "f_k_quotation_item_package_package_id",
                        column: x => x.package_id,
                        principalTable: "package",
                        principalColumn: "package_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "f_k_quotation_item_quotation_quotation_id",
                        column: x => x.quotation_id,
                        principalTable: "quotation",
                        principalColumn: "quotation_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "analysis_group",
                columns: new[] { "analysis_group_id", "analysis_group_code", "created_at", "name_en", "name_vi", "notes", "status", "updated_at" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-0001-0001-0001-000000000001"), "AG-001", new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(2902), "Hematology", "Huyết học", "Nhóm chỉ tiêu về huyết học", "Active", null },
                    { new Guid("aaaaaaaa-0002-0002-0002-000000000002"), "AG-002", new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(2908), "Biochemistry", "Sinh hóa", "Nhóm chỉ tiêu về sinh hóa", "Active", null },
                    { new Guid("aaaaaaaa-0003-0003-0003-000000000003"), "AG-003", new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(2910), "Microbiology", "Vi sinh", "Nhóm chỉ tiêu về vi sinh", "Active", null },
                    { new Guid("aaaaaaaa-0004-0004-0004-000000000004"), "AG-004", new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(2912), "Immunology", "Miễn dịch", "Nhóm chỉ tiêu về miễn dịch", "Active", null },
                    { new Guid("aaaaaaaa-0005-0005-0005-000000000005"), "AG-005", new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(2914), "Urine Analysis", "Nước tiểu", "Nhóm chỉ tiêu về nước tiểu", "Active", null },
                    { new Guid("aaaaaaaa-0006-0006-0006-000000000006"), "AG-006", new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(2916), "Serology", "Huyết thanh học", "Nhóm chỉ tiêu về huyết thanh học", "Active", null }
                });

            migrationBuilder.InsertData(
                table: "branch",
                columns: new[] { "branch_id", "branch_code", "license", "name_en", "name_vi", "notes", "status" },
                values: new object[,]
                {
                    { new Guid("66666666-6666-6666-6666-666666666666"), "BR-001", "CN-2023-HN-001", "Hanoi Branch", "Chi nhánh Hà Nội", "Trụ sở chính miền Bắc", "Active" },
                    { new Guid("77777777-7777-7777-7777-777777777777"), "BR-002", "CN-2023-HCM-002", "HCMC Branch", "Chi nhánh TP. Hồ Chí Minh", "Trụ sở chính miền Nam", "Active" },
                    { new Guid("88888888-8888-8888-8888-888888888888"), "BR-003", "CN-2023-DN-003", "Da Nang Branch", "Chi nhánh Đà Nẵng", "Chi nhánh miền Trung", "Active" },
                    { new Guid("99999999-9999-9999-9999-999999999999"), "BR-004", "CN-2023-CT-004", "Can Tho Branch", "Chi nhánh Cần Thơ", "Chi nhánh Đồng bằng sông Cửu Long", "Active" }
                });

            migrationBuilder.InsertData(
                table: "client",
                columns: new[] { "client_id", "address", "agent_name", "bank_account_number", "blacklist_reason", "city", "company_name", "country", "created_date", "customer_type", "debt_contact_email", "debt_contact_name", "debt_contact_phone", "discount_rate", "forecast", "internal_code", "is_blacklisted", "last_contact_date", "notes", "payment_method", "profession", "representative_email", "representative_name", "representative_phone", "representative_title", "revenue", "sales_owner_email", "sales_owner_name", "sales_owner_phone", "scale", "status", "tax_code" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "123 Đường XYZ", null, "1234567890", "", "Hà Nội", "Công ty ABC", "Việt Nam", new DateTime(2026, 1, 30, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2482), "Enterprise", null, null, null, 5m, null, "CLI-ABC-001", false, new DateTime(2026, 1, 25, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2515), "Khách hàng tiềm năng cao", null, "Công nghệ thông tin", "contact@abc.com", "Nguyễn Văn A", "0123456789", "Giám đốc", null, "an.nguyen@viet-labs.com", "Nguyễn Văn An", "0900000001", "200 nhân sự", "Active", "0101234567" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "456 Đường ABC", null, "2233445566", "", "TP. Hồ Chí Minh", "Công ty XYZ", "Việt Nam", new DateTime(2025, 12, 31, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2599), "SMB", null, null, null, 3m, null, "CLI-XYZ-002", false, new DateTime(2026, 1, 28, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2600), "Khách hàng thân thiết", null, "Thương mại điện tử", "info@xyz.com", "Trần Thị B", "0987654321", "Trưởng phòng mua hàng", null, "mai.pham@viet-labs.com", "Phạm Thị Mai", "0900000002", "120 nhân sự", "Active", "0202345678" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "789 Đường DEF", null, "9988776655", "Đang rà soát công nợ", "Đà Nẵng", "Công ty DEF", "Việt Nam", new DateTime(2025, 12, 1, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2607), "Prospect", null, null, null, 0m, null, "CLI-DEF-003", true, null, "Đang trong quá trình tư vấn", null, "Sản xuất", "hello@def.com", "Lê Văn C", "0912345678", "Phó giám đốc", null, "binh.tran@viet-labs.com", "Trần Văn Bình", "0900000003", "80 nhân sự", "Prospect", "0303456789" },
                    { new Guid("44444444-1111-1111-1111-111111111111"), "321 Đường GHI", null, "1122334455", "", "Hà Nội", "Công ty GHI", "Việt Nam", new DateTime(2026, 1, 15, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2616), "Enterprise", null, null, null, 7m, null, "CLI-GHI-004", false, new DateTime(2026, 1, 29, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2617), "Khách hàng VIP", null, "Tài chính - Ngân hàng", "contact@ghi.com", "Phạm Văn D", "0123456780", "Tổng giám đốc", null, "an.nguyen@viet-labs.com", "Nguyễn Văn An", "0900000001", "350 nhân sự", "Active", "0404567890" },
                    { new Guid("55555555-2222-2222-2222-222222222222"), "654 Đường JKL", null, "5566778899", "", "TP. Hồ Chí Minh", "Công ty JKL", "Việt Nam", new DateTime(2025, 12, 16, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2624), "SMB", null, null, null, 4m, null, "CLI-JKL-005", false, new DateTime(2026, 1, 27, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2625), "Khách hàng ổn định", null, "Bán lẻ", "info@jkl.com", "Hoàng Thị E", "0987654320", "Giám đốc Kinh doanh", null, "mai.pham@viet-labs.com", "Phạm Thị Mai", "0900000002", "150 nhân sự", "Active", "0505678901" },
                    { new Guid("66666666-3333-3333-3333-333333333333"), "987 Đường MNO", null, "9988776655", "", "Hải Phòng", "Công ty MNO", "Việt Nam", new DateTime(2026, 1, 10, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2633), "SMB", null, null, null, 2m, null, "CLI-MNO-006", false, new DateTime(2026, 1, 23, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2635), "Khách hàng mới", null, "Logistics", "contact@mno.com", "Vũ Văn F", "0912345670", "Giám đốc", null, "an.nguyen@viet-labs.com", "Nguyễn Văn An", "0900000001", "90 nhân sự", "Active", "0606789012" },
                    { new Guid("77777777-4444-4444-4444-444444444444"), "147 Đường PQR", null, "3344556677", "", "Đà Nẵng", "Công ty PQR", "Việt Nam", new DateTime(2026, 1, 20, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2642), "Prospect", null, null, null, 0m, null, "CLI-PQR-007", false, new DateTime(2026, 1, 26, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2643), "Đang tư vấn", null, "Du lịch", "info@pqr.com", "Đỗ Thị G", "0123456709", "Trưởng phòng", null, "binh.tran@viet-labs.com", "Trần Văn Bình", "0900000003", "60 nhân sự", "Prospect", "0707890123" },
                    { new Guid("88888888-5555-5555-5555-555555555555"), "258 Đường STU", null, "7788990011", "", "Cần Thơ", "Công ty STU", "Việt Nam", new DateTime(2026, 1, 5, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2652), "SMB", null, null, null, 3m, null, "CLI-STU-008", false, new DateTime(2026, 1, 24, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2653), "Khách hàng tiềm năng", null, "Nông nghiệp", "contact@stu.com", "Bùi Văn H", "0987654309", "Giám đốc", null, "mai.pham@viet-labs.com", "Phạm Thị Mai", "0900000002", "100 nhân sự", "Active", "0808901234" },
                    { new Guid("99999999-6666-6666-6666-666666666666"), "369 Đường VWX", null, "2233445566", "", "Hà Nội", "Công ty VWX", "Việt Nam", new DateTime(2025, 12, 21, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2660), "Enterprise", null, null, null, 6m, null, "CLI-VWX-009", false, new DateTime(2026, 1, 28, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2661), "Khách hàng thân thiết", null, "Giáo dục", "info@vwx.com", "Lý Thị I", "0912345608", "Hiệu trưởng", null, "an.nguyen@viet-labs.com", "Nguyễn Văn An", "0900000001", "180 nhân sự", "Active", "0909012345" },
                    { new Guid("aaaaaaaa-7777-7777-7777-777777777777"), "741 Đường YZA", null, "4455667788", "", "TP. Hồ Chí Minh", "Công ty YZA", "Việt Nam", new DateTime(2025, 12, 26, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2667), "Enterprise", null, null, null, 8m, null, "CLI-YZA-010", false, new DateTime(2026, 1, 29, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2667), "Khách hàng VIP", null, "Y tế", "contact@yza.com", "Ngô Văn J", "0123456708", "Giám đốc", null, "mai.pham@viet-labs.com", "Phạm Thị Mai", "0900000002", "250 nhân sự", "Active", "1010123456" },
                    { new Guid("bbbbbbbb-8888-8888-8888-888888888888"), "852 Đường BCD", null, "6677889900", "", "Đà Nẵng", "Công ty BCD", "Việt Nam", new DateTime(2025, 12, 11, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2674), "SMB", null, null, null, 3m, null, "CLI-BCD-011", false, new DateTime(2026, 1, 22, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2675), "Khách hàng ổn định", null, "Xây dựng", "info@bcd.com", "Trương Thị K", "0987654308", "Giám đốc", null, "binh.tran@viet-labs.com", "Trần Văn Bình", "0900000003", "110 nhân sự", "Active", "1111234567" },
                    { new Guid("cccccccc-9999-9999-9999-999999999999"), "963 Đường EFG", null, "8899001122", "", "Hà Nội", "Công ty EFG", "Việt Nam", new DateTime(2026, 1, 25, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2684), "Prospect", null, null, null, 0m, null, "CLI-EFG-012", false, new DateTime(2026, 1, 29, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2685), "Đang tư vấn", null, "Truyền thông", "contact@efg.com", "Đinh Văn L", "0912345607", "Trưởng phòng", null, "an.nguyen@viet-labs.com", "Nguyễn Văn An", "0900000001", "70 nhân sự", "Prospect", "1212345678" },
                    { new Guid("dddddddd-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "159 Đường HIJ", null, "0011223344", "", "TP. Hồ Chí Minh", "Công ty HIJ", "Việt Nam", new DateTime(2025, 12, 6, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2691), "SMB", null, null, null, 4m, null, "CLI-HIJ-013", false, new DateTime(2026, 1, 26, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2691), "Khách hàng thân thiết", null, "Thực phẩm", "info@hij.com", "Phan Thị M", "0123456707", "Giám đốc", null, "mai.pham@viet-labs.com", "Phạm Thị Mai", "0900000002", "140 nhân sự", "Active", "1313456789" },
                    { new Guid("eeeeeeee-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "357 Đường KLM", null, "2233445566", "", "Cần Thơ", "Công ty KLM", "Việt Nam", new DateTime(2025, 11, 21, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2696), "SMB", null, null, null, 2m, null, "CLI-KLM-014", false, new DateTime(2026, 1, 20, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2697), "Khách hàng tạm dừng", null, "Năng lượng", "contact@klm.com", "Võ Văn N", "0987654307", "Giám đốc", null, "mai.pham@viet-labs.com", "Phạm Thị Mai", "0900000002", "95 nhân sự", "Inactive", "1414567890" },
                    { new Guid("ffffffff-cccc-cccc-cccc-cccccccccccc"), "753 Đường NOP", null, "5566778899", "", "Hà Nội", "Công ty NOP", "Việt Nam", new DateTime(2025, 11, 1, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2703), "Enterprise", null, null, null, 10m, null, "CLI-NOP-015", false, new DateTime(2026, 1, 29, 22, 28, 15, 133, DateTimeKind.Local).AddTicks(2704), "Khách hàng chiến lược", null, "Công nghệ thông tin", "info@nop.com", "Lê Văn O", "0912345606", "Tổng giám đốc", null, "an.nguyen@viet-labs.com", "Nguyễn Văn An", "0900000001", "300 nhân sự", "Active", "1515678901" }
                });

            migrationBuilder.InsertData(
                table: "employee",
                columns: new[] { "employee_id", "department", "email", "employee_code", "full_name", "notes", "role", "status", "title" },
                values: new object[,]
                {
                    { new Guid("11111111-eeee-eeee-eeee-eeeeeeeeeeee"), "Kinh doanh", "binh.tran@viet-labs.com", "EMP-003", "Trần Văn Bình", "Phụ trách khách hàng miền Trung", "Sales Executive", "Active", "Chuyên viên Kinh doanh" },
                    { new Guid("22222222-ffff-ffff-ffff-ffffffffffff"), "Kinh doanh", "mai.pham@viet-labs.com", "EMP-004", "Phạm Thị Mai", "Phụ trách khách hàng miền Nam", "Sales Executive", "Active", "Chuyên viên Kinh doanh" },
                    { new Guid("33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Kỹ thuật", "duc.hoang@viet-labs.com", "EMP-005", "Hoàng Văn Đức", "Phát triển hệ thống CRM", "Senior Developer", "Active", "Kỹ sư phần mềm" },
                    { new Guid("44444444-1111-1111-1111-111111111111"), "Hành chính", "lan.vu@viet-labs.com", "EMP-006", "Vũ Thị Lan", "Quản lý nhân sự và tuyển dụng", "HR Manager", "Active", "Trưởng phòng Nhân sự" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Kinh doanh", "an.nguyen@viet-labs.com", "EMP-001", "Nguyễn Văn An", "Phụ trách khách hàng miền Bắc", "Sales Manager", "Active", "Giám đốc Kinh doanh" },
                    { new Guid("55555555-2222-2222-2222-222222222222"), "Tài chính", "hung.do@viet-labs.com", "EMP-007", "Đỗ Văn Hùng", "Quản lý tài chính và kế toán", "Finance Manager", "Active", "Trưởng phòng Tài chính" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "Kỹ thuật", "huong.le@viet-labs.com", "EMP-002", "Lê Thị Hương", "Phụ trách tích hợp kỹ thuật", "Tech Lead", "Active", "Trưởng phòng Kỹ thuật" },
                    { new Guid("66666666-3333-3333-3333-333333333333"), "Marketing", "hoa.bui@viet-labs.com", "EMP-008", "Bùi Thị Hoa", "Phụ trách marketing và truyền thông", "Marketing Specialist", "Active", "Chuyên viên Marketing" }
                });

            migrationBuilder.InsertData(
                table: "package",
                columns: new[] { "package_id", "created_at", "default_price", "description", "name_en", "name_vi", "notes", "package_code", "published_group_code", "sample_matrix_id", "status", "updated_at" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-0001-0001-0001-000000000001"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(2969), 1500000.00m, "Gói xét nghiệm tổng quát bao gồm các chỉ tiêu cơ bản về huyết học, sinh hóa và nước tiểu", "General Health Check Package", "Gói xét nghiệm tổng quát", "Gói phù hợp cho khám sức khỏe định kỳ", "PKG-001", "PP-001", null, "Active", null },
                    { new Guid("bbbbbbbb-0002-0002-0002-000000000002"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(2973), 3500000.00m, "Gói xét nghiệm nâng cao bao gồm đầy đủ các chỉ tiêu: huyết học, sinh hóa, vi sinh, miễn dịch", "Advanced Health Check Package", "Gói xét nghiệm nâng cao", "Gói phù hợp cho khám sức khỏe toàn diện", "PKG-002", "PP-002", null, "Active", null },
                    { new Guid("bbbbbbbb-0003-0003-0003-000000000003"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(2981), 800000.00m, "Gói xét nghiệm cơ bản chỉ bao gồm huyết học và sinh hóa", "Basic Health Check Package", "Gói xét nghiệm cơ bản", "Gói phù hợp cho khám sức khỏe đơn giản", "PKG-003", "PP-003", null, "Active", null },
                    { new Guid("bbbbbbbb-0004-0004-0004-000000000004"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(2984), 2500000.00m, "Gói xét nghiệm chuyên sâu về vi sinh và miễn dịch", "Microbiology Package", "Gói xét nghiệm vi sinh", "Gói phù hợp cho xét nghiệm nhiễm trùng", "PKG-004", "PP-004", null, "Active", null },
                    { new Guid("bbbbbbbb-0005-0005-0005-000000000005"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(2987), 5000000.00m, "Gói xét nghiệm đầy đủ tất cả các chỉ tiêu có sẵn", "Comprehensive Health Package", "Gói xét nghiệm chuyên sâu", "Gói phù hợp cho khám sức khỏe toàn diện nhất", "PKG-005", "PP-005", null, "Active", null }
                });

            migrationBuilder.InsertData(
                table: "permission",
                columns: new[] { "permission_id", "name", "notes", "permission_code", "status" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Quản trị hệ thống", "Toàn quyền", "PERM-ADMIN", "Active" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Người dùng", "Quyền người dùng tiêu chuẩn", "PERM-USER", "Active" },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Quản lý", "Quyền quản lý phòng ban", "PERM-MANAGER", "Active" },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Kinh doanh", "Quyền truy cập module kinh doanh", "PERM-SALES", "Active" }
                });

            migrationBuilder.InsertData(
                table: "account",
                columns: new[] { "account_id", "employee_id", "password_hash", "permission_id", "status", "user_name" },
                values: new object[,]
                {
                    { new Guid("17171717-1717-1717-1717-171717171717"), new Guid("55555555-2222-2222-2222-222222222222"), "hashed-password-7", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Active", "hung.do" },
                    { new Guid("28282828-2828-2828-2828-282828282828"), new Guid("66666666-3333-3333-3333-333333333333"), "hashed-password-8", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Active", "hoa.bui" },
                    { new Guid("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1"), new Guid("44444444-4444-4444-4444-444444444444"), "hashed-password-1", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Active", "an.nguyen" },
                    { new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"), new Guid("55555555-5555-5555-5555-555555555555"), "hashed-password-2", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Active", "huong.le" },
                    { new Guid("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3"), new Guid("11111111-eeee-eeee-eeee-eeeeeeeeeeee"), "hashed-password-3", new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Active", "binh.tran" },
                    { new Guid("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4"), new Guid("22222222-ffff-ffff-ffff-ffffffffffff"), "hashed-password-4", new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Active", "mai.pham" },
                    { new Guid("e5e5e5e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5"), new Guid("33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "hashed-password-5", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Active", "duc.hoang" },
                    { new Guid("f6f6f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6"), new Guid("44444444-1111-1111-1111-111111111111"), "hashed-password-6", new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Active", "lan.vu" }
                });

            migrationBuilder.InsertData(
                table: "contact",
                columns: new[] { "contact_id", "client_id", "department", "email", "full_name", "is_payer", "is_primary", "is_result_receiver", "is_sample_sender", "notes", "phone", "title" },
                values: new object[,]
                {
                    { new Guid("aaaaaaa1-0000-0000-0000-000000000001"), new Guid("11111111-1111-1111-1111-111111111111"), "Kinh doanh", "an.nguyen@company.com", "Nguyễn Văn An", false, true, false, false, null, "0901234567", "Giám đốc Kinh doanh" },
                    { new Guid("aaaaaaa1-0000-0000-0000-000000000002"), new Guid("11111111-1111-1111-1111-111111111111"), "Kỹ thuật", "huong.le@company.com", "Lê Thị Hương", false, false, false, false, null, "0912345678", "Trưởng phòng Kỹ thuật" },
                    { new Guid("aaaaaaa1-0000-0000-0000-000000000003"), new Guid("22222222-2222-2222-2222-222222222222"), "Mua hàng", "b.tran@xyz.com", "Trần Thị B", false, true, false, false, null, "0987654321", "Trưởng phòng mua hàng" },
                    { new Guid("aaaaaaa1-0000-0000-0000-000000000004"), new Guid("22222222-2222-2222-2222-222222222222"), "Kế toán", "c.pham@xyz.com", "Phạm Văn C", false, false, false, false, null, "0987654322", "Kế toán trưởng" },
                    { new Guid("aaaaaaa1-0000-0000-0000-000000000005"), new Guid("44444444-1111-1111-1111-111111111111"), "Điều hành", "d.pham@ghi.com", "Phạm Văn D", false, true, false, false, null, "0123456780", "Tổng giám đốc" },
                    { new Guid("aaaaaaa1-0000-0000-0000-000000000006"), new Guid("44444444-1111-1111-1111-111111111111"), "Tài chính", "e.nguyen@ghi.com", "Nguyễn Thị E", false, false, false, false, null, "0123456781", "Giám đốc Tài chính" },
                    { new Guid("aaaaaaa1-0000-0000-0000-000000000007"), new Guid("55555555-2222-2222-2222-222222222222"), "Kinh doanh", "e.hoang@jkl.com", "Hoàng Thị E", false, true, false, false, null, "0987654320", "Giám đốc Kinh doanh" },
                    { new Guid("aaaaaaa1-0000-0000-0000-000000000008"), new Guid("66666666-3333-3333-3333-333333333333"), "Điều hành", "f.vu@mno.com", "Vũ Văn F", false, true, false, false, null, "0912345670", "Giám đốc" },
                    { new Guid("aaaaaaa1-0000-0000-0000-000000000009"), new Guid("99999999-6666-6666-6666-666666666666"), "Điều hành", "i.ly@vwx.com", "Lý Thị I", false, true, false, false, null, "0912345608", "Hiệu trưởng" },
                    { new Guid("aaaaaaa1-0000-0000-0000-000000000010"), new Guid("99999999-6666-6666-6666-666666666666"), "Hành chính", "j.tran@vwx.com", "Trần Văn J", false, false, false, false, null, "0912345609", "Phó hiệu trưởng" },
                    { new Guid("aaaaaaa1-0000-0000-0000-000000000011"), new Guid("aaaaaaaa-7777-7777-7777-777777777777"), "Điều hành", "j.ngo@yza.com", "Ngô Văn J", false, true, false, false, null, "0123456708", "Giám đốc" },
                    { new Guid("aaaaaaa1-0000-0000-0000-000000000012"), new Guid("aaaaaaaa-7777-7777-7777-777777777777"), "Y tế", "k.le@yza.com", "Lê Thị K", false, false, false, false, null, "0123456709", "Trưởng khoa" },
                    { new Guid("aaaaaaa1-0000-0000-0000-000000000013"), new Guid("ffffffff-cccc-cccc-cccc-cccccccccccc"), "Điều hành", "o.le@nop.com", "Lê Văn O", false, true, false, false, null, "0912345606", "Tổng giám đốc" },
                    { new Guid("aaaaaaa1-0000-0000-0000-000000000014"), new Guid("ffffffff-cccc-cccc-cccc-cccccccccccc"), "Kỹ thuật", "p.pham@nop.com", "Phạm Thị P", false, false, false, false, null, "0912345607", "CTO" },
                    { new Guid("aaaaaaa1-0000-0000-0000-000000000015"), new Guid("ffffffff-cccc-cccc-cccc-cccccccccccc"), "Kinh doanh", "q.hoang@nop.com", "Hoàng Văn Q", false, false, false, false, null, "0912345608", "Giám đốc Kinh doanh" }
                });

            migrationBuilder.InsertData(
                table: "department",
                columns: new[] { "department_id", "branch_id", "department_code", "name_en", "name_vi", "notes", "status" },
                values: new object[,]
                {
                    { new Guid("88888888-8888-8888-8888-888888888888"), new Guid("66666666-6666-6666-6666-666666666666"), "DEP-HN-KD", "Hanoi Sales Department", "Phòng Kinh doanh Hà Nội", "Phòng ban kinh doanh tại chi nhánh Hà Nội", "Active" },
                    { new Guid("99999999-9999-9999-9999-999999999999"), new Guid("77777777-7777-7777-7777-777777777777"), "DEP-HCM-KT", "HCMC Engineering Department", "Phòng Kỹ thuật HCM", "Phòng ban kỹ thuật tại chi nhánh HCM", "Active" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("66666666-6666-6666-6666-666666666666"), "DEP-HN-KT", "Hanoi Engineering Department", "Phòng Kỹ thuật Hà Nội", "Phòng ban kỹ thuật tại chi nhánh Hà Nội", "Active" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("77777777-7777-7777-7777-777777777777"), "DEP-HCM-KD", "HCMC Sales Department", "Phòng Kinh doanh HCM", "Phòng ban kinh doanh tại chi nhánh HCM", "Active" },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("66666666-6666-6666-6666-666666666666"), "DEP-HN-HC", "Hanoi Administration Department", "Phòng Hành chính Hà Nội", "Phòng ban hành chính tại chi nhánh Hà Nội", "Active" },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new Guid("88888888-8888-8888-8888-888888888888"), "DEP-DN-KD", "Da Nang Sales Department", "Phòng Kinh doanh Đà Nẵng", "Phòng ban kinh doanh tại chi nhánh Đà Nẵng", "Active" },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new Guid("88888888-8888-8888-8888-888888888888"), "DEP-DN-KT", "Da Nang Engineering Department", "Phòng Kỹ thuật Đà Nẵng", "Phòng ban kỹ thuật tại chi nhánh Đà Nẵng", "Active" },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), new Guid("99999999-9999-9999-9999-999999999999"), "DEP-CT-KD", "Can Tho Sales Department", "Phòng Kinh doanh Cần Thơ", "Phòng ban kinh doanh tại chi nhánh Cần Thơ", "Active" }
                });

            migrationBuilder.InsertData(
                table: "package_analysis_group",
                columns: new[] { "package_analysis_group_id", "analysis_group_id", "created_at", "display_order", "is_required", "notes", "package_id" },
                values: new object[,]
                {
                    { new Guid("cccccccc-0001-0001-0001-000000000001"), new Guid("aaaaaaaa-0001-0001-0001-000000000001"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(3020), 1, true, "Nhóm chỉ tiêu bắt buộc", new Guid("bbbbbbbb-0001-0001-0001-000000000001") },
                    { new Guid("cccccccc-0001-0002-0002-000000000002"), new Guid("aaaaaaaa-0002-0002-0002-000000000002"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(3025), 2, true, "Nhóm chỉ tiêu bắt buộc", new Guid("bbbbbbbb-0001-0001-0001-000000000001") },
                    { new Guid("cccccccc-0001-0003-0003-000000000003"), new Guid("aaaaaaaa-0005-0005-0005-000000000005"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(3029), 3, true, "Nhóm chỉ tiêu bắt buộc", new Guid("bbbbbbbb-0001-0001-0001-000000000001") },
                    { new Guid("cccccccc-0002-0001-0001-000000000001"), new Guid("aaaaaaaa-0001-0001-0001-000000000001"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(3031), 1, true, "Nhóm chỉ tiêu bắt buộc", new Guid("bbbbbbbb-0002-0002-0002-000000000002") },
                    { new Guid("cccccccc-0002-0002-0002-000000000002"), new Guid("aaaaaaaa-0002-0002-0002-000000000002"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(3034), 2, true, "Nhóm chỉ tiêu bắt buộc", new Guid("bbbbbbbb-0002-0002-0002-000000000002") },
                    { new Guid("cccccccc-0002-0003-0003-000000000003"), new Guid("aaaaaaaa-0003-0003-0003-000000000003"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(3038), 3, true, "Nhóm chỉ tiêu bắt buộc", new Guid("bbbbbbbb-0002-0002-0002-000000000002") },
                    { new Guid("cccccccc-0002-0004-0004-000000000004"), new Guid("aaaaaaaa-0004-0004-0004-000000000004"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(3043), 4, true, "Nhóm chỉ tiêu bắt buộc", new Guid("bbbbbbbb-0002-0002-0002-000000000002") },
                    { new Guid("cccccccc-0002-0005-0005-000000000005"), new Guid("aaaaaaaa-0005-0005-0005-000000000005"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(3046), 5, true, "Nhóm chỉ tiêu bắt buộc", new Guid("bbbbbbbb-0002-0002-0002-000000000002") },
                    { new Guid("cccccccc-0002-0006-0006-000000000006"), new Guid("aaaaaaaa-0006-0006-0006-000000000006"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(3048), 6, true, "Nhóm chỉ tiêu bắt buộc", new Guid("bbbbbbbb-0002-0002-0002-000000000002") },
                    { new Guid("cccccccc-0003-0001-0001-000000000001"), new Guid("aaaaaaaa-0001-0001-0001-000000000001"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(3052), 1, true, "Nhóm chỉ tiêu bắt buộc", new Guid("bbbbbbbb-0003-0003-0003-000000000003") },
                    { new Guid("cccccccc-0003-0002-0002-000000000002"), new Guid("aaaaaaaa-0002-0002-0002-000000000002"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(3054), 2, true, "Nhóm chỉ tiêu bắt buộc", new Guid("bbbbbbbb-0003-0003-0003-000000000003") },
                    { new Guid("cccccccc-0004-0001-0001-000000000001"), new Guid("aaaaaaaa-0003-0003-0003-000000000003"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(3057), 1, true, "Nhóm chỉ tiêu bắt buộc", new Guid("bbbbbbbb-0004-0004-0004-000000000004") },
                    { new Guid("cccccccc-0004-0002-0002-000000000002"), new Guid("aaaaaaaa-0004-0004-0004-000000000004"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(3060), 2, true, "Nhóm chỉ tiêu bắt buộc", new Guid("bbbbbbbb-0004-0004-0004-000000000004") }
                });

            migrationBuilder.InsertData(
                table: "package_analysis_group",
                columns: new[] { "package_analysis_group_id", "analysis_group_id", "created_at", "display_order", "notes", "package_id" },
                values: new object[] { new Guid("cccccccc-0004-0003-0003-000000000003"), new Guid("aaaaaaaa-0006-0006-0006-000000000006"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(3062), 3, "Nhóm chỉ tiêu tùy chọn", new Guid("bbbbbbbb-0004-0004-0004-000000000004") });

            migrationBuilder.InsertData(
                table: "package_analysis_group",
                columns: new[] { "package_analysis_group_id", "analysis_group_id", "created_at", "display_order", "is_required", "notes", "package_id" },
                values: new object[,]
                {
                    { new Guid("cccccccc-0005-0001-0001-000000000001"), new Guid("aaaaaaaa-0001-0001-0001-000000000001"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(3065), 1, true, "Nhóm chỉ tiêu bắt buộc", new Guid("bbbbbbbb-0005-0005-0005-000000000005") },
                    { new Guid("cccccccc-0005-0002-0002-000000000002"), new Guid("aaaaaaaa-0002-0002-0002-000000000002"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(3068), 2, true, "Nhóm chỉ tiêu bắt buộc", new Guid("bbbbbbbb-0005-0005-0005-000000000005") },
                    { new Guid("cccccccc-0005-0003-0003-000000000003"), new Guid("aaaaaaaa-0003-0003-0003-000000000003"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(3071), 3, true, "Nhóm chỉ tiêu bắt buộc", new Guid("bbbbbbbb-0005-0005-0005-000000000005") },
                    { new Guid("cccccccc-0005-0004-0004-000000000004"), new Guid("aaaaaaaa-0004-0004-0004-000000000004"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(3074), 4, true, "Nhóm chỉ tiêu bắt buộc", new Guid("bbbbbbbb-0005-0005-0005-000000000005") },
                    { new Guid("cccccccc-0005-0005-0005-000000000005"), new Guid("aaaaaaaa-0005-0005-0005-000000000005"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(3077), 5, true, "Nhóm chỉ tiêu bắt buộc", new Guid("bbbbbbbb-0005-0005-0005-000000000005") }
                });

            migrationBuilder.InsertData(
                table: "package_analysis_group",
                columns: new[] { "package_analysis_group_id", "analysis_group_id", "created_at", "display_order", "notes", "package_id" },
                values: new object[] { new Guid("cccccccc-0005-0006-0006-000000000006"), new Guid("aaaaaaaa-0006-0006-0006-000000000006"), new DateTime(2026, 1, 30, 15, 28, 15, 133, DateTimeKind.Utc).AddTicks(3079), 6, "Nhóm chỉ tiêu tùy chọn - có thể bỏ qua", new Guid("bbbbbbbb-0005-0005-0005-000000000005") });

            migrationBuilder.CreateIndex(
                name: "i_x_account_employee_id",
                table: "account",
                column: "employee_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_account_permission_id",
                table: "account",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "i_x_analysis_item_analysis_group_id",
                table: "analysis_item",
                column: "analysis_group_id");

            migrationBuilder.CreateIndex(
                name: "i_x_analysis_item_equipment_type_id",
                table: "analysis_item",
                column: "equipment_type_id");

            migrationBuilder.CreateIndex(
                name: "i_x_analysis_item_sample_matrix_group_id",
                table: "analysis_item",
                column: "sample_matrix_group_id");

            migrationBuilder.CreateIndex(
                name: "i_x_analysis_item_sample_matrix_id",
                table: "analysis_item",
                column: "sample_matrix_id");

            migrationBuilder.CreateIndex(
                name: "i_x_analysis_item_tat_analysis_item_id",
                table: "analysis_item_tat",
                column: "analysis_item_id");

            migrationBuilder.CreateIndex(
                name: "i_x_analysis_item_tat_item_type",
                table: "analysis_item_tat",
                columns: new[] { "analysis_item_id", "tat_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_analysis_item_tat_tat_type",
                table: "analysis_item_tat",
                column: "tat_type");

            migrationBuilder.CreateIndex(
                name: "i_x_client_debt_client_id",
                table: "client_debt",
                column: "client_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_client_forecast_client_id",
                table: "client_forecast",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "i_x_client_history_changed_by_account_id",
                table: "client_history",
                column: "changed_by_account_id");

            migrationBuilder.CreateIndex(
                name: "i_x_client_history_changed_date",
                table: "client_history",
                column: "changed_date");

            migrationBuilder.CreateIndex(
                name: "i_x_client_history_client_id",
                table: "client_history",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "i_x_contact_client_id",
                table: "contact",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "i_x_country_alpha_2",
                table: "country",
                column: "alpha_2");

            migrationBuilder.CreateIndex(
                name: "i_x_country_alpha_3",
                table: "country",
                column: "alpha_3");

            migrationBuilder.CreateIndex(
                name: "i_x_country_name_en",
                table: "country",
                column: "name_en");

            migrationBuilder.CreateIndex(
                name: "i_x_country_status",
                table: "country",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "i_x_department_branch_id",
                table: "department",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "i_x_department_analysis_capability_analysis_item_id",
                table: "department_analysis_capability",
                column: "analysis_item_id");

            migrationBuilder.CreateIndex(
                name: "i_x_department_analysis_capability_unique",
                table: "department_analysis_capability",
                columns: new[] { "department_id", "branch_id", "analysis_item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_package_sample_matrix_id",
                table: "package",
                column: "sample_matrix_id");

            migrationBuilder.CreateIndex(
                name: "i_x_package_analysis_group_analysis_group_id",
                table: "package_analysis_group",
                column: "analysis_group_id");

            migrationBuilder.CreateIndex(
                name: "u_q_package_analysis_group_package_group",
                table: "package_analysis_group",
                columns: new[] { "package_id", "analysis_group_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_province_country_id",
                table: "province",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "i_x_province_name",
                table: "province",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "i_x_province_status",
                table: "province",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "i_x_quotation_client_id",
                table: "quotation",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "i_x_quotation_contact_id",
                table: "quotation",
                column: "contact_id");

            migrationBuilder.CreateIndex(
                name: "i_x_quotation_employee_id",
                table: "quotation",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "i_x_quotation_item_analysis_group_id",
                table: "quotation_item",
                column: "analysis_group_id");

            migrationBuilder.CreateIndex(
                name: "i_x_quotation_item_analysis_item_id",
                table: "quotation_item",
                column: "analysis_item_id");

            migrationBuilder.CreateIndex(
                name: "i_x_quotation_item_package_id",
                table: "quotation_item",
                column: "package_id");

            migrationBuilder.CreateIndex(
                name: "i_x_quotation_item_quotation_id",
                table: "quotation_item",
                column: "quotation_id");

            migrationBuilder.CreateIndex(
                name: "i_x_refresh_token_account_id",
                table: "refresh_token",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "i_x_sample_matrix_sample_matrix_group_id",
                table: "sample_matrix",
                column: "sample_matrix_group_id");

            migrationBuilder.CreateIndex(
                name: "i_x_store_record_client_id",
                table: "store_record",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "i_x_ward_code",
                table: "ward",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "i_x_ward_country_id",
                table: "ward",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "i_x_ward_name",
                table: "ward",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "i_x_ward_province_id",
                table: "ward",
                column: "province_id");

            migrationBuilder.CreateIndex(
                name: "i_x_ward_status",
                table: "ward",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "analysis_item_tat");

            migrationBuilder.DropTable(
                name: "client_debt");

            migrationBuilder.DropTable(
                name: "client_forecast");

            migrationBuilder.DropTable(
                name: "client_history");

            migrationBuilder.DropTable(
                name: "department_analysis_capability");

            migrationBuilder.DropTable(
                name: "package_analysis_group");

            migrationBuilder.DropTable(
                name: "quotation_item");

            migrationBuilder.DropTable(
                name: "refresh_token");

            migrationBuilder.DropTable(
                name: "store_record");

            migrationBuilder.DropTable(
                name: "ward");

            migrationBuilder.DropTable(
                name: "department");

            migrationBuilder.DropTable(
                name: "analysis_item");

            migrationBuilder.DropTable(
                name: "package");

            migrationBuilder.DropTable(
                name: "quotation");

            migrationBuilder.DropTable(
                name: "account");

            migrationBuilder.DropTable(
                name: "province");

            migrationBuilder.DropTable(
                name: "branch");

            migrationBuilder.DropTable(
                name: "analysis_group");

            migrationBuilder.DropTable(
                name: "equipment_type");

            migrationBuilder.DropTable(
                name: "sample_matrix");

            migrationBuilder.DropTable(
                name: "contact");

            migrationBuilder.DropTable(
                name: "employee");

            migrationBuilder.DropTable(
                name: "permission");

            migrationBuilder.DropTable(
                name: "country");

            migrationBuilder.DropTable(
                name: "sample_matrix_group");

            migrationBuilder.DropTable(
                name: "client");
        }
    }
}
