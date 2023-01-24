CREATE TABLE [dbo].[NotificationEmail]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[NotificationId] INT NOT NULL,
	[Email] NVARCHAR(320) NOT NULL,
	[VerificationToken] UNIQUEIDENTIFIER NULL,
	[CreatedOn] DATETIME2(7) NOT NULL DEFAULT (GETUTCDATE()),
	[ModifiedOn] DATETIME2(7) NULL,
	CONSTRAINT [FK_NotificationEmail_Notification] FOREIGN KEY([NotificationId]) REFERENCES [dbo].[Notification] ([Id]),
)
