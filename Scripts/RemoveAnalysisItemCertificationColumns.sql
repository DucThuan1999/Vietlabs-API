-- =============================================
-- Xóa các cột chứng nhận (Iso, CucBvtv, BoCongThuong, Nafi, CucChanNuoi)
-- Chỉ giữ nd_107 và nd_107_expired_date trên bảng analysis_item
-- Trước khi DROP COLUMN phải xóa Default Constraint gắn với cột đó
-- =============================================

USE [VietLabs]
GO

DECLARE @tableId INT = OBJECT_ID(N'[dbo].[analysis_item]');
DECLARE @constraintName SYSNAME;
DECLARE @sql NVARCHAR(MAX);

-- Bước 1: Xóa tất cả default constraint của các cột cần drop
DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
    SELECT dc.name
    FROM sys.default_constraints dc
    INNER JOIN sys.columns c ON dc.parent_column_id = c.column_id AND dc.parent_object_id = c.object_id
    WHERE dc.parent_object_id = @tableId
      AND c.name IN (N'iso', N'cuc_bvtv', N'bo_cong_thuong', N'nafi', N'cuc_chan_nuoi');

OPEN cur;
FETCH NEXT FROM cur INTO @constraintName;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @sql = N'ALTER TABLE [dbo].[analysis_item] DROP CONSTRAINT [' + REPLACE(@constraintName, N']', N']]') + N'];';
    EXEC sp_executesql @sql;
    PRINT N'Đã xóa constraint: ' + @constraintName;
    FETCH NEXT FROM cur INTO @constraintName;
END;
CLOSE cur;
DEALLOCATE cur;

-- Bước 2: Drop các cột (mỗi cột một lệnh để tránh lỗi)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = @tableId AND name = N'iso')
BEGIN
    ALTER TABLE [dbo].[analysis_item] DROP COLUMN [iso];
    PRINT N'Đã xóa cột iso.';
END
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = @tableId AND name = N'iso_expired_date')
BEGIN
    ALTER TABLE [dbo].[analysis_item] DROP COLUMN [iso_expired_date];
    PRINT N'Đã xóa cột iso_expired_date.';
END
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = @tableId AND name = N'cuc_bvtv')
BEGIN
    ALTER TABLE [dbo].[analysis_item] DROP COLUMN [cuc_bvtv];
    PRINT N'Đã xóa cột cuc_bvtv.';
END
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = @tableId AND name = N'cuc_bvtv_expired_date')
BEGIN
    ALTER TABLE [dbo].[analysis_item] DROP COLUMN [cuc_bvtv_expired_date];
    PRINT N'Đã xóa cột cuc_bvtv_expired_date.';
END
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = @tableId AND name = N'bo_cong_thuong')
BEGIN
    ALTER TABLE [dbo].[analysis_item] DROP COLUMN [bo_cong_thuong];
    PRINT N'Đã xóa cột bo_cong_thuong.';
END
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = @tableId AND name = N'bo_cong_thuong_expired_date')
BEGIN
    ALTER TABLE [dbo].[analysis_item] DROP COLUMN [bo_cong_thuong_expired_date];
    PRINT N'Đã xóa cột bo_cong_thuong_expired_date.';
END
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = @tableId AND name = N'nafi')
BEGIN
    ALTER TABLE [dbo].[analysis_item] DROP COLUMN [nafi];
    PRINT N'Đã xóa cột nafi.';
END
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = @tableId AND name = N'nafi_expired_date')
BEGIN
    ALTER TABLE [dbo].[analysis_item] DROP COLUMN [nafi_expired_date];
    PRINT N'Đã xóa cột nafi_expired_date.';
END
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = @tableId AND name = N'cuc_chan_nuoi')
BEGIN
    ALTER TABLE [dbo].[analysis_item] DROP COLUMN [cuc_chan_nuoi];
    PRINT N'Đã xóa cột cuc_chan_nuoi.';
END
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = @tableId AND name = N'cuc_chan_nuoi_expired_date')
BEGIN
    ALTER TABLE [dbo].[analysis_item] DROP COLUMN [cuc_chan_nuoi_expired_date];
    PRINT N'Đã xóa cột cuc_chan_nuoi_expired_date.';
END

-- Đảm bảo nd_107_expired_date tồn tại (nếu chưa có thì thêm)
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = @tableId AND name = N'nd_107_expired_date')
BEGIN
    ALTER TABLE [dbo].[analysis_item] ADD [nd_107_expired_date] DATE NULL;
    PRINT N'Đã thêm cột nd_107_expired_date.';
END
ELSE
    PRINT N'Cột nd_107_expired_date đã tồn tại.';

PRINT N'Hoàn tất.';
GO
