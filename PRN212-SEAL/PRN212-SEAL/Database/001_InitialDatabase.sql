IF DB_ID(N'PRN212_SEAL_DB') IS NULL
BEGIN
    EXEC(N'CREATE DATABASE PRN212_SEAL_DB');
END;
GO

USE PRN212_SEAL_DB;
GO

-- 1. Account
IF OBJECT_ID(N'dbo.Account', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Account
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL,
        PasswordHash VARCHAR(255) NOT NULL,
        FullName NVARCHAR(100) NOT NULL,
        Email VARCHAR(100) NOT NULL,
        Role VARCHAR(20) NOT NULL,
        CreatedAt DATETIME2 NOT NULL
            CONSTRAINT DF_Account_CreatedAt DEFAULT SYSDATETIME(),

        CONSTRAINT UQ_Account_Username UNIQUE (Username),
        CONSTRAINT UQ_Account_Email UNIQUE (Email),
        CONSTRAINT CK_Account_Role
            CHECK (Role IN ('Leader', 'Judge'))
    );
END;
GO

-- 2. Team
IF OBJECT_ID(N'dbo.Team', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Team
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TeamName NVARCHAR(100) NOT NULL,
        LeaderId INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL
            CONSTRAINT DF_Team_CreatedAt DEFAULT SYSDATETIME(),

        CONSTRAINT UQ_Team_TeamName UNIQUE (TeamName),
        CONSTRAINT UQ_Team_LeaderId UNIQUE (LeaderId),
        CONSTRAINT FK_Team_Account_Leader
            FOREIGN KEY (LeaderId) REFERENCES dbo.Account(Id)
    );
END;
GO

-- 3. TeamMember
IF OBJECT_ID(N'dbo.TeamMember', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TeamMember
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TeamId INT NOT NULL,
        FullName NVARCHAR(100) NOT NULL,
        StudentCode VARCHAR(20) NOT NULL,
        IsLeader BIT NOT NULL
            CONSTRAINT DF_TeamMember_IsLeader DEFAULT 0,

        CONSTRAINT UQ_TeamMember_StudentCode UNIQUE (StudentCode),
        CONSTRAINT FK_TeamMember_Team
            FOREIGN KEY (TeamId) REFERENCES dbo.Team(Id)
    );
END;
GO

-- 4. Challenge
IF OBJECT_ID(N'dbo.Challenge', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Challenge
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL,
        Description NVARCHAR(MAX) NOT NULL,
        Deadline DATETIME2 NOT NULL,
        CreatedAt DATETIME2 NOT NULL
            CONSTRAINT DF_Challenge_CreatedAt DEFAULT SYSDATETIME()
    );
END;
GO

-- 5. Submission
IF OBJECT_ID(N'dbo.Submission', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Submission
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        TeamId INT NOT NULL,
        ChallengeId INT NOT NULL,
        GithubUrl NVARCHAR(500) NOT NULL,
        Description NVARCHAR(1000) NULL,
        SubmittedAt DATETIME2 NOT NULL
            CONSTRAINT DF_Submission_SubmittedAt DEFAULT SYSDATETIME(),
        UpdatedAt DATETIME2 NULL,

        CONSTRAINT UQ_Submission_Team_Challenge UNIQUE (TeamId, ChallengeId),
        CONSTRAINT FK_Submission_Team
            FOREIGN KEY (TeamId) REFERENCES dbo.Team(Id),
        CONSTRAINT FK_Submission_Challenge
            FOREIGN KEY (ChallengeId) REFERENCES dbo.Challenge(Id)
    );
END;
GO

-- 6. Score
IF OBJECT_ID(N'dbo.Score', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Score
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        SubmissionId INT NOT NULL,
        JudgeId INT NOT NULL,
        ScoreValue DECIMAL(5,2) NOT NULL,
        Comment NVARCHAR(1000) NULL,
        ScoredAt DATETIME2 NOT NULL
            CONSTRAINT DF_Score_ScoredAt DEFAULT SYSDATETIME(),
        UpdatedAt DATETIME2 NULL,

        CONSTRAINT UQ_Score_Submission_Judge UNIQUE (SubmissionId, JudgeId),
        CONSTRAINT CK_Score_Value CHECK (ScoreValue BETWEEN 0 AND 100),
        CONSTRAINT FK_Score_Submission
            FOREIGN KEY (SubmissionId) REFERENCES dbo.Submission(Id),
        CONSTRAINT FK_Score_Account_Judge
            FOREIGN KEY (JudgeId) REFERENCES dbo.Account(Id)
    );
END;
GO
