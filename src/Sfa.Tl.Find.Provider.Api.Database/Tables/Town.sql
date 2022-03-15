CREATE TABLE [dbo].[Town]
(
	[Id] INT NOT NULL,
	[Name] [NVARCHAR](100) NOT NULL,
	[County] [NVARCHAR](50) NULL,
	[LocalAuthorityName] [NVARCHAR](50) NULL,
	[Latitude] DECIMAL(9, 6) NOT NULL,
	[Longitude] DECIMAL(9, 6) NOT NULL,
    CONSTRAINT [PK_Town] PRIMARY KEY ([Id])
)
