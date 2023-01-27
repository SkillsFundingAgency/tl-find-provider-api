CREATE PROCEDURE [dbo].[DeleteNotification]
	@notificationId INT
AS
	SET NOCOUNT ON;
	
	DELETE FROM [dbo].[NotificationLocationRoute]
	WHERE [NotificationLocationId] IN (SELECT [Id] 
										FROM [dbo].[NotificationLocation]
									WHERE [ProviderNotificationId] = @notificationId);
	
	DELETE FROM [dbo].[NotificationLocation]
	WHERE [ProviderNotificationId] = @notificationId;
		
	DELETE FROM [dbo].[ProviderNotification]
	WHERE [Id] = @notificationId;

