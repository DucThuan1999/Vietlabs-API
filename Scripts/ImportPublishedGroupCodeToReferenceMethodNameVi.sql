-- =============================================
-- Nhập dữ liệu: lấy distinct PublishedGroupCode từ analysis_item
-- insert vào reference_method.name_vi (chỉ thêm bản ghi mới, không ghi đè)
-- reference_method_code = 'PP-{SequenceNumber}', updated_at / updated_by set
-- =============================================

USE [VietLabs]
GO

DECLARE @Now DATETIME2 = SYSUTCDATETIME();
DECLARE @UpdatedBy UNIQUEIDENTIFIER = NULL;  -- Gán account_id nếu cần ghi nhận người cập nhật

;WITH DistinctCodes AS (
    SELECT DISTINCT LTRIM(RTRIM([published_group_code])) AS code
    FROM [dbo].[analysis_item]
    WHERE [published_group_code] IS NOT NULL
      AND LTRIM(RTRIM([published_group_code])) <> N''
),
NewCodes AS (
    SELECT src.code
    FROM DistinctCodes src
    WHERE NOT EXISTS (
        SELECT 1
        FROM [dbo].[reference_method] rm
        WHERE LTRIM(RTRIM(rm.name_vi)) = LTRIM(RTRIM(src.code))
    )
),
Numbered AS (
    SELECT code, ROW_NUMBER() OVER (ORDER BY code) AS rn
    FROM NewCodes
),
BaseSeq AS (
    SELECT ISNULL(MAX(sequence_number), 0) AS base FROM [dbo].[reference_method]
)
INSERT INTO [dbo].[reference_method] (
    [reference_method_id],
    [sequence_number],
    [name_vi],
    [name_en],
    [reference_method_code],
    [status],
    [created_at],
    [updated_at],
    [updated_by]
)
SELECT
    NEWID(),
    b.base + n.rn,
    n.code,
    n.code,
    N'PP-' + RIGHT(N'000' + CAST(b.base + n.rn AS NVARCHAR(10)), 3),
    N'Active',
    @Now,
    @Now,
    @UpdatedBy
FROM Numbered n
CROSS JOIN BaseSeq b;

PRINT N'Đã nhập PublishedGroupCode từ analysis_item vào reference_method.name_vi (chỉ bản ghi mới).';
GO
