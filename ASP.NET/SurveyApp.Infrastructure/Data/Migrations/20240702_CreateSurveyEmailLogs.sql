
IF OBJECT_ID(N'dbo.SurveyEmailLogs', N'U') IS NULL 
BEGIN
    CREATE TABLE [dbo].[SurveyEmailLogs] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [SurveyId] INT NOT NULL,
        [Recipients] NVARCHAR(MAX) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL,
        [ErrorMessage] NVARCHAR(MAX) NULL,
        [CreatedAt] DATETIME2 NOT NULL,
        CONSTRAINT [FK_SurveyEmailLogs_Surveys] FOREIGN KEY ([SurveyId]) 
            REFERENCES [dbo].[Surveys] ([Id]) ON DELETE CASCADE
    )
END
