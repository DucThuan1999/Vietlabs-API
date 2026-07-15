using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <summary>
    /// Bảng phụ phí báo giá (quotation_surcharge).
    /// </summary>
    public partial class AddQuotationSurcharges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'quotation_surcharge')
BEGIN
    CREATE TABLE quotation_surcharge (
        quotation_surcharge_id uniqueidentifier NOT NULL CONSTRAINT PK_quotation_surcharge PRIMARY KEY,
        quotation_id uniqueidentifier NOT NULL,
        surcharge_type nvarchar(50) NOT NULL,
        description nvarchar(500) NULL,
        amount decimal(18,2) NOT NULL CONSTRAINT DF_quotation_surcharge_amount DEFAULT 0,
        display_order int NULL,
        notes nvarchar(2000) NULL,
        created_at datetime2 NOT NULL CONSTRAINT DF_quotation_surcharge_created_at DEFAULT SYSUTCDATETIME(),
        updated_at datetime2 NULL,
        updated_by uniqueidentifier NULL,
        CONSTRAINT CK_quotation_surcharge_amount CHECK (amount >= 0),
        CONSTRAINT CK_quotation_surcharge_type CHECK (surcharge_type IN ('Transportation','PrintResult','SamplingLabor','SamplingTools','Other')),
        CONSTRAINT CK_quotation_surcharge_other_description CHECK (
            surcharge_type <> 'Other' OR (description IS NOT NULL AND LTRIM(RTRIM(description)) <> '')
        )
    );
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'i_x_quotation_surcharge_quotation_id' AND object_id = OBJECT_ID('quotation_surcharge'))
BEGIN
    CREATE INDEX i_x_quotation_surcharge_quotation_id ON quotation_surcharge (quotation_id);
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'i_x_quotation_surcharge_updated_by' AND object_id = OBJECT_ID('quotation_surcharge'))
BEGIN
    CREATE INDEX i_x_quotation_surcharge_updated_by ON quotation_surcharge (updated_by);
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys WHERE name = 'f_k_quotation_surcharge_quotation_quotation_id'
)
BEGIN
    ALTER TABLE quotation_surcharge
    ADD CONSTRAINT f_k_quotation_surcharge_quotation_quotation_id
        FOREIGN KEY (quotation_id) REFERENCES quotation (quotation_id) ON DELETE CASCADE;
END
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.account', N'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys WHERE name = 'f_k_quotation_surcharge_account_updated_by'
)
BEGIN
    ALTER TABLE quotation_surcharge
    ADD CONSTRAINT f_k_quotation_surcharge_account_updated_by
        FOREIGN KEY (updated_by) REFERENCES account (account_id) ON DELETE NO ACTION;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'f_k_quotation_surcharge_account_updated_by')
    ALTER TABLE quotation_surcharge DROP CONSTRAINT f_k_quotation_surcharge_account_updated_by;

IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'f_k_quotation_surcharge_quotation_quotation_id')
    ALTER TABLE quotation_surcharge DROP CONSTRAINT f_k_quotation_surcharge_quotation_quotation_id;

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'i_x_quotation_surcharge_updated_by' AND object_id = OBJECT_ID('quotation_surcharge'))
    DROP INDEX i_x_quotation_surcharge_updated_by ON quotation_surcharge;

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'i_x_quotation_surcharge_quotation_id' AND object_id = OBJECT_ID('quotation_surcharge'))
    DROP INDEX i_x_quotation_surcharge_quotation_id ON quotation_surcharge;

IF OBJECT_ID(N'dbo.quotation_surcharge', N'U') IS NOT NULL
    DROP TABLE dbo.quotation_surcharge;
");
        }
    }
}
