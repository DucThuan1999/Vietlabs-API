-- =============================================
-- Script tạo bảng QuotationApprovalThreshold và các cột liên quan
-- =============================================

USE [VietLabs] -- Thay đổi tên database nếu cần
GO

-- =============================================
-- 1. Tạo bảng quotation_approval_threshold
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[quotation_approval_threshold]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[quotation_approval_threshold] (
        [quotation_approval_threshold_id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [min_discount_percent] DECIMAL(5,2) NOT NULL,
        [max_discount_percent] DECIMAL(5,2) NOT NULL,
        [approval_levels] INT NOT NULL,
        [description] NVARCHAR(2000) NULL,
        [status] NVARCHAR(50) NOT NULL DEFAULT 'Active',
        [created_at] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [updated_at] DATETIME2 NULL,
        [created_by] UNIQUEIDENTIFIER NULL,
        [updated_by] UNIQUEIDENTIFIER NULL
    );
    
    PRINT 'Bảng quotation_approval_threshold đã được tạo thành công.';
END
ELSE
BEGIN
    PRINT 'Bảng quotation_approval_threshold đã tồn tại.';
END
GO

-- =============================================
-- 2. Thêm cột manager_id vào bảng employee (nếu chưa có)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[employee]') AND name = 'manager_id')
BEGIN
    ALTER TABLE [dbo].[employee]
    ADD [manager_id] UNIQUEIDENTIFIER NULL;
    
    -- Thêm foreign key constraint (sử dụng NO ACTION để tránh lỗi multiple cascade paths)
    ALTER TABLE [dbo].[employee]
    ADD CONSTRAINT [FK_employee_manager] 
    FOREIGN KEY ([manager_id]) 
    REFERENCES [dbo].[employee] ([employee_id])
    ON DELETE NO ACTION
    ON UPDATE NO ACTION;
    
    PRINT 'Cột manager_id đã được thêm vào bảng employee.';
END
ELSE
BEGIN
    PRINT 'Cột manager_id đã tồn tại trong bảng employee.';
END
GO

-- =============================================
-- 3. Thêm các cột phê duyệt vào bảng quotation (nếu chưa có)
-- =============================================

-- ApproverLevel1Id
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[quotation]') AND name = 'approver_level1_id')
BEGIN
    ALTER TABLE [dbo].[quotation]
    ADD [approver_level1_id] UNIQUEIDENTIFIER NULL;
    
    ALTER TABLE [dbo].[quotation]
    ADD CONSTRAINT [FK_quotation_approver_level1] 
    FOREIGN KEY ([approver_level1_id]) 
    REFERENCES [dbo].[employee] ([employee_id])
    ON DELETE SET NULL;
    
    PRINT 'Cột approver_level1_id đã được thêm vào bảng quotation.';
END
ELSE
BEGIN
    PRINT 'Cột approver_level1_id đã tồn tại trong bảng quotation.';
END
GO

-- ApproverLevel2Id
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[quotation]') AND name = 'approver_level2_id')
BEGIN
    ALTER TABLE [dbo].[quotation]
    ADD [approver_level2_id] UNIQUEIDENTIFIER NULL;
    
    ALTER TABLE [dbo].[quotation]
    ADD CONSTRAINT [FK_quotation_approver_level2] 
    FOREIGN KEY ([approver_level2_id]) 
    REFERENCES [dbo].[employee] ([employee_id])
    ON DELETE SET NULL;
    
    PRINT 'Cột approver_level2_id đã được thêm vào bảng quotation.';
END
ELSE
BEGIN
    PRINT 'Cột approver_level2_id đã tồn tại trong bảng quotation.';
END
GO

-- ApprovedLevel1At
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[quotation]') AND name = 'approved_level1_at')
BEGIN
    ALTER TABLE [dbo].[quotation]
    ADD [approved_level1_at] DATETIME2 NULL;
    
    PRINT 'Cột approved_level1_at đã được thêm vào bảng quotation.';
END
ELSE
BEGIN
    PRINT 'Cột approved_level1_at đã tồn tại trong bảng quotation.';
END
GO

-- ApprovedLevel2At
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[quotation]') AND name = 'approved_level2_at')
BEGIN
    ALTER TABLE [dbo].[quotation]
    ADD [approved_level2_at] DATETIME2 NULL;
    
    PRINT 'Cột approved_level2_at đã được thêm vào bảng quotation.';
END
ELSE
BEGIN
    PRINT 'Cột approved_level2_at đã tồn tại trong bảng quotation.';
END
GO

-- ApprovalLevel1Status
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[quotation]') AND name = 'approval_level1_status')
BEGIN
    ALTER TABLE [dbo].[quotation]
    ADD [approval_level1_status] NVARCHAR(50) NULL;
    
    PRINT 'Cột approval_level1_status đã được thêm vào bảng quotation.';
END
ELSE
BEGIN
    PRINT 'Cột approval_level1_status đã tồn tại trong bảng quotation.';
END
GO

-- ApprovalLevel2Status
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[quotation]') AND name = 'approval_level2_status')
BEGIN
    ALTER TABLE [dbo].[quotation]
    ADD [approval_level2_status] NVARCHAR(50) NULL;
    
    PRINT 'Cột approval_level2_status đã được thêm vào bảng quotation.';
END
ELSE
BEGIN
    PRINT 'Cột approval_level2_status đã tồn tại trong bảng quotation.';
END
GO

-- ApprovalLevel1Comment
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[quotation]') AND name = 'approval_level1_comment')
BEGIN
    ALTER TABLE [dbo].[quotation]
    ADD [approval_level1_comment] NVARCHAR(2000) NULL;
    
    PRINT 'Cột approval_level1_comment đã được thêm vào bảng quotation.';
END
ELSE
BEGIN
    PRINT 'Cột approval_level1_comment đã tồn tại trong bảng quotation.';
END
GO

-- ApprovalLevel2Comment
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[quotation]') AND name = 'approval_level2_comment')
BEGIN
    ALTER TABLE [dbo].[quotation]
    ADD [approval_level2_comment] NVARCHAR(2000) NULL;
    
    PRINT 'Cột approval_level2_comment đã được thêm vào bảng quotation.';
END
ELSE
BEGIN
    PRINT 'Cột approval_level2_comment đã tồn tại trong bảng quotation.';
END
GO

-- =============================================
-- 4. Insert dữ liệu mẫu vào quotation_approval_threshold
-- =============================================

-- Xóa dữ liệu cũ nếu muốn reset (bỏ comment nếu cần)
-- DELETE FROM [dbo].[quotation_approval_threshold];

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
GO

-- =============================================
-- 5. Kiểm tra dữ liệu đã insert
-- =============================================
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

PRINT 'Hoàn tất script tạo bảng và insert dữ liệu mẫu.';
GO

