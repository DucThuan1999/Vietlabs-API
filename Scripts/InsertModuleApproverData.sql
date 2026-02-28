-- =============================================
-- Script insert dữ liệu mẫu cho ModuleApprover
-- Ví dụ: Cấu hình người phê duyệt cho module Quotation
-- =============================================

USE [VietLabs] -- Thay đổi tên database nếu cần
GO

-- =============================================
-- Ví dụ 1: Phê duyệt cấp 2 cho module Quotation theo User chỉ định
-- Áp dụng cho Permission PERM-SALES (Kinh doanh)
-- =============================================

-- Lấy PermissionId của PERM-SALES
DECLARE @PermSalesId UNIQUEIDENTIFIER;
SELECT @PermSalesId = permission_id FROM [dbo].[permission] WHERE permission_code = 'PERM-SALES';

-- Lấy một EmployeeId mẫu (thay đổi theo dữ liệu thực tế)
DECLARE @ApproverEmployeeId UNIQUEIDENTIFIER;
SELECT TOP 1 @ApproverEmployeeId = employee_id FROM [dbo].[employee] WHERE status = 'Active' ORDER BY employee_code;

IF @PermSalesId IS NOT NULL AND @ApproverEmployeeId IS NOT NULL
BEGIN
    -- Kiểm tra xem đã tồn tại chưa
    IF NOT EXISTS (
        SELECT 1 FROM [dbo].[module_approver] 
        WHERE module_code = 'Quotation' 
        AND approval_level = 2 
        AND permission_id = @PermSalesId
    )
    BEGIN
        INSERT INTO [dbo].[module_approver] (
            [module_approver_id],
            [module_code],
            [approval_level],
            [approver_type],
            [approver_employee_id],
            [approver_title],
            [permission_id],
            [notes],
            [status],
            [created_at]
        )
        VALUES (
            NEWID(),
            'Quotation',
            2,
            'User',
            @ApproverEmployeeId,
            NULL,
            @PermSalesId,
            'Người phê duyệt cấp 2 cho module báo giá - Permission Kinh doanh',
            'Active',
            GETUTCDATE()
        );
        
        PRINT 'Đã thêm ModuleApprover: Quotation - Level 2 - User - Permission Sales';
    END
    ELSE
    BEGIN
        PRINT 'ModuleApprover cho Quotation Level 2 Permission Sales đã tồn tại.';
    END
END
ELSE
BEGIN
    PRINT 'Không tìm thấy Permission PERM-SALES hoặc Employee để tạo ModuleApprover.';
END
GO

-- =============================================
-- Ví dụ 2: Phê duyệt cấp 2 cho module Quotation theo Title (chức vụ)
-- Áp dụng cho toàn hệ thống (permission_id = NULL)
-- =============================================

IF NOT EXISTS (
    SELECT 1 FROM [dbo].[module_approver] 
    WHERE module_code = 'Quotation' 
    AND approval_level = 2 
    AND approver_type = 'Title'
    AND permission_id IS NULL
)
BEGIN
    INSERT INTO [dbo].[module_approver] (
        [module_approver_id],
        [module_code],
        [approval_level],
        [approver_type],
        [approver_employee_id],
        [approver_title],
        [permission_id],
        [notes],
        [status],
        [created_at]
    )
    VALUES (
        NEWID(),
        'Quotation',
        2,
        'Title',
        NULL,
        'Giám đốc',
        NULL,
        'Người phê duyệt cấp 2 cho module báo giá - Áp dụng cho toàn hệ thống (theo chức vụ Giám đốc)',
        'Active',
        GETUTCDATE()
    );
    
    PRINT 'Đã thêm ModuleApprover: Quotation - Level 2 - Title (Giám đốc) - Toàn hệ thống';
END
ELSE
BEGIN
    PRINT 'ModuleApprover cho Quotation Level 2 Title đã tồn tại.';
END
GO

-- =============================================
-- Ví dụ 3: Phê duyệt cấp 2 cho module Client theo User chỉ định
-- Áp dụng cho Permission PERM-MANAGER
-- =============================================

DECLARE @PermManagerId UNIQUEIDENTIFIER;
SELECT @PermManagerId = permission_id FROM [dbo].[permission] WHERE permission_code = 'PERM-MANAGER';

DECLARE @ManagerApproverId UNIQUEIDENTIFIER;
SELECT TOP 1 @ManagerApproverId = employee_id FROM [dbo].[employee] 
WHERE status = 'Active' AND title LIKE '%Manager%' OR title LIKE '%Quản lý%'
ORDER BY employee_code;

IF @PermManagerId IS NOT NULL AND @ManagerApproverId IS NOT NULL
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM [dbo].[module_approver] 
        WHERE module_code = 'Client' 
        AND approval_level = 2 
        AND permission_id = @PermManagerId
    )
    BEGIN
        INSERT INTO [dbo].[module_approver] (
            [module_approver_id],
            [module_code],
            [approval_level],
            [approver_type],
            [approver_employee_id],
            [approver_title],
            [permission_id],
            [notes],
            [status],
            [created_at]
        )
        VALUES (
            NEWID(),
            'Client',
            2,
            'User',
            @ManagerApproverId,
            NULL,
            @PermManagerId,
            'Người phê duyệt cấp 2 cho module Khách hàng - Permission Manager',
            'Active',
            GETUTCDATE()
        );
        
        PRINT 'Đã thêm ModuleApprover: Client - Level 2 - User - Permission Manager';
    END
    ELSE
    BEGIN
        PRINT 'ModuleApprover cho Client Level 2 Permission Manager đã tồn tại.';
    END
END
ELSE
BEGIN
    PRINT 'Không tìm thấy Permission PERM-MANAGER hoặc Employee Manager để tạo ModuleApprover.';
END
GO

PRINT 'Hoàn thành: Đã thêm dữ liệu mẫu cho ModuleApprover.';
PRINT 'Lưu ý: Cần kiểm tra và cập nhật EmployeeId và PermissionId theo dữ liệu thực tế của bạn.';
GO

