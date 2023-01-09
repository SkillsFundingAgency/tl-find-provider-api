CREATE TABLE [dbo].[RouteQualification]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[RouteId] INT NOT NULL,
	[QualificationId] INT NOT NULL,
	[CreatedOn] DATETIME2(7) NOT NULL DEFAULT (GETUTCDATE()),
	CONSTRAINT [PK_RouteQualification] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_RouteQualification_Route] FOREIGN KEY([RouteId]) REFERENCES [dbo].[Route] ([Id]),
	CONSTRAINT [FK_RouteQualification_Qualification] FOREIGN KEY([QualificationId]) REFERENCES [dbo].[Qualification] ([Id])
)
