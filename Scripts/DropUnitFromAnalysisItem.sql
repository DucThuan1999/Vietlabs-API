-- =============================================
-- Xóa cột unit khỏi analysis_item (ĐVT dùng unit_of_measure_id)
-- Chạy sau khi đã mapping unit_of_measure_id (MapAnalysisItemUnitToUnitOfMeasure.sql)
-- =============================================

USE [VietLabs]
GO

IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.analysis_item') AND name = N'unit'
)
BEGIN
    ALTER TABLE [dbo].[analysis_item]
    DROP COLUMN [unit];
    PRINT N'Đã xóa cột unit khỏi analysis_item.';
END
ELSE
    PRINT N'Cột unit không tồn tại trong analysis_item.';
GO
