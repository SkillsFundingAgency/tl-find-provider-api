CREATE PROCEDURE [dbo].[GetNotifications]
	@ukPrn BIGINT,
	@includeAdditionalData BIT
AS
	SET NOCOUNT ON;
		
	WITH ProvidersCTE AS (
	SELECT	p.[Id],
			ROW_NUMBER() OVER(PARTITION BY p.[UkPrn] ORDER BY p.[IsAdditionalData]) AS ProviderRowNum
	FROM	[Provider] p
	WHERE	p.[UkPrn] = @ukPrn
	  AND	p.[IsDeleted] = 0
	  AND	(@includeAdditionalData = 1 OR (@includeAdditionalData = 0 AND p.[IsAdditionalData] = 0))
	)
	 SELECT n.[Id],
		   n.[Email],
		   n.[Frequency],
		   n.[SearchRadius],
		   l.[Id] AS [LocationId],
		   l.[Name] AS [LocationName],
		   l.[Postcode],
		   r.[Id] AS [RouteId],
		   r.[Name] AS [RouteName]
		FROM ProvidersCTE p
		LEFT JOIN	[dbo].[Location] l
	  	ON	p.[Id] = l.[ProviderId]
		LEFT JOIN [dbo].[Notification] n
		ON n.[LocationId] = l.[Id]
		INNER JOIN [dbo].[NotificationRoute] nr
		ON nr.[NotificationId] = n.[Id]
		LEFT JOIN [Route] r
		ON r.[Id] = nr.[RouteId]
		WHERE	l.[IsDeleted] = 0
		  --Only include the first row to make sure main data set takes priority
		  AND	p.[ProviderRowNum] = 1
	ORDER BY	[LocationName],
				n.[Email],
				r.[Name]
