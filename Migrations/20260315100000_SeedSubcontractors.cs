using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VietLab.Migrations
{
    /// <summary>
    /// Seed danh sách nhà thầu phụ từ DS NTP.csv.
    /// MERGE theo code: chỉ INSERT khi chưa tồn tại.
    /// </summary>
    public partial class SeedSubcontractors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = @"
MERGE subcontractor AS t
USING (
  SELECT N'CASE CT' AS code, N'CHI NHÁNH CẦN THƠ - TRUNG TÂM DỊCH VỤ PHÂN TÍCH THÍ NGHIỆM THÀNH PHỐ HỒ CHÍ MINH' AS name, N' F2-67, F2-68 đường Nguyễn Thị Sáu, Phường Hưng Phú, TP Cần Thơ, Việt Nam.' AS address, N'0319238568-001' AS tax_code, N'128000151681' AS bank_account_number, N'NH TMCP Công thương Việt Nam - CN TPHCM' AS bank_name, N'Yes' AS contract_status, N'AfterInvoice' AS payment_cycle, 30 AS payment_days, N'Đang đợi kí lại' AS notes
  UNION ALL SELECT N'CASE HCM', N'CHI NHÁNH HCM - TRUNG TÂM DỊCH VỤ PHÂN TÍCH THÍ NGHIỆM THÀNH PHỐ HỒ CHÍ MINH', N'02 Nguyễn Văn Thủ, Phường Tân Định, Thành phố Hồ Chí Minh, Việt Nam', N'0302554935', N'116000004970', N'Vietinbank - Chi nhánh TP. HCM', N'Yes', N'AfterInvoice', 30, NULL
  UNION ALL SELECT N'CATECH', N'CÔNG TY CỔ PHẦN PHÁT TRIỂN CÔNG NGHỆ CATECH VIỆT NAM', N'Số 45 đường 3/2, P. Xuân Khánh, Q. Ninh Kiều, TP. Cần Thơ', N'1800271434', N'111000014122', N'Ngân hàng TMCP Công Thương Việt Nam - Chi nhánh Cần Thơ', N'No', NULL, NULL, N'Không hợp đồng, thanh toán ngay sau khi báo thanh toán'
  UNION ALL SELECT N'CCIC', N'CÔNG TY TNHH GIÁM ĐỊNH VÀ CHỨNG NHẬN CCIC VIỆT NAM CHI NHÁNH THÀNH PHỐ HỒ CHÍ MINH', N'Nhà xưởng cao tầng Long Hậu, Lô L2, Khu công nghiệp Long Hậu, Cần Giuộc, Long An', N'4101399525-001', N'19040128624018', N'Techcombank- Chi nhánh Tân Qui', N'No', NULL, NULL, N'Không hợp đồng, thanh toán ngay sau khi báo thanh toán'
  UNION ALL SELECT N'CHẤN NAM', N'CÔNG TY CP DỊCH VỤ KHOA HỌC CÔNG NGHỆ CHẤN NAM', N'80-82-84 Thăng Long, Phường Tân Sơn Nhất, Thành phố Hồ Chí Minh, Việt Nam', N'0311927735', N'71000717137', N'Ngân hàng Vietcombank HCM', N'No', NULL, NULL, N'Không hợp đồng, thanh toán ngay sau khi báo thanh toán'
  UNION ALL SELECT N'CNT', N'TRUNG TÂM HẠT NHÂN TP.HỒ CHÍ MINH', N'217 Nguyễn Trãi, Phường Cầu Ông Lãnh, Thành phố Hồ Chí Minh, Việt Nam.', N'0301514584', N'0071000077779', N'Ngân hàng TMCP Ngoại Thương Việt Nam - Chi nhánh TP. HCM', N'No', NULL, NULL, N'Không hợp đồng, thanh toán ngay sau khi báo thanh toán'
  UNION ALL SELECT N'CONTROL UNION', N'CÔNG TY TNHH CONTROL UNION VIỆT NAM', N'Số 61-63 Đường Đặng Như Mai, Phường Cát Lái, Thành phố Hồ Chí Minh, Việt Nam.', N'0302854488', N'007 100 0891902', N'Ngân hàng TMCP Ngoại Thương Việt Nam – Chi nhánh Hồ Chí Minh', N'Yes', N'AfterInvoice', 30, N'Hợp đồng tự động gia hạn từng năm'
  UNION ALL SELECT N'EUROFINS CT', N'CÔNG TY TNHH EUROFINS SẮC KÝ HẢI ĐĂNG - chi nhánh Cần Thơ', N'Phòng 319 , Vườn Ươm Công Nghệ Công Nghiệp VN-HQ , KCN Trà Nóc , Phước Thới , Ô Môn, TP. Cần Thơ', N'0311526885', N'071000695552', N'Ngân hàng TMCP Ngoại Thương Việt Nam (Vietcombank) Chi nhánh TP. HCM', N'Yes', N'AfterInvoice', 30, N'Đã liên hệ kí lại hợp đồng'
  UNION ALL SELECT N'EUROFINS HCM', N'CÔNG TY TNHH EUROFINS SẮC KÝ HẢI ĐĂNG - chi nhánh Hồ Chí Minh', N'Lô E2b-3, Đường D6, Khu Công Nghệ Cao, Phường Tăng Nhơn Phú, Thành phố Hồ Chí Minh, Việt Nam', N'0311526885', N'071000695552', N'Ngân hàng TMCP Ngoại Thương Việt Nam (Vietcombank) Chi nhánh TP. HCM', N'No', NULL, NULL, N'Không hợp đồng, thanh toán ngay sau khi báo thanh toán'
  UNION ALL SELECT N'HẢI ÂU', N'CÔNG TY CỔ PHẦN DỊCH VỤ TƯ VẤN MÔI TRƯỜNG HẢI ÂU', N'3, Đường Tân Thới Nhất 20, Khu Phố 4, Phường Đông Hưng Thuận, Thành phố Hồ Chí Minh, Việt Nam', N'0309387095', N'140214851021608', N'Ngân hàng xuất nhập khẩu Việt Nam ( EIB) Quận 4, TP. HCM', N'Yes', N'AfterInvoice', 15, NULL
  UNION ALL SELECT N'HNDL', N'TRUNG TÂM HẠT NHÂN ĐÀ LẠT', N'01 Nguyên Tử Lực, Phường Lâm Viên - Đà Lạt, tỉnh Lâm Đồng, Việt Nam', N'5800197629', N'1365686879', N'Ngân hàng TMCP Ngoại thương Việt Nam Chi nhánh Lâm Đồng', N'Yes', N'AfterInvoice', 30, N'Đã kí mới 2026, đang đợi gửi lại'
  UNION ALL SELECT N'HOÀN VŨ', N'CÔNG TY TNHH MỘT THÀNH VIÊN KHOA HỌC CÔNG NGHỆ HOÀN VŨ', N'169B Thích Quảng Đức, Phường Đức Nhuận, TP. HCM', N'0304932124', N'47870519', N'Tại Ngân hàng ACB - Chi Nhánh Tân Phú', N'No', NULL, NULL, N'Không hợp đồng, thanh toán ngay sau khi báo thanh toán'
  UNION ALL SELECT N'INTERTEK', N'CÔNG TY TNHH INTERTEK VIỆT NAM-CHI NHÁNH CẦN THƠ', N'M10, 11, 12, 13 Khu đô thị Nam Sông Cần Thơ, Khu vực Thạnh Thuận, Phường Hưng Phú, TP. Cần Thơ, Việt Nam', N'0100773892-002', N'0111000808520', N'Ngân Hàng Ngoại Thương Cần Thơ', N'Yes', N'AfterInvoice', 30, N'Tạm ngưng gửi mẫu'
  UNION ALL SELECT N'IRDOP', N'VIỆN NGHIÊN CỨU VÀ PHÁT TRIỂN SẢN PHẨM THIÊN NHIÊN', N'176 Phùng Khoang, Phường Đại Mỗ, Thành phố Hà Nội, Việt Nam', N'0107149919', N'16356688', N'Ngân hàng Á Châu-PGD Trung Văn', N'No', NULL, NULL, N'Không hợp đồng, thanh toán ngay sau khi báo thanh toán'
  UNION ALL SELECT N'NAFI 4', N'TRUNG TÂM CHẤT LƯỢNG, CHẾ BIẾN VÀ PHÁT TRIỂN THỊ TRƯỜNG VÙNG 4', N'271 Tô Ngọc Vân, Phường Hiệp Bình, Thành phố Hồ Chí Minh, Việt Nam', N'0301464728', N'117000004257', N'Ngân Hàng TMCP Công Thương VN-Chi nhánh 5', N'Yes', N'AfterInvoice', 7, NULL
  UNION ALL SELECT N'NAFI 6', N'TRUNG TÂM CHẤT LƯỢNG, CHẾ BIẾN VÀ PHÁT TRIỂN THỊ TRƯỜNG VÙNG 6', N'Số 386C đường Cách Mạng Tháng Tám, phường Bình Thủy, thành phố Cần Thơ, Việt Nam', N'1800329469', N'0111000325999', N'Ngân hàng Vietcombank Cần Thơ', N'No', NULL, NULL, N'Không hợp đồng, thanh toán ngay sau khi báo thanh toán'
  UNION ALL SELECT N'NAM KHOA', N'CÔNG TY CỔ PHẦN DỊCH VỤ VÀ THƯƠNG MẠI NAM KHOA BIOTEK', N'793/58 Trần Xuân Soạn, Phường Tân Hưng, Thành phố Hồ Chí Minh, Việt Nam', N'0301888910', N'1400033839', N'Ngân Hàng TMCP Đầu Tư & Phát Triển Việt Nam - CN Sài Gòn', N'No', NULL, NULL, N'Không hợp đồng, thanh toán ngay sau khi báo thanh toán'
  UNION ALL SELECT N'NAVITEK', N'CÔNG TY CỔ PHẦN KIỂM NGHIỆM THỰC PHẨM VÀ MÔI TRƯỜNG NAVITEK', N'Lô II-1, đường số 1, nhóm CN2, khu công nghiệp Tân Bình, Phường Tây Thạnh, Thành phố Hồ Chí Minh, Việt Nam', N'0316383817', N'00001572772', N'Ngân hàng TMCP Tiên Phong - CN Gia Định', N'Yes', N'AfterInvoice', 15, NULL
  UNION ALL SELECT N'NIFC', N'VIỆN KIỂM NGHIỆM AN TOÀN VỆ SINH THỰC PHẨM QUỐC GIA', N'65 Phạm Thận Duật, Phường Mai Dịch, Quận Cầu Giấy, Thành phố Hà Nội', N'0103991698', N'12410001239696', N'Ngân hàng Đầu tư và Phát triển Việt Nam (BIDV)-CN Hoàn Kiếm', N'No', NULL, NULL, N'Không hợp đồng, thanh toán ngay sau khi báo thanh toán'
  UNION ALL SELECT N'PHƯƠNG NAM', N'CÔNG TY CỔ PHẦN HUẤN LUYỆN VÀ KIỂM ĐỊNH KỸ THUẬT PHƯƠNG NAM', N'Số 15 Đoàn Thị Điểm, Phường Vũng Tàu, Thành phố Hồ Chí Minh, Việt Nam', N'3501822119', N'2751100011002', N'NH TMCP Quân Đội, chi nhánh Tây Sài gòn', N'No', NULL, NULL, N'Không hợp đồng, thanh toán ngay sau khi báo thanh toán'
  UNION ALL SELECT N'QUATEST 1', N'TRUNG TÂM KỸ THUẬT TIÊU CHUẨN ĐO LƯỜNG CHẤT LƯỢNG 1', N'Số 8 Hoàng Quốc Việt, Phường Nghĩa Đô, Quận Cầu Giấy, Thành phố Hà Nội', N'0100111602', N'115000002973', N'NH TMCP Công Thương Việt Nam, CN Nam Thăng Long', N'No', NULL, NULL, N'Không hợp đồng, thanh toán ngay sau khi báo thanh toán'
  UNION ALL SELECT N'QUATEST 3', N'TRUNG TÂM KỸ THUẬT TIÊU CHUẨN ĐO LƯỜNG CHẤT LƯỢNG 3', N'49 Pasteur, phường Sài Gòn, TP Hồ Chí Minh', N'0301281040', N'118000004544', N'Ngân Hàng TMCP Công thương Việt Nam - CN. Thành phố Hồ Chí Minh', N'No', NULL, NULL, N'Không hợp đồng, thanh toán ngay sau khi báo thanh toán'
  UNION ALL SELECT N'RIFISH', N'PHÂN VIỆN NGHIÊN CỨU THỦY SẢN NAM SÔNG HẬU', N'Số 91 Phan Ngọc Hiển, Phường Tân Thành, Tỉnh Cà Mau', N'2000266444', N'7500201002693', N'Ngân Hàng NN&PT Nông Thôn Việt Nam chi nhánh Cà Mau', N'No', NULL, NULL, N'Không hợp đồng, thanh toán ngay sau khi báo thanh toán'
  UNION ALL SELECT N'SAIGONCERT', N'CÔNG TY CỔ PHẦN CHỨNG NHẬN VÀ GIÁM ĐỊNH SAIGONCERT', N'139 Đường Man Thiện , Phường Tăng Nhơn Phú, Thành phố Hồ Chí Minh, Việt Nam', N'0314196466', N'115002619665', N'Ngân hàng VietinBank – Chi nhánh Đông Sài Gòn', N'Yes', N'AfterInvoice', 30, NULL
  UNION ALL SELECT N'SKTP', N'CÔNG TY CỔ PHẦN KHOA HỌC CÔNG NGHỆ SẮC KÝ TIÊN PHONG', N'114 Trương Văn Bang, Phường Cát Lái, Thành phố Hồ Chí Minh, Việt Nam', N'0316445414', N'15000168', N'Ngân hàng Á Châu (ACB) - Chi nhánh Đông Sài Gòn', N'Yes', N'AfterInvoice', 30, N'Tạm ngưng gửi mẫu'
  UNION ALL SELECT N'TRẠM 3', N'TRUNG TÂM CHẨN ĐOÁN VÀ XÉT NGHIỆM THÚ Y TRUNG ƯƠNG II', N'521/1 Hoàng Văn Thụ, Phường Tân Sơn Nhất, Thành phố Hồ Chí Minh, Việt Nam', N'0319038054', N'113637798868', N'Ngân hàng TMCP Công thương Việt Nam Chi nhánh 12', N'No', NULL, NULL, N'Không hợp đồng, thanh toán ngay sau khi báo thanh toán'
  UNION ALL SELECT N'VIỆT TÍN', N'CÔNG TY TNHH PHÂN TÍCH KIỂM NGHIỆM VIỆT TÍN', N'42 Trần Quang Khải, Phường Tân Định, Quận 1 , TP Hồ Chí Minh', N'0314042018', N'060143579037', N'Ngân hàng TMCP Sài Gòn Thương Tín (Sacombank)', N'Yes', N'AfterInvoice', 30, NULL
  UNION ALL SELECT N'VINACONTROL', N'TRUNG TÂM PHÂN TÍCH VÀ THỬ NGHIỆM 2 - VINACONTROL', N'Lô U18A Đường Số 22 KCX Tân Thuận, Phường Tân Thuận, Thành Phố Hồ Chí Minh, Việt Nam', N'0311506751-004', N'181003403621', N'VIETCOMBANK CHI NHÁNH NAM SÀI GÒN', N'No', NULL, NULL, N'Không hợp đồng, thanh toán ngay sau khi báo thanh toán'
  UNION ALL SELECT N'YTCC', N'VIỆN Y TẾ CÔNG CỘNG THÀNH PHỐ HỒ CHÍ MINH', N'159 Hưng Phú, Phường Chánh Hưng, Thành phố Hồ Chí Minh, Việt Nam', N'0301260925', N'113000007490', N'Ngân hàng TMCP Công Thương Việt Nam - CN 8 TPHCM', N'No', NULL, NULL, N'Không hợp đồng, thanh toán ngay sau khi báo thanh toán'
) AS src ON t.code = LTRIM(RTRIM(src.code))
WHEN NOT MATCHED BY TARGET THEN
  INSERT (subcontractor_id, code, name, address, tax_code, bank_account_number, bank_name, contract_status, payment_cycle, payment_days, notes, status, created_at)
  VALUES (NEWID(), LTRIM(RTRIM(src.code)), src.name, src.address, src.tax_code, src.bank_account_number, src.bank_name, src.contract_status, src.payment_cycle, src.payment_days, src.notes, N'Active', GETUTCDATE());
";
            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Không xóa dữ liệu khi Down để tránh ảnh hưởng dữ liệu đã dùng
        }
    }
}
