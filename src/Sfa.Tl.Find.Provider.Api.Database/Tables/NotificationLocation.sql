CREATE TABLE [dbo].[NotificationLocation]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[ProviderNotificationId] INT NOT NULL,
	[LocationId] INT NULL,
	[Frequency] INT NOT NULL,
	[SearchRadius] INT NOT NULL,
	[LastNotificationDate] DATETIME2(7) NULL,
	[CreatedOn] DATETIME2(7) NOT NULL DEFAULT (GETUTCDATE()),
	CONSTRAINT [PK_NotificationLocation] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_NotificationLocation_ProviderNotification] FOREIGN KEY([ProviderNotificationId]) REFERENCES [dbo].[ProviderNotification] ([Id]),
	CONSTRAINT [FK_NotificationLocation_Location] FOREIGN KEY([LocationId]) REFERENCES [dbo].[Location] ([Id])
)
