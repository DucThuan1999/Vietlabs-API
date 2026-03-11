-- =============================================
-- Insert dữ liệu Tiêu chuẩn/Qui chuẩn (standard)
-- Mã: TC-001, TC-002, ... ; Người cập nhật: 94eab415-1624-49de-85a6-a80916db3ab2
-- =============================================

USE [VietLabs]
GO

DECLARE @UpdatedBy UNIQUEIDENTIFIER = '94eab415-1624-49de-85a6-a80916db3ab2';
DECLARE @Now DATETIME2 = SYSUTCDATETIME();

-- Chỉ insert nếu bảng standard trống (tránh trùng khi chạy lại)
IF NOT EXISTS (SELECT 1 FROM [dbo].[standard])
BEGIN
    INSERT INTO [dbo].[standard] (
        [standard_id],
        [sequence_number],
        [standard_code],
        [name_vi],
        [name_en],
        [status],
        [notes],
        [created_at],
        [updated_at],
        [updated_by]
    ) VALUES
        (NEWID(), 1, N'TC-001', N'QCVN 12-1:2011/BYT', N'QCVN 12-1:2011/BYT', N'Active', NULL, @Now, @Now, @UpdatedBy),
        (NEWID(), 2, N'TC-002', N'QCVN 12-2:2011/BYT', N'QCVN 12-2:2011/BYT', N'Active', NULL, @Now, @Now, @UpdatedBy),
        (NEWID(), 3, N'TC-003', N'QCVN 12-3:2011/BYT', N'QCVN 12-3:2011/BYT', N'Active', NULL, @Now, @Now, @UpdatedBy),
        (NEWID(), 4, N'TC-004', N'QCVN 12-4:2015/BYT', N'QCVN 12-4:2015/BYT', N'Active', NULL, @Now, @Now, @UpdatedBy),
        (NEWID(), 5, N'TC-005', N'Quyết định số 46/2007/QĐ-BYT', N'Decision No. 46/2007/QD-BYT', N'Active', NULL, @Now, @Now, @UpdatedBy),
        (NEWID(), 6, N'TC-006', N'TCVN 7146-1:2002', N'TCVN 7146-1:2002', N'Active', NULL, @Now, @Now, @UpdatedBy),
        (NEWID(), 7, N'TC-007', N'TCVN 7147-1:2002', N'TCVN 7147-1:2002', N'Active', NULL, @Now, @Now, @UpdatedBy),
        (NEWID(), 8, N'TC-008', N'TCVN 7542-1:2005', N'TCVN 7542-1:2005', N'Active', NULL, @Now, @Now, @UpdatedBy);

    PRINT N'Đã insert 8 bản ghi Tiêu chuẩn/Qui chuẩn.';
END
ELSE
    PRINT N'Bảng standard đã có dữ liệu, bỏ qua insert.';
GO
