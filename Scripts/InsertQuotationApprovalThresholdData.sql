-- =============================================
-- Script insert dữ liệu mẫu vào quotation_approval_threshold
-- Sử dụng script này nếu bảng đã được tạo bằng EF Core Migration
-- =============================================

USE [VietLabs] -- Thay đổi tên database nếu cần
GO

-- Xóa dữ liệu cũ nếu muốn reset (bỏ comment nếu cần)
-- DELETE FROM [dbo].[quotation_approval_threshold];
-- GO

-- Insert: 0-9% - Không cần phê duyệt
IF NOT EXISTS (SELECT * FROM [dbo].[quotation_approval_threshold] WHERE [min_discount_percent] = 0 AND [max_discount_percent] = 9)
BEGIN
    INSERT INTO [dbo].[quotation_approval_threshold] (
        [quotation_approval_threshold_id],
        [min_discount_percent],
        [max_discount_percent],
        [approval_levels],
        [description],
        [status],
        [created_at]
    )
    VALUES (
        NEWID(),
        0.00,
        9.00,
        0,
        N'0-9%: Không cần phê duyệt',
        'Active',
        GETUTCDATE()
    );
    
    PRINT 'Đã thêm cấu hình: 0-9% - Không cần phê duyệt';
END
ELSE
BEGIN
    PRINT 'Cấu hình 0-9% đã tồn tại.';
END
GO

-- Insert: 10-20% - Phê duyệt 1 cấp (Manager)
IF NOT EXISTS (SELECT * FROM [dbo].[quotation_approval_threshold] WHERE [min_discount_percent] = 10 AND [max_discount_percent] = 20)
BEGIN
    INSERT INTO [dbo].[quotation_approval_threshold] (
        [quotation_approval_threshold_id],
        [min_discount_percent],
        [max_discount_percent],
        [approval_levels],
        [description],
        [status],
        [created_at]
    )
    VALUES (
        NEWID(),
        10.00,
        20.00,
        1,
        N'10-20%: Phê duyệt 1 cấp (Manager của nhân viên)',
        'Active',
        GETUTCDATE()
    );
    
    PRINT 'Đã thêm cấu hình: 10-20% - Phê duyệt 1 cấp';
END
ELSE
BEGIN
    PRINT 'Cấu hình 10-20% đã tồn tại.';
END
GO

-- Insert: >20% - Phê duyệt 2 cấp (Manager + Người chỉ định)
IF NOT EXISTS (SELECT * FROM [dbo].[quotation_approval_threshold] WHERE [min_discount_percent] = 21 AND [max_discount_percent] = 100)
BEGIN
    INSERT INTO [dbo].[quotation_approval_threshold] (
        [quotation_approval_threshold_id],
        [min_discount_percent],
        [max_discount_percent],
        [approval_levels],
        [description],
        [status],
        [created_at]
    )
    VALUES (
        NEWID(),
        21.00,
        100.00,
        2,
        N'>20%: Phê duyệt 2 cấp (Manager + Người chỉ định)',
        'Active',
        GETUTCDATE()
    );
    
    PRINT 'Đã thêm cấu hình: >20% - Phê duyệt 2 cấp';
END
ELSE
BEGIN
    PRINT 'Cấu hình >20% đã tồn tại.';
END
GO

-- Kiểm tra dữ liệu đã insert
SELECT 
    [quotation_approval_threshold_id],
    [min_discount_percent],
    [max_discount_percent],
    [approval_levels],
    [description],
    [status],
    [created_at]
FROM [dbo].[quotation_approval_threshold]
ORDER BY [min_discount_percent];

PRINT 'Hoàn tất insert dữ liệu mẫu.';
GO

