CREATE PROCEDURE [dbo].[GetNotificationDetail]
	@notificationId INT
AS
	SELECT n.[Id],
		   e.[Email],
		   CASE	WHEN e.VerificationToken IS NULL THEN 1
				ELSE 0
			END AS [IsEmailVerified],
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
	INNER JOIN [dbo].[NotificationEmail] e
	ON e.[NotificationId] = n.[Id]
	LEFT JOIN [dbo].[NotificationRoute] nr
	ON nr.[NotificationId] = n.[Id]
	LEFT JOIN [Route] r
	ON r.[Id] = nr.[RouteId]
	WHERE n.[Id] = @notificationId
	ORDER BY	[LocationName],
				e.[Email],
				r.[Name]
