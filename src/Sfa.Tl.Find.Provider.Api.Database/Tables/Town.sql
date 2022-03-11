CREATE TABLE [dbo].[Town]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[Name] [varchar](256) NOT NULL,
	[County] [varchar](256) NULL,
	[LocalAuthority] [varchar](256) NULL,
	[Latitude] DECIMAL(9, 6) NOT NULL,
	[Longitude] DECIMAL(9, 6) NOT NULL,
    CONSTRAINT [PK_Town] PRIMARY KEY ([Id])
)
