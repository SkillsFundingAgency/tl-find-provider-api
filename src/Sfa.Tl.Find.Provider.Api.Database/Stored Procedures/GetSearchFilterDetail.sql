CREATE PROCEDURE [dbo].[GetSearchFilterDetail]
	@searchFilterId INT
AS
	SET NOCOUNT ON;

	SELECT l.[Name] AS [LocationName],
		   [SearchRadius],		   
		   r.[Id] AS [RouteId],
		   r.[Name] AS [RouteName]
	FROM [dbo].[SearchFilter] sf
	LEFT JOIN [dbo].[SearchFilterRoute] sfsa
	ON sfsa.[SearchFilterId] = sf.[Id]
	INNER JOIN [Location] l
	ON l.[Id] = sf.[LocationId]
	LEFT JOIN [Route] r
	ON r.[Id] = sfsa.[RouteId]
	WHERE sf.[Id] = @searchFilterId
	ORDER BY	r.[Name]

