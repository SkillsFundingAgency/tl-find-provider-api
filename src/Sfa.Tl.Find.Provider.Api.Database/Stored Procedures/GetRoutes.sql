CREATE PROCEDURE [dbo].[GetRoutes]
AS
	SET NOCOUNT ON;

	WITH ProvidersCTE AS (
		SELECT	p.[Id]
		FROM	[Provider] p
		WHERE	p.[IsDeleted] = 0),
	
	--Need to group here to remove multiple delivery years
	LocationQualificationCTE AS (
		SELECT [LocationId], [QualificationId]
		FROM [dbo].[LocationQualification] 
		GROUP BY [LocationId], [QualificationId]),

	LocationsCTE AS (
		SELECT	rq.[RouteId] AS [RouteId],
				rq.[QualificationId] AS [QualificationId]
		FROM	[ProvidersCTE] p
		INNER JOIN	[dbo].[Location] l
		ON		p.[Id] = l.[ProviderId]
		INNER JOIN	[LocationQualificationCTE] lq
		ON		lq.[LocationId] = l.[Id]
		INNER JOIN	[dbo].[Qualification] q
		ON		q.[Id] = lq.[QualificationId]
		  AND	q.[IsDeleted] = 0
		INNER JOIN	[dbo].[RouteQualification] rq
		ON		rq.[QualificationId] = q.[Id]
		INNER JOIN	[dbo].[Route] r
		ON		r.[Id] = rq.[RouteId]
		  AND	r.[IsDeleted] = 0
		WHERE	l.[IsDeleted] = 0
	  )
	SELECT	r.[Id] AS [RouteId], 
			r.[Name] AS [RouteName], 
			q.[Id] AS [QualificationId], 
			q.[Name] AS [QualificationName],
			COUNT(cte.[QualificationId]) AS [NumberOfQualificationsOffered]
	FROM [Route] r
	LEFT JOIN	[RouteQualification] rq
	ON		r.[Id] = rq.[RouteId]
	  AND	r.[IsDeleted] = 0
	LEFT JOIN [Qualification] q
	ON q.Id = rq.[QualificationId]
	  AND	q.[IsDeleted] = 0
	LEFT JOIN LocationsCTE cte
	ON cte.[RouteId] = r.[Id]
	AND cte.[QualificationId] = q.[Id]
	GROUP BY q.[Id],
			 q.[Name],
			 r.[Id],
			 r.[Name]
	ORDER BY r.[Name],
		     q.[Name];
