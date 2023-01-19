CREATE PROCEDURE [dbo].[GetNotificationDetail]
	@notificationId INT
AS
	SELECT n.[Id],
		   n.[Email],
		   n.[Frequency],
		   n.[SearchRadius],
		   l.[Id] AS [LocationId],
		   l.[Name] AS [LocationName],
		   l.[Postcode],
		   r.[Id] AS [RouteId],
		   r.[Name] AS [RouteName]
	FROM [Notification] n
	LEFT JOIN	[dbo].[Location] l
	ON	l.[Id] = n.[LocationId]
	LEFT JOIN [dbo].[NotificationRoute] nr
	ON nr.[NotificationId] = n.[Id]
	LEFT JOIN [Route] r
	ON r.[Id] = nr.[RouteId]
	WHERE n.[Id] = @notificationId
	ORDER BY	[LocationName],
				n.[Email],
				r.[Name]
