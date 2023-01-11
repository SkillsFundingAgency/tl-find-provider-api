CREATE TABLE [dbo].[SearchFilterRoute]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[SearchFilterId] INT NOT NULL,
	[RouteId] INT NOT NULL,
	[CreatedOn] DATETIME2(7) NOT NULL DEFAULT (GETUTCDATE()),
	CONSTRAINT [PK_SearchFilterRoute] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_SearchFilterRoute_SearchFilter] FOREIGN KEY([SearchFilterId]) REFERENCES [dbo].[SearchFilter] ([Id]),
	CONSTRAINT [FK_SearchFilterRoute_Route] FOREIGN KEY([RouteId]) REFERENCES [dbo].[Route] ([Id])

)
