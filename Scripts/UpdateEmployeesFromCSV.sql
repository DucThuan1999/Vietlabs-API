-- Script to update employees from CSV
BEGIN TRANSACTION;
DECLARE @branch_id UNIQUEIDENTIFIER;
DECLARE @dept_id UNIQUEIDENTIFIER;
DECLARE @sect_id UNIQUEIDENTIFIER;
DECLARE @manager_id UNIQUEIDENTIFIER;

-- Manage Branches
IF NOT EXISTS (SELECT 1 FROM branch WHERE name_vi = N'Bạc liêu')
    INSERT INTO branch (branch_id, name_vi, status, branch_code) VALUES (NEWID(), N'Bạc liêu', 'Active', 'BR-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
IF NOT EXISTS (SELECT 1 FROM branch WHERE name_vi = N'Cà Mau')
    INSERT INTO branch (branch_id, name_vi, status, branch_code) VALUES (NEWID(), N'Cà Mau', 'Active', 'BR-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
IF NOT EXISTS (SELECT 1 FROM branch WHERE name_vi = N'Cần Thơ')
    INSERT INTO branch (branch_id, name_vi, status, branch_code) VALUES (NEWID(), N'Cần Thơ', 'Active', 'BR-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
IF NOT EXISTS (SELECT 1 FROM branch WHERE name_vi = N'Hồ Chí Minh')
    INSERT INTO branch (branch_id, name_vi, status, branch_code) VALUES (NEWID(), N'Hồ Chí Minh', 'Active', 'BR-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));

-- Manage Departments
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Ban Giám đốc' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Ban Giám đốc', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Dịch vụ khách hàng', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Hành chính-Nhân sự', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Kinh doanh' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Kinh doanh', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Phòng thí nghiệm' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Phòng thí nghiệm', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Quản lý Chất lượng' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Quản lý Chất lượng', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Quang phổ - Cổ điển' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Quang phổ - Cổ điển', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Sắc ký', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Tài chính-Kế toán' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Tài chính-Kế toán', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Vi sinh' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Vi sinh', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Dịch vụ khách hàng', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Hành chính-Nhân sự', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Kinh doanh' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Kinh doanh', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Sắc ký', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Tài chính-Kế toán' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Tài chính-Kế toán', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Vi sinh' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Vi sinh', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cà Mau');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Hành chính-Nhân sự', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cà Mau');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Sắc ký', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Bạc liêu');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Dịch vụ khách hàng', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Bạc liêu');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Hành chính-Nhân sự', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Bạc liêu');
IF NOT EXISTS (SELECT 1 FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id)
    INSERT INTO department (department_id, name_vi, branch_id, status, department_code) VALUES (NEWID(), N'Sắc ký', @branch_id, 'Active', 'DE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));

-- Manage Sections
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Ban Giám đốc' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Ban Giám đốc' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Ban Giám đốc', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Chăm sóc khách hàng' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Chăm sóc khách hàng', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Nhận mẫu' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Nhận mẫu', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Bảo trì (kĩ thuật)' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Bảo trì (kĩ thuật)', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Bảo vệ' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Bảo vệ', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Điều phối mẫu' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Điều phối mẫu', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Mua hàng' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Mua hàng', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Nhân sự' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Nhân sự', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Tạp vụ' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Tạp vụ', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Vận chuyển mẫu' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Vận chuyển mẫu', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Kinh doanh' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Kinh doanh' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Kinh doanh', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Phòng thí nghiệm' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Trả kết quả' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Trả kết quả', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quản lý Chất lượng' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Quản lý Chất lượng' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Quản lý Chất lượng', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quang phổ - Cổ điển' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Quang phổ - Cổ điển' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Quang phổ - Cổ điển', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quang phổ - Cổ điển' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Xử lý mẫu' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Xử lý mẫu', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Sắc ký', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Phòng thí nghiệm' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Nghiên cứu và phát triển' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Nghiên cứu và phát triển', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Xử lý mẫu' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Xử lý mẫu', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Tài chính-Kế toán' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Kế toán' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Kế toán', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Vi sinh' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Vi sinh' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Vi sinh', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Chăm sóc khách hàng' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Chăm sóc khách hàng', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Nhận mẫu' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Nhận mẫu', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Trợ lý kinh doanh' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Trợ lý kinh doanh', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Vận chuyển mẫu' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Vận chuyển mẫu', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Kinh doanh' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Kinh doanh' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Kinh doanh', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Sắc ký', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Tài chính-Kế toán' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Kế toán' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Kế toán', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Vi sinh' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Vi sinh' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Vi sinh', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cà Mau'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Hành chính' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Hành chính', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cà Mau'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Vận chuyển mẫu' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Vận chuyển mẫu', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cà Mau'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Sắc ký', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Bạc liêu'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Nhận mẫu' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Nhận mẫu', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Bạc liêu'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Tạp vụ' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Tạp vụ', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Bạc liêu'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Vận chuyển mẫu' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Vận chuyển mẫu', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Bạc liêu'));
IF NOT EXISTS (SELECT 1 FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id)
    INSERT INTO section (section_id, name_vi, department_id, status, section_code) VALUES (NEWID(), N'Sắc ký', @dept_id, 'Active', 'SE-' + CAST(ABS(CHECKSUM(NEWID()) % 1000) AS VARCHAR));

-- Update Employees

-- Processing NV066: Nguyễn Quốc Toàn
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Ban Giám đốc' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Ban Giám đốc' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV066')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Quốc Toàn', department_id = @dept_id, section_id = @sect_id, 
        title = N'Tổng giám đốc điều hành', email = N'toan.nguyen@viet-labs.com', mobile = N'0939773328', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV066';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV066', N'Nguyễn Quốc Toàn', @dept_id, @sect_id, N'Tổng giám đốc điều hành', N'toan.nguyen@viet-labs.com', N'0939773328', N'Active', N'', GETUTCDATE());
END

-- Processing NV177: Nguyễn Thị Hồng Vân
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Chăm sóc khách hàng' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV177')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Thị Hồng Vân', department_id = @dept_id, section_id = @sect_id, 
        title = N'Trưởng phòng', email = N'van.nguyen@viet-labs.com', mobile = N'0973956023', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV177';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV177', N'Nguyễn Thị Hồng Vân', @dept_id, @sect_id, N'Trưởng phòng', N'van.nguyen@viet-labs.com', N'0973956023', N'Active', N'', GETUTCDATE());
END

-- Processing NV286: Lê Thị Mỹ Duyên
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Chăm sóc khách hàng' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV286')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Lê Thị Mỹ Duyên', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'cs_04@viet-labs.com', mobile = N'0941087039', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV286';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV286', N'Lê Thị Mỹ Duyên', @dept_id, @sect_id, N'Nhân viên', N'cs_04@viet-labs.com', N'0941087039', N'Active', N'', GETUTCDATE());
END

-- Processing NV261: Nguyễn Ngọc Quỳnh Mai
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Chăm sóc khách hàng' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV261')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Ngọc Quỳnh Mai', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'cs_02@viet-labs.com', mobile = N'0326736632', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV261';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV261', N'Nguyễn Ngọc Quỳnh Mai', @dept_id, @sect_id, N'Nhân viên', N'cs_02@viet-labs.com', N'0326736632', N'Active', N'', GETUTCDATE());
END

-- Processing NV292: Nguyễn Thị Thanh Thuỳ
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Chăm sóc khách hàng' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV292')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Thị Thanh Thuỳ', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'cs_01@viet-labs.com', mobile = N'0794862619', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV292';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV292', N'Nguyễn Thị Thanh Thuỳ', @dept_id, @sect_id, N'Nhân viên', N'cs_01@viet-labs.com', N'0794862619', N'Active', N'', GETUTCDATE());
END

-- Processing NV204: Trần Nguyễn Yến Nhi
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Nhận mẫu' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV204')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Trần Nguyễn Yến Nhi', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'tranyennhimk@gmail.com', mobile = N'0902963351', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV204';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV204', N'Trần Nguyễn Yến Nhi', @dept_id, @sect_id, N'Nhân viên', N'tranyennhimk@gmail.com', N'0902963351', N'Active', N'', GETUTCDATE());
END

-- Processing NV216: Lương Nữ Huyền Diệu
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Nhận mẫu' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV216')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Lương Nữ Huyền Diệu', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'huyendieu5320@gmail.com', mobile = N'0342386140', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV216';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV216', N'Lương Nữ Huyền Diệu', @dept_id, @sect_id, N'Nhân viên', N'huyendieu5320@gmail.com', N'0342386140', N'Active', N'', GETUTCDATE());
END

-- Processing NV221: Lưu Phương Thuý
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Nhận mẫu' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV221')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Lưu Phương Thuý', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'lpthuyluu2020@gmail.com', mobile = N'0969423814', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV221';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV221', N'Lưu Phương Thuý', @dept_id, @sect_id, N'Nhân viên', N'lpthuyluu2020@gmail.com', N'0969423814', N'Active', N'', GETUTCDATE());
END

-- Processing NV225: Nguyễn Thị Cẩm Tiên
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Nhận mẫu' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV225')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Thị Cẩm Tiên', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'ntcamtien0703@gmail.com', mobile = N'0772801769', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV225';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV225', N'Nguyễn Thị Cẩm Tiên', @dept_id, @sect_id, N'Nhân viên', N'ntcamtien0703@gmail.com', N'0772801769', N'Active', N'', GETUTCDATE());
END

-- Processing NV038: Nguyễn Thị Kim Anh
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Nhận mẫu' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV038')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Thị Kim Anh', department_id = @dept_id, section_id = @sect_id, 
        title = N'Giám sát viên', email = N'anh.nguyen@viet-labs.com', mobile = N'0385727213', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV038';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV038', N'Nguyễn Thị Kim Anh', @dept_id, @sect_id, N'Giám sát viên', N'anh.nguyen@viet-labs.com', N'0385727213', N'Active', N'', GETUTCDATE());
END

-- Processing NV283: Trịnh Xuân Hoàng
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Bảo trì (kĩ thuật)' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV283')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Trịnh Xuân Hoàng', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'hoanglacahoaca@gmail.com', mobile = N'0383183625', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV283';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV283', N'Trịnh Xuân Hoàng', @dept_id, @sect_id, N'Nhân viên', N'hoanglacahoaca@gmail.com', N'0383183625', N'Active', N'', GETUTCDATE());
END

-- Processing NV193: Thạch Sơn
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Bảo vệ' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV193')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Thạch Sơn', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'niengu0211@gmail.com', mobile = N'0965235432', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV193';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV193', N'Thạch Sơn', @dept_id, @sect_id, N'Nhân viên', N'niengu0211@gmail.com', N'0965235432', N'Active', N'', GETUTCDATE());
END

-- Processing NV227: Nguyễn Thị Thu Thuỷ
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Điều phối mẫu' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV227')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Thị Thu Thuỷ', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'thuthuy30071401@gmail.com', mobile = N'0384658735', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV227';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV227', N'Nguyễn Thị Thu Thuỷ', @dept_id, @sect_id, N'Nhân viên', N'thuthuy30071401@gmail.com', N'0384658735', N'Active', N'', GETUTCDATE());
END

-- Processing NV196: Trần Huy Dũng
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Mua hàng' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV196')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Trần Huy Dũng', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'muahang@viet-labs.com', mobile = N'0986731251', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV196';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV196', N'Trần Huy Dũng', @dept_id, @sect_id, N'Nhân viên', N'muahang@viet-labs.com', N'0986731251', N'Active', N'', GETUTCDATE());
END

-- Processing NV049: Huỳnh Thị Ngọc Kiều
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Nhân sự' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV049')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Huỳnh Thị Ngọc Kiều', department_id = @dept_id, section_id = @sect_id, 
        title = N'Trưởng phòng', email = N'kieu.huynh@viet-labs.com', mobile = N'0963523787', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV049';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV049', N'Huỳnh Thị Ngọc Kiều', @dept_id, @sect_id, N'Trưởng phòng', N'kieu.huynh@viet-labs.com', N'0963523787', N'Active', N'', GETUTCDATE());
END

-- Processing NV116: Trần Thị Huế
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Nhân sự' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV116')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Trần Thị Huế', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'hc-ns@viet-labs.com', mobile = N'0372858550', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV116';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV116', N'Trần Thị Huế', @dept_id, @sect_id, N'Nhân viên', N'hc-ns@viet-labs.com', N'0372858550', N'Active', N'', GETUTCDATE());
END

-- Processing NV144: Lê Thu Hiền
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Tạp vụ' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV144')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Lê Thu Hiền', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'lethuhien21284@gmail.com', mobile = N'0989939809', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV144';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV144', N'Lê Thu Hiền', @dept_id, @sect_id, N'Nhân viên', N'lethuhien21284@gmail.com', N'0989939809', N'Active', N'', GETUTCDATE());
END

-- Processing NV137: Đào Thị Mộng Điệp
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Tạp vụ' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV137')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Đào Thị Mộng Điệp', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'niengu0211@gmail.com', mobile = N'0974556416', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV137';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV137', N'Đào Thị Mộng Điệp', @dept_id, @sect_id, N'Nhân viên', N'niengu0211@gmail.com', N'0974556416', N'Active', N'', GETUTCDATE());
END

-- Processing NV108: Hồng Đức Minh
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Vận chuyển mẫu' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV108')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Hồng Đức Minh', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'minhmeo2007357@gmail.com', mobile = N'0788652748', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV108';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV108', N'Hồng Đức Minh', @dept_id, @sect_id, N'Nhân viên', N'minhmeo2007357@gmail.com', N'0788652748', N'Active', N'', GETUTCDATE());
END

-- Processing NV194: Châu Thanh Sử
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Vận chuyển mẫu' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV194')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Châu Thanh Sử', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'chauthanhsu6991@gmail.com', mobile = N'0914327358', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV194';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV194', N'Châu Thanh Sử', @dept_id, @sect_id, N'Nhân viên', N'chauthanhsu6991@gmail.com', N'0914327358', N'Active', N'', GETUTCDATE());
END

-- Processing NV197: Đặng Văn Trang
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Vận chuyển mẫu' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV197')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Đặng Văn Trang', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'dangvantrang2207@gmail.com', mobile = N'0862648897', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV197';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV197', N'Đặng Văn Trang', @dept_id, @sect_id, N'Nhân viên', N'dangvantrang2207@gmail.com', N'0862648897', N'Active', N'', GETUTCDATE());
END

-- Processing NV200: Ngô Thảo Ly
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Kinh doanh' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Kinh doanh' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV200')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Ngô Thảo Ly', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'thaoly.ngo@viet-labs.com', mobile = N'0363286068', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV200';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV200', N'Ngô Thảo Ly', @dept_id, @sect_id, N'Nhân viên', N'thaoly.ngo@viet-labs.com', N'0363286068', N'Active', N'', GETUTCDATE());
END

-- Processing NV109: Lê Thị Diễm My
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Kinh doanh' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Kinh doanh' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV109')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Lê Thị Diễm My', department_id = @dept_id, section_id = @sect_id, 
        title = N'Giám đốc', email = N'diemmy.le@viet-labs.com', mobile = N'0909350893', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV109';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV109', N'Lê Thị Diễm My', @dept_id, @sect_id, N'Giám đốc', N'diemmy.le@viet-labs.com', N'0909350893', N'Active', N'', GETUTCDATE());
END

-- Processing NV158: Đỗ Thị Kim Huệ
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Phòng thí nghiệm' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Trả kết quả' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV158')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Đỗ Thị Kim Huệ', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'dothikimhue11052000@gmail.com', mobile = N'0964672775', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV158';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV158', N'Đỗ Thị Kim Huệ', @dept_id, @sect_id, N'Nhân viên', N'dothikimhue11052000@gmail.com', N'0964672775', N'Active', N'', GETUTCDATE());
END

-- Processing NV199: Nguyễn Thanh Trang
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Phòng thí nghiệm' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Trả kết quả' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV199')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Thanh Trang', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'ngthtrang1988@gmail.com', mobile = N'0909177154', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV199';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV199', N'Nguyễn Thanh Trang', @dept_id, @sect_id, N'Nhân viên', N'ngthtrang1988@gmail.com', N'0909177154', N'Active', N'', GETUTCDATE());
END

-- Processing NV250: Phan Mỹ Ngọc
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Phòng thí nghiệm' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Trả kết quả' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV250')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Phan Mỹ Ngọc', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'pmngoc1114@gmail.com', mobile = N'0848454544', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV250';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV250', N'Phan Mỹ Ngọc', @dept_id, @sect_id, N'Nhân viên', N'pmngoc1114@gmail.com', N'0848454544', N'Active', N'', GETUTCDATE());
END

-- Processing NV274: Hồ Thị Hồng Thương
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Phòng thí nghiệm' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Trả kết quả' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV274')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Hồ Thị Hồng Thương', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'hongthuong261199@gmail.com', mobile = N'0963206898', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV274';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV274', N'Hồ Thị Hồng Thương', @dept_id, @sect_id, N'Nhân viên', N'hongthuong261199@gmail.com', N'0963206898', N'Active', N'', GETUTCDATE());
END

-- Processing NV267: Trần Thị Nam
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Phòng thí nghiệm' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Trả kết quả' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV267')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Trần Thị Nam', department_id = @dept_id, section_id = @sect_id, 
        title = N'Giám sát viên', email = N'namtran136@gmail.com', mobile = N'0975768044', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV267';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV267', N'Trần Thị Nam', @dept_id, @sect_id, N'Giám sát viên', N'namtran136@gmail.com', N'0975768044', N'Active', N'', GETUTCDATE());
END

-- Processing NV201: Cao Nguyễn Trúc Anh
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quản lý Chất lượng' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Quản lý Chất lượng' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV201')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Cao Nguyễn Trúc Anh', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'anhcao95vn@gmail.com', mobile = N'0985777133', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV201';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV201', N'Cao Nguyễn Trúc Anh', @dept_id, @sect_id, N'Nhân viên', N'anhcao95vn@gmail.com', N'0985777133', N'Active', N'', GETUTCDATE());
END

-- Processing NV287: Lê Bảo Ngọc
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quản lý Chất lượng' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Quản lý Chất lượng' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV287')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Lê Bảo Ngọc', department_id = @dept_id, section_id = @sect_id, 
        title = N'Giám đốc', email = N'ngoc.le@viet-labs.com', mobile = N'0919253305', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV287';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV287', N'Lê Bảo Ngọc', @dept_id, @sect_id, N'Giám đốc', N'ngoc.le@viet-labs.com', N'0919253305', N'Active', N'', GETUTCDATE());
END

-- Processing NV153: Võ Thị Cẩm Nhung
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quang phổ - Cổ điển' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Quang phổ - Cổ điển' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV153')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Võ Thị Cẩm Nhung', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'vtcnhungg@gmail.com', mobile = N'0368133489', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV153';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV153', N'Võ Thị Cẩm Nhung', @dept_id, @sect_id, N'Thử nghiệm viên', N'vtcnhungg@gmail.com', N'0368133489', N'Active', N'', GETUTCDATE());
END

-- Processing NV154: Phan Bảo Kim Xuân
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quang phổ - Cổ điển' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Quang phổ - Cổ điển' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV154')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Phan Bảo Kim Xuân', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'xuanbt06112001@gmail.com', mobile = N'0949280094', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV154';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV154', N'Phan Bảo Kim Xuân', @dept_id, @sect_id, N'Thử nghiệm viên', N'xuanbt06112001@gmail.com', N'0949280094', N'Active', N'', GETUTCDATE());
END

-- Processing NV155: Nguyễn Nhật An
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quang phổ - Cổ điển' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Quang phổ - Cổ điển' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV155')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Nhật An', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'nhatan73@gmail.com', mobile = N'0708955406', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV155';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV155', N'Nguyễn Nhật An', @dept_id, @sect_id, N'Thử nghiệm viên', N'nhatan73@gmail.com', N'0708955406', N'Active', N'', GETUTCDATE());
END

-- Processing NV202: Nguyễn Thế Toàn
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quang phổ - Cổ điển' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Quang phổ - Cổ điển' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV202')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Thế Toàn', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'thetoan15032017@gmail.com', mobile = N'0395649364', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV202';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV202', N'Nguyễn Thế Toàn', @dept_id, @sect_id, N'Thử nghiệm viên', N'thetoan15032017@gmail.com', N'0395649364', N'Active', N'', GETUTCDATE());
END

-- Processing NV220: Quảng Thị Thanh Trúc
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quang phổ - Cổ điển' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Quang phổ - Cổ điển' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV220')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Quảng Thị Thanh Trúc', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'trucquang00@gmail.com', mobile = N'0348489949', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV220';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV220', N'Quảng Thị Thanh Trúc', @dept_id, @sect_id, N'Thử nghiệm viên', N'trucquang00@gmail.com', N'0348489949', N'Active', N'', GETUTCDATE());
END

-- Processing NV222: Lê Ngọc Thanh Thảo
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quang phổ - Cổ điển' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Quang phổ - Cổ điển' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV222')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Lê Ngọc Thanh Thảo', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'lnthanhthao0812@gmail.com', mobile = N'0932759206', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV222';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV222', N'Lê Ngọc Thanh Thảo', @dept_id, @sect_id, N'Thử nghiệm viên', N'lnthanhthao0812@gmail.com', N'0932759206', N'Active', N'', GETUTCDATE());
END

-- Processing NV262: Đặng Huy Bảo Châu
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quang phổ - Cổ điển' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Quang phổ - Cổ điển' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV262')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Đặng Huy Bảo Châu', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'chaudang2703@gmail.com', mobile = N'0828963760', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV262';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV262', N'Đặng Huy Bảo Châu', @dept_id, @sect_id, N'Thử nghiệm viên', N'chaudang2703@gmail.com', N'0828963760', N'Active', N'', GETUTCDATE());
END

-- Processing NV264: Từ Vĩ Đạt
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quang phổ - Cổ điển' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Quang phổ - Cổ điển' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV264')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Từ Vĩ Đạt', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'tvdat99@gmail.com', mobile = N'0843546428', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV264';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV264', N'Từ Vĩ Đạt', @dept_id, @sect_id, N'Thử nghiệm viên', N'tvdat99@gmail.com', N'0843546428', N'Active', N'', GETUTCDATE());
END

-- Processing NV270: Huỳnh Thị Kim Huỳnh
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quang phổ - Cổ điển' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Quang phổ - Cổ điển' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV270')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Huỳnh Thị Kim Huỳnh', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'huynhkimhuynh2106@gmail.com', mobile = N'0949920206', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV270';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV270', N'Huỳnh Thị Kim Huỳnh', @dept_id, @sect_id, N'Thử nghiệm viên', N'huynhkimhuynh2106@gmail.com', N'0949920206', N'Active', N'', GETUTCDATE());
END

-- Processing NV282: Thạch Hoài Nhân
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quang phổ - Cổ điển' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Quang phổ - Cổ điển' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV282')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Thạch Hoài Nhân', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'thnhan20022808@gmail.com', mobile = N'0857223577', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV282';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV282', N'Thạch Hoài Nhân', @dept_id, @sect_id, N'Thử nghiệm viên', N'thnhan20022808@gmail.com', N'0857223577', N'Active', N'', GETUTCDATE());
END

-- Processing NV284: Nguyễn Thị Bảo Ngọc
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quang phổ - Cổ điển' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Quang phổ - Cổ điển' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV284')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Thị Bảo Ngọc', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'nguyenngoc050302@gmail.com', mobile = N'0866129204', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV284';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV284', N'Nguyễn Thị Bảo Ngọc', @dept_id, @sect_id, N'Thử nghiệm viên', N'nguyenngoc050302@gmail.com', N'0866129204', N'Active', N'', GETUTCDATE());
END

-- Processing NV289: Nguyễn Hiếu Kiên
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quang phổ - Cổ điển' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Quang phổ - Cổ điển' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV289')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Hiếu Kiên', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'nguyenhieukien01@gmail.com', mobile = N'0968312973', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV289';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV289', N'Nguyễn Hiếu Kiên', @dept_id, @sect_id, N'Thử nghiệm viên', N'nguyenhieukien01@gmail.com', N'0968312973', N'Active', N'', GETUTCDATE());
END

-- Processing NV291: Nguyễn Ngọc Xuân Trúc
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quang phổ - Cổ điển' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Quang phổ - Cổ điển' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV291')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Ngọc Xuân Trúc', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'ngoctrucnguyenxuan@gmail.com', mobile = N'0795863438', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV291';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV291', N'Nguyễn Ngọc Xuân Trúc', @dept_id, @sect_id, N'Thử nghiệm viên', N'ngoctrucnguyenxuan@gmail.com', N'0795863438', N'Active', N'', GETUTCDATE());
END

-- Processing NV012: Nguyễn Ngọc Hân
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quang phổ - Cổ điển' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Quang phổ - Cổ điển' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV012')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Ngọc Hân', department_id = @dept_id, section_id = @sect_id, 
        title = N'Phó phòng', email = N'han.nguyen@viet-labs.com', mobile = N'0943033225', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV012';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV012', N'Nguyễn Ngọc Hân', @dept_id, @sect_id, N'Phó phòng', N'han.nguyen@viet-labs.com', N'0943033225', N'Active', N'', GETUTCDATE());
END

-- Processing NV275: Phan Tấn Lập
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quang phổ - Cổ điển' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Quang phổ - Cổ điển' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV275')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Phan Tấn Lập', department_id = @dept_id, section_id = @sect_id, 
        title = N'Phó phòng', email = N'lap.phan@viet-labs.com', mobile = N'0373673911', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV275';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV275', N'Phan Tấn Lập', @dept_id, @sect_id, N'Phó phòng', N'lap.phan@viet-labs.com', N'0373673911', N'Active', N'', GETUTCDATE());
END

-- Processing NV258: Huỳnh Nguyễn Anh Thi
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Quang phổ - Cổ điển' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Xử lý mẫu' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV258')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Huỳnh Nguyễn Anh Thi', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'anhthinguyen201@gmail.com', mobile = N'0798920103', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV258';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV258', N'Huỳnh Nguyễn Anh Thi', @dept_id, @sect_id, N'Nhân viên', N'anhthinguyen201@gmail.com', N'0798920103', N'Active', N'', GETUTCDATE());
END

-- Processing NV223: Phạm Lê Tiến Khánh
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV223')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Phạm Lê Tiến Khánh', department_id = @dept_id, section_id = @sect_id, 
        title = N'Trưởng phòng', email = N'khanh.pham@viet-labs.com', mobile = N'9008819544', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV223';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV223', N'Phạm Lê Tiến Khánh', @dept_id, @sect_id, N'Trưởng phòng', N'khanh.pham@viet-labs.com', N'9008819544', N'Active', N'', GETUTCDATE());
END

-- Processing NV171: Nguyễn Hải Phương
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV171')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Hải Phương', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'nguyenhaiphuongcv@gmail.com', mobile = N'0399268624', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV171';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV171', N'Nguyễn Hải Phương', @dept_id, @sect_id, N'Thử nghiệm viên', N'nguyenhaiphuongcv@gmail.com', N'0399268624', N'Active', N'', GETUTCDATE());
END

-- Processing NV180: Nguyễn Thị Thuỳ Hân
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV180')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Thị Thuỳ Hân', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'nthuyhan.work@gmail.com', mobile = N'0766806692', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV180';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV180', N'Nguyễn Thị Thuỳ Hân', @dept_id, @sect_id, N'Thử nghiệm viên', N'nthuyhan.work@gmail.com', N'0766806692', N'Active', N'', GETUTCDATE());
END

-- Processing NV206: Nguyễn Hải Đăng
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV206')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Hải Đăng', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'nhdang1401@gmail.com', mobile = N'0328999752', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV206';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV206', N'Nguyễn Hải Đăng', @dept_id, @sect_id, N'Thử nghiệm viên', N'nhdang1401@gmail.com', N'0328999752', N'Active', N'', GETUTCDATE());
END

-- Processing NV247: Đỗ Bích Thủy
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV247')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Đỗ Bích Thủy', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'dobichthuy0905@gmail.com', mobile = N'0359354151', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV247';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV247', N'Đỗ Bích Thủy', @dept_id, @sect_id, N'Thử nghiệm viên', N'dobichthuy0905@gmail.com', N'0359354151', N'Active', N'', GETUTCDATE());
END

-- Processing NV273: Lê Thanh Huy
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV273')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Lê Thanh Huy', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'thanhhuy40920082003@gmail.com', mobile = N'0932010627', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV273';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV273', N'Lê Thanh Huy', @dept_id, @sect_id, N'Thử nghiệm viên', N'thanhhuy40920082003@gmail.com', N'0932010627', N'Active', N'', GETUTCDATE());
END

-- Processing NV243: Nguyễn Huỳnh Kim Ngân
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Phòng thí nghiệm' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Nghiên cứu và phát triển' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV243')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Huỳnh Kim Ngân', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'nhkngan94@gmail.com', mobile = N'0859154675', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV243';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV243', N'Nguyễn Huỳnh Kim Ngân', @dept_id, @sect_id, N'Nhân viên', N'nhkngan94@gmail.com', N'0859154675', N'Active', N'', GETUTCDATE());
END

-- Processing NV280: Nguyễn Huỳnh Linh Nhi
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV280')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Huỳnh Linh Nhi', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'nguyenhuynhlinhnhi303@gmail.com', mobile = N'0346916833', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV280';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV280', N'Nguyễn Huỳnh Linh Nhi', @dept_id, @sect_id, N'Thử nghiệm viên', N'nguyenhuynhlinhnhi303@gmail.com', N'0346916833', N'Active', N'', GETUTCDATE());
END

-- Processing NV276: Trần Thị Diễm My
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV276')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Trần Thị Diễm My', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'my.tranthidiem2003@gmail.com', mobile = N'0914896294', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV276';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV276', N'Trần Thị Diễm My', @dept_id, @sect_id, N'Thử nghiệm viên', N'my.tranthidiem2003@gmail.com', N'0914896294', N'Active', N'', GETUTCDATE());
END

-- Processing NV288: Trần Thuỳ Quyên
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV288')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Trần Thuỳ Quyên', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'thuyquyen.lop93@gmail.com', mobile = N'0395706715', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV288';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV288', N'Trần Thuỳ Quyên', @dept_id, @sect_id, N'Thử nghiệm viên', N'thuyquyen.lop93@gmail.com', N'0395706715', N'Active', N'', GETUTCDATE());
END

-- Processing NV290: Nguyễn Thành Vũ
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV290')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Thành Vũ', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'vuvu5245@gmail.com', mobile = N'0367758292', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV290';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV290', N'Nguyễn Thành Vũ', @dept_id, @sect_id, N'Thử nghiệm viên', N'vuvu5245@gmail.com', N'0367758292', N'Active', N'', GETUTCDATE());
END

-- Processing NV294: Nguyễn Minh Hân
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV294')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Minh Hân', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'choco30082002@gmail.com', mobile = N'0928070847', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV294';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV294', N'Nguyễn Minh Hân', @dept_id, @sect_id, N'Thử nghiệm viên', N'choco30082002@gmail.com', N'0928070847', N'Active', N'', GETUTCDATE());
END

-- Processing NV293: Phan Triệu Duy
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Xử lý mẫu' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV293')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Phan Triệu Duy', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'tduy2702@gmail.com', mobile = N'0856276277', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV293';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV293', N'Phan Triệu Duy', @dept_id, @sect_id, N'Nhân viên', N'tduy2702@gmail.com', N'0856276277', N'Active', N'', GETUTCDATE());
END

-- Processing NV248: Nguyễn Thị Mai Vy
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Tài chính-Kế toán' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Kế toán' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV248')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Thị Mai Vy', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'ntmvy552003@gmail.com', mobile = N'0387817509', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV248';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV248', N'Nguyễn Thị Mai Vy', @dept_id, @sect_id, N'Nhân viên', N'ntmvy552003@gmail.com', N'0387817509', N'Active', N'', GETUTCDATE());
END

-- Processing NV005: Phạm Huỳnh Như
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Tài chính-Kế toán' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Kế toán' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV005')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Phạm Huỳnh Như', department_id = @dept_id, section_id = @sect_id, 
        title = N'Giám sát viên', email = N'phnhu8811@gmail.com', mobile = N'0838194124', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV005';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV005', N'Phạm Huỳnh Như', @dept_id, @sect_id, N'Giám sát viên', N'phnhu8811@gmail.com', N'0838194124', N'Active', N'', GETUTCDATE());
END

-- Processing NV120: Nguyễn Thị Thu Thuỷ
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Vi sinh' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Vi sinh' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV120')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Thị Thu Thuỷ', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'nguyenttthuy197320@gmail.com', mobile = N'0773063368', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV120';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV120', N'Nguyễn Thị Thu Thuỷ', @dept_id, @sect_id, N'Thử nghiệm viên', N'nguyenttthuy197320@gmail.com', N'0773063368', N'Active', N'', GETUTCDATE());
END

-- Processing NV152: Lê Hải Dương
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Vi sinh' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Vi sinh' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV152')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Lê Hải Dương', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'lehaiduong1904@gmail.com', mobile = N'0378202481', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV152';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV152', N'Lê Hải Dương', @dept_id, @sect_id, N'Thử nghiệm viên', N'lehaiduong1904@gmail.com', N'0378202481', N'Active', N'', GETUTCDATE());
END

-- Processing NV165: Nguyễn Kiều My
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Vi sinh' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Vi sinh' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV165')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Kiều My', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'kieumynguyen1609@gmail.com', mobile = N'0375349706', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV165';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV165', N'Nguyễn Kiều My', @dept_id, @sect_id, N'Thử nghiệm viên', N'kieumynguyen1609@gmail.com', N'0375349706', N'Active', N'', GETUTCDATE());
END

-- Processing NV217: Tiết Thị Diễm My
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Vi sinh' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Vi sinh' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV217')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Tiết Thị Diễm My', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'diemm4355@gmail.com', mobile = N'0337759036', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV217';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV217', N'Tiết Thị Diễm My', @dept_id, @sect_id, N'Thử nghiệm viên', N'diemm4355@gmail.com', N'0337759036', N'Active', N'', GETUTCDATE());
END

-- Processing NV233: Phan Trúc Quân
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Vi sinh' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Vi sinh' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV233')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Phan Trúc Quân', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'quan180520@gmail.com', mobile = N'0909566726', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV233';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV233', N'Phan Trúc Quân', @dept_id, @sect_id, N'Thử nghiệm viên', N'quan180520@gmail.com', N'0909566726', N'Active', N'', GETUTCDATE());
END

-- Processing NV271: Dương Thị Kim Dị
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Hồ Chí Minh');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Vi sinh' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Vi sinh' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV271')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Dương Thị Kim Dị', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'duongkimdi220724@gmail.com', mobile = N'0337818723', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV271';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV271', N'Dương Thị Kim Dị', @dept_id, @sect_id, N'Thử nghiệm viên', N'duongkimdi220724@gmail.com', N'0337818723', N'Active', N'', GETUTCDATE());
END

-- Processing NV189: Phan Thị Bích Phượng
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Chăm sóc khách hàng' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV189')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Phan Thị Bích Phượng', department_id = @dept_id, section_id = @sect_id, 
        title = N'Trưởng phòng', email = N'phuong.phan@viet-labs.com', mobile = N'0932866817', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV189';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV189', N'Phan Thị Bích Phượng', @dept_id, @sect_id, N'Trưởng phòng', N'phuong.phan@viet-labs.com', N'0932866817', N'Active', N'', GETUTCDATE());
END

-- Processing NV084: Lê Ngọc Ngân
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Chăm sóc khách hàng' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV084')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Lê Ngọc Ngân', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'ngan.le@viet-labs.com', mobile = N'0919433112', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV084';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV084', N'Lê Ngọc Ngân', @dept_id, @sect_id, N'Nhân viên', N'ngan.le@viet-labs.com', N'0919433112', N'Active', N'', GETUTCDATE());
END

-- Processing NV183: Nguyễn Viết Quỳnh Thy
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Chăm sóc khách hàng' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV183')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Viết Quỳnh Thy', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'thy.nguyen@viet-labs.com', mobile = N'0932967656', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV183';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV183', N'Nguyễn Viết Quỳnh Thy', @dept_id, @sect_id, N'Nhân viên', N'thy.nguyen@viet-labs.com', N'0932967656', N'Active', N'', GETUTCDATE());
END

-- Processing NV281: Nguyễn Thị Hải Yến
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Nhận mẫu' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV281')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Thị Hải Yến', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'haiyen.5700@gmail.com', mobile = N'0939835107', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV281';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV281', N'Nguyễn Thị Hải Yến', @dept_id, @sect_id, N'Nhân viên', N'haiyen.5700@gmail.com', N'0939835107', N'Active', N'', GETUTCDATE());
END

-- Processing NV192: Nguyễn Tú Thanh
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Trợ lý kinh doanh' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV192')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Tú Thanh', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'thanh.nguyen@viet-labs.com', mobile = N'0918552789', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV192';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV192', N'Nguyễn Tú Thanh', @dept_id, @sect_id, N'Nhân viên', N'thanh.nguyen@viet-labs.com', N'0918552789', N'Active', N'', GETUTCDATE());
END

-- Processing NV241: Nguyễn Thị Anh Thư
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Trợ lý kinh doanh' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV241')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Thị Anh Thư', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'nguyenthianhthu6530@gmail.com', mobile = N'0931016530', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV241';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV241', N'Nguyễn Thị Anh Thư', @dept_id, @sect_id, N'Nhân viên', N'nguyenthianhthu6530@gmail.com', N'0931016530', N'Active', N'', GETUTCDATE());
END

-- Processing NV161: Nguyễn Việt Tân
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Vận chuyển mẫu' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV161')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Việt Tân', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'nnvvtt899@gmail.com', mobile = N'0377839341', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV161';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV161', N'Nguyễn Việt Tân', @dept_id, @sect_id, N'Nhân viên', N'nnvvtt899@gmail.com', N'0377839341', N'Active', N'', GETUTCDATE());
END

-- Processing NV269: Lê Chí Cường
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Vận chuyển mẫu' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV269')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Lê Chí Cường', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'lechicuongtv84@gmail.com', mobile = N'0356089390', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV269';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV269', N'Lê Chí Cường', @dept_id, @sect_id, N'Nhân viên', N'lechicuongtv84@gmail.com', N'0356089390', N'Active', N'', GETUTCDATE());
END

-- Processing NV024: Nguyễn Thị Mỹ Tiên
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Kinh doanh' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Kinh doanh' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV024')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Thị Mỹ Tiên', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'tien.nguyen@viet-labs.com', mobile = N'0939176997', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV024';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV024', N'Nguyễn Thị Mỹ Tiên', @dept_id, @sect_id, N'Nhân viên', N'tien.nguyen@viet-labs.com', N'0939176997', N'Active', N'', GETUTCDATE());
END

-- Processing NV224: Huỳnh Lương Anh Khoa
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Kinh doanh' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Kinh doanh' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV224')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Huỳnh Lương Anh Khoa', department_id = @dept_id, section_id = @sect_id, 
        title = N'Giám đốc', email = N'khoa.huynh@viet-labs.com', mobile = N'0989717343', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV224';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV224', N'Huỳnh Lương Anh Khoa', @dept_id, @sect_id, N'Giám đốc', N'khoa.huynh@viet-labs.com', N'0989717343', N'Active', N'', GETUTCDATE());
END

-- Processing NV124: Trần Thị Hồng An
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV124')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Trần Thị Hồng An', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'antran13233@gmail.com', mobile = N'0376008026', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV124';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV124', N'Trần Thị Hồng An', @dept_id, @sect_id, N'Thử nghiệm viên', N'antran13233@gmail.com', N'0376008026', N'Active', N'', GETUTCDATE());
END

-- Processing NV128: Trang Hoàng Hải Lân
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV128')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Trang Hoàng Hải Lân', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'hailanph12326@gmail.com', mobile = N'0948241998', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV128';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV128', N'Trang Hoàng Hải Lân', @dept_id, @sect_id, N'Thử nghiệm viên', N'hailanph12326@gmail.com', N'0948241998', N'Active', N'', GETUTCDATE());
END

-- Processing NV086: Nguyễn Thị Thanh Thanh
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV086')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Thị Thanh Thanh', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'nguyenthithanht802@gmail.com', mobile = N'0927117593', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV086';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV086', N'Nguyễn Thị Thanh Thanh', @dept_id, @sect_id, N'Thử nghiệm viên', N'nguyenthithanht802@gmail.com', N'0927117593', N'Active', N'', GETUTCDATE());
END

-- Processing NV110: Lê Minh Vương
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV110')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Lê Minh Vương', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'vuong31898@gmail.com', mobile = N'0799507454', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV110';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV110', N'Lê Minh Vương', @dept_id, @sect_id, N'Thử nghiệm viên', N'vuong31898@gmail.com', N'0799507454', N'Active', N'', GETUTCDATE());
END

-- Processing NV112: Lâm Chấn Dũ
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV112')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Lâm Chấn Dũ', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'lamchanvu2022ct@gmail.com', mobile = N'0983773328', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV112';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV112', N'Lâm Chấn Dũ', @dept_id, @sect_id, N'Thử nghiệm viên', N'lamchanvu2022ct@gmail.com', N'0983773328', N'Active', N'', GETUTCDATE());
END

-- Processing NV087: Võ Thị Hồng Tươi
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV087')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Võ Thị Hồng Tươi', department_id = @dept_id, section_id = @sect_id, 
        title = N'Giám sát viên', email = N'vthtuoi212@gmail.com', mobile = N'0986824214', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV087';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV087', N'Võ Thị Hồng Tươi', @dept_id, @sect_id, N'Giám sát viên', N'vthtuoi212@gmail.com', N'0986824214', N'Active', N'', GETUTCDATE());
END

-- Processing NV215: Nguyễn Thị Nhưng
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Tài chính-Kế toán' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Kế toán' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV215')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Thị Nhưng', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'nhungnttlcc@gmail.com', mobile = N'0915290898', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV215';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV215', N'Nguyễn Thị Nhưng', @dept_id, @sect_id, N'Nhân viên', N'nhungnttlcc@gmail.com', N'0915290898', N'Active', N'', GETUTCDATE());
END

-- Processing NV266: Dương Thái Thảo
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Tài chính-Kế toán' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Kế toán' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV266')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Dương Thái Thảo', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'thaithao.k35ueh@gmail.com', mobile = N'0987379348', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV266';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV266', N'Dương Thái Thảo', @dept_id, @sect_id, N'Nhân viên', N'thaithao.k35ueh@gmail.com', N'0987379348', N'Active', N'', GETUTCDATE());
END

-- Processing NV246: Trần An Bình
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Vi sinh' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Vi sinh' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV246')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Trần An Bình', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'tabinh0509@gmail.com', mobile = N'0388045750', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV246';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV246', N'Trần An Bình', @dept_id, @sect_id, N'Thử nghiệm viên', N'tabinh0509@gmail.com', N'0388045750', N'Active', N'', GETUTCDATE());
END

-- Processing NV244: Trương Tô Hải Đăng
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cần Thơ');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Vi sinh' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Vi sinh' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV244')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Trương Tô Hải Đăng', department_id = @dept_id, section_id = @sect_id, 
        title = N'Phó phòng', email = N'dang.truong@viet-labs.com', mobile = N'0907355649', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV244';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV244', N'Trương Tô Hải Đăng', @dept_id, @sect_id, N'Phó phòng', N'dang.truong@viet-labs.com', N'0907355649', N'Active', N'', GETUTCDATE());
END

-- Processing NV249: Phạm Thu Trang
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cà Mau');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Hành chính' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV249')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Phạm Thu Trang', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'thutrangbaclieu123@gmail.com', mobile = N'0946223125', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV249';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV249', N'Phạm Thu Trang', @dept_id, @sect_id, N'Nhân viên', N'thutrangbaclieu123@gmail.com', N'0946223125', N'Active', N'', GETUTCDATE());
END

-- Processing NV143: Nguyễn Thành Tài
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cà Mau');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Vận chuyển mẫu' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV143')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Thành Tài', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'tainguyencm1998@gmail.com', mobile = N'0947644955', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV143';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV143', N'Nguyễn Thành Tài', @dept_id, @sect_id, N'Nhân viên', N'tainguyencm1998@gmail.com', N'0947644955', N'Active', N'', GETUTCDATE());
END

-- Processing NV059: Huỳnh Khoa Đãnh
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cà Mau');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV059')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Huỳnh Khoa Đãnh', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'khoadanh266.hkd@gmail.com', mobile = N'0949878951', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV059';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV059', N'Huỳnh Khoa Đãnh', @dept_id, @sect_id, N'Thử nghiệm viên', N'khoadanh266.hkd@gmail.com', N'0949878951', N'Active', N'', GETUTCDATE());
END

-- Processing NV133: Lê Hoài Nam
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cà Mau');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV133')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Lê Hoài Nam', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'lehoainam2016cm@gmail.com', mobile = N'0835394794', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV133';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV133', N'Lê Hoài Nam', @dept_id, @sect_id, N'Thử nghiệm viên', N'lehoainam2016cm@gmail.com', N'0835394794', N'Active', N'', GETUTCDATE());
END

-- Processing NV138: Lê Văn Tạo
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cà Mau');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV138')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Lê Văn Tạo', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'letao928@gmail.com', mobile = N'0942002854', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV138';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV138', N'Lê Văn Tạo', @dept_id, @sect_id, N'Thử nghiệm viên', N'letao928@gmail.com', N'0942002854', N'Active', N'', GETUTCDATE());
END

-- Processing NV208: Nguyễn Hoài Linh
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cà Mau');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV208')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Hoài Linh', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'hoailinh19170175@gmail.com', mobile = N'0832811446', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV208';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV208', N'Nguyễn Hoài Linh', @dept_id, @sect_id, N'Thử nghiệm viên', N'hoailinh19170175@gmail.com', N'0832811446', N'Active', N'', GETUTCDATE());
END

-- Processing NV253: Tăng Thị Ngọc Hân
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Cà Mau');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV253')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Tăng Thị Ngọc Hân', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'ngochan04101999@gmail.com', mobile = N'0948368652', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV253';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV253', N'Tăng Thị Ngọc Hân', @dept_id, @sect_id, N'Thử nghiệm viên', N'ngochan04101999@gmail.com', N'0948368652', N'Active', N'', GETUTCDATE());
END

-- Processing NV022: Hứa Thị Như Nguyệt
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Bạc liêu');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Dịch vụ khách hàng' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Nhận mẫu' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV022')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Hứa Thị Như Nguyệt', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'nguyethuavl@gmail.com', mobile = N'0917208232', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV022';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV022', N'Hứa Thị Như Nguyệt', @dept_id, @sect_id, N'Nhân viên', N'nguyethuavl@gmail.com', N'0917208232', N'Active', N'', GETUTCDATE());
END

-- Processing NV212: Lê Thị Lương
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Bạc liêu');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Tạp vụ' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV212')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Lê Thị Lương', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'lethiluong200997@gmail.com', mobile = N'0945002946', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV212';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV212', N'Lê Thị Lương', @dept_id, @sect_id, N'Nhân viên', N'lethiluong200997@gmail.com', N'0945002946', N'Active', N'', GETUTCDATE());
END

-- Processing NV207: Huỳnh Anh Tuấn
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Bạc liêu');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Vận chuyển mẫu' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV207')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Huỳnh Anh Tuấn', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'2001huynhanhtuan@gmail.com', mobile = N'0854888330', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV207';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV207', N'Huỳnh Anh Tuấn', @dept_id, @sect_id, N'Nhân viên', N'2001huynhanhtuan@gmail.com', N'0854888330', N'Active', N'', GETUTCDATE());
END

-- Processing NV285: Trần Chí Ngoan
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Bạc liêu');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Hành chính-Nhân sự' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Vận chuyển mẫu' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV285')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Trần Chí Ngoan', department_id = @dept_id, section_id = @sect_id, 
        title = N'Nhân viên', email = N'tranchingoan21@gmail.com', mobile = N'0382098292', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV285';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV285', N'Trần Chí Ngoan', @dept_id, @sect_id, N'Nhân viên', N'tranchingoan21@gmail.com', N'0382098292', N'Active', N'', GETUTCDATE());
END

-- Processing NV053: Đặng Phước Thi
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Bạc liêu');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV053')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Đặng Phước Thi', department_id = @dept_id, section_id = @sect_id, 
        title = N'Trưởng phòng', email = N'dpthi.vietlab@gmail.com', mobile = N'0901228953', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV053';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV053', N'Đặng Phước Thi', @dept_id, @sect_id, N'Trưởng phòng', N'dpthi.vietlab@gmail.com', N'0901228953', N'Active', N'', GETUTCDATE());
END

-- Processing NV028: Nguyễn Ngọc Linh
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Bạc liêu');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV028')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Ngọc Linh', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'linhngocnguyen14052000@gmail.com', mobile = N'0353137558', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV028';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV028', N'Nguyễn Ngọc Linh', @dept_id, @sect_id, N'Thử nghiệm viên', N'linhngocnguyen14052000@gmail.com', N'0353137558', N'Active', N'', GETUTCDATE());
END

-- Processing NV057: Nguyễn Văn Thành
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Bạc liêu');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV057')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Văn Thành', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'nguyenvanthanh15102022@gmail.com', mobile = N'0378254154', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV057';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV057', N'Nguyễn Văn Thành', @dept_id, @sect_id, N'Thử nghiệm viên', N'nguyenvanthanh15102022@gmail.com', N'0378254154', N'Active', N'', GETUTCDATE());
END

-- Processing NV064: Tăng Thành Lộc
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Bạc liêu');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV064')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Tăng Thành Lộc', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'ttloc356@gmail.com', mobile = N'0949 931 401', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV064';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV064', N'Tăng Thành Lộc', @dept_id, @sect_id, N'Thử nghiệm viên', N'ttloc356@gmail.com', N'0949 931 401', N'Active', N'', GETUTCDATE());
END

-- Processing NV142: Đái Thị Huỳnh Như
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Bạc liêu');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV142')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Đái Thị Huỳnh Như', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'daithihuynhnhu18@gmail.com', mobile = N'0939522200', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV142';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV142', N'Đái Thị Huỳnh Như', @dept_id, @sect_id, N'Thử nghiệm viên', N'daithihuynhnhu18@gmail.com', N'0939522200', N'Active', N'', GETUTCDATE());
END

-- Processing NV218: Nguyễn Văn Bữu
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Bạc liêu');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV218')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Nguyễn Văn Bữu', department_id = @dept_id, section_id = @sect_id, 
        title = N'Thử nghiệm viên', email = N'nguyenvanbuu11.2@gmail.com', mobile = N'0376890058', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV218';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV218', N'Nguyễn Văn Bữu', @dept_id, @sect_id, N'Thử nghiệm viên', N'nguyenvanbuu11.2@gmail.com', N'0376890058', N'Active', N'', GETUTCDATE());
END

-- Processing NV052: Trần Trọng Hồ
SET @branch_id = (SELECT branch_id FROM branch WHERE name_vi = N'Bạc liêu');
SET @dept_id = (SELECT department_id FROM department WHERE name_vi = N'Sắc ký' AND branch_id = @branch_id);
SET @sect_id = (SELECT section_id FROM section WHERE name_vi = N'Sắc ký' AND department_id = @dept_id);
IF EXISTS (SELECT 1 FROM [employee] WHERE employee_code = N'NV052')
BEGIN
    UPDATE [employee] SET 
        full_name = N'Trần Trọng Hồ', department_id = @dept_id, section_id = @sect_id, 
        title = N'Phó phòng', email = N'hotran11111@gmail.com', mobile = N'0386383191', 
        status = N'Active', notes = N'', updated_at = GETUTCDATE()
    WHERE employee_code = N'NV052';
END
ELSE
BEGIN
    INSERT INTO [employee] (employee_id, employee_code, full_name, department_id, section_id, title, email, mobile, status, notes, updated_at)
    VALUES (NEWID(), N'NV052', N'Trần Trọng Hồ', @dept_id, @sect_id, N'Phó phòng', N'hotran11111@gmail.com', N'0386383191', N'Active', N'', GETUTCDATE());
END

-- Part 2: Linking Managers (All employees must exist first)
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Quốc Toàn');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV177';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Thị Hồng Vân');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV286';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Thị Hồng Vân');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV261';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Thị Hồng Vân');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV292';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Thị Kim Anh');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV204';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Thị Kim Anh');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV216';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Thị Kim Anh');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV221';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Thị Kim Anh');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV225';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Thị Hồng Vân');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV038';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Huỳnh Thị Ngọc Kiều');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV283';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Huỳnh Thị Ngọc Kiều');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV193';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Huỳnh Thị Ngọc Kiều');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV227';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Huỳnh Thị Ngọc Kiều');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV196';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Quốc Toàn');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV049';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Huỳnh Thị Ngọc Kiều');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV116';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Huỳnh Thị Ngọc Kiều');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV144';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Huỳnh Thị Ngọc Kiều');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV137';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Huỳnh Thị Ngọc Kiều');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV108';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Huỳnh Thị Ngọc Kiều');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV194';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Huỳnh Thị Ngọc Kiều');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV197';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Lê Thị Diễm My');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV200';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Quốc Toàn');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV109';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Trần Thị Nam');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV158';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Trần Thị Nam');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV199';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Trần Thị Nam');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV250';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Trần Thị Nam');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV274';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Quốc Toàn');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV267';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Lê Bảo Ngọc');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV201';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Quốc Toàn');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV287';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phan Tấn Lập');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV153';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phan Tấn Lập');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV154';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phan Tấn Lập');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV155';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phan Tấn Lập');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV202';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phan Tấn Lập');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV220';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phan Tấn Lập');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV222';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phan Tấn Lập');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV262';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phan Tấn Lập');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV264';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phan Tấn Lập');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV270';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phan Tấn Lập');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV282';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phan Tấn Lập');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV284';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phan Tấn Lập');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV289';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phan Tấn Lập');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV291';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phạm Lê Tiến Khánh');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV012';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phạm Lê Tiến Khánh');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV275';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phạm Lê Tiến Khánh');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV258';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Quốc Toàn');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV223';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phạm Lê Tiến Khánh');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV171';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phạm Lê Tiến Khánh');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV180';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phạm Lê Tiến Khánh');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV206';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phạm Lê Tiến Khánh');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV247';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phạm Lê Tiến Khánh');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV273';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phạm Lê Tiến Khánh');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV243';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phạm Lê Tiến Khánh');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV280';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phạm Lê Tiến Khánh');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV276';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phạm Lê Tiến Khánh');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV288';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phạm Lê Tiến Khánh');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV290';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phạm Lê Tiến Khánh');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV294';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phạm Lê Tiến Khánh');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV293';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Quốc Toàn');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV248';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Quốc Toàn');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV005';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Trương Tô Hải Đăng');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV120';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Trương Tô Hải Đăng');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV152';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Trương Tô Hải Đăng');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV165';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Trương Tô Hải Đăng');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV217';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Trương Tô Hải Đăng');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV233';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Trương Tô Hải Đăng');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV271';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Quốc Toàn');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV189';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phan Thị Bích Phượng');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV084';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phan Thị Bích Phượng');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV183';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phan Thị Bích Phượng');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV281';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Quốc Toàn');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV192';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Quốc Toàn');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV241';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phan Thị Bích Phượng');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV161';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Phan Thị Bích Phượng');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV269';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Huỳnh Lương Anh Khoa');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV024';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Quốc Toàn');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV224';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Võ Thị Hồng Tươi');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV124';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Võ Thị Hồng Tươi');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV128';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Võ Thị Hồng Tươi');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV086';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Võ Thị Hồng Tươi');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV110';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Võ Thị Hồng Tươi');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV112';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Quốc Toàn');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV087';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Quốc Toàn');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV215';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Quốc Toàn');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV266';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Trương Tô Hải Đăng');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV246';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Quốc Toàn');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV244';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Tăng Thị Ngọc Hân');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV249';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Tăng Thị Ngọc Hân');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV143';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Tăng Thị Ngọc Hân');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV059';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Tăng Thị Ngọc Hân');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV133';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Tăng Thị Ngọc Hân');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV138';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Tăng Thị Ngọc Hân');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV208';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Quốc Toàn');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV253';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Đặng Phước Thi');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV022';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Đặng Phước Thi');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV212';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Đặng Phước Thi');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV207';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Đặng Phước Thi');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV285';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Nguyễn Quốc Toàn');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV053';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Đặng Phước Thi');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV028';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Đặng Phước Thi');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV057';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Đặng Phước Thi');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV064';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Đặng Phước Thi');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV142';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Đặng Phước Thi');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV218';
SET @manager_id = (SELECT TOP 1 employee_id FROM [employee] WHERE full_name = N'Đặng Phước Thi');
IF @manager_id IS NOT NULL
    UPDATE [employee] SET manager_id = @manager_id WHERE employee_code = N'NV052';

COMMIT;