CREATE TABLE [dbo].[NotificationRoute]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[NotificationId] INT NOT NULL,
	[RouteId] INT NOT NULL,
	[CreatedOn] DATETIME2(7) NOT NULL DEFAULT (GETUTCDATE()),
	CONSTRAINT [PK_NotificationRoute] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_NotificationRoute_Notification] FOREIGN KEY([NotificationId]) REFERENCES [dbo].[Notification] ([Id]),
	CONSTRAINT [FK_NotificationRoute_Route] FOREIGN KEY([RouteId]) REFERENCES [dbo].[Route] ([Id])
)
