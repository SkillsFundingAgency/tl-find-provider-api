﻿CREATE TYPE [dbo].[ProviderDataTableType] AS TABLE
(
	[UkPrn] BIGINT NOT NULL,
	[Name] NVARCHAR(400) NOT NULL,
	[AddressLine1] NVARCHAR(100) NULL,
	[AddressLine2] NVARCHAR(100) NULL,
	[Town] NVARCHAR(100) NULL,
	[County] NVARCHAR(50) NULL,
	[Postcode] NVARCHAR(10) NULL,
	[Email] NVARCHAR(320) NULL,
	[Telephone] NVARCHAR(150) NULL,
	[Website] NVARCHAR(500) NULL,
	[EmployerContactEmail] NVARCHAR(320) NULL,
	[EmployerContactTelephone] NVARCHAR(150) NULL,
	[EmployerContactWebsite] NVARCHAR(500) NULL,
	[StudentContactEmail] NVARCHAR(320) NULL,
	[StudentContactTelephone] NVARCHAR(150) NULL,
	[StudentContactWebsite] NVARCHAR(500) NULL
)
