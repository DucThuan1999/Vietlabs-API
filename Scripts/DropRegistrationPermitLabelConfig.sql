/*
  CHỈ dùng môi trường dev hoặc rollback có chủ đích.
  Xóa toàn bộ cấu hình tên hiển thị giấy phép đăng ký.
*/

SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.registration_permit_label_config', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.registration_permit_label_config;
    PRINT N'Đã xóa bảng dbo.registration_permit_label_config';
END
ELSE
    PRINT N'Bỏ qua: bảng không tồn tại';

GO
