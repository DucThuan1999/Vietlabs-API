-- =============================================
-- Thêm cột updated_by (người cập nhật) vào analysis_item
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.analysis_item') AND name = N'updated_by'
)
BEGIN
    ALTER TABLE [dbo].[analysis_item]
    ADD [updated_by] UNIQUEIDENTIFIER NULL;

    ALTER TABLE [dbo].[analysis_item]
    ADD CONSTRAINT [FK_analysis_item_updated_by]
        FOREIGN KEY ([updated_by]) REFERENCES [dbo].[account] ([account_id]) ON DELETE NO ACTION;

    PRINT N'Đã thêm cột updated_by và FK vào analysis_item.';
END
ELSE
    PRINT N'Cột updated_by đã tồn tại trong analysis_item.';
GO

-- Gán giá trị mặc định cho bản ghi hiện có (account_id đã dùng lúc trước)
UPDATE [dbo].[analysis_item]
SET [updated_by] = '94eab415-1624-49de-85a6-a80916db3ab2',
    [updated_at] = SYSUTCDATETIME()
WHERE [updated_by] IS NULL;

PRINT N'Hoàn tất gán người cập nhật cho analysis_item.';
GO
