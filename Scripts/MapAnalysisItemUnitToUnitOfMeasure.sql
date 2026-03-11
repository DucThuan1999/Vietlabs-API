-- =============================================
-- Mapping AnalysisItem -> UnitOfMeasure:
-- Gán analysis_item.unit_of_measure_id theo analysis_item.unit
-- Chuẩn hóa unit (bỏ CR/LF, gộp khoảng trắng, trim) rồi khớp với unit_of_measure.name_vi
-- Chạy sau InsertUnitOfMeasureData.sql
-- =============================================

USE [VietLabs]
GO

-- Chuẩn hóa unit: bỏ CR/LF, gộp khoảng trắng, trim rồi so khớp với name_vi
DECLARE @Updated INT;

UPDATE ai
SET ai.unit_of_measure_id = u.unit_of_measure_id
FROM [dbo].[analysis_item] ai
INNER JOIN [dbo].[unit_of_measure] u
    ON LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(ai.unit, CHAR(13), N' '), CHAR(10), N' '), N'  ', N' '), N'  ', N' ')))
     = LTRIM(RTRIM(u.name_vi))
WHERE ai.unit IS NOT NULL
  AND LTRIM(RTRIM(ai.unit)) <> N'';

SET @Updated = @@ROWCOUNT;
PRINT N'Đã mapping analysis_item.unit_of_measure_id theo unit = unit_of_measure.name_vi.';
PRINT N'Số dòng được cập nhật: ' + CAST(@Updated AS NVARCHAR(20));
GO
