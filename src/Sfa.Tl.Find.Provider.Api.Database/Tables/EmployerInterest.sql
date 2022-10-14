CREATE TABLE [dbo].[EmployerInterest]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[UniqueId] UNIQUEIDENTIFIER NOT NULL,
	[OrganisationName] NVARCHAR(400) NOT NULL,
	[ContactName] NVARCHAR(400) NOT NULL,
	[Postcode] NVARCHAR(10) NOT NULL,
	[HasMultipleLocations] BIT NOT NULL DEFAULT(0),
	[LocationCount] INT NULL,
	[OtherIndustry] NVARCHAR(400) NULL,
	[AdditionalInformation] NVARCHAR(MAX) NULL,
	[Email] NVARCHAR(320) NULL,
	[Website] NVARCHAR(500) NULL,
	[Telephone] NVARCHAR(150) NULL,
	[ContactPreferenceType] INT NOT NULL DEFAULT (0),
		-- 0 = No preference
		-- 1 = Email
		-- 2 = Telephone
	[CreatedOn] DATETIME2(7) NOT NULL DEFAULT (GETUTCDATE()),
	[ModifiedOn] DATETIME2(7) NULL
    CONSTRAINT [PK_EmployerInterest] PRIMARY KEY ([Id])
	CONSTRAINT [UQ_UniqueId] UNIQUE([UniqueId])
)
