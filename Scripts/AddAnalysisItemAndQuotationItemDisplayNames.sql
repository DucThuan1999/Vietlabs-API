/*
  DisplayName cho tên chỉ tiêu
  - analysis_item: display_name_vi, display_name_en (JSON Tiptap khi có format)
  - quotation_item: item_display_name_vi, item_display_name_en (snapshot báo giá)
  - name_vi / name_en: text thuần (search/filter)

  Chạy trên SQL Server 2017+ (cần ISJSON, OPENJSON, STRING_AGG).
  Khuyến nghị: backup DB trước khi chạy.

  Thứ tự:
    1) Phần A — Thêm cột (idempotent)
    2) Phần B — Migrate dữ liệu JSON cũ trong name_vi/name_en (tùy chọn)
    3) Phần C — Kiểm tra
    4) Phần D — Rollback (chỉ khi cần hoàn tác)
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

/* =============================================================================
   A. SCHEMA — thêm cột nếu chưa có
   ============================================================================= */

IF COL_LENGTH('dbo.analysis_item', 'display_name_vi') IS NULL
BEGIN
    ALTER TABLE dbo.analysis_item
    ADD display_name_vi NVARCHAR(MAX) NULL;
    PRINT N'Đã thêm analysis_item.display_name_vi';
END
ELSE
    PRINT N'Bỏ qua: analysis_item.display_name_vi đã tồn tại';

IF COL_LENGTH('dbo.analysis_item', 'display_name_en') IS NULL
BEGIN
    ALTER TABLE dbo.analysis_item
    ADD display_name_en NVARCHAR(MAX) NULL;
    PRINT N'Đã thêm analysis_item.display_name_en';
END
ELSE
    PRINT N'Bỏ qua: analysis_item.display_name_en đã tồn tại';

IF COL_LENGTH('dbo.quotation_item', 'item_display_name_vi') IS NULL
BEGIN
    ALTER TABLE dbo.quotation_item
    ADD item_display_name_vi NVARCHAR(MAX) NULL;
    PRINT N'Đã thêm quotation_item.item_display_name_vi';
END
ELSE
    PRINT N'Bỏ qua: quotation_item.item_display_name_vi đã tồn tại';

IF COL_LENGTH('dbo.quotation_item', 'item_display_name_en') IS NULL
BEGIN
    ALTER TABLE dbo.quotation_item
    ADD item_display_name_en NVARCHAR(MAX) NULL;
    PRINT N'Đã thêm quotation_item.item_display_name_en';
END
ELSE
    PRINT N'Bỏ qua: quotation_item.item_display_name_en đã tồn tại';

/* Đảm bảo name_vi/name_en đủ lớn cho text thuần (nếu vẫn đang NVARCHAR(500)) */
IF EXISTS (
    SELECT 1
    FROM sys.columns c
    INNER JOIN sys.tables t ON t.object_id = c.object_id
    WHERE t.name = N'analysis_item' AND c.name = N'name_vi'
      AND c.max_length > 0 AND c.max_length <> -1
)
BEGIN
    ALTER TABLE dbo.analysis_item ALTER COLUMN name_vi NVARCHAR(MAX) NULL;
    PRINT N'Đã đổi analysis_item.name_vi -> NVARCHAR(MAX)';
END

IF EXISTS (
    SELECT 1
    FROM sys.columns c
    INNER JOIN sys.tables t ON t.object_id = c.object_id
    WHERE t.name = N'analysis_item' AND c.name = N'name_en'
      AND c.max_length > 0 AND c.max_length <> -1
)
BEGIN
    ALTER TABLE dbo.analysis_item ALTER COLUMN name_en NVARCHAR(MAX) NULL;
    PRINT N'Đã đổi analysis_item.name_en -> NVARCHAR(MAX)';
END

GO

/* =============================================================================
   B. DATA — chuyển JSON Tiptap từ name_* sang display_name_* và strip plain text
   Chỉ xử lý dòng: ISJSON=1, type=doc, display_name_* đang NULL
   ============================================================================= */

BEGIN TRANSACTION;

/* Helper: gom text từ document JSON Tiptap (paragraph -> text nodes) */
IF OBJECT_ID('tempdb..#JsonNameMigration') IS NOT NULL
    DROP TABLE #JsonNameMigration;

;WITH Candidates AS (
    SELECT
        ai.analysis_item_id,
        ai.name_vi,
        ai.name_en
    FROM dbo.analysis_item ai
    WHERE (ai.display_name_vi IS NULL OR ai.display_name_en IS NULL)
),
ViText AS (
    SELECT
        c.analysis_item_id,
        PlainVi = LTRIM(RTRIM((
            SELECT STRING_AGG(ISNULL(x.text_part, N''), N'') WITHIN GROUP (ORDER BY x.seq)
            FROM (
                SELECT
                    ROW_NUMBER() OVER (ORDER BY p.[key]) * 1000 + ROW_NUMBER() OVER (PARTITION BY p.[key] ORDER BY n.[key]) AS seq,
                    CASE
                        WHEN JSON_VALUE(n.value, '$.type') = N'text'
                            THEN JSON_VALUE(n.value, '$.text')
                        WHEN JSON_VALUE(n.value, '$.type') = N'hardBreak'
                            THEN N' '
                        ELSE N''
                    END AS text_part
                FROM Candidates c2
                CROSS APPLY OPENJSON(c2.name_vi, '$.content') p
                CROSS APPLY OPENJSON(p.value, '$.content') n
                WHERE c2.analysis_item_id = c.analysis_item_id
                  AND c2.name_vi IS NOT NULL
                  AND LTRIM(RTRIM(c2.name_vi)) LIKE N'{%'
                  AND ISJSON(c2.name_vi) = 1
                  AND JSON_VALUE(c2.name_vi, '$.type') = N'doc'
            ) x
        )))
    FROM Candidates c
),
EnText AS (
    SELECT
        c.analysis_item_id,
        PlainEn = LTRIM(RTRIM((
            SELECT STRING_AGG(ISNULL(x.text_part, N''), N'') WITHIN GROUP (ORDER BY x.seq)
            FROM (
                SELECT
                    ROW_NUMBER() OVER (ORDER BY p.[key]) * 1000 + ROW_NUMBER() OVER (PARTITION BY p.[key] ORDER BY n.[key]) AS seq,
                    CASE
                        WHEN JSON_VALUE(n.value, '$.type') = N'text'
                            THEN JSON_VALUE(n.value, '$.text')
                        WHEN JSON_VALUE(n.value, '$.type') = N'hardBreak'
                            THEN N' '
                        ELSE N''
                    END AS text_part
                FROM Candidates c2
                CROSS APPLY OPENJSON(c2.name_en, '$.content') p
                CROSS APPLY OPENJSON(p.value, '$.content') n
                WHERE c2.analysis_item_id = c.analysis_item_id
                  AND c2.name_en IS NOT NULL
                  AND LTRIM(RTRIM(c2.name_en)) LIKE N'{%'
                  AND ISJSON(c2.name_en) = 1
                  AND JSON_VALUE(c2.name_en, '$.type') = N'doc'
            ) x
        )))
    FROM Candidates c
)
UPDATE ai
SET
    display_name_vi = CASE
        WHEN ai.display_name_vi IS NULL
             AND ai.name_vi IS NOT NULL
             AND LTRIM(RTRIM(ai.name_vi)) LIKE N'{%'
             AND ISJSON(ai.name_vi) = 1
             AND JSON_VALUE(ai.name_vi, '$.type') = N'doc'
        THEN ai.name_vi
        ELSE ai.display_name_vi
    END,
    name_vi = CASE
        WHEN ai.display_name_vi IS NULL
             AND ai.name_vi IS NOT NULL
             AND LTRIM(RTRIM(ai.name_vi)) LIKE N'{%'
             AND ISJSON(ai.name_vi) = 1
             AND JSON_VALUE(ai.name_vi, '$.type') = N'doc'
             AND vt.PlainVi IS NOT NULL
             AND LEN(vt.PlainVi) > 0
        THEN vt.PlainVi
        ELSE ai.name_vi
    END,
    display_name_en = CASE
        WHEN ai.display_name_en IS NULL
             AND ai.name_en IS NOT NULL
             AND LTRIM(RTRIM(ai.name_en)) LIKE N'{%'
             AND ISJSON(ai.name_en) = 1
             AND JSON_VALUE(ai.name_en, '$.type') = N'doc'
        THEN ai.name_en
        ELSE ai.display_name_en
    END,
    name_en = CASE
        WHEN ai.display_name_en IS NULL
             AND ai.name_en IS NOT NULL
             AND LTRIM(RTRIM(ai.name_en)) LIKE N'{%'
             AND ISJSON(ai.name_en) = 1
             AND JSON_VALUE(ai.name_en, '$.type') = N'doc'
             AND et.PlainEn IS NOT NULL
             AND LEN(et.PlainEn) > 0
        THEN et.PlainEn
        ELSE ai.name_en
    END
FROM dbo.analysis_item ai
LEFT JOIN ViText vt ON vt.analysis_item_id = ai.analysis_item_id
LEFT JOIN EnText et ON et.analysis_item_id = ai.analysis_item_id;

DECLARE @migratedVi INT = @@ROWCOUNT;
PRINT N'Đã cập nhật analysis_item (migrate JSON name_vi/en): ' + CAST(@migratedVi AS NVARCHAR(20)) + N' dòng touched';

/* quotation_item: nếu item_name_vi/en đang là JSON doc, copy sang item_display_name_* */
UPDATE qi
SET
    item_display_name_vi = CASE
        WHEN qi.item_display_name_vi IS NULL
             AND qi.item_name_vi IS NOT NULL
             AND LTRIM(RTRIM(qi.item_name_vi)) LIKE N'{%'
             AND ISJSON(qi.item_name_vi) = 1
             AND JSON_VALUE(qi.item_name_vi, '$.type') = N'doc'
        THEN qi.item_name_vi
        ELSE qi.item_display_name_vi
    END,
    item_name_vi = CASE
        WHEN qi.item_display_name_vi IS NULL
             AND qi.item_name_vi IS NOT NULL
             AND LTRIM(RTRIM(qi.item_name_vi)) LIKE N'{%'
             AND ISJSON(qi.item_name_vi) = 1
             AND JSON_VALUE(qi.item_name_vi, '$.type') = N'doc'
        THEN (
            SELECT LTRIM(RTRIM(STRING_AGG(ISNULL(
                CASE
                    WHEN JSON_VALUE(n.value, '$.type') = N'text' THEN JSON_VALUE(n.value, '$.text')
                    WHEN JSON_VALUE(n.value, '$.type') = N'hardBreak' THEN N' '
                    ELSE N''
                END, N''), N'') WITHIN GROUP (ORDER BY p.[key], n.[key])))
            FROM OPENJSON(qi.item_name_vi, '$.content') p
            CROSS APPLY OPENJSON(p.value, '$.content') n
        )
        ELSE qi.item_name_vi
    END,
    item_display_name_en = CASE
        WHEN qi.item_display_name_en IS NULL
             AND qi.item_name_en IS NOT NULL
             AND LTRIM(RTRIM(qi.item_name_en)) LIKE N'{%'
             AND ISJSON(qi.item_name_en) = 1
             AND JSON_VALUE(qi.item_name_en, '$.type') = N'doc'
        THEN qi.item_name_en
        ELSE qi.item_display_name_en
    END,
    item_name_en = CASE
        WHEN qi.item_display_name_en IS NULL
             AND qi.item_name_en IS NOT NULL
             AND LTRIM(RTRIM(qi.item_name_en)) LIKE N'{%'
             AND ISJSON(qi.item_name_en) = 1
             AND JSON_VALUE(qi.item_name_en, '$.type') = N'doc'
        THEN (
            SELECT LTRIM(RTRIM(STRING_AGG(ISNULL(
                CASE
                    WHEN JSON_VALUE(n.value, '$.type') = N'text' THEN JSON_VALUE(n.value, '$.text')
                    WHEN JSON_VALUE(n.value, '$.type') = N'hardBreak' THEN N' '
                    ELSE N''
                END, N''), N'') WITHIN GROUP (ORDER BY p.[key], n.[key])))
            FROM OPENJSON(qi.item_name_en, '$.content') p
            CROSS APPLY OPENJSON(p.value, '$.content') n
        )
        ELSE qi.item_name_en
    END
FROM dbo.quotation_item qi
WHERE (qi.item_display_name_vi IS NULL OR qi.item_display_name_en IS NULL)
  AND (
        (qi.item_name_vi IS NOT NULL AND LTRIM(RTRIM(qi.item_name_vi)) LIKE N'{%' AND ISJSON(qi.item_name_vi) = 1)
     OR (qi.item_name_en IS NOT NULL AND LTRIM(RTRIM(qi.item_name_en)) LIKE N'{%' AND ISJSON(qi.item_name_en) = 1)
  );

PRINT N'Đã cập nhật quotation_item (JSON snapshot): ' + CAST(@@ROWCOUNT AS NVARCHAR(20)) + N' dòng';

COMMIT TRANSACTION;

GO

/* =============================================================================
   C. KIỂM TRA
   ============================================================================= */

SELECT
    COUNT(*) AS total_analysis_items,
    SUM(CASE WHEN display_name_vi IS NOT NULL AND LEN(display_name_vi) > 0 THEN 1 ELSE 0 END) AS has_display_name_vi,
    SUM(CASE WHEN display_name_en IS NOT NULL AND LEN(display_name_en) > 0 THEN 1 ELSE 0 END) AS has_display_name_en,
    SUM(CASE WHEN name_vi IS NOT NULL AND LTRIM(name_vi) LIKE N'{%' AND ISJSON(name_vi) = 1 THEN 1 ELSE 0 END) AS name_vi_still_json,
    SUM(CASE WHEN name_en IS NOT NULL AND LTRIM(name_en) LIKE N'{%' AND ISJSON(name_en) = 1 THEN 1 ELSE 0 END) AS name_en_still_json
FROM dbo.analysis_item;

SELECT TOP (20)
    analysis_item_code,
    LEFT(name_vi, 80) AS name_vi_preview,
    LEFT(display_name_vi, 80) AS display_name_vi_preview
FROM dbo.analysis_item
WHERE display_name_vi IS NOT NULL
ORDER BY updated_at DESC;

/*
=============================================================================
D. ROLLBACK (chỉ chạy khi cần hoàn tác schema — MẤT dữ liệu display name)
=============================================================================

ALTER TABLE dbo.analysis_item DROP COLUMN IF EXISTS display_name_vi;
ALTER TABLE dbo.analysis_item DROP COLUMN IF EXISTS display_name_en;
ALTER TABLE dbo.quotation_item DROP COLUMN IF EXISTS item_display_name_vi;
ALTER TABLE dbo.quotation_item DROP COLUMN IF EXISTS item_display_name_en;
*/
