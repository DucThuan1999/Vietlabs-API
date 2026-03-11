-- =============================================
-- Insert danh mục Đơn vị tính (unit_of_measure)
-- Các giá trị ĐVT từ analysis_item.unit
-- Chạy sau CreateUnitOfMeasure.sql và AddUnitOfMeasureIdToAnalysisItem.sql (nếu cần cột FK)
-- =============================================

USE [VietLabs]
GO

-- Chỉ insert nếu bảng trống (tránh insert trùng khi chạy lại)
IF NOT EXISTS (SELECT 1 FROM [dbo].[unit_of_measure])
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[unit_of_measure] ([unit_of_measure_id], [sequence_number], [unit_of_measure_code], [name_vi], [name_en], [status])
    VALUES
        (NEWID(), 1, N'UOM001', N'0', NULL, N'Active'),
        (NEWID(), 2, N'UOM002', N'mg/kg', NULL, N'Active'),
        (NEWID(), 3, N'UOM003', N'-', NULL, N'Active'),
        (NEWID(), 4, N'UOM004', N'%', NULL, N'Active'),
        (NEWID(), 5, N'UOM005', N'µg/kg', NULL, N'Active'),
        (NEWID(), 6, N'UOM006', N'pgTEQ/g', NULL, N'Active'),
        (NEWID(), 7, N'UOM007', N'CFU/g', NULL, N'Active'),
        (NEWID(), 8, N'UOM008', N'Kcal/100g', NULL, N'Active'),
        (NEWID(), 9, N'UOM009', N'g/100g', NULL, N'Active'),
        (NEWID(), 10, N'UOM010', N'mg/L', NULL, N'Active'),
        (NEWID(), 11, N'UOM011', N'Bq/lit', NULL, N'Active'),
        (NEWID(), 12, N'UOM012', N'Bản sao/phản ứng (copies/reaction)', NULL, N'Active'),
        (NEWID(), 13, N'UOM013', N'µg/L', NULL, N'Active'),
        (NEWID(), 14, N'UOM014', N'/25g', NULL, N'Active'),
        (NEWID(), 15, N'UOM015', N'mg/g', NULL, N'Active'),
        (NEWID(), 16, N'UOM016', N' ', NULL, N'Active'),
        (NEWID(), 17, N'UOM017', N'/g', NULL, N'Active'),
        (NEWID(), 18, N'UOM018', N'copies/reaction', NULL, N'Active'),
        (NEWID(), 19, N'UOM019', N'Bq/lít', NULL, N'Active'),
        (NEWID(), 20, N'UOM020', N'mgHg/kg', NULL, N'Active'),
        (NEWID(), 21, N'UOM021', N'DN', NULL, N'Active'),
        (NEWID(), 22, N'UOM022', N'mS/cm', NULL, N'Active'),
        (NEWID(), 23, N'UOM023', N'ml NaOH 1N/kg', NULL, N'Active'),
        (NEWID(), 24, N'UOM024', N'mg/kg As PO4(3-)', NULL, N'Active'),
        (NEWID(), 25, N'UOM025', N'Phát hiện/mẫu', NULL, N'Active'),
        (NEWID(), 26, N'UOM026', N'mg/100g', NULL, N'Active'),
        (NEWID(), 27, N'UOM027', N'/10L', NULL, N'Active'),
        (NEWID(), 28, N'UOM028', N'CFU/ml', NULL, N'Active'),
        (NEWID(), 29, N'UOM029', N'/100ml', NULL, N'Active'),
        (NEWID(), 30, N'UOM030', N'µg/k', NULL, N'Active'),
        (NEWID(), 31, N'UOM031', N'mgL', NULL, N'Active'),
        (NEWID(), 32, N'UOM032', N'mg/kg as P2O5', NULL, N'Active'),
        (NEWID(), 33, N'UOM033', N'µg/g', NULL, N'Active'),
        (NEWID(), 34, N'UOM034', N'mg/kg ', NULL, N'Active'),
        (NEWID(), 35, N'UOM035', N'µg/ml', NULL, N'Active'),
        (NEWID(), 36, N'UOM036', N'%(m/m)', NULL, N'Active'),
        (NEWID(), 37, N'UOM037', N'% as P', NULL, N'Active');

    PRINT N'Đã insert ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + N' đơn vị tính vào unit_of_measure.';
END
ELSE
    PRINT N'Bảng unit_of_measure đã có dữ liệu, bỏ qua insert.';
GO
