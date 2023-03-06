CREATE PROCEDURE [dbo].[CreateProviderNotification]
	@ukPrn BIGINT,
	@email NVARCHAR(320),
	@emailVerificationToken UNIQUEIDENTIFIER,
	@frequency INT,
	@searchRadius INT,
	@locationId INT,
	@routeIds [dbo].[IdListTableType] READONLY
AS
	SET NOCOUNT ON;

    DECLARE @notificationLocationId INT
	DECLARE @providerNotificationId INT

	INSERT INTO [dbo].[ProviderNotification] (
		[ProviderId],
		[Email],
		[EmailVerificationToken]
		)
	SELECT p.[Id],
		@email,
		@emailVerificationToken
	FROM [Provider] p
	 WHERE	p.[UkPrn] = @ukPrn
	   AND	p.[IsDeleted] = 0

    SELECT @providerNotificationId = SCOPE_IDENTITY();

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

    RETURN @providerNotificationId
