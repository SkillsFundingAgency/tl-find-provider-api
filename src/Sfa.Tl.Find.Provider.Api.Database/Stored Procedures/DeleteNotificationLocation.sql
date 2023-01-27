CREATE PROCEDURE [dbo].[DeleteNotificationLocation]
	@notificationLocationId INT
AS
	SET NOCOUNT ON;
	
	DELETE FROM [dbo].[NotificationLocationRoute]
	WHERE [NotificationLocationId] = @notificationLocationId;

	DELETE FROM [dbo].[NotificationLocation]
	WHERE [Id] = @notificationLocationId;	
