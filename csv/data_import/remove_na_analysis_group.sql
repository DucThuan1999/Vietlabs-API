/*
  Xóa nhóm chỉ tiêu có name_vi = N/A (sau trim).
  Gỡ liên kết: analysis_item, quotation_item; xóa package_analysis_group / quotation_analysis_group.

  Chạy trong SSMS / sqlcmd sau khi đã ALTER analysis_item.analysis_group_id NULL
  (migration 20260413120000_RemoveNaAnalysisGroupAndNullableAnalysisItemGroup đã làm bước đó).

  Nếu cột analysis_item.analysis_group_id vẫn NOT NULL, chạy trước:
*/

-- Bỏ comment 2 dòng dưới nếu cột vẫn NOT NULL và chưa chạy migration EF:
-- ALTER TABLE dbo.analysis_item ALTER COLUMN analysis_group_id uniqueidentifier NULL;
-- GO

SET NOCOUNT ON;
BEGIN TRANSACTION;

DECLARE @ids TABLE (id UNIQUEIDENTIFIER NOT NULL);

INSERT INTO @ids (id)
SELECT analysis_group_id
FROM dbo.analysis_group
WHERE LTRIM(RTRIM(ISNULL(name_vi, N''))) = N'N/A';

IF NOT EXISTS (SELECT 1 FROM @ids)
BEGIN
    SELECT N'Không có nhóm nào name_vi = N/A (sau trim). Không thay đổi.' AS message;
    ROLLBACK TRANSACTION;
    RETURN;
END

UPDATE ai
SET analysis_group_id = NULL,
    updated_at = SYSUTCDATETIME()
FROM dbo.analysis_item ai
INNER JOIN @ids i ON ai.analysis_group_id = i.id;

DELETE pag
FROM dbo.package_analysis_group pag
INNER JOIN @ids i ON pag.analysis_group_id = i.id;

UPDATE qi
SET analysis_group_id = NULL,
    updated_at = SYSUTCDATETIME()
FROM dbo.quotation_item qi
INNER JOIN @ids i ON qi.analysis_group_id = i.id;

IF OBJECT_ID(N'dbo.quotation_analysis_group', N'U') IS NOT NULL
BEGIN
    DELETE qag
    FROM dbo.quotation_analysis_group qag
    INNER JOIN @ids i ON qag.analysis_group_id = i.id;
END

DELETE ag
FROM dbo.analysis_group ag
INNER JOIN @ids i ON ag.analysis_group_id = i.id;

COMMIT TRANSACTION;
SELECT N'Hoàn tất: đã xóa nhóm N/A và gỡ liên kết.' AS message;
