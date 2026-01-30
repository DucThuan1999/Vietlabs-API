-- Script SQL để import dữ liệu từ CSV vào database
-- LƯU Ý: Script này giả định đã có dữ liệu CSV được load vào bảng tạm
-- Hoặc sử dụng BULK INSERT / OPENROWSET

USE [VietLabs];
GO

-- =============================================
-- BƯỚC 1: TẠO BẢNG TẠM ĐỂ LƯU DỮ LIỆU CSV
-- =============================================

-- Bảng tạm cho Country
IF OBJECT_ID('tempdb..#CountryTemp') IS NOT NULL DROP TABLE #CountryTemp;
CREATE TABLE #CountryTemp (
    STT INT,
    NameEn NVARCHAR(200),
    FullNameVi NVARCHAR(500),
    FullNameEn NVARCHAR(500),
    Alpha2 NVARCHAR(2),
    Alpha3 NVARCHAR(3),
    Status NVARCHAR(50),
    Notes NVARCHAR(2000)
);

-- Bảng tạm cho Province
IF OBJECT_ID('tempdb..#ProvinceTemp') IS NOT NULL DROP TABLE #ProvinceTemp;
CREATE TABLE #ProvinceTemp (
    STT INT,
    Name NVARCHAR(200),
    Type NVARCHAR(100),
    FullName NVARCHAR(500),
    CountryName NVARCHAR(500), -- Tên quốc gia (text)
    Status NVARCHAR(50),
    Notes NVARCHAR(2000)
);

-- Bảng tạm cho Ward
IF OBJECT_ID('tempdb..#WardTemp') IS NOT NULL DROP TABLE #WardTemp;
CREATE TABLE #WardTemp (
    STT INT,
    Code NVARCHAR(50),
    Name NVARCHAR(200),
    Type NVARCHAR(100),
    ProvinceName NVARCHAR(500), -- Tên tỉnh/thành phố (text)
    CountryName NVARCHAR(500), -- Tên quốc gia (text)
    Status NVARCHAR(50),
    Notes NVARCHAR(2000)
);

PRINT 'Đã tạo các bảng tạm.';
GO

-- =============================================
-- BƯỚC 1.5: LOAD DỮ LIỆU CSV VÀO BẢNG TẠM
-- =============================================
-- LƯU Ý: Bạn cần thay đổi đường dẫn file CSV cho phù hợp
-- Có 2 cách: BULK INSERT (file trên server) hoặc OPENROWSET (file local/network)

-- CÁCH 1: Sử dụng BULK INSERT (file CSV phải ở trên SQL Server)
-- Bạn cần copy file CSV lên SQL Server trước, ví dụ: C:\Temp\csv\

/*
-- Load country.csv
BULK INSERT #CountryTemp
FROM 'C:\Temp\csv\country.csv'
WITH (
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '\n',
    FIRSTROW = 2,
    CODEPAGE = '65001' -- UTF-8
);

-- Load provinces.csv
BULK INSERT #ProvinceTemp
FROM 'C:\Temp\csv\provinces.csv'
WITH (
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '\n',
    FIRSTROW = 2,
    CODEPAGE = '65001' -- UTF-8
);

-- Load ward.csv
BULK INSERT #WardTemp
FROM 'C:\Temp\csv\ward.csv'
WITH (
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '\n',
    FIRSTROW = 2,
    CODEPAGE = '65001' -- UTF-8
);
*/

-- CÁCH 2: Sử dụng OPENROWSET (cần enable Ad Hoc Distributed Queries)
-- EXEC sp_configure 'show advanced options', 1;
-- RECONFIGURE;
-- EXEC sp_configure 'Ad Hoc Distributed Queries', 1;
-- RECONFIGURE;

/*
-- Load country.csv
INSERT INTO #CountryTemp (STT, NameEn, FullNameVi, FullNameEn, Alpha2, Alpha3, Status, Notes)
SELECT * FROM OPENROWSET(
    'Microsoft.ACE.OLEDB.12.0',
    'Text;Database=C:\Temp\csv\;HDR=YES;',
    'SELECT * FROM [country.csv]'
);

-- Load provinces.csv
INSERT INTO #ProvinceTemp (STT, Name, Type, FullName, CountryName, Status, Notes)
SELECT * FROM OPENROWSET(
    'Microsoft.ACE.OLEDB.12.0',
    'Text;Database=C:\Temp\csv\;HDR=YES;',
    'SELECT * FROM [provinces.csv]'
);

-- Load ward.csv
INSERT INTO #WardTemp (STT, Code, Name, Type, ProvinceName, CountryName, Status, Notes)
SELECT * FROM OPENROWSET(
    'Microsoft.ACE.OLEDB.12.0',
    'Text;Database=C:\Temp\csv\;HDR=YES;',
    'SELECT * FROM [ward.csv]'
);
*/

-- CÁCH 3: Sử dụng SQL Server Import/Export Wizard
-- 1. Right-click database → Tasks → Import Data
-- 2. Chọn CSV file làm source
-- 3. Chọn bảng tạm (#CountryTemp, #ProvinceTemp, #WardTemp) làm destination
-- 4. Map columns và chạy import

PRINT 'LƯU Ý: Bạn cần uncomment và chạy một trong các cách trên để load CSV vào bảng tạm.';
PRINT 'Hoặc sử dụng SQL Server Import/Export Wizard để import CSV vào bảng tạm.';
PRINT 'Sau đó tiếp tục chạy phần import bên dưới.';
GO

-- =============================================
-- BƯỚC 2: IMPORT COUNTRY (Phải import trước)
-- =============================================

-- Tạo mapping table để lưu CountryId
IF OBJECT_ID('tempdb..#CountryMapping') IS NOT NULL DROP TABLE #CountryMapping;
CREATE TABLE #CountryMapping (
    FullNameVi NVARCHAR(500),
    CountryId UNIQUEIDENTIFIER
);

-- Insert countries và tạo mapping
INSERT INTO [dbo].[country] (
    [country_id],
    [sequence_number],
    [name_en],
    [full_name_vi],
    [full_name_en],
    [alpha_2],
    [alpha_3],
    [status],
    [notes]
)
OUTPUT inserted.full_name_vi, inserted.country_id INTO #CountryMapping
SELECT 
    NEWID() AS country_id,
    STT AS sequence_number,
    NameEn AS name_en,
    FullNameVi AS full_name_vi,
    FullNameEn AS full_name_en,
    Alpha2 AS alpha_2,
    Alpha3 AS alpha_3,
    CASE WHEN Status = 'Actived' THEN 'Active' ELSE Status END AS status,
    Notes AS notes
FROM #CountryTemp
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[country] c 
    WHERE c.full_name_vi = #CountryTemp.FullNameVi
);

PRINT 'Đã import ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' quốc gia.';
GO

-- =============================================
-- BƯỚC 3: IMPORT PROVINCE (Sau khi có Country)
-- =============================================

-- Tạo mapping table để lưu ProvinceId
IF OBJECT_ID('tempdb..#ProvinceMapping') IS NOT NULL DROP TABLE #ProvinceMapping;
CREATE TABLE #ProvinceMapping (
    Name NVARCHAR(200),
    ProvinceId UNIQUEIDENTIFIER
);

-- Insert provinces và tạo mapping
INSERT INTO [dbo].[province] (
    [province_id],
    [sequence_number],
    [name],
    [type],
    [full_name],
    [country_id],
    [status],
    [notes]
)
OUTPUT inserted.name, inserted.province_id INTO #ProvinceMapping
SELECT 
    NEWID() AS province_id,
    pt.STT AS sequence_number,
    pt.Name AS name,
    pt.Type AS type,
    pt.FullName AS full_name,
    cm.CountryId AS country_id, -- Map từ CountryName text → CountryId
    CASE WHEN pt.Status = 'Actived' THEN 'Active' ELSE pt.Status END AS status,
    pt.Notes AS notes
FROM #ProvinceTemp pt
INNER JOIN #CountryMapping cm ON pt.CountryName = cm.FullNameVi
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[province] p 
    WHERE p.name = pt.Name
);

PRINT 'Đã import ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' tỉnh/thành phố.';
GO

-- =============================================
-- BƯỚC 4: IMPORT WARD (Sau khi có Province và Country)
-- =============================================

-- Insert wards
INSERT INTO [dbo].[ward] (
    [ward_id],
    [sequence_number],
    [code],
    [name],
    [type],
    [province_id],
    [country_id],
    [status],
    [notes]
)
SELECT 
    NEWID() AS ward_id,
    wt.STT AS sequence_number,
    wt.Code AS code,
    wt.Name AS name,
    wt.Type AS type,
    pm.ProvinceId AS province_id, -- Map từ ProvinceName text → ProvinceId
    cm.CountryId AS country_id, -- Map từ CountryName text → CountryId
    CASE WHEN wt.Status = 'Actived' THEN 'Active' ELSE wt.Status END AS status,
    wt.Notes AS notes
FROM #WardTemp wt
INNER JOIN #ProvinceMapping pm ON wt.ProvinceName = pm.Name
INNER JOIN #CountryMapping cm ON wt.CountryName = cm.FullNameVi
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[ward] w 
    WHERE w.code = wt.Code AND wt.Code IS NOT NULL
    OR (w.name = wt.Name AND w.province_id = pm.ProvinceId)
);

PRINT 'Đã import ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' phường/xã.';
GO

-- =============================================
-- BƯỚC 5: VALIDATION - KIỂM TRA DỮ LIỆU
-- =============================================

PRINT '';
PRINT '=== KẾT QUẢ VALIDATION ===';

-- Kiểm tra Provinces không có CountryId hợp lệ
DECLARE @InvalidProvinces INT;
SELECT @InvalidProvinces = COUNT(*)
FROM #ProvinceTemp pt
LEFT JOIN #CountryMapping cm ON pt.CountryName = cm.FullNameVi
WHERE cm.CountryId IS NULL;

IF @InvalidProvinces > 0
BEGIN
    PRINT '❌ Có ' + CAST(@InvalidProvinces AS NVARCHAR(10)) + ' Tỉnh/Thành phố tham chiếu Quốc gia không tồn tại:';
    SELECT DISTINCT CountryName FROM #ProvinceTemp pt
    LEFT JOIN #CountryMapping cm ON pt.CountryName = cm.FullNameVi
    WHERE cm.CountryId IS NULL;
END
ELSE
BEGIN
    PRINT '✅ Tất cả Tỉnh/Thành phố đều có Quốc gia hợp lệ.';
END

-- Kiểm tra Wards không có ProvinceId hợp lệ
DECLARE @InvalidWards INT;
SELECT @InvalidWards = COUNT(*)
FROM #WardTemp wt
LEFT JOIN #ProvinceMapping pm ON wt.ProvinceName = pm.Name
WHERE pm.ProvinceId IS NULL;

IF @InvalidWards > 0
BEGIN
    PRINT '❌ Có ' + CAST(@InvalidWards AS NVARCHAR(10)) + ' Phường/Xã tham chiếu Tỉnh/Thành phố không tồn tại:';
    SELECT DISTINCT ProvinceName FROM #WardTemp wt
    LEFT JOIN #ProvinceMapping pm ON wt.ProvinceName = pm.Name
    WHERE pm.ProvinceId IS NULL;
END
ELSE
BEGIN
    PRINT '✅ Tất cả Phường/Xã đều có Tỉnh/Thành phố hợp lệ.';
END

-- Kiểm tra Wards không có CountryId hợp lệ
DECLARE @InvalidWardCountries INT;
SELECT @InvalidWardCountries = COUNT(*)
FROM #WardTemp wt
LEFT JOIN #CountryMapping cm ON wt.CountryName = cm.FullNameVi
WHERE cm.CountryId IS NULL;

IF @InvalidWardCountries > 0
BEGIN
    PRINT '❌ Có ' + CAST(@InvalidWardCountries AS NVARCHAR(10)) + ' Phường/Xã tham chiếu Quốc gia không tồn tại:';
    SELECT DISTINCT CountryName FROM #WardTemp wt
    LEFT JOIN #CountryMapping cm ON wt.CountryName = cm.FullNameVi
    WHERE cm.CountryId IS NULL;
END
ELSE
BEGIN
    PRINT '✅ Tất cả Phường/Xã đều có Quốc gia hợp lệ.';
END

-- Thống kê
PRINT '';
PRINT '=== THỐNG KÊ ===';

DECLARE @CountryCount INT;
DECLARE @ProvinceCount INT;
DECLARE @WardCount INT;

SELECT @CountryCount = COUNT(*) FROM [dbo].[country];
SELECT @ProvinceCount = COUNT(*) FROM [dbo].[province];
SELECT @WardCount = COUNT(*) FROM [dbo].[ward];

PRINT 'Số quốc gia: ' + CAST(@CountryCount AS NVARCHAR(10));
PRINT 'Số tỉnh/thành phố: ' + CAST(@ProvinceCount AS NVARCHAR(10));
PRINT 'Số phường/xã: ' + CAST(@WardCount AS NVARCHAR(10));

-- Dọn dẹp
DROP TABLE #CountryTemp;
DROP TABLE #ProvinceTemp;
DROP TABLE #WardTemp;
DROP TABLE #CountryMapping;
DROP TABLE #ProvinceMapping;

PRINT '';
PRINT 'Hoàn tất import dữ liệu!';
GO

