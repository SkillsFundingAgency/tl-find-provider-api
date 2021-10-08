﻿CREATE TABLE [dbo].[Route]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[Name] NVARCHAR(400) NOT NULL,
	[IsDeleted] BIT NOT NULL DEFAULT (0),
	[CreatedOn] DATETIME2(7) NOT NULL DEFAULT (getutcdate()),
	[ModifiedOn] DATETIME2(7) NULL
	CONSTRAINT [PK_Route] PRIMARY KEY ([Id])
)