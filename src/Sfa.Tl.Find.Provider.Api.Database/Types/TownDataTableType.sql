CREATE TYPE [dbo].[TownDataTableType] AS TABLE
(
	[Name] NVARCHAR(400) NOT NULL,
	[County] [varchar](256) NULL,
	[Latitude] DECIMAL(9, 6) NOT NULL,
	[Longitude] DECIMAL(9, 6) NOT NULL
)
