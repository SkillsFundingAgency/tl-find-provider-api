﻿CREATE PROCEDURE [dbo].[DeleteNotificationLocation]
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
