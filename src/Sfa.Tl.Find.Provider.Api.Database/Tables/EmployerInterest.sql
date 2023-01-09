CREATE TABLE [dbo].[EmployerInterest]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[UniqueId] UNIQUEIDENTIFIER NOT NULL,
	[OrganisationName] NVARCHAR(400) NOT NULL,
	[ContactName] NVARCHAR(400) NOT NULL,
	[Postcode] NVARCHAR(10) NOT NULL,
	[OtherIndustry] NVARCHAR(400) NULL,
	[AdditionalInformation] NVARCHAR(MAX) NULL,
	[Email] NVARCHAR(320) NULL,
	[Telephone] NVARCHAR(150) NULL,
	[Website] NVARCHAR(500) NULL,
	[ContactPreferenceType] INT NULL,
	[ExpiryDate] DATETIME2(7) NULL,
	[ExtensionEmailSentDate] DATETIME2(7) NULL,
	[CreatedOn] DATETIME2(7) NOT NULL DEFAULT (GETUTCDATE()),
	[ModifiedOn] DATETIME2(7) NULL,
    CONSTRAINT [PK_EmployerInterest] PRIMARY KEY ([Id]),
	CONSTRAINT [UQ_UniqueId] UNIQUE([UniqueId])
)
