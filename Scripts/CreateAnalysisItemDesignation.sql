-- =============================================
-- Bảng trung gian: Chỉ tiêu - Chỉ định (có ngày hết hạn)
-- 1 AnalysisItem có nhiều Designation, mỗi bản ghi có expired_date
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'analysis_item_designation')
BEGIN
    CREATE TABLE [dbo].[analysis_item_designation] (
        [analysis_item_designation_id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [analysis_item_id] UNIQUEIDENTIFIER NOT NULL,
        [designation_id] UNIQUEIDENTIFIER NOT NULL,
        [expired_date] DATE NULL,
        CONSTRAINT [FK_analysis_item_designation_analysis_item]
            FOREIGN KEY ([analysis_item_id])
            REFERENCES [dbo].[analysis_item] ([analysis_item_id]) ON DELETE CASCADE,
        CONSTRAINT [FK_analysis_item_designation_designation]
            FOREIGN KEY ([designation_id])
            REFERENCES [dbo].[designation] ([designation_id]) ON DELETE NO ACTION,
        CONSTRAINT [UQ_analysis_item_designation]
            UNIQUE ([analysis_item_id], [designation_id])
    );
    PRINT N'Đã tạo bảng analysis_item_designation.';
END
ELSE
    PRINT N'Bảng analysis_item_designation đã tồn tại.';
GO

