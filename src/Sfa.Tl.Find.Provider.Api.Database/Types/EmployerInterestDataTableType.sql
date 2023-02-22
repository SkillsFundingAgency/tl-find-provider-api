CREATE TYPE [dbo].[EmployerInterestDataTableType] AS TABLE
(
	[UniqueId] UNIQUEIDENTIFIER NOT NULL,
	[OrganisationName] NVARCHAR(400) NOT NULL,
	[ContactName] NVARCHAR(400) NOT NULL,
	[LocationName] NVARCHAR(400) NOT NULL,
	[Postcode] NVARCHAR(10) NULL,
	[Latitude] DECIMAL(9, 6) NULL,
	[Longitude] DECIMAL(9, 6) NULL,
	[OtherIndustry] NVARCHAR(400) NULL,
	[Email] NVARCHAR(320) NULL,
	[Telephone] NVARCHAR(150) NULL,
	[Website] NVARCHAR(500) NULL,
	[ContactPreferenceType] INT NULL,
	[AdditionalInformation] NVARCHAR(MAX) NULL,
	[ExpiryDate] DATETIME2 NULL
)
