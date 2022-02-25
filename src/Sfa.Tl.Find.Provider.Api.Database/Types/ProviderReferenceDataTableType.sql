CREATE TYPE [dbo].[ProviderReferenceDataTableType] AS TABLE
(
	[Id] INT NOT NULL,
	[UkPrn] BIGINT NOT NULL,
	[Urn] BIGINT NOT NULL,
	[Name] NVARCHAR(400) NOT NULL
)
