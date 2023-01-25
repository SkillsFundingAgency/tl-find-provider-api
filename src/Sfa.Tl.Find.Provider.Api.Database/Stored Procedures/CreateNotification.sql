CREATE PROCEDURE [dbo].[CreateNotification]
	@ukPrn BIGINT,
	@email NVARCHAR(320),
	@verificationToken UNIQUEIDENTIFIER,
	@frequency INT,
	@searchRadius INT,
	@locationId INT,
	@routeIds [dbo].[IdListTableType] READONLY
AS
	SET NOCOUNT ON;

    DECLARE @newId INT

	INSERT INTO [dbo].[Notification] (
		Frequency,
		SearchRadius,
		LocationId)
	VALUES (@frequency,
		@searchRadius,
		@locationId)
	
    SELECT @newId = SCOPE_IDENTITY();

	INSERT INTO [dbo].[ProviderNotification] (
		[ProviderId],
		[NotificationId])
	SELECT p.[Id],
		@newId
	FROM 	(SELECT	p.[Id],
				ROW_NUMBER() OVER(PARTITION BY p.[UkPrn] ORDER BY p.[IsAdditionalData]) AS ProviderRowNum
			 FROM	[Provider] p
			 WHERE	p.[UkPrn] = @ukPrn
			   AND	p.[IsDeleted] = 0) p
	  WHERE ProviderRowNum = 1

	INSERT INTO [dbo].[NotificationEmail] (
		NotificationId, 
		Email,
		VerificationToken)
	VALUES (@newId,
		@email,
		@verificationToken)

	INSERT INTO [dbo].[NotificationRoute] (
		NotificationId, 
		RouteId)
	SELECT @newId,
		[Id]
		FROM @routeIds

    RETURN @newId
