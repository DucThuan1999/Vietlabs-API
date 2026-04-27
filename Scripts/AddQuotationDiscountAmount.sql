IF COL_LENGTH('quotation', 'discount_amount') IS NULL
BEGIN
    ALTER TABLE quotation
    ADD discount_amount DECIMAL(18, 2) NULL;
END;
