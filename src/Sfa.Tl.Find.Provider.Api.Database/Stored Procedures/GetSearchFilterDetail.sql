CREATE PROCEDURE [dbo].[GetSearchFilterDetail]
	@locationId INT
AS
	SET NOCOUNT ON;

	SELECT sf.[Id],
		   l.[Id] AS [LocationId],
		   l.[Name] AS [LocationName],
		   l.[Postcode],
		   sf.[SearchRadius],		   
		   r.[Id] AS [RouteId],
		   r.[Name] AS [RouteName]
	FROM [Location] l
	LEFT JOIN [dbo].[SearchFilter] sf
	ON sf.[LocationId] = l.[Id]
	LEFT JOIN [dbo].[SearchFilterRoute] sfsa
	ON sfsa.[SearchFilterId] = sf.[Id]
	LEFT JOIN [Route] r
	ON r.[Id] = sfsa.[RouteId]
	WHERE l.[Id] = @locationId
	  AND l.[IsDeleted] = 0
	ORDER BY	r.[Name]