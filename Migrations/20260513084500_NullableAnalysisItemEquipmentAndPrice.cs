using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <summary>
    /// Cho phep chi tieu khong nhap loai thiet bi va gia chuan.
    /// </summary>
    public partial class NullableAnalysisItemEquipmentAndPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "equipment_type_id",
                table: "analysis_item",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<decimal>(
                name: "unit_price",
                table: "analysis_item",
                type: "decimal(18,2)",
                nullable: true,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE analysis_item
SET unit_price = 0
WHERE unit_price IS NULL;

DECLARE @fallbackEquipmentTypeId UNIQUEIDENTIFIER = (
    SELECT TOP (1) equipment_type_id
    FROM equipment_type
    ORDER BY equipment_type_code
);

IF @fallbackEquipmentTypeId IS NOT NULL
BEGIN
    UPDATE analysis_item
    SET equipment_type_id = @fallbackEquipmentTypeId
    WHERE equipment_type_id IS NULL;
END
");

            migrationBuilder.AlterColumn<decimal>(
                name: "unit_price",
                table: "analysis_item",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<Guid>(
                name: "equipment_type_id",
                table: "analysis_item",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
