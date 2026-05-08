using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations;

/// <inheritdoc />
public partial class OrderConfirmationAsString : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "test_request_confirmation",
            table: "order",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(bool),
            oldType: "bit",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "mail_document_confirmation",
            table: "order",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(bool),
            oldType: "bit",
            oldNullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<bool>(
            name: "test_request_confirmation",
            table: "order",
            type: "bit",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AlterColumn<bool>(
            name: "mail_document_confirmation",
            table: "order",
            type: "bit",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);
    }
}
