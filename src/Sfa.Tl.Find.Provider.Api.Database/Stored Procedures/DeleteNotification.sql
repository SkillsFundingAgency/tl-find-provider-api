CREATE PROCEDURE [dbo].[DeleteNotification]
	@notificationId INT
AS
	SET NOCOUNT ON;
	
	DELETE FROM [dbo].[NotificationEmail]
	WHERE [NotificationId] = @notificationId;

	DELETE FROM [dbo].[NotificationRoute]
	WHERE [NotificationId] = @notificationId;

	--DELETE FROM [dbo].[NotificationLocation]
	--WHERE [NotificationId] = @notificationId;
	
	DELETE FROM [dbo].[ProviderNotification]
	WHERE [NotificationId] = @notificationId;
	
	DELETE FROM [dbo].[Notification]
	WHERE [Id] = @notificationId;
