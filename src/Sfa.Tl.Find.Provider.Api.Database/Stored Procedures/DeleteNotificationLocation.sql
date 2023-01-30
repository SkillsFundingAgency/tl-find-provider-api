CREATE PROCEDURE [dbo].[DeleteNotificationLocation]
	@notificationLocationId INT
AS
	SET NOCOUNT ON;
	
	DECLARE @providerNotificationId INT
	
	SELECT @providerNotificationId  = [ProviderNotificationId] 
	FROM [dbo].[NotificationLocation]
	WHERE [Id] = @notificationLocationId;

	DELETE FROM [dbo].[NotificationLocationRoute]
	WHERE [NotificationLocationId] = @notificationLocationId;

	DELETE FROM [dbo].[NotificationLocation]
	WHERE [Id] = @notificationLocationId;
	

	IF NOT EXISTS (SELECT *
				   FROM [dbo].[NotificationLocation]
				   WHERE [Id] = @notificationLocationId)	
	BEGIN
		--Remove top-level record if the last location has been removed
		DELETE FROM [dbo].[ProviderNotification]
		WHERE [Id] = @providerNotificationId;
	END

	DELETE FROM [dbo].[ProviderNotification]
	WHERE [Id] = @providerNotificationId;
