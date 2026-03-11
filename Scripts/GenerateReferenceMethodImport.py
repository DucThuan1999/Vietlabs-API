# -*- coding: utf-8 -*-
"""Đọc csv/referencemethod.csv và sinh Scripts/ImportReferenceMethodFromCsv.sql"""
import os

CSV_PATH = os.path.join(os.path.dirname(__file__), '..', 'csv', 'referencemethod.csv')
OUT_PATH = os.path.join(os.path.dirname(__file__), 'ImportReferenceMethodFromCsv.sql')

def main():
    with open(CSV_PATH, 'r', encoding='utf-8-sig') as f:
        lines = [line.strip() for line in f if line.strip()]

    # Bỏ dòng chỉ là "0" nếu muốn (giữ lại cũng được)
    lines = [l for l in lines if l != '0' or True]  # giữ tất cả

    batch_size = 80
    values_list = []
    for i, line in enumerate(lines, start=1):
        esc = line.replace("'", "''")
        code = 'PP-' + str(i).zfill(3)
        values_list.append("(NEWID(), %d, N'%s', N'%s', N'%s', N'Active', SYSUTCDATETIME(), SYSUTCDATETIME(), NULL)" % (i, esc, esc, code))

    header = """-- =============================================
-- Import reference_method từ csv/referencemethod.csv
-- Mỗi dòng CSV -> 1 bản ghi: name_vi = name_en = nội dung dòng, reference_method_code = PP-001, PP-002, ...
-- =============================================

USE [VietLabs]
GO

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
)
VALUES
"""

    with open(OUT_PATH, 'w', encoding='utf-8') as out:
        out.write(header)
        for start in range(0, len(values_list), batch_size):
            batch = values_list[start:start + batch_size]
            out.write("    " + ",\n    ".join(batch))
            if start + batch_size < len(values_list):
                out.write(",\n")
            out.write("\n")
        out.write(";\n\nPRINT N'Đã import %d bản ghi từ referencemethod.csv.';\nGO\n" % len(values_list))

    print("Generated %s with %d rows" % (OUT_PATH, len(values_list)))

if __name__ == '__main__':
    main()
