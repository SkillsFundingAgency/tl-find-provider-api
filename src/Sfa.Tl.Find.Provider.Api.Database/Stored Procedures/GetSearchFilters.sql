CREATE PROCEDURE [dbo].[GetSearchFilters]
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
	 SELECT sf.[Id],
		   sf.[LocationId],
		   l.[Name] AS [LocationName],
		   [SearchRadius],
		   r.[Id] AS [RouteId],
		   r.[Name] AS [RouteName]
		FROM ProvidersCTE p
		INNER JOIN	[dbo].[Location] l
	  	ON		p.[Id] = l.[ProviderId]
		INNER JOIN [dbo].[SearchFilter] sf
		ON sf.[LocationId] = l.[Id]
		LEFT JOIN [dbo].[SearchFilterRoute] sfsa
		ON sfsa.[SearchFilterId] = sf.[Id]
		LEFT JOIN [Route] r
		ON r.[Id] = sfsa.[RouteId]
		--WHERE sf.[LocationId] = @locationId
		WHERE	l.[IsDeleted] = 0
		  --Only include the first row to make sure main data set takes priority
		  AND	p.[ProviderRowNum] = 1
	ORDER BY	[LocationName],
				r.[Name]
