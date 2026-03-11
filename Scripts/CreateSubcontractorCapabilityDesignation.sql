-- =============================================
-- Bảng trung gian: Năng lực nhà thầu phụ - Chỉ định (có ngày hết hạn)
-- 1 SubcontractorCapability có nhiều Designation, mỗi bản ghi có expired_date
-- =============================================

USE [VietLabs]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'subcontractor_capability_designation')
BEGIN
    CREATE TABLE [dbo].[subcontractor_capability_designation] (
        [subcontractor_capability_designation_id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [subcontractor_capability_id] UNIQUEIDENTIFIER NOT NULL,
        [designation_id] UNIQUEIDENTIFIER NOT NULL,
        [expired_date] DATE NULL,
        CONSTRAINT [FK_subcontractor_capability_designation_capability]
            FOREIGN KEY ([subcontractor_capability_id])
            REFERENCES [dbo].[subcontractor_capability] ([subcontractor_capability_id]) ON DELETE CASCADE,
        CONSTRAINT [FK_subcontractor_capability_designation_designation]
            FOREIGN KEY ([designation_id])
            REFERENCES [dbo].[designation] ([designation_id]) ON DELETE NO ACTION,
        CONSTRAINT [UQ_subcontractor_capability_designation]
            UNIQUE ([subcontractor_capability_id], [designation_id])
    );
    PRINT N'Đã tạo bảng subcontractor_capability_designation.';
END
ELSE
    PRINT N'Bảng subcontractor_capability_designation đã tồn tại.';
GO
