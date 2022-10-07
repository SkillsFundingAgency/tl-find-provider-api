CREATE TYPE [dbo].[EmployerInterestDataTableType] AS TABLE
(
	[UniqueId] UNIQUEIDENTIFIER NOT NULL,
	[OrganisationName] NVARCHAR(400) NOT NULL,
	[ContactName] NVARCHAR(400) NOT NULL,
	[Postcode] NVARCHAR(10) NULL,
	[Latitude] DECIMAL(9, 6) NULL,
	[Longitude] DECIMAL(9, 6) NULL,
	[HasMultipleLocations] BIT NOT NULL,
	[LocationCount] INT NULL,
	[IndustryId] INT NOT NULL,
	[SpecificRequirements] NVARCHAR(MAX) NULL,
	[Email] NVARCHAR(320) NULL,
	[Telephone] NVARCHAR(150) NULL,
	[ContactPreferenceType] INT NOT NULL
)
