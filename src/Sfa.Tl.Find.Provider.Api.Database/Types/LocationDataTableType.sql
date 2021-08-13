CREATE TYPE [dbo].[LocationDataTableType] AS TABLE
(
	[UkPrn] BIGINT NOT NULL,
	[Postcode] NVARCHAR(10) NULL,
	[Name] NVARCHAR(400) NOT NULL,
	[AddressLine1] NVARCHAR(100) NULL,
	[AddressLine2] NVARCHAR(100) NULL,
	[Town] NVARCHAR(100) NULL,
	[County] NVARCHAR(50) NULL,
	[Email] NVARCHAR(320) NULL,
	[Telephone] NVARCHAR(150) NULL,
	[Website] NVARCHAR(100) NULL,
	[Latitude] DECIMAL(9, 6) NULL,
	[Longitude] DECIMAL(9, 6) NULL
)
