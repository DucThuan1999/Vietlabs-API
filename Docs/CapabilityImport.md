# Import năng lực từ Capability.xlsx

Tài liệu này mô tả **thứ tự dữ liệu cần có trước** và **quy tắc map** sang `DepartmentAnalysisCapability` / `DepartmentAnalysisCapabilityDesignation`. Dùng khi bạn nhập các master data khác trước, tới bước năng lực theo chi nhánh mới áp dụng phần cuối.

**Mã hằng trong code:** `VietLab.Data.CapabilityImportRules` và seed layer 0: `Layer0ReferenceDataSeeder` (bật qua `Seed:Layer0:Enabled` trong cấu hình).

---

## 1. Nguồn file


| Sheet        | Nội dung chính                                                                                        |
| ------------ | ----------------------------------------------------------------------------------------------------- |
| **Vietlabs** | Chỉ tiêu + năng lực HCM/CT/BL/CM (NĐ 107) + chỉ định (ISO, cục…) theo HCM; CT có thêm ISO.            |
| **NTP**      | Chỉ tiêu / giá NTP / đối tác — xử lý sau, không nằm trong phạm vi bảng dưới đây trừ khi bổ sung spec. |


**Map chỉ tiêu / nhóm chỉ tiêu (khi đọc full dòng từ Excel):** cột **Giá nhóm chuẩn_new** trên file → cột DB `analysis_group.whole_group_standard_price` (`WholeGroupStandardPrice`). Tên hằng trong code: `CapabilityImportRules.AnalysisGroupWholeGroupStandardColumnVi`.

---

## 2. Dữ liệu cần có *trước* khi import năng lực (thứ tự gợi ý)

Import năng lực phòng ban **phụ thuộc** đã có `analysis_item` và lưới tổ chức. Thứ tự thực tế nên là:

1. **Chi nhánh** (`branch`) — đã đủ trên hệ thống; **không** tạo thêm trong seed layer 0 import.
2. **Phòng ban** (`department`) — gắn `branch_id`; với cột *Bộ phận phụ trách (Kỹ thuật)* (Vi sinh, Sắc ký, Quang phổ, Cổ điển) cần phòng tương ứng từng chi nhánh (ví dụ mã `DEP-HCM-VSINH`, … sau `Layer0ReferenceDataSeeder`).
3. **Danh mục chỉ định** (`designation`) — seed: `ISO`, `CUC_BVTV`, `BO_CONG_THUONG`, `NAFI`, `CUC_CHAN_NUOI` (tên hiển thị: ISO, Cục BVTV, Bộ Công thương, Nafi, Cục chăn nuôi).
4. **Chỉ tiêu phân tích** (`analysis_item`) — đã có `analysis_item_id` / mã `CT-xxxx` khớp cột *Mã chỉ tiêu* trên file.
5. *(Tuỳ pipeline)* **Nhóm nền mẫu, nền mẫu, nhóm chỉ tiêu, phương pháp, tiêu chuẩn, ĐVT, TAT, …** — nếu import full dòng chỉ tiêu từ Excel thì phải xử lý các layer đó *trước* hoặc song song theo [kế hoạch layer đã thống nhất].

**Khi nào được tạo bản ghi năng lực:** đã xác định được bộ ba `(department_id, branch_id, analysis_item_id)` đúng nghiệp vụ cho từng ô năng lực (HCM / CT / BL / CM). Script `import_department_capability_vietlabs_xlsx.py`: **kỹ thuật** trên UI lọc theo chỉ tiêu, load từ `analysis_item.laboratory_technique_id` (bắt buộc có FK); **không** tự điền `notes` (Ghi chú) trên DAC. **Phòng ban** theo `department.name_vi` và cột **Bộ phận phụ trách**; **chi nhánh** theo khối năng lực (HCM→SG, CT→CT, BL→BL, CM→CM).

---

## 3. Bảng đích chính (năng lực)


| Bảng                                         | Ý nghĩa                                                                                          |
| -------------------------------------------- | ------------------------------------------------------------------------------------------------ |
| `department_analysis_capability`             | Một “năng lực” của một phòng trên một chi nhánh cho một chỉ tiêu: `Nd107`, `Nd107ExpiredDate`, … |
| `department_analysis_capability_designation` | Chỉ định gắn với **một** bản ghi năng lực trên: `designation_id`, `expired_date`.                |


**Lưu ý:** `DepartmentAnalysisCapability.BranchId` trong model là **chuỗi** (`nvarchar`) — cần thống nhất với cách lưu thực tế (ví dụ GUID chi nhánh dạng string hoặc mã site); import phải dùng **cùng format** với dữ liệu hiện có.

---

## 4. Map cột Excel → HCM (`branch_code` thường **`SG`** — đối chiếu DB; seed demo có thể là `BR-002`)


| Cột / nhóm trên file  | Ghi vào                                                                                 | Quy tắc                                                                                                                                                              |
| --------------------- | --------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Năng lực HCM (NĐ 107) | `DepartmentAnalysisCapability`                                                          | Ô `**Chưa có`** → `nd_107 = false`, `nd_107_expired_date = null`. Có **ngày** → `nd_107 = true`, `nd_107_expired_date` = ngày đó.                                    |
| ISO (a)               | `DepartmentAnalysisCapabilityDesignation` + chỉ định **ISO** (`designation_code = ISO`) | Giá trị là **ngày** → `expired_date` = ngày. Giá trị `Chưa có` / `x` / khác → **quy tắc chi tiết do bước import chốt** (có tạo dòng hay không, `expired_date` null). |
| Cục BVTV (b)          | Tương tự                                                                                | `designation_code = CUC_BVTV`, tên: Cục BVTV.                                                                                                                        |
| Bộ Công thương (e)    | Tương tự                                                                                | `BO_CONG_THUONG`.                                                                                                                                                    |
| NAFI (d)              | Tương tự                                                                                | `NAFI`, tên: Nafi.                                                                                                                                                   |
| Cục Chăn nuôi (c)     | Tương tự                                                                                | `CUC_CHAN_NUOI`, tên: Cục chăn nuôi.                                                                                                                                 |


Mỗi cột chỉ định tạo bản ghi **trên đúng** `department_analysis_capability` của **chi nhánh HCM** (cùng chỉ tiêu + phòng ban đã chọn).

---

## 5. Map cột Excel → Cần Thơ (`branch_code` thường **`CT`**; legacy `BR-004`)


| Cột                  | Ghi vào                                   | Quy tắc                                                                       |
| -------------------- | ----------------------------------------- | ----------------------------------------------------------------------------- |
| Năng lực CT (NĐ 107) | `DepartmentAnalysisCapability`            | Giống HCM: `Chưa có` → `nd_107 = false`; có ngày → `nd_107 = true` + ngày.    |
| ISO (a)              | `DepartmentAnalysisCapabilityDesignation` | Chỉ định **ISO**, `expired_date` theo ô (cùng lưu ý `Chưa có` / `x` như HCM). |


---

## 6. Bạc Liêu & Cà Mau


| Site | Cột                  | Ghi vào                                               |
| ---- | -------------------- | ----------------------------------------------------- |
| BL   | Năng lực BL (NĐ 107) | `DepartmentAnalysisCapability` (cùng quy tắc NĐ 107). |
| CM   | Năng lực CM (NĐ 107) | `DepartmentAnalysisCapability` (cùng quy tắc NĐ 107). |


Theo spec hiện tại **không** có thêm cột chỉ định riêng cho BL/CM trên sheet (chỉ NĐ 107).

**Mã chi nhánh BL/CM:** trên DB thường là **`BL`** / **`CM`** (legacy `BR-005` / `BR-006` trong seed demo).

---

## 7. Ví dụ input / output (tóm tắt)

**Input (một dòng, phần HCM):** ô NĐ 107 = `13/06/2026`, ISO = `29/09/2029`, Cục BVTV = `Chưa có`.

**Output:**

- Một dòng `department_analysis_capability`: `nd_107 = true`, `nd_107_expired_date = 2026-06-13`.
- Một dòng `department_analysis_capability_designation`: designation **ISO**, `expired_date = 2029-09-29`.
- **Bỏ qua** ô Cục BVTV = `Chưa có`: không INSERT/UPDATE designation; **không xóa** designation đã có trước đó.

**Script `import_department_capability_vietlabs_xlsx.py`:** mỗi dòng = một chỉ tiêu, nhiều DAC theo cột. Ô NĐ 107 / chỉ định chỉ ghi DB khi giá trị là **ngày**; `Chưa có` / rỗng / `x` → không thay đổi. DAC mới chỉ **INSERT** khi ô là ngày và chưa có bản ghi `(department, branch_id, analysis_item_id)`.

---

## 8. Việc cần chốt trước khi code import

1. ~~Ô không phải ngày~~ — đã chốt: **bỏ qua**, không xóa designation cũ.
2. Format chính xác của `**DepartmentAnalysisCapability.branch_id**` so với bảng `branch`.
3. Mã `branch_code` thực tế: **`SG`** (HCM), **`CT`**, **`BL`**, **`CM`** — đã khớp import / `CapabilityImportRules`.

---

## 9. Tham chiếu code


| Thành phần                                             | Đường dẫn                           |
| ------------------------------------------------------ | ----------------------------------- |
| Hằng mã chỉ định + gợi ý mã chi nhánh                  | `Data/CapabilityImportRules.cs`     |
| Seed danh mục layer 0 (designation, phòng kỹ thuật, …) | `Data/Layer0ReferenceDataSeeder.cs` |


