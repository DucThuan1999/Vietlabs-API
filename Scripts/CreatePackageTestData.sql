-- Script tạo dữ liệu test cho Package và PackageAnalysisGroup
-- Dựa trên dữ liệu AnalysisGroup có sẵn trong database

-- Bước 1: Tạo dữ liệu AnalysisGroup mẫu (nếu chưa có)
-- Lưu ý: Cần thay thế các GUID này bằng GUID thực tế từ AnalysisGroup trong database của bạn
-- Hoặc sử dụng các AnalysisGroup đã tồn tại

-- Giả sử có các AnalysisGroup với các GUID sau (bạn cần thay thế bằng GUID thực tế):
-- AnalysisGroup 1: Huyết học (Hematology)
-- AnalysisGroup 2: Sinh hóa (Biochemistry)  
-- AnalysisGroup 3: Vi sinh (Microbiology)
-- AnalysisGroup 4: Miễn dịch (Immunology)
-- AnalysisGroup 5: Nước tiểu (Urine Analysis)
-- AnalysisGroup 6: Huyết thanh học (Serology)

-- Bước 2: Tạo các Package mẫu
DECLARE @Package1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Package2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Package3Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Package4Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Package5Id UNIQUEIDENTIFIER = NEWID();

-- Lấy các AnalysisGroupId từ database (giả sử có ít nhất 6 AnalysisGroup)
DECLARE @AnalysisGroup1Id UNIQUEIDENTIFIER;
DECLARE @AnalysisGroup2Id UNIQUEIDENTIFIER;
DECLARE @AnalysisGroup3Id UNIQUEIDENTIFIER;
DECLARE @AnalysisGroup4Id UNIQUEIDENTIFIER;
DECLARE @AnalysisGroup5Id UNIQUEIDENTIFIER;
DECLARE @AnalysisGroup6Id UNIQUEIDENTIFIER;

-- Lấy ID của AnalysisGroup (hoặc bạn có thể hardcode GUID cụ thể)
SELECT @AnalysisGroup1Id = analysis_group_id FROM analysis_group ORDER BY created_at OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;
SELECT @AnalysisGroup2Id = analysis_group_id FROM analysis_group ORDER BY created_at OFFSET 1 ROWS FETCH NEXT 1 ROWS ONLY;
SELECT @AnalysisGroup3Id = analysis_group_id FROM analysis_group ORDER BY created_at OFFSET 2 ROWS FETCH NEXT 1 ROWS ONLY;
SELECT @AnalysisGroup4Id = analysis_group_id FROM analysis_group ORDER BY created_at OFFSET 3 ROWS FETCH NEXT 1 ROWS ONLY;
SELECT @AnalysisGroup5Id = analysis_group_id FROM analysis_group ORDER BY created_at OFFSET 4 ROWS FETCH NEXT 1 ROWS ONLY;
SELECT @AnalysisGroup6Id = analysis_group_id FROM analysis_group ORDER BY created_at OFFSET 5 ROWS FETCH NEXT 1 ROWS ONLY;

-- Nếu không có đủ AnalysisGroup, tạo mới
IF @AnalysisGroup1Id IS NULL
BEGIN
    SET @AnalysisGroup1Id = NEWID();
    INSERT INTO analysis_group (analysis_group_id, analysis_group_code, name_vi, name_en, status, created_at)
    VALUES (@AnalysisGroup1Id, 'AG-001', N'Huyết học', 'Hematology', 'Active', GETUTCDATE());
END

IF @AnalysisGroup2Id IS NULL
BEGIN
    SET @AnalysisGroup2Id = NEWID();
    INSERT INTO analysis_group (analysis_group_id, analysis_group_code, name_vi, name_en, status, created_at)
    VALUES (@AnalysisGroup2Id, 'AG-002', N'Sinh hóa', 'Biochemistry', 'Active', GETUTCDATE());
END

IF @AnalysisGroup3Id IS NULL
BEGIN
    SET @AnalysisGroup3Id = NEWID();
    INSERT INTO analysis_group (analysis_group_id, analysis_group_code, name_vi, name_en, status, created_at)
    VALUES (@AnalysisGroup3Id, 'AG-003', N'Vi sinh', 'Microbiology', 'Active', GETUTCDATE());
END

IF @AnalysisGroup4Id IS NULL
BEGIN
    SET @AnalysisGroup4Id = NEWID();
    INSERT INTO analysis_group (analysis_group_id, analysis_group_code, name_vi, name_en, status, created_at)
    VALUES (@AnalysisGroup4Id, 'AG-004', N'Miễn dịch', 'Immunology', 'Active', GETUTCDATE());
END

IF @AnalysisGroup5Id IS NULL
BEGIN
    SET @AnalysisGroup5Id = NEWID();
    INSERT INTO analysis_group (analysis_group_id, analysis_group_code, name_vi, name_en, status, created_at)
    VALUES (@AnalysisGroup5Id, 'AG-005', N'Nước tiểu', 'Urine Analysis', 'Active', GETUTCDATE());
END

IF @AnalysisGroup6Id IS NULL
BEGIN
    SET @AnalysisGroup6Id = NEWID();
    INSERT INTO analysis_group (analysis_group_id, analysis_group_code, name_vi, name_en, status, created_at)
    VALUES (@AnalysisGroup6Id, 'AG-006', N'Huyết thanh học', 'Serology', 'Active', GETUTCDATE());
END

-- Xóa dữ liệu cũ nếu đã tồn tại (tùy chọn - comment nếu không muốn xóa)
-- DELETE FROM package_analysis_group WHERE package_id IN (SELECT package_id FROM package WHERE package_code IN ('PKG-001', 'PKG-002', 'PKG-003', 'PKG-004', 'PKG-005'));
-- DELETE FROM package WHERE package_code IN ('PKG-001', 'PKG-002', 'PKG-003', 'PKG-004', 'PKG-005');

-- Tạo các Package (chỉ insert nếu chưa tồn tại)
IF NOT EXISTS (SELECT 1 FROM package WHERE package_code = 'PKG-001')
INSERT INTO package (package_id, package_code, name_vi, name_en, description, default_price, published_group_code, sample_matrix_id, status, notes, created_at)
VALUES 
    (@Package1Id, 'PKG-001', 
     N'Gói xét nghiệm tổng quát', 
     'General Health Check Package',
     N'Gói xét nghiệm tổng quát bao gồm các chỉ tiêu cơ bản về huyết học, sinh hóa và nước tiểu',
     1500000.00,
     'PP-001',
     NULL,
     'Active',
     N'Gói phù hợp cho khám sức khỏe định kỳ',
     GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM package WHERE package_code = 'PKG-002')
INSERT INTO package (package_id, package_code, name_vi, name_en, description, default_price, published_group_code, sample_matrix_id, status, notes, created_at)
VALUES 
    (@Package2Id, 'PKG-002',
     N'Gói xét nghiệm nâng cao',
     'Advanced Health Check Package',
     N'Gói xét nghiệm nâng cao bao gồm đầy đủ các chỉ tiêu: huyết học, sinh hóa, vi sinh, miễn dịch',
     3500000.00,
     'PP-002',
     NULL,
     'Active',
     N'Gói phù hợp cho khám sức khỏe toàn diện',
     GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM package WHERE package_code = 'PKG-003')
INSERT INTO package (package_id, package_code, name_vi, name_en, description, default_price, published_group_code, sample_matrix_id, status, notes, created_at)
VALUES 
    (@Package3Id, 'PKG-003',
     N'Gói xét nghiệm cơ bản',
     'Basic Health Check Package',
     N'Gói xét nghiệm cơ bản chỉ bao gồm huyết học và sinh hóa',
     800000.00,
     'PP-003',
     NULL,
     'Active',
     N'Gói phù hợp cho khám sức khỏe đơn giản',
     GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM package WHERE package_code = 'PKG-004')
INSERT INTO package (package_id, package_code, name_vi, name_en, description, default_price, published_group_code, sample_matrix_id, status, notes, created_at)
VALUES 
    (@Package4Id, 'PKG-004',
     N'Gói xét nghiệm vi sinh',
     'Microbiology Package',
     N'Gói xét nghiệm chuyên sâu về vi sinh và miễn dịch',
     2500000.00,
     'PP-004',
     NULL,
     'Active',
     N'Gói phù hợp cho xét nghiệm nhiễm trùng',
     GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM package WHERE package_code = 'PKG-005')
INSERT INTO package (package_id, package_code, name_vi, name_en, description, default_price, published_group_code, sample_matrix_id, status, notes, created_at)
VALUES 
    (@Package5Id, 'PKG-005',
     N'Gói xét nghiệm chuyên sâu',
     'Comprehensive Health Package',
     N'Gói xét nghiệm đầy đủ tất cả các chỉ tiêu có sẵn',
     5000000.00,
     'PP-005',
     NULL,
     'Active',
     N'Gói phù hợp cho khám sức khỏe toàn diện nhất',
     GETUTCDATE());

-- Lấy lại PackageId nếu đã tồn tại
SELECT @Package1Id = package_id FROM package WHERE package_code = 'PKG-001';
SELECT @Package2Id = package_id FROM package WHERE package_code = 'PKG-002';
SELECT @Package3Id = package_id FROM package WHERE package_code = 'PKG-003';
SELECT @Package4Id = package_id FROM package WHERE package_code = 'PKG-004';
SELECT @Package5Id = package_id FROM package WHERE package_code = 'PKG-005';

-- Tạo PackageAnalysisGroup - liên kết Package với AnalysisGroup
-- Chỉ insert nếu chưa tồn tại (sử dụng NOT EXISTS để tránh duplicate key)

-- Package 1: Gói tổng quát (Huyết học, Sinh hóa, Nước tiểu)
IF NOT EXISTS (SELECT 1 FROM package_analysis_group WHERE package_id = @Package1Id AND analysis_group_id = @AnalysisGroup1Id)
    INSERT INTO package_analysis_group (package_analysis_group_id, package_id, analysis_group_id, display_order, is_required, notes, created_at)
    VALUES (NEWID(), @Package1Id, @AnalysisGroup1Id, 1, 1, N'Nhóm chỉ tiêu bắt buộc', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM package_analysis_group WHERE package_id = @Package1Id AND analysis_group_id = @AnalysisGroup2Id)
    INSERT INTO package_analysis_group (package_analysis_group_id, package_id, analysis_group_id, display_order, is_required, notes, created_at)
    VALUES (NEWID(), @Package1Id, @AnalysisGroup2Id, 2, 1, N'Nhóm chỉ tiêu bắt buộc', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM package_analysis_group WHERE package_id = @Package1Id AND analysis_group_id = @AnalysisGroup5Id)
    INSERT INTO package_analysis_group (package_analysis_group_id, package_id, analysis_group_id, display_order, is_required, notes, created_at)
    VALUES (NEWID(), @Package1Id, @AnalysisGroup5Id, 3, 1, N'Nhóm chỉ tiêu bắt buộc', GETUTCDATE());

-- Package 2: Gói nâng cao (Tất cả các nhóm)
IF NOT EXISTS (SELECT 1 FROM package_analysis_group WHERE package_id = @Package2Id AND analysis_group_id = @AnalysisGroup1Id)
    INSERT INTO package_analysis_group (package_analysis_group_id, package_id, analysis_group_id, display_order, is_required, notes, created_at)
    VALUES (NEWID(), @Package2Id, @AnalysisGroup1Id, 1, 1, N'Nhóm chỉ tiêu bắt buộc', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM package_analysis_group WHERE package_id = @Package2Id AND analysis_group_id = @AnalysisGroup2Id)
    INSERT INTO package_analysis_group (package_analysis_group_id, package_id, analysis_group_id, display_order, is_required, notes, created_at)
    VALUES (NEWID(), @Package2Id, @AnalysisGroup2Id, 2, 1, N'Nhóm chỉ tiêu bắt buộc', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM package_analysis_group WHERE package_id = @Package2Id AND analysis_group_id = @AnalysisGroup3Id)
    INSERT INTO package_analysis_group (package_analysis_group_id, package_id, analysis_group_id, display_order, is_required, notes, created_at)
    VALUES (NEWID(), @Package2Id, @AnalysisGroup3Id, 3, 1, N'Nhóm chỉ tiêu bắt buộc', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM package_analysis_group WHERE package_id = @Package2Id AND analysis_group_id = @AnalysisGroup4Id)
    INSERT INTO package_analysis_group (package_analysis_group_id, package_id, analysis_group_id, display_order, is_required, notes, created_at)
    VALUES (NEWID(), @Package2Id, @AnalysisGroup4Id, 4, 1, N'Nhóm chỉ tiêu bắt buộc', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM package_analysis_group WHERE package_id = @Package2Id AND analysis_group_id = @AnalysisGroup5Id)
    INSERT INTO package_analysis_group (package_analysis_group_id, package_id, analysis_group_id, display_order, is_required, notes, created_at)
    VALUES (NEWID(), @Package2Id, @AnalysisGroup5Id, 5, 1, N'Nhóm chỉ tiêu bắt buộc', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM package_analysis_group WHERE package_id = @Package2Id AND analysis_group_id = @AnalysisGroup6Id)
    INSERT INTO package_analysis_group (package_analysis_group_id, package_id, analysis_group_id, display_order, is_required, notes, created_at)
    VALUES (NEWID(), @Package2Id, @AnalysisGroup6Id, 6, 1, N'Nhóm chỉ tiêu bắt buộc', GETUTCDATE());

-- Package 3: Gói cơ bản (Chỉ Huyết học và Sinh hóa)
IF NOT EXISTS (SELECT 1 FROM package_analysis_group WHERE package_id = @Package3Id AND analysis_group_id = @AnalysisGroup1Id)
    INSERT INTO package_analysis_group (package_analysis_group_id, package_id, analysis_group_id, display_order, is_required, notes, created_at)
    VALUES (NEWID(), @Package3Id, @AnalysisGroup1Id, 1, 1, N'Nhóm chỉ tiêu bắt buộc', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM package_analysis_group WHERE package_id = @Package3Id AND analysis_group_id = @AnalysisGroup2Id)
    INSERT INTO package_analysis_group (package_analysis_group_id, package_id, analysis_group_id, display_order, is_required, notes, created_at)
    VALUES (NEWID(), @Package3Id, @AnalysisGroup2Id, 2, 1, N'Nhóm chỉ tiêu bắt buộc', GETUTCDATE());

-- Package 4: Gói vi sinh (Vi sinh và Miễn dịch)
IF NOT EXISTS (SELECT 1 FROM package_analysis_group WHERE package_id = @Package4Id AND analysis_group_id = @AnalysisGroup3Id)
    INSERT INTO package_analysis_group (package_analysis_group_id, package_id, analysis_group_id, display_order, is_required, notes, created_at)
    VALUES (NEWID(), @Package4Id, @AnalysisGroup3Id, 1, 1, N'Nhóm chỉ tiêu bắt buộc', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM package_analysis_group WHERE package_id = @Package4Id AND analysis_group_id = @AnalysisGroup4Id)
    INSERT INTO package_analysis_group (package_analysis_group_id, package_id, analysis_group_id, display_order, is_required, notes, created_at)
    VALUES (NEWID(), @Package4Id, @AnalysisGroup4Id, 2, 1, N'Nhóm chỉ tiêu bắt buộc', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM package_analysis_group WHERE package_id = @Package4Id AND analysis_group_id = @AnalysisGroup6Id)
    INSERT INTO package_analysis_group (package_analysis_group_id, package_id, analysis_group_id, display_order, is_required, notes, created_at)
    VALUES (NEWID(), @Package4Id, @AnalysisGroup6Id, 3, 0, N'Nhóm chỉ tiêu tùy chọn', GETUTCDATE());

-- Package 5: Gói chuyên sâu (Tất cả các nhóm, một số tùy chọn)
IF NOT EXISTS (SELECT 1 FROM package_analysis_group WHERE package_id = @Package5Id AND analysis_group_id = @AnalysisGroup1Id)
    INSERT INTO package_analysis_group (package_analysis_group_id, package_id, analysis_group_id, display_order, is_required, notes, created_at)
    VALUES (NEWID(), @Package5Id, @AnalysisGroup1Id, 1, 1, N'Nhóm chỉ tiêu bắt buộc', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM package_analysis_group WHERE package_id = @Package5Id AND analysis_group_id = @AnalysisGroup2Id)
    INSERT INTO package_analysis_group (package_analysis_group_id, package_id, analysis_group_id, display_order, is_required, notes, created_at)
    VALUES (NEWID(), @Package5Id, @AnalysisGroup2Id, 2, 1, N'Nhóm chỉ tiêu bắt buộc', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM package_analysis_group WHERE package_id = @Package5Id AND analysis_group_id = @AnalysisGroup3Id)
    INSERT INTO package_analysis_group (package_analysis_group_id, package_id, analysis_group_id, display_order, is_required, notes, created_at)
    VALUES (NEWID(), @Package5Id, @AnalysisGroup3Id, 3, 1, N'Nhóm chỉ tiêu bắt buộc', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM package_analysis_group WHERE package_id = @Package5Id AND analysis_group_id = @AnalysisGroup4Id)
    INSERT INTO package_analysis_group (package_analysis_group_id, package_id, analysis_group_id, display_order, is_required, notes, created_at)
    VALUES (NEWID(), @Package5Id, @AnalysisGroup4Id, 4, 1, N'Nhóm chỉ tiêu bắt buộc', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM package_analysis_group WHERE package_id = @Package5Id AND analysis_group_id = @AnalysisGroup5Id)
    INSERT INTO package_analysis_group (package_analysis_group_id, package_id, analysis_group_id, display_order, is_required, notes, created_at)
    VALUES (NEWID(), @Package5Id, @AnalysisGroup5Id, 5, 1, N'Nhóm chỉ tiêu bắt buộc', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM package_analysis_group WHERE package_id = @Package5Id AND analysis_group_id = @AnalysisGroup6Id)
    INSERT INTO package_analysis_group (package_analysis_group_id, package_id, analysis_group_id, display_order, is_required, notes, created_at)
    VALUES (NEWID(), @Package5Id, @AnalysisGroup6Id, 6, 0, N'Nhóm chỉ tiêu tùy chọn - có thể bỏ qua', GETUTCDATE());

-- Hiển thị kết quả
SELECT 'Dữ liệu test đã được tạo thành công!' AS Message;

-- Xem các Package đã tạo
SELECT 
    p.package_id,
    p.package_code,
    p.name_vi,
    p.name_en,
    p.default_price,
    p.status,
    COUNT(pag.package_analysis_group_id) AS so_nhom_chi_tieu
FROM package p
LEFT JOIN package_analysis_group pag ON p.package_id = pag.package_id
WHERE p.package_code IN ('PKG-001', 'PKG-002', 'PKG-003', 'PKG-004', 'PKG-005')
GROUP BY p.package_id, p.package_code, p.name_vi, p.name_en, p.default_price, p.status
ORDER BY p.package_code;

-- Xem chi tiết Package và AnalysisGroup
SELECT 
    p.package_code,
    p.name_vi AS ten_goi,
    ag.analysis_group_code,
    ag.name_vi AS ten_nhom_chi_tieu,
    pag.display_order,
    pag.is_required,
    pag.notes
FROM package p
INNER JOIN package_analysis_group pag ON p.package_id = pag.package_id
INNER JOIN analysis_group ag ON pag.analysis_group_id = ag.analysis_group_id
WHERE p.package_code IN ('PKG-001', 'PKG-002', 'PKG-003', 'PKG-004', 'PKG-005')
ORDER BY p.package_code, pag.display_order;

