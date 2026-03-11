-- =============================================
-- Cập nhật lại reference_method: gán reference_method_code = PP-001, PP-002, ...
-- theo thứ tự sequence_number (dùng khi đã nhập data nhưng bị sai)
-- =============================================

USE [VietLabs]
GO

DECLARE @Now DATETIME2 = SYSUTCDATETIME();
DECLARE @UpdatedBy UNIQUEIDENTIFIER = NULL;  -- Gán account_id nếu cần

;WITH Ordered AS (
    SELECT
        reference_method_id,
        ROW_NUMBER() OVER (ORDER BY sequence_number, reference_method_id) AS rn
    FROM [dbo].[reference_method]
)
UPDATE rm
SET
    rm.reference_method_code = N'PP-' + RIGHT(N'000' + CAST(o.rn AS NVARCHAR(10)), 3),
    rm.updated_at             = @Now,
    rm.updated_by             = @UpdatedBy
FROM [dbo].[reference_method] rm
INNER JOIN Ordered o ON o.reference_method_id = rm.reference_method_id;

PRINT N'Đã cập nhật reference_method_code = PP-001, PP-002, ... và updated_at/updated_by.';
GO
