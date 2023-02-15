CREATE TABLE [dbo].[NotificationLocationRoute]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[NotificationLocationId] INT NOT NULL,
	[RouteId] INT NOT NULL,
	[CreatedOn] DATETIME2(7) NOT NULL DEFAULT (GETUTCDATE()),
	CONSTRAINT [PK_NotificationLocationRoute] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_NotificationLocationRoute_NotificationLocation] FOREIGN KEY([NotificationLocationId]) REFERENCES [dbo].[NotificationLocation] ([Id]),
	CONSTRAINT [FK_NotificationLocationRoute_Route] FOREIGN KEY([RouteId]) REFERENCES [dbo].[Route] ([Id])
)
