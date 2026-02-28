-- Kiểm tra đã có data Nhà thầu phụ (CustomerType) trong bảng Clients chưa
-- Chạy script này trên database VietLabs

USE VietLabs;
GO

-- 1) Liệt kê tất cả giá trị CustomerType đang có trong DB (để biết có "Nhà thầu phụ" hay không)
SELECT CustomerType, COUNT(*) AS SoLuong
FROM Clients
WHERE CustomerType IS NOT NULL AND LTRIM(RTRIM(CustomerType)) <> N''
GROUP BY CustomerType
ORDER BY SoLuong DESC;

-- 2) Đếm số khách hàng có CustomerType = N'Nhà thầu phụ' (so sánh không phân biệt dấu cách đầu/cuối)
SELECT COUNT(*) AS SoKhachHangNhaThauPhu
FROM Clients
WHERE LTRIM(RTRIM(CustomerType)) = N'Nhà thầu phụ';

-- 3) Nếu có, xem chi tiết các bản ghi đó
SELECT ClientId, CompanyName, CustomerType, InternalCode, TaxCode, CreatedDate
FROM Clients
WHERE LTRIM(RTRIM(CustomerType)) = N'Nhà thầu phụ'
ORDER BY CreatedDate DESC;
