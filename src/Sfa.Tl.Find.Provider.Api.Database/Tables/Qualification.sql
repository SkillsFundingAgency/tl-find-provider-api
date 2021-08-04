CREATE TABLE [dbo].[Qualification]
(
	[Id] INT NOT NULL,
	[DefinitionId] UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(400) NOT NULL,
	[CreatedOn] DATETIME2(7) NOT NULL DEFAULT (getutcdate()),
	[ModifiedOn] DATETIME2(7) NULL
	CONSTRAINT [PK_Qualification] PRIMARY KEY ([Id])
)
