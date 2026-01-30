-- Script tạo bảng analysis_item_tat
-- Bảng này lưu thông tin TAT (Turn Around Time) cho AnalysisItem
-- Mỗi AnalysisItem có thể có 3 loại TAT: Thường (Normal), Nhanh (Fast), Khẩn (Urgent)
-- Chạy script này nếu bảng chưa tồn tại trong database

USE [VietLabs]; -- Thay đổi tên database nếu cần
GO

-- =============================================
-- TẠO BẢNG analysis_item_tat
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[analysis_item_tat]') AND type in (N'U'))
BEGIN
    -- Tạo bảng analysis_item_tat
    CREATE TABLE [dbo].[analysis_item_tat] (
        [analysis_item_tat_id] UNIQUEIDENTIFIER NOT NULL,
        [analysis_item_id] UNIQUEIDENTIFIER NOT NULL,
        [tat_type] NVARCHAR(50) NOT NULL, -- "Normal", "Fast", "Urgent" hoặc "Thường", "Nhanh", "Khẩn"
        [tat_value] INT NOT NULL, -- Giá trị TAT (số ngày hoặc giờ)
        [tat_unit] NVARCHAR(20) NOT NULL DEFAULT 'Days', -- "Days" hoặc "Hours"
        [notes] NVARCHAR(1000) NULL,
        [created_at] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [updated_at] DATETIME2 NULL,
        CONSTRAINT [PK_analysis_item_tat] PRIMARY KEY CLUSTERED ([analysis_item_tat_id] ASC),
        CONSTRAINT [FK_analysis_item_tat_analysis_item] FOREIGN KEY ([analysis_item_id]) 
            REFERENCES [dbo].[analysis_item] ([analysis_item_id]) 
            ON DELETE CASCADE
    );
    
    -- Unique constraint: Mỗi AnalysisItem chỉ có 1 TAT cho mỗi loại
    CREATE UNIQUE NONCLUSTERED INDEX [IX_analysis_item_tat_item_type] 
        ON [dbo].[analysis_item_tat] ([analysis_item_id], [tat_type]);
    
    -- Index cho analysis_item_id để tăng hiệu suất query
    CREATE NONCLUSTERED INDEX [IX_analysis_item_tat_analysis_item_id] 
        ON [dbo].[analysis_item_tat] ([analysis_item_id]);
    
    -- Index cho tat_type để tăng hiệu suất query theo loại TAT
    CREATE NONCLUSTERED INDEX [IX_analysis_item_tat_tat_type] 
        ON [dbo].[analysis_item_tat] ([tat_type]);
    
    PRINT 'Bảng analysis_item_tat đã được tạo thành công!';
END
ELSE
BEGIN
    PRINT 'Bảng analysis_item_tat đã tồn tại.';
END
GO

-- =============================================
-- THÊM CHECK CONSTRAINT ĐỂ ĐẢM BẢO TAT_TYPE HỢP LỆ
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_analysis_item_tat_tat_type')
BEGIN
    ALTER TABLE [dbo].[analysis_item_tat]
    ADD CONSTRAINT [CK_analysis_item_tat_tat_type] 
    CHECK ([tat_type] IN ('Normal', 'Fast', 'Urgent', 'Thường', 'Nhanh', 'Khẩn'));
    
    PRINT 'Check constraint cho tat_type đã được thêm thành công!';
END
ELSE
BEGIN
    PRINT 'Check constraint cho tat_type đã tồn tại.';
END
GO

-- =============================================
-- THÊM CHECK CONSTRAINT ĐỂ ĐẢM BẢO TAT_UNIT HỢP LỆ
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_analysis_item_tat_tat_unit')
BEGIN
    ALTER TABLE [dbo].[analysis_item_tat]
    ADD CONSTRAINT [CK_analysis_item_tat_tat_unit] 
    CHECK ([tat_unit] IN ('Days', 'Hours', 'Ngày', 'Giờ'));
    
    PRINT 'Check constraint cho tat_unit đã được thêm thành công!';
END
ELSE
BEGIN
    PRINT 'Check constraint cho tat_unit đã tồn tại.';
END
GO

-- =============================================
-- THÊM CHECK CONSTRAINT ĐỂ ĐẢM BẢO TAT_VALUE > 0
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_analysis_item_tat_tat_value')
BEGIN
    ALTER TABLE [dbo].[analysis_item_tat]
    ADD CONSTRAINT [CK_analysis_item_tat_tat_value] 
    CHECK ([tat_value] > 0);
    
    PRINT 'Check constraint cho tat_value đã được thêm thành công!';
END
ELSE
BEGIN
    PRINT 'Check constraint cho tat_value đã tồn tại.';
END
GO

PRINT 'Hoàn tất tạo bảng analysis_item_tat và các constraints!';
GO

