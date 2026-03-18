using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <inheritdoc />
    public partial class StoreRecordModuleCodeOwnerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add new columns (nullable first for backfill)
            migrationBuilder.AddColumn<string>(
                name: "module_code",
                table: "store_record",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "owner_id",
                table: "store_record",
                type: "uniqueidentifier",
                nullable: true);

            // Backfill: ClientId -> ModuleCode='Client', OwnerId=client_id; null -> 'Unassigned', 00000000-0000-0000-0000-000000000000
            migrationBuilder.Sql(@"
                UPDATE store_record SET module_code = 'Client', owner_id = client_id WHERE client_id IS NOT NULL;
                UPDATE store_record SET module_code = 'Unassigned', owner_id = '00000000-0000-0000-0000-000000000000' WHERE client_id IS NULL;
            ");

            // Drop FK and index on client_id
            migrationBuilder.DropForeignKey(
                name: "f_k_store_record_client_client_id",
                table: "store_record");

            migrationBuilder.DropIndex(
                name: "i_x_store_record_client_id",
                table: "store_record");

            migrationBuilder.DropColumn(
                name: "client_id",
                table: "store_record");

            // Make new columns required
            migrationBuilder.AlterColumn<string>(
                name: "module_code",
                table: "store_record",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "owner_id",
                table: "store_record",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "i_x_store_record_module_owner",
                table: "store_record",
                columns: new[] { "module_code", "owner_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_store_record_module_owner",
                table: "store_record");

            migrationBuilder.AddColumn<Guid>(
                name: "client_id",
                table: "store_record",
                type: "uniqueidentifier",
                nullable: true);

            // Restore client_id from owner_id where module was Client
            migrationBuilder.Sql(@"
                UPDATE store_record SET client_id = owner_id WHERE module_code = 'Client';
            ");

            migrationBuilder.DropColumn(
                name: "module_code",
                table: "store_record");

            migrationBuilder.DropColumn(
                name: "owner_id",
                table: "store_record");

            migrationBuilder.CreateIndex(
                name: "i_x_store_record_client_id",
                table: "store_record",
                column: "client_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_store_record_client_client_id",
                table: "store_record",
                column: "client_id",
                principalTable: "client",
                principalColumn: "client_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
