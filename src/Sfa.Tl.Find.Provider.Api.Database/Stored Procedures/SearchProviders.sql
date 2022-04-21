CREATE PROCEDURE [dbo].[SearchProviders]
	@fromLatitude DECIMAL(9, 6),
	@fromLongitude DECIMAL(9, 6),
	@routeIds [dbo].[IdListTableType] READONLY,
	@qualificationIds [dbo].[IdListTableType] READONLY,
	@page INT,
	@pageSize INT,
	@includeAdditionalData BIT
AS

	SET NOCOUNT ON;

	DECLARE @fromLocation GEOGRAPHY = geography::Point(@fromLatitude, @fromLongitude, 4326);

	DECLARE @allQualificationIds TABLE ([Id] INT)
	--DECLARE @hasQualificationIds BIT = 0

	INSERT INTO @allQualificationIds
	SELECT [QualificationId] 
	FROM @routeIds r
	INNER JOIN [RouteQualification] rq
	ON	rq.[RouteId] = r.[Id]
	UNION
	SELECT [Id] 
	FROM @qualificationIds;
	--IF(EXISTS(SELECT 1 from @routeIds) OR 
	--   EXISTS(SELECT 1 from @qualificationIds))
	--BEGIN
	--	SET @hasQualificationIds = 1

	--	INSERT INTO @allQualificationIds
	--	SELECT [QualificationId] 
	--	FROM @routeIds r
	--	INNER JOIN [RouteQualification] rq
	--	ON	rq.[RouteId] = r.[Id]
	--	UNION
	--	SELECT [Id]
	--	FROM @qualificationIds
	--END;
		
	WITH ProvidersCTE AS (
		SELECT	p.[Id],
				p.[UkPrn],
				p.[Name], 
				p.[Email],
				p.[Telephone],
				p.[Website],
				--Need to filter so providers in the additional data set are overidden by ones in the main data set
				ROW_NUMBER() OVER(PARTITION BY p.[UkPrn] ORDER BY p.[IsAdditionalData]) AS ProviderRowNum
		FROM	[Provider] p
		WHERE	p.[IsDeleted] = 0
		  --If not merging additional data, have to make sure we only include non-additional data
		  AND	(@includeAdditionalData = 1 OR (@includeAdditionalData = 0 AND p.[IsAdditionalData] = 0))),

	NearestLocationsCTE AS (
		SELECT	p.[UkPrn],
				p.[Name] AS [ProviderName],
				l.[Id] AS [LocationId],
				l.[Postcode],
				l.[Name] AS [LocationName],
				l.[AddressLine1],
				l.[AddressLine2],
				l.[Town],
				l.[County],
				COALESCE(NULLIF(l.[Email], ''), p.[Email]) AS [Email],
				COALESCE(NULLIF(l.[Telephone], ''), p.[Telephone]) AS [Telephone],
				COALESCE(NULLIF(l.[Website], ''), p.[Website]) AS [Website],
				l.[Location].STDistance(@fromLocation) / 1609.3399999999999E0 AS [Distance] --(Miles)
		FROM	[ProvidersCTE] p
		INNER JOIN	[dbo].[Location] l
		ON		p.[Id] = l.[ProviderId]
		WHERE	l.[IsDeleted] = 0
		  --Only include the first row to make sure main data set takes priority
		  AND	p.[ProviderRowNum] = 1
		  AND	EXISTS (
	  			SELECT	lq.[QualificationId]
				FROM	[dbo].[LocationQualification] lq
				INNER JOIN	[dbo].[Qualification] q
				ON		q.[Id] = lq.[QualificationId]
				  AND	q.[IsDeleted] = 0
				INNER JOIN	[dbo].[RouteQualification] rq
				ON		rq.[QualificationId] = q.[Id]
				INNER JOIN	[dbo].[Route] r
				ON		r.[Id] = rq.[RouteId]
				  AND	r.[IsDeleted] = 0
				WHERE	lq.[LocationId] = l.[Id]
				  AND	((NOT EXISTS(SELECT [Id] FROM @allQualificationIds WHERE [Id] <> 0)
						   OR q.[Id] IN (SELECT [Id] FROM @allQualificationIds))
						))
--				  AND	(@hasQualificationIds = 0
--						 OR q.[Id] IN (SELECT [Id] FROM @allQualificationIds)))
		ORDER BY [Distance],
				 p.[Name],
				 l.[Name]
		OFFSET @page * @pageSize ROWS
		FETCH NEXT @pageSize ROWS ONLY),
		
		TownNamesCTE AS (
			SELECT	[LocationId],
					COALESCE(t.[Name], l.[Town]) AS [TownName]
			FROM NearestLocationsCTE l
			OUTER APPLY (SELECT Top(1) [Name] 
						 FROM [dbo].[Town]
						 WHERE [Name] = l.[Town]
						 OR [Name] = REPLACE(l.[Town], ' ', '-')
						 ) t
			GROUP BY [LocationId],
					 l.[Town],
					 t.[Name])

		--Step 2 - add in the qualifications (no filter for qualifications - return all for selected locations)
		SELECT 	[UkPrn],
				[ProviderName],
				[Postcode],
				[LocationName],
				[AddressLine1],
				[AddressLine2],
				t.[TownName] AS [Town],
				[County],
				[Email],
				[Telephone],
				[Website],
				[Distance],
				rq.[RouteId],
				r.[Name] AS [RouteName],
				lq.[DeliveryYear] AS [Year],
				q.[Id] AS [Id],
				q.[Name] AS [Name]
		FROM NearestLocationsCTE l
		INNER JOIN	[dbo].[LocationQualification] lq
		ON		lq.[LocationId] = l.[LocationId]
		INNER JOIN [TownNamesCTE] t
		ON		t.[LocationId] = l.[LocationId]
		INNER JOIN	[dbo].[Qualification] q
		ON		q.[Id] = lq.[QualificationId]
		  AND	q.[IsDeleted] = 0
		INNER JOIN	[dbo].[RouteQualification] rq
		ON		rq.[QualificationId] = q.[Id]
		INNER JOIN	[dbo].[Route] r
		ON		r.[Id] = rq.[RouteId]
		  AND	r.[IsDeleted] = 0
		ORDER BY [Distance],
				 [ProviderName],
				 [LocationName],
				 lq.[DeliveryYear],
				 q.[Name];
