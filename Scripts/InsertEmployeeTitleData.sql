-- =============================================
-- Script INSERT danh mục chức vụ nhân viên (EmployeeTitle)
-- Mã chức vụ: CV-001, CV-002, ...
-- =============================================

USE [VietLabs] -- Thay đổi tên database nếu cần
GO

-- Chạy script này sau khi đã tạo bảng employee_title (CreateEmployeeTitle.sql)
-- Chỉ chèn những title_code chưa tồn tại (tránh trùng khi chạy nhiều lần)

INSERT INTO [dbo].[employee_title] (
    [employee_title_id],
    [sequence_number],
    [title_code],
    [name_vi],
    [name_en],
    [status],
    [notes],
    [created_at],
    [updated_at],
    [created_by],
    [updated_by]
)
SELECT id, seq, code, name_vi, name_en, st, notes, created_at, updated_at, created_by, updated_by
FROM (VALUES
    (NEWID(), 1,  N'CV-001', N'Sale Admin',                    NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 2,  N'CV-002', N'Bảo vệ',                        NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 3,  N'CV-003', N'Chăm sóc khách hàng',           NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 4,  N'CV-004', N'Điều phối mẫu',                 NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 5,  N'CV-005', N'Giám Đốc',                     NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 6,  N'CV-006', N'Giám Đốc kinh doanh khu vực miền tây', NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 7,  N'CV-007', N'Giám sát viên',                 NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 8,  N'CV-008', N'Giám sát viên trả kết quả',     NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 9,  N'CV-009', N'Hành chính',                    NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 10, N'CV-010', N'Hành chính Mua hàng',           NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 11, N'CV-011', N'Kế toán Viên',                  NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 12, N'CV-012', N'Kiểm nghiệm viên',              NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 13, N'CV-013', N'Kỹ thuật Điện',                 NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 14, N'CV-014', N'Lấy mẫu',                       NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 15, N'CV-015', N'Nhân sự',                       NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 16, N'CV-016', N'Nhân viên đào tạo - R&D',       NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 17, N'CV-017', N'Nhân viên xử lí mẫu',           NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 18, N'CV-018', N'Nhận mẫu',                      NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 19, N'CV-019', N'Nhận mẫu- Trả kết quả',         NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 20, N'CV-020', N'NVKD',                          NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 21, N'CV-021', N'Phó phòng',                     NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 22, N'CV-022', N'QA cum R&D',                    NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 23, N'CV-023', N'Quản lý chất lượng',            NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 24, N'CV-024', N'Tạp Vụ',                        NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 25, N'CV-025', N'Tổng Giám Đốc',                  NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 26, N'CV-026', N'Trả kết quả',                   NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 27, N'CV-027', N'Trưởng phòng',                  NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL),
    (NEWID(), 28, N'CV-028', N'Xử lý mẫu',                     NULL, N'Active', NULL, GETUTCDATE(), NULL, NULL, NULL)
) AS v(id, seq, code, name_vi, name_en, st, notes, created_at, updated_at, created_by, updated_by)
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[employee_title] t WHERE t.title_code = v.code);

GO

PRINT 'Đã chèn (hoặc bỏ qua nếu đã có) 28 chức vụ vào bảng employee_title.';
