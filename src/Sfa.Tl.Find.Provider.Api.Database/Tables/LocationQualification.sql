CREATE TABLE [dbo].[LocationQualification]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[DeliveryYear] SMALLINT NOT NULL,
	[LocationId] INT NOT NULL,
	[QualificationId] INT NOT NULL,
	[IsAdditionalData] BIT NOT NULL DEFAULT (0),
	[CreatedOn] DATETIME2(7) NOT NULL DEFAULT (GETUTCDATE())
	CONSTRAINT [PK_LocationQualification] PRIMARY KEY ([Id])
	CONSTRAINT [FK_LocationQualification_Location] FOREIGN KEY([LocationId]) REFERENCES [dbo].[Location] ([Id])
	CONSTRAINT [FK_LocationQualification_Qualification] FOREIGN KEY([QualificationId]) REFERENCES [dbo].[Qualification] ([Id])
)
