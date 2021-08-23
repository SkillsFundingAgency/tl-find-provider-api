﻿CREATE TABLE [dbo].[Location]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[ProviderId] INT NOT NULL,
	[Postcode] NVARCHAR(10) NOT NULL,
	[Name] NVARCHAR(400) NOT NULL,
	[AddressLine1] NVARCHAR(100) NULL,
	[AddressLine2] NVARCHAR(100) NULL,
	[Town] NVARCHAR(100) NULL,
	[County] NVARCHAR(50) NULL,
	[Email] NVARCHAR(320) NULL,
	[Telephone] NVARCHAR(150) NULL,
	[Website] NVARCHAR(500) NULL,
	[Latitude] DECIMAL(9, 6) NOT NULL,
	[Longitude] DECIMAL(9, 6) NOT NULL,
	[Location] GEOGRAPHY NULL, 
	[IsDeleted] BIT NOT NULL DEFAULT (0),
	[CreatedOn] DATETIME2(7) NOT NULL DEFAULT (GETUTCDATE()),
	[ModifiedOn] DATETIME2(7) NULL
    CONSTRAINT [PK_Location] PRIMARY KEY ([Id])
	CONSTRAINT [FK_Location_Provider] FOREIGN KEY([ProviderId]) REFERENCES [dbo].[Provider] ([Id])
)