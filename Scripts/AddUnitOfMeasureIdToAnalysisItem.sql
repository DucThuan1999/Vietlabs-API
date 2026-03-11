-- =============================================
-- Thêm cột unit_of_measure_id vào analysis_item (ĐVT – Đơn vị tính)
-- Liên kết với bảng unit_of_measure, tương ứng field Unit (ĐVT) hiện tại
-- Chạy Scripts/CreateUnitOfMeasure.sql TRƯỚC script này.
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'unit_of_measure')
BEGIN
    RAISERROR(N'Bảng unit_of_measure chưa tồn tại. Hãy chạy Scripts/CreateUnitOfMeasure.sql trước.', 16, 1);
    RETURN;
END

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.analysis_item') AND name = N'unit_of_measure_id'
)
BEGIN
    ALTER TABLE [dbo].[analysis_item]
    ADD [unit_of_measure_id] UNIQUEIDENTIFIER NULL;

    ALTER TABLE [dbo].[analysis_item]
    ADD CONSTRAINT [FK_analysis_item_unit_of_measure]
        FOREIGN KEY ([unit_of_measure_id]) REFERENCES [dbo].[unit_of_measure] ([unit_of_measure_id]) ON DELETE NO ACTION;

    PRINT N'Đã thêm cột unit_of_measure_id và FK vào analysis_item.';
END
ELSE
    PRINT N'Cột unit_of_measure_id đã tồn tại trong analysis_item.';
GO
