/*
  Tạo bảng phụ phí báo giá (quotation_surcharge).
  Script idempotent — có thể chạy nhiều lần an toàn.

  Chạy trong SSMS / sqlcmd sau khi backup DB nếu cần.
*/

SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.quotation_surcharge', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.quotation_surcharge (
        quotation_surcharge_id UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_quotation_surcharge PRIMARY KEY,
        quotation_id UNIQUEIDENTIFIER NOT NULL,
        surcharge_type NVARCHAR(50) NOT NULL,
        description NVARCHAR(500) NULL,
        amount DECIMAL(18,2) NOT NULL
            CONSTRAINT DF_quotation_surcharge_amount DEFAULT 0,
        display_order INT NULL,
        notes NVARCHAR(2000) NULL,
        created_at DATETIME2 NOT NULL
            CONSTRAINT DF_quotation_surcharge_created_at DEFAULT SYSUTCDATETIME(),
        updated_at DATETIME2 NULL,
        updated_by UNIQUEIDENTIFIER NULL,
        CONSTRAINT CK_quotation_surcharge_amount CHECK (amount >= 0),
        CONSTRAINT CK_quotation_surcharge_type CHECK (
            surcharge_type IN ('Transportation','PrintResult','SamplingLabor','SamplingTools','Other')
        ),
        CONSTRAINT CK_quotation_surcharge_other_description CHECK (
            surcharge_type <> 'Other' OR (description IS NOT NULL AND LTRIM(RTRIM(description)) <> '')
        )
    );
    PRINT N'Đã tạo bảng dbo.quotation_surcharge';
END
ELSE
    PRINT N'Bỏ qua: dbo.quotation_surcharge đã tồn tại';

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'i_x_quotation_surcharge_quotation_id'
      AND object_id = OBJECT_ID(N'dbo.quotation_surcharge')
)
BEGIN
    CREATE INDEX i_x_quotation_surcharge_quotation_id
        ON dbo.quotation_surcharge (quotation_id);
    PRINT N'Đã tạo index i_x_quotation_surcharge_quotation_id';
END

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'i_x_quotation_surcharge_updated_by'
      AND object_id = OBJECT_ID(N'dbo.quotation_surcharge')
)
BEGIN
    CREATE INDEX i_x_quotation_surcharge_updated_by
        ON dbo.quotation_surcharge (updated_by);
    PRINT N'Đã tạo index i_x_quotation_surcharge_updated_by';
END

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'f_k_quotation_surcharge_quotation_quotation_id'
)
BEGIN
    ALTER TABLE dbo.quotation_surcharge
    ADD CONSTRAINT f_k_quotation_surcharge_quotation_quotation_id
        FOREIGN KEY (quotation_id) REFERENCES dbo.quotation (quotation_id)
        ON DELETE CASCADE;
    PRINT N'Đã tạo FK f_k_quotation_surcharge_quotation_quotation_id';
END

IF OBJECT_ID(N'dbo.account', N'U') IS NOT NULL
AND NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'f_k_quotation_surcharge_account_updated_by'
)
BEGIN
    ALTER TABLE dbo.quotation_surcharge
    ADD CONSTRAINT f_k_quotation_surcharge_account_updated_by
        FOREIGN KEY (updated_by) REFERENCES dbo.account (account_id)
        ON DELETE NO ACTION;
    PRINT N'Đã tạo FK f_k_quotation_surcharge_account_updated_by';
END

PRINT N'Hoàn tất cập nhật schema quotation_surcharge.';
