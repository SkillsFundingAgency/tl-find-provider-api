CREATE PROCEDURE [dbo].[UpdateNotificationLocation]
	@notificationLocationId INT,
	@frequency INT,
	@searchRadius INT,
	@routeIds [dbo].[IdListTableType] READONLY
AS
	SET NOCOUNT ON;

	UPDATE [dbo].[NotificationLocation] 
	SET [Frequency] = @frequency,
		[SearchRadius] = @searchRadius,
		[ModifiedOn] = GETUTCDATE()
	WHERE Id = @notificationLocationId
	  AND ([Frequency] <> @frequency
		   OR [SearchRadius] <> @searchRadius);

	WITH TARGET AS 
	(
		SELECT [NotificationLocationId], 
			   [RouteId]
		FROM [NotificationLocationRoute]
		WHERE [NotificationLocationId] = @notificationLocationId
	)
	MERGE INTO TARGET
	USING 
	(
		SELECT [Id]
		FROM @routeIds
	) AS SOURCE ON
		SOURCE.[Id] = TARGET.[RouteId]
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (NotificationLocationId, RouteId)
		VALUES (@notificationLocationId, Id)
	WHEN NOT MATCHED BY SOURCE THEN
		DELETE
	;
