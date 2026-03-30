-- Script manual to add Section column to Employee table
-- Required because EF migrations are blocked by an unrelated mapping error

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[employee]') AND name = N'section_id')
BEGIN
    ALTER TABLE [employee] ADD [section_id] uniqueidentifier NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'i_x_employee_section_id' AND object_id = OBJECT_ID(N'[employee]'))
BEGIN
    CREATE INDEX [i_x_employee_section_id] ON [employee] ([section_id]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'f_k_employee_section_section_id' AND parent_object_id = OBJECT_ID(N'[employee]'))
BEGIN
    ALTER TABLE [employee] ADD CONSTRAINT [f_k_employee_section_section_id] FOREIGN KEY ([section_id]) REFERENCES [section] ([section_id]) ON DELETE NO ACTION;
END
GO
