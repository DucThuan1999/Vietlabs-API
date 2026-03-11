-- =============================================
-- Insert dữ liệu Phương pháp tham chiếu (reference_method)
-- Chỉ insert nếu bảng trống (tránh trùng khi chạy lại)
-- =============================================

USE [VietLabs]
GO

DECLARE @Now DATETIME2 = SYSUTCDATETIME();

IF NOT EXISTS (SELECT 1 FROM [dbo].[reference_method])
BEGIN
    INSERT INTO [dbo].[reference_method] (
        [reference_method_id],
        [sequence_number],
        [name_vi],
        [name_en],
        [reference_method_code],
        [status],
        [created_at],
        [updated_at],
        [updated_by]
    ) VALUES
        -- QCVN 12-1:2011/BYT
        (NEWID(), 1,  N'QCVN 12-1:2011/BYT', N'QCVN 12-1:2011/BYT', N'VLAB-CH-TP-631', N'Active', @Now, NULL, NULL),
        (NEWID(), 2,  N'QCVN 12-1:2011/BYT', N'QCVN 12-1:2011/BYT', N'VLAB-CH-TP-632', N'Active', @Now, NULL, NULL),
        (NEWID(), 3,  N'QCVN 12-1:2011/BYT', N'QCVN 12-1:2011/BYT', N'VLAB-CH-TP-633', N'Active', @Now, NULL, NULL),
        (NEWID(), 4,  N'QCVN 12-1:2011/BYT', N'QCVN 12-1:2011/BYT', N'VLAB-CH-TP-634', N'Active', @Now, NULL, NULL),
        -- QCVN 12-2:2011/BYT
        (NEWID(), 5,  N'QCVN 12-2:2011/BYT', N'QCVN 12-2:2011/BYT', N'AOAC 2015.01', N'Active', @Now, NULL, NULL),
        -- QCVN 12-3:2011/BYT
        (NEWID(), 6,  N'QCVN 12-3:2011/BYT', N'QCVN 12-3:2011/BYT', N'AOAC 2015.01', N'Active', @Now, NULL, NULL),
        -- QCVN 12-4:2015/BYT
        (NEWID(), 7,  N'QCVN 12-4:2015/BYT', N'QCVN 12-4:2015/BYT', N'ISO 4531:2018', N'Active', @Now, NULL, NULL),
        (NEWID(), 8,  N'QCVN 12-4:2015/BYT', N'QCVN 12-4:2015/BYT', N'ISO 6486-1:2019', N'Active', @Now, NULL, NULL),
        (NEWID(), 9,  N'QCVN 12-4:2015/BYT', N'QCVN 12-4:2015/BYT', N'ISO 7068-1:2000', N'Active', @Now, NULL, NULL),
        -- Quyết định số 46/2007/QĐ-BYT
        (NEWID(), 10, N'Quyết định số 46/2007/QĐ-BYT', N'Decision No. 46/2007/QD-BYT', NULL, N'Active', @Now, NULL, NULL),
        -- TCVN 10088:2013
        (NEWID(), 11, N'TCVN 10088:2013', N'TCVN 10088:2013', NULL, N'Active', @Now, NULL, NULL),
        -- TCVN 7146-1:2002
        (NEWID(), 12, N'TCVN 7146-1:2002', N'TCVN 7146-1:2002', N'AOAC 2015.01', N'Active', @Now, NULL, NULL),
        -- TCVN 7147-1:2002
        (NEWID(), 13, N'TCVN 7147-1:2002', N'TCVN 7147-1:2002', N'AOAC 2015.01', N'Active', @Now, NULL, NULL),
        -- TCVN 7542-1:2005
        (NEWID(), 14, N'TCVN 7542-1:2005', N'TCVN 7542-1:2005', N'AOAC 2015.01', N'Active', @Now, NULL, NULL),
        -- VLAB-CH-TP-042
        (NEWID(), 15, N'VLAB-CH-TP-042', N'VLAB-CH-TP-042', NULL, N'Active', @Now, NULL, NULL),
        -- VLAB-CH-TP-761
        (NEWID(), 16, N'VLAB-CH-TP-761', N'VLAB-CH-TP-761', NULL, N'Active', @Now, NULL, NULL);

    PRINT N'Đã insert 16 bản ghi Phương pháp tham chiếu.';
END
ELSE
    PRINT N'Bảng reference_method đã có dữ liệu, bỏ qua insert.';
GO
