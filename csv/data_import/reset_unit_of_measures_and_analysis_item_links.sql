/*
  Reset hoàn toàn đơn vị tính (ĐVT) để nhập lại từ đầu.

  1) Gỡ liên kết trên chỉ tiêu: analysis_item.unit_of_measure_id,
     analysis_item.standard_quantity_unit_of_measure_id → NULL.
  2) Xóa toàn bộ dòng dbo.unit_of_measure.

  Theo schema EF hiện tại, chỉ analysis_item tham chiếu unit_of_measure qua hai cột trên.
  Cột quotation_item.Unit là chuỗi snapshot — script này không sửa báo giá cũ.

  Chạy trong SSMS / sqlcmd sau khi đã backup DB nếu cần.
*/

SET NOCOUNT ON;

BEGIN TRANSACTION;

DECLARE @ai_uom INT = (
    SELECT COUNT(*)
    FROM dbo.analysis_item
    WHERE unit_of_measure_id IS NOT NULL
       OR standard_quantity_unit_of_measure_id IS NOT NULL
);
DECLARE @uom INT = (SELECT COUNT(*) FROM dbo.unit_of_measure);

UPDATE dbo.analysis_item
SET
    unit_of_measure_id = NULL,
    standard_quantity_unit_of_measure_id = NULL,
    updated_at = SYSUTCDATETIME()
WHERE unit_of_measure_id IS NOT NULL
   OR standard_quantity_unit_of_measure_id IS NOT NULL;

DELETE FROM dbo.unit_of_measure;

SELECT
    @ai_uom AS analysis_item_rows_had_uom_fk,
    @uom AS unit_of_measure_rows_deleted;

COMMIT TRANSACTION;

SELECT N'Hoàn tất: đã gỡ ĐVT trên chỉ tiêu và xóa danh mục unit_of_measure.' AS message;
