CREATE PROCEDURE [dbo].[CreateNotificationLocation]
	@providerNotificationId INT,
	@frequency INT,
	@searchRadius INT,
	@locationId INT,
	@routeIds [dbo].[IdListTableType] READONLY
AS
	SET NOCOUNT ON;

    DECLARE @notificationLocationId INT

	INSERT INTO [dbo].[NotificationLocation] (
		[ProviderNotificationId], 
		[LocationId],
		[Frequency],
		[SearchRadius])
	VALUES (@providerNotificationId,
		@locationId,
		@frequency,
		@searchRadius)

	SELECT @notificationLocationId = SCOPE_IDENTITY();

	INSERT INTO [dbo].[NotificationLocationRoute] (
		[NotificationLocationId] ,
		[RouteId])
	SELECT @notificationLocationId,
		[Id]
		FROM @routeIds

    RETURN @notificationLocationId
