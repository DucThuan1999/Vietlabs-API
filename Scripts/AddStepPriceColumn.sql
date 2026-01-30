-- =============================================
-- Script nhanh: Thêm cột step_price vào bảng analysis_group
-- =============================================

USE [VietLabs];
GO

-- Thêm cột step_price nếu chưa tồn tại
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[analysis_group]') 
    AND name = 'step_price'
)
BEGIN
    ALTER TABLE [dbo].[analysis_group]
    ADD [step_price] DECIMAL(18,2) NULL;
    
    PRINT 'Đã thêm cột step_price thành công!';
END
ELSE
BEGIN
    PRINT 'Cột step_price đã tồn tại.';
END
GO

