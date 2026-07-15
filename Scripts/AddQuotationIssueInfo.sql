/*
  Vietlabs: bảng lịch "thông tin ban hành" PDF báo giá
  - Bảng: quotation_issue_info
  - Khớp EF entity QuotationIssueInfo + migration 20260518120000_AddQuotationIssueInfo
  - Idempotent — chạy nhiều lần an toàn
*/

SET NOCOUNT ON;

DECLARE @SeedId UNIQUEIDENTIFIER = 'bb5703ee-219a-4a2e-a81f-3674bf00614b';
DECLARE @LegacySeedId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
DECLARE @DefaultContent NVARCHAR(MAX) = N'VLAB01.KD   Lần BH: 02    Ngày BH: 05/05/2022';

-- 1) Tạo bảng
IF OBJECT_ID(N'dbo.quotation_issue_info', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.quotation_issue_info (
        quotation_issue_info_id UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT PK_quotation_issue_info PRIMARY KEY,
        content NVARCHAR(MAX) NOT NULL,
        start_date DATETIME2 NOT NULL,
        end_date DATETIME2 NULL,
        description NVARCHAR(MAX) NULL,
        status NVARCHAR(MAX) NOT NULL,
        created_at DATETIME2 NOT NULL,
        updated_at DATETIME2 NULL,
        created_by UNIQUEIDENTIFIER NULL,
        updated_by UNIQUEIDENTIFIER NULL
    );
    PRINT N'Đã tạo bảng dbo.quotation_issue_info';
END
ELSE
    PRINT N'Bỏ qua: dbo.quotation_issue_info đã tồn tại';

-- 2) Migrate placeholder seed id (nếu đã chạy bản cũ)
IF EXISTS (
    SELECT 1 FROM dbo.quotation_issue_info
    WHERE quotation_issue_info_id = @LegacySeedId
)
AND NOT EXISTS (
    SELECT 1 FROM dbo.quotation_issue_info
    WHERE quotation_issue_info_id = @SeedId
)
BEGIN
    UPDATE dbo.quotation_issue_info
    SET quotation_issue_info_id = @SeedId
    WHERE quotation_issue_info_id = @LegacySeedId;
    PRINT N'Đã migrate seed id placeholder -> id thật';
END

-- 3) Seed bản ghi mặc định
IF NOT EXISTS (
    SELECT 1
    FROM dbo.quotation_issue_info
    WHERE quotation_issue_info_id = @SeedId
)
BEGIN
    INSERT INTO dbo.quotation_issue_info (
        quotation_issue_info_id,
        content,
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
        @DefaultContent,
        '2000-01-01T00:00:00',
        NULL,
        N'Mặc định',
        N'Active',
        SYSUTCDATETIME(),
        NULL,
        NULL,
        NULL
    );
    PRINT N'Đã seed quotation_issue_info mặc định';
END
ELSE
    PRINT N'Bỏ qua seed: bản ghi mặc định đã tồn tại';

-- 4) Sửa content rỗng trên bản ghi seed (nếu có)
UPDATE dbo.quotation_issue_info
SET
    content = @DefaultContent,
    updated_at = SYSUTCDATETIME()
WHERE quotation_issue_info_id = @SeedId
  AND (content IS NULL OR LTRIM(RTRIM(content)) = N'');

PRINT N'Hoàn tất cập nhật schema quotation_issue_info.';

GO
