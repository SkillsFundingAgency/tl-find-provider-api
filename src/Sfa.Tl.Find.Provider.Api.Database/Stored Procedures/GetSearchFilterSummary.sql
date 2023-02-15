CREATE PROCEDURE [dbo].[GetSearchFilterSummary]
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
		   l.[Id] AS [LocationId],
		   l.[Name] AS [LocationName],
		   l.[Postcode],
		   sf.[SearchRadius],
		   r.[Id] AS [RouteId],
		   r.[Name] AS [RouteName]
		FROM ProvidersCTE p
		INNER JOIN	[dbo].[Location] l
	  	ON	p.[Id] = l.[ProviderId]
		LEFT JOIN [dbo].[SearchFilter] sf
		ON sf.[LocationId] = l.[Id]
		LEFT JOIN [dbo].[SearchFilterRoute] sfr
		ON sfr.[SearchFilterId] = sf.[Id]
		LEFT JOIN [Route] r
		ON r.[Id] = sfr.[RouteId]
		WHERE	l.[IsDeleted] = 0
		  --Only include the first row to make sure main data set takes priority
		  AND	p.[ProviderRowNum] = 1
	ORDER BY	[LocationName],
				r.[Name]
