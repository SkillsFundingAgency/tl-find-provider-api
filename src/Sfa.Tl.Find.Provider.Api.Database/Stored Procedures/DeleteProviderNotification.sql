CREATE PROCEDURE [dbo].[DeleteProviderNotification]
	@providerNotificationId INT
AS
	SET NOCOUNT ON;
	
	DELETE FROM [dbo].[NotificationLocationRoute]
	WHERE [NotificationLocationId] 
		IN (SELECT [Id]
			FROM [dbo].[NotificationLocation]
			WHERE [ProviderNotificationId] = @providerNotificationId);
	
	DELETE FROM [dbo].[NotificationLocation]
	WHERE [ProviderNotificationId] = @providerNotificationId;

	DELETE FROM [dbo].[ProviderNotification]
	WHERE [Id] = @providerNotificationId;
