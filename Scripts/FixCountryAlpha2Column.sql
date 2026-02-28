-- Script to fix alpha_2 column in country table
-- This script ensures the alpha_2 column has the correct length (2 characters)

USE VietLabs;
GO

-- Check current column definition
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'country' 
    AND COLUMN_NAME = 'alpha_2';
GO

-- Alter the column to ensure it has max length of 2
-- This will work even if the column already has the correct length
ALTER TABLE [dbo].[country]
ALTER COLUMN [alpha_2] NVARCHAR(2) NULL;
GO

-- Verify the change
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'country' 
    AND COLUMN_NAME = 'alpha_2';
GO

PRINT 'Column alpha_2 has been updated to NVARCHAR(2)';
GO

