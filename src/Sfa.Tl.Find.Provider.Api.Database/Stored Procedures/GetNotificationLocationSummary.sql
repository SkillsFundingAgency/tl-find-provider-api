CREATE PROCEDURE [dbo].[GetNotificationLocationSummary]
	@notificationId INT
AS
	SET NOCOUNT ON;

	SELECT n.[Id],
		   --e.[Id] AS [EmailId],
		   --e.[Email],
		   n.[Frequency],
		   n.[SearchRadius],
		   l.[Id] AS [LocationId],
		   l.[Name] AS [LocationName],
		   l.[Postcode],
		   r.[Id] AS [RouteId],
		   r.[Name] AS [RouteName]
	FROM [Notification] n
	INNER JOIN [dbo].[NotificationEmail] e
	ON e.[NotificationId] = n.[Id]
	LEFT JOIN	[dbo].[Location] l
	ON	l.[Id] = n.[LocationId]
	LEFT JOIN [dbo].[NotificationRoute] nr
	ON nr.[NotificationId] = n.[Id]
	LEFT JOIN [Route] r
	ON r.[Id] = nr.[RouteId]
	WHERE n.[Id] = @notificationId
	ORDER BY	--e.[Email],
				[LocationName],
				r.[Name]