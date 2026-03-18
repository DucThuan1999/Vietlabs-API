using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <summary>
    /// Seed loại thiết bị: equipment_type_code theo TB-001, TB-002, ...
    /// Nếu đã tồn tại name_vi thì UPDATE (mã + status), chưa có thì INSERT.
    /// </summary>
    public partial class SeedEquipmentTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = @"
-- Mã TB-xxx tiếp theo số lớn nhất đã có; nếu tồn tại name_vi thì UPDATE, không thì INSERT
;WITH base AS (
  SELECT ISNULL(MAX(TRY_CAST(SUBSTRING(equipment_type_code, 5, 10) AS INT)), 0) AS mx
  FROM equipment_type WHERE equipment_type_code LIKE 'TB-[0-9]%'
),
seed_data AS (
  SELECT 1 AS seq, N'GC-MS/MS' AS name_vi UNION ALL SELECT 2, N'HPLC-PDA' UNION ALL SELECT 3, N'ICP-MS' UNION ALL SELECT 4, N'F-AAS' UNION ALL SELECT 5, N'GF-AAS' UNION ALL
  SELECT 6, N'Định tính' UNION ALL SELECT 7, N'UV-Vis' UNION ALL SELECT 8, N'Trọng lượng' UNION ALL SELECT 9, N'Realtime RT-PCR' UNION ALL SELECT 10, N'LC-MS/MS' UNION ALL
  SELECT 11, N'Chuẩn độ' UNION ALL SELECT 12, N'Khối lượng' UNION ALL SELECT 13, N'HG-AAS' UNION ALL SELECT 14, N'Phân lập' UNION ALL SELECT 15, N'MPN' UNION ALL
  SELECT 16, N'Màng lọc' UNION ALL SELECT 17, N'Kiểm tra hình thái vi khuẩn' UNION ALL SELECT 18, N'Realtime PCR' UNION ALL SELECT 19, N'Đếm khuẩn lạc' UNION ALL SELECT 20, N'Sấy' UNION ALL
  SELECT 21, N'Dùng chất chiết' UNION ALL SELECT 22, N'Kháng vi khuẩn' UNION ALL SELECT 23, N'Elisa' UNION ALL SELECT 24, N'GC-FID' UNION ALL SELECT 25, N'HPLC-PAD' UNION ALL
  SELECT 26, N'Biochrom' UNION ALL SELECT 27, N'HPLC-FLD' UNION ALL SELECT 28, N'HPLC' UNION ALL SELECT 29, N'HPLC-UV' UNION ALL SELECT 30, N'Kjeldahl' UNION ALL
  SELECT 31, N'PCR' UNION ALL SELECT 32, N'CV-AAS' UNION ALL SELECT 33, N'Chuẩn độ EDTA' UNION ALL SELECT 34, N'Quang kế ngọn lửa' UNION ALL SELECT 35, N'Thể tích' UNION ALL
  SELECT 36, N'FES' UNION ALL SELECT 37, N'Tính toán' UNION ALL SELECT 38, N'Phát hiện' UNION ALL SELECT 39, N'Nung' UNION ALL SELECT 40, N'C/N' UNION ALL
  SELECT 41, N'Cảm quan' UNION ALL SELECT 42, N'Cấy trang bề mặt' UNION ALL SELECT 43, N'RAPID''E. coli 2 Agar' UNION ALL SELECT 44, N'Chiết Soxhlet' UNION ALL SELECT 45, N'RAPID'' Samonella' UNION ALL
  SELECT 46, N'Kỹ thuật khoanh giấy khuếch tán'
),
src AS (
  SELECT name_vi, 'TB-' + RIGHT('000' + CAST((SELECT mx FROM base) + seq AS VARCHAR(3)), 3) AS code FROM seed_data
)
MERGE equipment_type AS t
USING src ON LTRIM(RTRIM(ISNULL(t.name_vi,''))) = LTRIM(RTRIM(src.name_vi))
WHEN MATCHED THEN UPDATE SET t.equipment_type_code = src.code, t.status = N'Active'
WHEN NOT MATCHED BY TARGET THEN INSERT (equipment_type_id, equipment_type_code, name_vi, name_en, status) VALUES (NEWID(), src.code, src.name_vi, NULL, N'Active');
";
            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Không xóa dữ liệu khi Down để tránh mất dữ liệu đã dùng ở analysis_item
        }
    }
}
