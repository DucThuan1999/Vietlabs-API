-- =============================================
-- Xóa toàn bộ data trong reference_method
-- (analysis_item có FK reference_method_id -> cần gán NULL trước khi xóa)
-- =============================================

USE [VietLabs]
GO

-- Bước 1: Gỡ liên kết từ analysis_item sang reference_method
UPDATE [dbo].[analysis_item]
SET [reference_method_id] = NULL
WHERE [reference_method_id] IS NOT NULL;

-- Bước 2: Xóa toàn bộ bản ghi trong reference_method
DELETE FROM [dbo].[reference_method];

PRINT N'Đã xóa toàn bộ data trong reference_method và gỡ FK từ analysis_item.';
GO
