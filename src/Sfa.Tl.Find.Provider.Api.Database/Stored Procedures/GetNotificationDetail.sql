CREATE PROCEDURE [dbo].[GetNotificationDetail]
	@notificationId INT
AS
	SET NOCOUNT ON;

	SELECT n.[Id],
		   n.[Email],
		   CASE	WHEN n.EmailVerificationToken IS NULL THEN 1
				ELSE 0
			END AS [IsEmailVerified],
		   nl.[Frequency],
		   nl.[SearchRadius],
		   l.[Id] AS [LocationId],
		   l.[Name] AS [LocationName],
		   l.[Postcode],
		   r.[Id] AS [RouteId],
		   r.[Name] AS [RouteName]
	FROM [ProviderNotification] n	
	LEFT JOIN [dbo].[NotificationLocation] nl
  	ON	nl.[ProviderNotificationId] = n.[Id]
	LEFT JOIN	[dbo].[Location] l
	ON	l.[Id] = nl.[LocationId]
	LEFT JOIN [dbo].[NotificationLocationRoute] nlr
	ON	nlr.[NotificationLocationId] = nl.[Id]
	LEFT JOIN [Route] r
	ON r.[Id] = nlr.[RouteId]
	WHERE n.[Id] = @notificationId
	ORDER BY	[LocationName],
				n.[Email],
				r.[Name]
