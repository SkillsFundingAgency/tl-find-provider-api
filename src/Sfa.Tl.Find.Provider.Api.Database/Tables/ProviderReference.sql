CREATE TABLE [dbo].[ProviderReference]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[UkPrn] BIGINT NOT NULL,
	[Urn] BIGINT NOT NULL,
	[Name] NVARCHAR(400) NOT NULL,
	[CreatedOn] DATETIME2(7) NOT NULL DEFAULT (getutcdate()),
	[ModifiedOn] DATETIME2(7) NULL
	CONSTRAINT [PK_ProviderReference] PRIMARY KEY ([Id])
)
