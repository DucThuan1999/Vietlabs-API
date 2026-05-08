using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderTrackingAndContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotent: supports re-run after a failed apply (e.g. invalid CHECK) or partial columns.
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'f_k_order_contact_contact_id' AND parent_object_id = OBJECT_ID(N'dbo.order'))
    ALTER TABLE dbo.[order] DROP CONSTRAINT [f_k_order_contact_contact_id];

IF COL_LENGTH('dbo.order', 'contact_id') IS NULL
    ALTER TABLE dbo.[order] ADD [contact_id] uniqueidentifier NULL;

IF COL_LENGTH('dbo.order', 'created_at') IS NULL
    ALTER TABLE dbo.[order] ADD [created_at] datetime2 NOT NULL CONSTRAINT [df_order_created_at] DEFAULT (GETUTCDATE());

IF COL_LENGTH('dbo.order', 'created_by') IS NULL
    ALTER TABLE dbo.[order] ADD [created_by] uniqueidentifier NULL;

IF COL_LENGTH('dbo.order', 'debt_status') IS NULL
    ALTER TABLE dbo.[order] ADD [debt_status] nvarchar(200) NULL;

IF COL_LENGTH('dbo.order', 'debt_type') IS NULL
    ALTER TABLE dbo.[order] ADD [debt_type] nvarchar(200) NULL;

IF COL_LENGTH('dbo.order', 'expected_completion_date') IS NULL
    ALTER TABLE dbo.[order] ADD [expected_completion_date] datetime2 NULL;

IF COL_LENGTH('dbo.order', 'order_status') IS NULL
    ALTER TABLE dbo.[order] ADD [order_status] nvarchar(100) NULL;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'i_x_order_contact_id' AND object_id = OBJECT_ID(N'dbo.order'))
    CREATE INDEX [i_x_order_contact_id] ON dbo.[order] ([contact_id]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'i_x_order_created_by' AND object_id = OBJECT_ID(N'dbo.order'))
    CREATE INDEX [i_x_order_created_by] ON dbo.[order] ([created_by]);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'f_k_order_account_created_by')
    ALTER TABLE dbo.[order] ADD CONSTRAINT [f_k_order_account_created_by] FOREIGN KEY ([created_by]) REFERENCES [account] ([account_id]) ON DELETE NO ACTION;

IF NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = N'AK_contact_contact_id_client_id' AND parent_object_id = OBJECT_ID(N'dbo.contact'))
    ALTER TABLE dbo.[contact] ADD CONSTRAINT [AK_contact_contact_id_client_id] UNIQUE ([contact_id], [client_id]);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'f_k_order_contact_contact_id_client_id')
    ALTER TABLE dbo.[order] ADD CONSTRAINT [f_k_order_contact_contact_id_client_id] FOREIGN KEY ([contact_id], [client_id]) REFERENCES [contact] ([contact_id], [client_id]) ON DELETE NO ACTION;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'f_k_order_contact_contact_id_client_id' AND parent_object_id = OBJECT_ID(N'dbo.order'))
    ALTER TABLE dbo.[order] DROP CONSTRAINT [f_k_order_contact_contact_id_client_id];

IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'f_k_order_contact_contact_id' AND parent_object_id = OBJECT_ID(N'dbo.order'))
    ALTER TABLE dbo.[order] DROP CONSTRAINT [f_k_order_contact_contact_id];

IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = N'AK_contact_contact_id_client_id' AND parent_object_id = OBJECT_ID(N'dbo.contact'))
    ALTER TABLE dbo.[contact] DROP CONSTRAINT [AK_contact_contact_id_client_id];

IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'f_k_order_account_created_by' AND parent_object_id = OBJECT_ID(N'dbo.order'))
    ALTER TABLE dbo.[order] DROP CONSTRAINT [f_k_order_account_created_by];

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'i_x_order_created_by' AND object_id = OBJECT_ID(N'dbo.order'))
    DROP INDEX [i_x_order_created_by] ON dbo.[order];

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'i_x_order_contact_id' AND object_id = OBJECT_ID(N'dbo.order'))
    DROP INDEX [i_x_order_contact_id] ON dbo.[order];

IF COL_LENGTH('dbo.order', 'order_status') IS NOT NULL
    ALTER TABLE dbo.[order] DROP COLUMN [order_status];

IF COL_LENGTH('dbo.order', 'expected_completion_date') IS NOT NULL
    ALTER TABLE dbo.[order] DROP COLUMN [expected_completion_date];

IF COL_LENGTH('dbo.order', 'debt_type') IS NOT NULL
    ALTER TABLE dbo.[order] DROP COLUMN [debt_type];

IF COL_LENGTH('dbo.order', 'debt_status') IS NOT NULL
    ALTER TABLE dbo.[order] DROP COLUMN [debt_status];

IF COL_LENGTH('dbo.order', 'created_by') IS NOT NULL
    ALTER TABLE dbo.[order] DROP COLUMN [created_by];

IF COL_LENGTH('dbo.order', 'created_at') IS NOT NULL
BEGIN
    IF EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = N'df_order_created_at' AND parent_object_id = OBJECT_ID(N'dbo.order'))
        ALTER TABLE dbo.[order] DROP CONSTRAINT [df_order_created_at];
    ALTER TABLE dbo.[order] DROP COLUMN [created_at];
END

IF COL_LENGTH('dbo.order', 'contact_id') IS NOT NULL
    ALTER TABLE dbo.[order] DROP COLUMN [contact_id];
");
        }
    }
}
