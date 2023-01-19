CREATE TABLE [dbo].[Notification]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[LocationId] INT NULL,
	[Email] NVARCHAR(320) NULL,
	[Frequency] INT NOT NULL,
	[SearchRadius] INT NOT NULL,
	[CreatedOn] DATETIME2(7) NOT NULL DEFAULT (GETUTCDATE()),
	[ModifiedOn] DATETIME2(7) NULL,
	CONSTRAINT [PK_Notification] PRIMARY KEY ([Id])
)