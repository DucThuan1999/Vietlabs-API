/*
  Rollback thủ công bảng phụ phí báo giá (quotation_surcharge).
  Chỉ dùng trên môi trường test/dev khi cần hoàn tác.
*/

SET NOCOUNT ON;

IF EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'f_k_quotation_surcharge_account_updated_by'
)
BEGIN
    ALTER TABLE dbo.quotation_surcharge
    DROP CONSTRAINT f_k_quotation_surcharge_account_updated_by;
    PRINT N'Đã drop FK f_k_quotation_surcharge_account_updated_by';
END

IF EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'f_k_quotation_surcharge_quotation_quotation_id'
)
BEGIN
    ALTER TABLE dbo.quotation_surcharge
    DROP CONSTRAINT f_k_quotation_surcharge_quotation_quotation_id;
    PRINT N'Đã drop FK f_k_quotation_surcharge_quotation_quotation_id';
END

IF EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'i_x_quotation_surcharge_updated_by'
      AND object_id = OBJECT_ID(N'dbo.quotation_surcharge')
)
BEGIN
    DROP INDEX i_x_quotation_surcharge_updated_by ON dbo.quotation_surcharge;
    PRINT N'Đã drop index i_x_quotation_surcharge_updated_by';
END

IF EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'i_x_quotation_surcharge_quotation_id'
      AND object_id = OBJECT_ID(N'dbo.quotation_surcharge')
)
BEGIN
    DROP INDEX i_x_quotation_surcharge_quotation_id ON dbo.quotation_surcharge;
    PRINT N'Đã drop index i_x_quotation_surcharge_quotation_id';
END

IF OBJECT_ID(N'dbo.quotation_surcharge', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.quotation_surcharge;
    PRINT N'Đã drop bảng dbo.quotation_surcharge';
END
ELSE
    PRINT N'Bỏ qua: dbo.quotation_surcharge không tồn tại';

PRINT N'Hoàn tất rollback quotation_surcharge.';
