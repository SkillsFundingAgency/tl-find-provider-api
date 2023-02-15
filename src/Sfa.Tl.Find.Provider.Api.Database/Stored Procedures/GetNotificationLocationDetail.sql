CREATE PROCEDURE [dbo].[GetNotificationLocationDetail]
	@notificationLocationId INT
AS
	SET NOCOUNT ON;

	SELECT nl.[Id],
		   nl.[Frequency],
		   nl.[SearchRadius],
		   l.[Id] AS [LocationId],
		   l.[Name] AS [LocationName],
		   l.[Postcode],
		   r.[Id] AS [RouteId],
		   r.[Name] AS [RouteName]
	FROM [dbo].[NotificationLocation] nl
	LEFT JOIN	[dbo].[Location] l
	ON	l.[Id] = nl.[LocationId]
	LEFT JOIN [dbo].[NotificationLocationRoute] nlr
	ON	nlr.[NotificationLocationId] = nl.[Id]
	LEFT JOIN [Route] r
	ON r.[Id] = nlr.[RouteId]
	WHERE nl.[Id] = @notificationLocationId
	ORDER BY	r.[Name]
