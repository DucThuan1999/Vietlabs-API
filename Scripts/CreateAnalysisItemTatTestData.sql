-- Script tạo dữ liệu test cho AnalysisItemTAT
-- Dựa trên dữ liệu AnalysisItem có sẵn trong database
-- Mỗi AnalysisItem sẽ có 3 loại TAT: Thường (Normal), Nhanh (Fast), Khẩn (Urgent)

USE [VietLabs]; -- Thay đổi tên database nếu cần
GO

-- =============================================
-- TẠO DỮ LIỆU TEST CHO ANALYSIS_ITEM_TAT
-- =============================================

-- Xóa dữ liệu cũ nếu muốn (tùy chọn - comment nếu không muốn xóa)
-- DELETE FROM analysis_item_tat;

-- Biến để đếm số lượng record đã tạo
DECLARE @RecordCount INT = 0;
DECLARE @AnalysisItemCount INT = 0;

-- Đếm số lượng AnalysisItem có sẵn
SELECT @AnalysisItemCount = COUNT(*) FROM analysis_item WHERE status = 'Active';

PRINT N'Số lượng AnalysisItem có sẵn: ' + CAST(@AnalysisItemCount AS NVARCHAR(10));

-- Tạo TAT cho tất cả AnalysisItem có status = 'Active'
-- Sử dụng INSERT với SELECT để tạo nhiều record cùng lúc (hiệu quả hơn CURSOR)

-- 1. TAT Thường (Normal) - 7 ngày
INSERT INTO analysis_item_tat (
    analysis_item_tat_id,
    analysis_item_id,
    tat_type,
    tat_value,
    tat_unit,
    notes,
    created_at
)
SELECT 
    NEWID() AS analysis_item_tat_id,
    ai.analysis_item_id,
    'Thường' AS tat_type,
    7 AS tat_value,
    'Days' AS tat_unit,
    N'TAT thường cho xét nghiệm này' AS notes,
    GETUTCDATE() AS created_at
FROM analysis_item ai
WHERE ai.status = 'Active'
AND NOT EXISTS (
    SELECT 1 
    FROM analysis_item_tat tat 
    WHERE tat.analysis_item_id = ai.analysis_item_id 
    AND tat.tat_type = 'Thường'
);

SET @RecordCount = @RecordCount + @@ROWCOUNT;

-- 2. TAT Nhanh (Fast) - 3 ngày
INSERT INTO analysis_item_tat (
    analysis_item_tat_id,
    analysis_item_id,
    tat_type,
    tat_value,
    tat_unit,
    notes,
    created_at
)
SELECT 
    NEWID() AS analysis_item_tat_id,
    ai.analysis_item_id,
    'Nhanh' AS tat_type,
    3 AS tat_value,
    'Days' AS tat_unit,
    N'TAT nhanh cho xét nghiệm này' AS notes,
    GETUTCDATE() AS created_at
FROM analysis_item ai
WHERE ai.status = 'Active'
AND NOT EXISTS (
    SELECT 1 
    FROM analysis_item_tat tat 
    WHERE tat.analysis_item_id = ai.analysis_item_id 
    AND tat.tat_type = 'Nhanh'
);

SET @RecordCount = @RecordCount + @@ROWCOUNT;

-- 3. TAT Khẩn (Urgent) - 1 ngày
INSERT INTO analysis_item_tat (
    analysis_item_tat_id,
    analysis_item_id,
    tat_type,
    tat_value,
    tat_unit,
    notes,
    created_at
)
SELECT 
    NEWID() AS analysis_item_tat_id,
    ai.analysis_item_id,
    'Khẩn' AS tat_type,
    1 AS tat_value,
    'Days' AS tat_unit,
    N'TAT khẩn cho xét nghiệm này' AS notes,
    GETUTCDATE() AS created_at
FROM analysis_item ai
WHERE ai.status = 'Active'
AND NOT EXISTS (
    SELECT 1 
    FROM analysis_item_tat tat 
    WHERE tat.analysis_item_id = ai.analysis_item_id 
    AND tat.tat_type = 'Khẩn'
);

SET @RecordCount = @RecordCount + @@ROWCOUNT;

PRINT N'Đã tạo ' + CAST(@RecordCount AS NVARCHAR(10)) + N' bản ghi AnalysisItemTAT!';
GO

-- =============================================
-- HIỂN THỊ KẾT QUẢ
-- =============================================

-- Thống kê số lượng TAT theo loại
SELECT 
    tat_type AS Loai_TAT,
    COUNT(*) AS So_luong,
    AVG(tat_value) AS Gia_tri_TAT_trung_binh,
    MIN(tat_value) AS Gia_tri_TAT_min,
    MAX(tat_value) AS Gia_tri_TAT_max
FROM analysis_item_tat
GROUP BY tat_type
ORDER BY tat_type;

-- Xem chi tiết TAT của một số AnalysisItem mẫu (10 item đầu tiên)
SELECT TOP 10
    ai.analysis_item_code AS Ma_chi_tieu,
    ai.name_vi AS Ten_chi_tieu,
    tat.tat_type AS Loai_TAT,
    tat.tat_value AS Gia_tri,
    tat.tat_unit AS Don_vi,
    tat.notes AS Ghi_chu,
    tat.created_at AS Ngay_tao
FROM analysis_item ai
INNER JOIN analysis_item_tat tat ON ai.analysis_item_id = tat.analysis_item_id
WHERE ai.status = 'Active'
ORDER BY ai.created_at, tat.tat_type;

-- Thống kê số lượng AnalysisItem có đầy đủ 3 loại TAT
SELECT 
    COUNT(DISTINCT analysis_item_id) AS So_chi_tieu_co_day_du_3_TAT
FROM (
    SELECT analysis_item_id, COUNT(*) AS tat_count
    FROM analysis_item_tat
    GROUP BY analysis_item_id
    HAVING COUNT(*) = 3
) AS subquery;

-- Thống kê số lượng AnalysisItem chưa có TAT nào
SELECT 
    COUNT(*) AS So_chi_tieu_chua_co_TAT
FROM analysis_item ai
WHERE ai.status = 'Active'
AND NOT EXISTS (
    SELECT 1 
    FROM analysis_item_tat tat 
    WHERE tat.analysis_item_id = ai.analysis_item_id
);

PRINT N'Hoàn tất tạo dữ liệu test cho AnalysisItemTAT!';
GO

