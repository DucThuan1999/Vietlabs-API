/*
  Vietlabs: bảng lịch VAT (vat_rate)
  - Khớp EF entity VietLab.Models.VatRate + migration 20260509120000_AddVatRate
  - SQL Server
  - Idempotent: chỉ tạo bảng nếu chưa có; chỉ seed dòng mặc định nếu chưa tồn tại GUID seed
*/

-- 1) Tạo bảng
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'vat_rate')
BEGIN
    CREATE TABLE vat_rate (
        vat_rate_id uniqueidentifier NOT NULL CONSTRAINT PK_vat_rate PRIMARY KEY,
        [percent] decimal(5,2) NOT NULL,
        start_date datetime2 NOT NULL,
        end_date datetime2 NULL,
        description nvarchar(max) NULL,
        status nvarchar(max) NOT NULL,
        created_at datetime2 NOT NULL,
        updated_at datetime2 NULL,
        created_by uniqueidentifier NULL,
        updated_by uniqueidentifier NULL
    );
END
GO

-- 2) Seed mặc định 8%, hiệu lực từ 2000-01-01, không điểm kết thúc (end_date NULL)
DECLARE @SeedId uniqueidentifier = '11111111-1111-1111-1111-111111111111';

IF NOT EXISTS (SELECT 1 FROM vat_rate WHERE vat_rate_id = @SeedId)
BEGIN
    INSERT INTO vat_rate (
        vat_rate_id,
        [percent],
        start_date,
        end_date,
        description,
        status,
        created_at,
        updated_at,
        created_by,
        updated_by
    )
    VALUES (
        @SeedId,
        8.00,
        '2000-01-01T00:00:00',
        NULL,
        N'Mặc định lịch sử 8%',
        N'Active',
        SYSUTCDATETIME(),
        NULL,
        NULL,
        NULL
    );
END
GO

/*
  3) (Tuỳ chọn, môi trường dev) Gỡ bảng — CHỈ chạy khi chắc chắn không còn dữ liệu cần giữ

IF OBJECT_ID(N'dbo.vat_rate', N'U') IS NOT NULL
    DROP TABLE dbo.vat_rate;
GO
*/
