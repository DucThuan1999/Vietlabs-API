-- =============================================
-- Mapping AnalysisItem -> ReferenceMethod:
-- Gán analysis_item.reference_method_id theo điều kiện
-- published_group_code (AnalysisItem) = name_vi (ReferenceMethod)
-- =============================================

USE [VietLabs]
GO

DECLARE @Updated INT;

UPDATE ai
SET ai.reference_method_id = rm.reference_method_id
FROM [dbo].[analysis_item] ai
INNER JOIN [dbo].[reference_method] rm
    ON LTRIM(RTRIM(ai.published_group_code)) = LTRIM(RTRIM(rm.name_vi))
WHERE ai.published_group_code IS NOT NULL
  AND LTRIM(RTRIM(ai.published_group_code)) <> N'';

SET @Updated = @@ROWCOUNT;
PRINT N'Đã mapping analysis_item.reference_method_id theo published_group_code = reference_method.name_vi.';
PRINT N'Số dòng được cập nhật: ' + CAST(@Updated AS NVARCHAR(20));
GO
