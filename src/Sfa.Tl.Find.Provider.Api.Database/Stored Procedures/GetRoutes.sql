CREATE PROCEDURE [dbo].[GetRoutes]
	@includeAdditionalData BIT
AS

	SET NOCOUNT ON;

	WITH ProvidersCTE AS (
		SELECT	p.[Id],
				--Need to filter so providers in the additional data set are overidden by ones in the main data set
				ROW_NUMBER() OVER(PARTITION BY p.[UkPrn] ORDER BY p.[IsAdditionalData]) AS ProviderRowNum
		FROM	[Provider] p
		WHERE	p.[IsDeleted] = 0
		  --If not merging additional data, have to make sure we only include non-additional data
		  AND	(@includeAdditionalData = 1 OR (@includeAdditionalData = 0 AND p.[IsAdditionalData] = 0))),
	
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
		--Only include the first row to make sure main data set takes priority
		  AND	p.[ProviderRowNum] = 1
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
	LEFT JOIN LocationsCTE cte
	ON cte.RouteId = r.[Id]
	LEFT JOIN [Qualification] q
	ON q.Id = rq.[QualificationId]
	  AND	r.[IsDeleted] = 0
	GROUP BY q.[Id],
			 q.[Name],
			 r.[Id],
			 r.[Name]
	ORDER BY r.[Name],
		     q.[Name];
