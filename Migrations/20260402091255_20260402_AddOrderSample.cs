using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <inheritdoc />
    public partial class _20260402_AddOrderSample : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotent: bảng có thể đã được tạo thủ công trên SQL trước đó.
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.order_sample', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[order_sample] (
        [order_sample_id] UNIQUEIDENTIFIER NOT NULL,
        [order_id] UNIQUEIDENTIFIER NOT NULL,
        [sample_identifier] NVARCHAR(100) NULL,
        [sample_code] NVARCHAR(100) NULL,
        [sample_matrix_id] UNIQUEIDENTIFIER NOT NULL,
        [sample_name] NVARCHAR(500) NULL,
        [sample_weight] DECIMAL(18,2) NULL,
        [sample_temperature] DECIMAL(18,2) NULL,
        [result_turnaround_time_requirement] NVARCHAR(200) NULL,
        [fee_percentage] DECIMAL(5,2) NULL,
        [sample_condition_description] NVARCHAR(2000) NULL,
        [notes] NVARCHAR(2000) NULL,
        [analysis_item_count] INT NULL,
        [ntp_analysis_item_count] INT NULL,
        [amount] DECIMAL(18,2) NULL,
        [sample_received_date] DATETIME2 NULL,
        CONSTRAINT [PK_order_sample] PRIMARY KEY ([order_sample_id]),
        CONSTRAINT [f_k_order_sample_order_order_id] FOREIGN KEY ([order_id])
            REFERENCES [dbo].[order] ([order_id]) ON DELETE CASCADE,
        CONSTRAINT [f_k_order_sample_sample_matrix_sample_matrix_id] FOREIGN KEY ([sample_matrix_id])
            REFERENCES [dbo].[sample_matrix] ([sample_matrix_id])
    );
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'i_x_order_sample_order_id' AND object_id = OBJECT_ID(N'dbo.order_sample'))
BEGIN
    CREATE INDEX [i_x_order_sample_order_id] ON [dbo].[order_sample] ([order_id]);
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'i_x_order_sample_sample_matrix_id' AND object_id = OBJECT_ID(N'dbo.order_sample'))
BEGIN
    CREATE INDEX [i_x_order_sample_sample_matrix_id] ON [dbo].[order_sample] ([sample_matrix_id]);
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.order_sample', N'U') IS NOT NULL
    DROP TABLE [dbo].[order_sample];
");
        }
    }
}
