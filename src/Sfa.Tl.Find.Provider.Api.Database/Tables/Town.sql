CREATE TABLE [dbo].[Town]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[TownName] [varchar](256) NOT NULL,
	[CountyName] [varchar](256) NULL,
	[LocalAuthorityName] [varchar](256) NULL,
	[Latitude] DECIMAL(9, 6) NOT NULL,
	[Longitude] DECIMAL(9, 6) NOT NULL,
    CONSTRAINT [PK_Town] PRIMARY KEY ([Id])
)
