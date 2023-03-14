CREATE PROCEDURE [dbo].[GetSearchFilterSummary]
	@ukPrn BIGINT
AS
	SET NOCOUNT ON;

	WITH ProvidersCTE AS (
		SELECT	p.[Id]
		FROM	[Provider] p
		WHERE	p.[UkPrn] = @ukPrn
		  AND	p.[IsDeleted] = 0
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
	ORDER BY	[LocationName],
				r.[Name]
