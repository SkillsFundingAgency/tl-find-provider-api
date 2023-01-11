CREATE TABLE [dbo].[SearchFilter]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[LocationId] INT NOT NULL,
	[SearchRadius] INT NOT NULL,
	[CreatedOn] DATETIME2(7) NOT NULL DEFAULT (GETUTCDATE()),
	[ModifiedOn] DATETIME2(7) NULL,
	CONSTRAINT [PK_SearchFilter] PRIMARY KEY ([Id])
)
