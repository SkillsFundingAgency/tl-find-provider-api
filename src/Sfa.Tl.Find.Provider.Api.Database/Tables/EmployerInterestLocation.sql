CREATE TABLE [dbo].[EmployerInterestLocation]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[EmployerInterestId] INT NOT NULL,
	[Postcode] NVARCHAR(10) NOT NULL,
	[Latitude] DECIMAL(9, 6) NOT NULL,
	[Longitude] DECIMAL(9, 6) NOT NULL,
	[Location] GEOGRAPHY NULL, 
	[CreatedOn] DATETIME2(7) NOT NULL DEFAULT (GETUTCDATE()),
	[ModifiedOn] DATETIME2(7) NULL,
    CONSTRAINT [PK_EmployerInterestLocation] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_EmployerInterestLocation_EmployerInterest] FOREIGN KEY([EmployerInterestId]) REFERENCES [dbo].[EmployerInterest] ([Id])
)
