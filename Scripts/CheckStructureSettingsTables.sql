/*
  Kiểm tra các bảng cấu hình Cấu trúc (Settings > Cấu trúc).
  Chạy trước khi deploy script Add*.sql tương ứng.
*/

SET NOCOUNT ON;

SELECT
    t.expected_table,
    CASE
        WHEN OBJECT_ID(N'dbo.' + t.expected_table, N'U') IS NOT NULL THEN N'CÓ'
        ELSE N'CHƯA CÓ'
    END AS table_status
FROM (
    VALUES
        (N'vat_rate'),
        (N'quotation_issue_info'),
        (N'registration_permit_label_config'),
        (N'quotation_surcharge')
) AS t(expected_table)
ORDER BY t.expected_table;

GO
