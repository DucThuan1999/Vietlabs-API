-- =============================================
-- Script kiểm tra các field snapshot của QuotationItem
-- Lấy top 10 quotation item có is_standalone = true
-- và kiểm tra các field snapshot có tồn tại không
-- =============================================

USE [VietLabs]; -- Thay đổi tên database nếu cần
GO

PRINT '=== Kiểm tra các field snapshot của QuotationItem ===';
PRINT '';
GO

-- =============================================
-- 1. Lấy top 10 quotation item có is_standalone = true
-- và kiểm tra các field snapshot
-- =============================================
SELECT TOP 10
    qi.quotation_item_id AS QuotationItemId,
    qi.quotation_id AS QuotationId,
    qi.item_type AS ItemType,
    qi.analysis_item_id AS AnalysisItemId,
    qi.is_standalone AS IsStandalone,
    
    -- Kiểm tra các field snapshot
    -- Field: sample_matrix_name
    CASE 
        WHEN qi.sample_matrix_name IS NULL THEN '❌ NULL'
        WHEN LTRIM(RTRIM(qi.sample_matrix_name)) = '' THEN '⚠️ EMPTY'
        ELSE '✅ Có giá trị'
    END AS SampleMatrixNameStatus,
    qi.sample_matrix_name AS SampleMatrixName,
    
    -- Field: published_group_code
    CASE 
        WHEN qi.published_group_code IS NULL THEN '❌ NULL'
        WHEN LTRIM(RTRIM(qi.published_group_code)) = '' THEN '⚠️ EMPTY'
        ELSE '✅ Có giá trị'
    END AS PublishedGroupCodeStatus,
    qi.published_group_code AS PublishedGroupCode,
    
    -- Field: unit
    CASE 
        WHEN qi.unit IS NULL THEN '❌ NULL'
        WHEN LTRIM(RTRIM(qi.unit)) = '' THEN '⚠️ EMPTY'
        ELSE '✅ Có giá trị'
    END AS UnitStatus,
    qi.unit AS Unit,
    
    -- Field: lod
    CASE 
        WHEN qi.lod IS NULL THEN '❌ NULL'
        WHEN LTRIM(RTRIM(qi.lod)) = '' THEN '⚠️ EMPTY'
        ELSE '✅ Có giá trị'
    END AS LodStatus,
    qi.lod AS Lod,
    
    -- Field: loq
    CASE 
        WHEN qi.loq IS NULL THEN '❌ NULL'
        WHEN LTRIM(RTRIM(qi.loq)) = '' THEN '⚠️ EMPTY'
        ELSE '✅ Có giá trị'
    END AS LoqStatus,
    qi.loq AS Loq,
    
    -- Field: tat
    CASE 
        WHEN qi.tat IS NULL THEN '❌ NULL'
        WHEN LTRIM(RTRIM(qi.tat)) = '' THEN '⚠️ EMPTY'
        ELSE '✅ Có giá trị'
    END AS TatStatus,
    qi.tat AS Tat,
    
    -- Field: default_price
    CASE 
        WHEN qi.default_price IS NULL THEN '❌ NULL'
        ELSE '✅ Có giá trị'
    END AS DefaultPriceStatus,
    qi.default_price AS DefaultPrice,
    
    -- Thông tin bổ sung
    qi.item_code AS ItemCode,
    qi.item_name_vi AS ItemNameVi,
    qi.unit_price AS UnitPrice,
    qi.created_at AS CreatedAt
    
FROM [dbo].[quotation_item] qi
WHERE qi.is_standalone = 1  -- true
ORDER BY qi.created_at DESC;  -- Lấy mới nhất trước
GO

-- =============================================
-- 2. Tổng hợp thống kê các field snapshot
-- =============================================
PRINT '';
PRINT '=== Tổng hợp thống kê các field snapshot ===';
PRINT '';

SELECT 
    'sample_matrix_name' AS FieldName,
    COUNT(*) AS TotalRecords,
    SUM(CASE WHEN sample_matrix_name IS NULL THEN 1 ELSE 0 END) AS NullCount,
    SUM(CASE WHEN sample_matrix_name IS NOT NULL AND LTRIM(RTRIM(sample_matrix_name)) = '' THEN 1 ELSE 0 END) AS EmptyCount,
    SUM(CASE WHEN sample_matrix_name IS NOT NULL AND LTRIM(RTRIM(sample_matrix_name)) != '' THEN 1 ELSE 0 END) AS HasValueCount
FROM [dbo].[quotation_item]
WHERE is_standalone = 1

UNION ALL

SELECT 
    'published_group_code' AS FieldName,
    COUNT(*) AS TotalRecords,
    SUM(CASE WHEN published_group_code IS NULL THEN 1 ELSE 0 END) AS NullCount,
    SUM(CASE WHEN published_group_code IS NOT NULL AND LTRIM(RTRIM(published_group_code)) = '' THEN 1 ELSE 0 END) AS EmptyCount,
    SUM(CASE WHEN published_group_code IS NOT NULL AND LTRIM(RTRIM(published_group_code)) != '' THEN 1 ELSE 0 END) AS HasValueCount
FROM [dbo].[quotation_item]
WHERE is_standalone = 1

UNION ALL

SELECT 
    'unit' AS FieldName,
    COUNT(*) AS TotalRecords,
    SUM(CASE WHEN unit IS NULL THEN 1 ELSE 0 END) AS NullCount,
    SUM(CASE WHEN unit IS NOT NULL AND LTRIM(RTRIM(unit)) = '' THEN 1 ELSE 0 END) AS EmptyCount,
    SUM(CASE WHEN unit IS NOT NULL AND LTRIM(RTRIM(unit)) != '' THEN 1 ELSE 0 END) AS HasValueCount
FROM [dbo].[quotation_item]
WHERE is_standalone = 1

UNION ALL

SELECT 
    'lod' AS FieldName,
    COUNT(*) AS TotalRecords,
    SUM(CASE WHEN lod IS NULL THEN 1 ELSE 0 END) AS NullCount,
    SUM(CASE WHEN lod IS NOT NULL AND LTRIM(RTRIM(lod)) = '' THEN 1 ELSE 0 END) AS EmptyCount,
    SUM(CASE WHEN lod IS NOT NULL AND LTRIM(RTRIM(lod)) != '' THEN 1 ELSE 0 END) AS HasValueCount
FROM [dbo].[quotation_item]
WHERE is_standalone = 1

UNION ALL

SELECT 
    'loq' AS FieldName,
    COUNT(*) AS TotalRecords,
    SUM(CASE WHEN loq IS NULL THEN 1 ELSE 0 END) AS NullCount,
    SUM(CASE WHEN loq IS NOT NULL AND LTRIM(RTRIM(loq)) = '' THEN 1 ELSE 0 END) AS EmptyCount,
    SUM(CASE WHEN loq IS NOT NULL AND LTRIM(RTRIM(loq)) != '' THEN 1 ELSE 0 END) AS HasValueCount
FROM [dbo].[quotation_item]
WHERE is_standalone = 1

UNION ALL

SELECT 
    'tat' AS FieldName,
    COUNT(*) AS TotalRecords,
    SUM(CASE WHEN tat IS NULL THEN 1 ELSE 0 END) AS NullCount,
    SUM(CASE WHEN tat IS NOT NULL AND LTRIM(RTRIM(tat)) = '' THEN 1 ELSE 0 END) AS EmptyCount,
    SUM(CASE WHEN tat IS NOT NULL AND LTRIM(RTRIM(tat)) != '' THEN 1 ELSE 0 END) AS HasValueCount
FROM [dbo].[quotation_item]
WHERE is_standalone = 1

UNION ALL

SELECT 
    'default_price' AS FieldName,
    COUNT(*) AS TotalRecords,
    SUM(CASE WHEN default_price IS NULL THEN 1 ELSE 0 END) AS NullCount,
    0 AS EmptyCount,  -- default_price không có empty string
    SUM(CASE WHEN default_price IS NOT NULL THEN 1 ELSE 0 END) AS HasValueCount
FROM [dbo].[quotation_item]
WHERE is_standalone = 1

ORDER BY FieldName;
GO

-- =============================================
-- 3. Kiểm tra các record có field snapshot bị thiếu
-- =============================================
PRINT '';
PRINT '=== Danh sách các record có field snapshot bị thiếu ===';
PRINT '';

SELECT 
    qi.quotation_item_id AS QuotationItemId,
    qi.analysis_item_id AS AnalysisItemId,
    qi.item_code AS ItemCode,
    qi.item_name_vi AS ItemNameVi,
    
    -- Đếm số field snapshot bị thiếu
    (CASE WHEN qi.sample_matrix_name IS NULL OR LTRIM(RTRIM(qi.sample_matrix_name)) = '' THEN 1 ELSE 0 END +
     CASE WHEN qi.published_group_code IS NULL OR LTRIM(RTRIM(qi.published_group_code)) = '' THEN 1 ELSE 0 END +
     CASE WHEN qi.unit IS NULL OR LTRIM(RTRIM(qi.unit)) = '' THEN 1 ELSE 0 END +
     CASE WHEN qi.lod IS NULL OR LTRIM(RTRIM(qi.lod)) = '' THEN 1 ELSE 0 END +
     CASE WHEN qi.loq IS NULL OR LTRIM(RTRIM(qi.loq)) = '' THEN 1 ELSE 0 END +
     CASE WHEN qi.tat IS NULL OR LTRIM(RTRIM(qi.tat)) = '' THEN 1 ELSE 0 END +
     CASE WHEN qi.default_price IS NULL THEN 1 ELSE 0 END) AS MissingFieldsCount,
    
    -- Liệt kê các field bị thiếu
    STUFF(
        (CASE WHEN qi.sample_matrix_name IS NULL OR LTRIM(RTRIM(qi.sample_matrix_name)) = '' THEN ', sample_matrix_name' ELSE '' END +
         CASE WHEN qi.published_group_code IS NULL OR LTRIM(RTRIM(qi.published_group_code)) = '' THEN ', published_group_code' ELSE '' END +
         CASE WHEN qi.unit IS NULL OR LTRIM(RTRIM(qi.unit)) = '' THEN ', unit' ELSE '' END +
         CASE WHEN qi.lod IS NULL OR LTRIM(RTRIM(qi.lod)) = '' THEN ', lod' ELSE '' END +
         CASE WHEN qi.loq IS NULL OR LTRIM(RTRIM(qi.loq)) = '' THEN ', loq' ELSE '' END +
         CASE WHEN qi.tat IS NULL OR LTRIM(RTRIM(qi.tat)) = '' THEN ', tat' ELSE '' END +
         CASE WHEN qi.default_price IS NULL THEN ', default_price' ELSE '' END),
        1, 2, '') AS MissingFieldsList
    
FROM [dbo].[quotation_item] qi
WHERE qi.is_standalone = 1
    AND (
        qi.sample_matrix_name IS NULL OR LTRIM(RTRIM(qi.sample_matrix_name)) = '' OR
        qi.published_group_code IS NULL OR LTRIM(RTRIM(qi.published_group_code)) = '' OR
        qi.unit IS NULL OR LTRIM(RTRIM(qi.unit)) = '' OR
        qi.lod IS NULL OR LTRIM(RTRIM(qi.lod)) = '' OR
        qi.loq IS NULL OR LTRIM(RTRIM(qi.loq)) = '' OR
        qi.tat IS NULL OR LTRIM(RTRIM(qi.tat)) = '' OR
        qi.default_price IS NULL
    )
ORDER BY MissingFieldsCount DESC, qi.created_at DESC;
GO

PRINT '';
PRINT 'Hoàn thành kiểm tra!';
GO

