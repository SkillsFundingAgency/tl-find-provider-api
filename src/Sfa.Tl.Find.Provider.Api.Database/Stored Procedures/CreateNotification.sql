CREATE PROCEDURE [dbo].[CreateNotification]
	@email NVARCHAR(320),
	@emailVerificationToken UNIQUEIDENTIFIER,
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

	INSERT INTO [dbo].[NotificationEmail] (
		NotificationId, 
		Email,
		VerificationToken)
	VALUES (@newId,
		@email,
		@emailVerificationToken)

	INSERT INTO [dbo].[NotificationRoute] (
		NotificationId, 
		RouteId)
	SELECT @newId,
		[Id]
		FROM @routeIds

    RETURN @newId
