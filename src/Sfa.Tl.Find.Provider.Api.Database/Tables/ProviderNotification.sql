CREATE TABLE [dbo].[ProviderNotification]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[ProviderId] INT NOT NULL,
	[NotificationId] INT NOT NULL,
	[CreatedOn] DATETIME2(7) NOT NULL DEFAULT (GETUTCDATE()),
	CONSTRAINT [PK_ProviderNotification] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_ProviderNotification_Provider] FOREIGN KEY([ProviderId]) REFERENCES [dbo].[Provider] ([Id]),
	CONSTRAINT [FK_ProviderNotification_Notification] FOREIGN KEY([NotificationId]) REFERENCES [dbo].[Notification] ([Id])
)
