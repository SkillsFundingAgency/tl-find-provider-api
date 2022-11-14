CREATE TABLE [dbo].[EmployerInterestRoute]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[EmployerInterestId] INT NOT NULL,
	[RouteId] INT NOT NULL,
	[CreatedOn] DATETIME2(7) NOT NULL DEFAULT (GETUTCDATE())
	CONSTRAINT [PK_EmployerInterestRoute] PRIMARY KEY ([Id])
	CONSTRAINT [FK_EmployerInterestRoute_EmployerInterest] FOREIGN KEY([EmployerInterestId]) REFERENCES [dbo].[EmployerInterest] ([Id])
	CONSTRAINT [FK_EmployerInterestRoute_Route] FOREIGN KEY([RouteId]) REFERENCES [dbo].[Route] ([Id])
)
