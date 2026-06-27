USE PRN212_SEAL_DB;
GO

IF COL_LENGTH(N'dbo.Account', N'StudentCode') IS NULL
BEGIN
    ALTER TABLE dbo.Account
        ADD StudentCode VARCHAR(8) NULL;
END;
GO

IF EXISTS
(
    SELECT 1
    FROM dbo.Account
    WHERE (Role = 'Leader' AND
           (StudentCode IS NULL OR
            StudentCode COLLATE Latin1_General_100_BIN2
                NOT LIKE '[A-Z][A-Z][0-9][0-9][0-9][0-9][0-9][0-9]'))
       OR (Role = 'Judge' AND StudentCode IS NOT NULL)
)
BEGIN
    RAISERROR('Account data is invalid: Leader requires StudentCode in format AA123456; Judge requires NULL.', 16, 1);
END;
GO

IF OBJECT_ID(N'dbo.CK_Account_StudentCode', N'C') IS NULL
BEGIN
    ALTER TABLE dbo.Account WITH CHECK
        ADD CONSTRAINT CK_Account_StudentCode
        CHECK
        (
            (Role = 'Leader'
                AND StudentCode IS NOT NULL
                AND StudentCode COLLATE Latin1_General_100_BIN2
                    LIKE '[A-Z][A-Z][0-9][0-9][0-9][0-9][0-9][0-9]')
            OR
            (Role = 'Judge' AND StudentCode IS NULL)
        );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Account_StudentCode'
      AND object_id = OBJECT_ID(N'dbo.Account')
)
BEGIN
    CREATE UNIQUE INDEX UX_Account_StudentCode
        ON dbo.Account(StudentCode)
        WHERE StudentCode IS NOT NULL;
END;
GO
