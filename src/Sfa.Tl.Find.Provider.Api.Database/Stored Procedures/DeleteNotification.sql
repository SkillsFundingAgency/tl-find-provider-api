﻿CREATE PROCEDURE [dbo].[DeleteNotification]
	@notificationId INT
AS

	SET NOCOUNT ON;
	
	DELETE FROM [dbo].[NotificationRoute]
	WHERE [NotificationId] = @notificationId;
	
	DELETE FROM [dbo].[Notification]
	WHERE [Id] = @notificationId;
