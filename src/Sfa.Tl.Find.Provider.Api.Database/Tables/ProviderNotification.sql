CREATE TABLE [dbo].[ProviderNotification]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[ProviderId] INT NOT NULL,
	[Email] NVARCHAR(320) NOT NULL,
	[EmailVerificationToken] UNIQUEIDENTIFIER NULL,
	[CreatedOn] DATETIME2(7) NOT NULL DEFAULT (GETUTCDATE()),
	[ModifiedOn] DATETIME2(7) NULL,
	CONSTRAINT [PK_ProviderNotification] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_ProviderNotification_Provider] FOREIGN KEY([ProviderId]) REFERENCES [dbo].[Provider] ([Id])
)
