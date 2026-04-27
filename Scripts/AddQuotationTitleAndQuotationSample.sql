-- Bổ sung tiêu đề báo giá (quotation.quotation_title) và bảng quotation_sample.
-- SQL Server. Chạy trực tiếp trên DB thay cho migration EF nếu cần.
-- An toàn khi chạy lại: chỉ thêm cột/bảng khi chưa tồn tại.

-- 1) Cột quotation_title trên bảng quotation
IF COL_LENGTH('quotation', 'quotation_title') IS NULL
BEGIN
    ALTER TABLE quotation
    ADD quotation_title NVARCHAR(MAX) NULL;
END;
GO

-- 2) Bảng quotation_sample (mẫu + khối lượng mẫu)
IF OBJECT_ID(N'quotation_sample', N'U') IS NULL
BEGIN
    CREATE TABLE quotation_sample (
        quotation_sample_id UNIQUEIDENTIFIER NOT NULL,
        quotation_id UNIQUEIDENTIFIER NOT NULL,
        sample_name NVARCHAR(2000) NULL,
        sample_volume NVARCHAR(2000) NULL,
        display_order INT NULL,
        created_at DATETIME2 NOT NULL,
        updated_at DATETIME2 NULL,
        CONSTRAINT PK_quotation_sample PRIMARY KEY (quotation_sample_id),
        CONSTRAINT f_k_quotation_sample_quotation_quotation_id
            FOREIGN KEY (quotation_id)
            REFERENCES quotation (quotation_id)
            ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX i_x_quotation_sample_quotation_id
        ON quotation_sample (quotation_id);
END;
GO
