CREATE PROCEDURE [dbo].[CreateNotification]
	@email NVARCHAR(320),
	@frequency INT,
	@searchRadius INT,
	@locationId INT,
	@routeIds [dbo].[IdListTableType] READONLY
AS
	SET NOCOUNT ON;

    DECLARE @newId INT

	INSERT INTO [dbo].[Notification] (
		Email, 
		Frequency,
		SearchRadius,
		LocationId)
	VALUES( @email,
		@frequency,
		@searchRadius,
		@locationId)
	
    SELECT @newId = SCOPE_IDENTITY();

	INSERT INTO [dbo].[NotificationRoute] (
		NotificationId, 
		RouteId)
	SELECT @newId,
		[Id]
		FROM @routeIds

    RETURN @newId
