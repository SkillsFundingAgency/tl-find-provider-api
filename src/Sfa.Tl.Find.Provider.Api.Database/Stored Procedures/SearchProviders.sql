CREATE PROCEDURE [dbo].[SearchProviders]
	@fromLatitude DECIMAL(9, 6),
	@fromLongitude DECIMAL(9, 6),
	@routeIds [dbo].[IdListTableType] READONLY,
	@qualificationIds [dbo].[IdListTableType] READONLY,
	@page INT,
	@pageSize INT,
	@mergeAdditionalData BIT
AS

	SET NOCOUNT ON;

	DECLARE @fromLocation GEOGRAPHY = geography::Point(@fromLatitude, @fromLongitude, 4326);
	
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
		  AND	(@mergeAdditionalData = 1 OR (@mergeAdditionalData = 0 AND p.[IsAdditionalData] = 0))),
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
			COALESCE(NULLIF(l.[Email],''), p.[Email]) AS [Email],
			COALESCE(NULLIF(l.[Telephone],''), p.[Telephone]) AS [Telephone],
			COALESCE(NULLIF(l.[Website],''), p.[Website]) AS [Website],
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
			  AND	((NOT EXISTS(SELECT [Id] FROM @qualificationIds WHERE [Id] <> 0)
					   OR q.[Id] IN (SELECT [Id] FROM @qualificationIds))
					 AND (NOT EXISTS(SELECT [Id] FROM @routeIds WHERE [Id] <> 0)
						  OR r.[Id] IN (SELECT [Id] FROM @routeIds))
					))
	ORDER BY [Distance],
			 p.[Name],
			 l.[Name]
	OFFSET @page * @pageSize ROWS
	FETCH NEXT @pageSize ROWS ONLY)
	--Step 2 - add in the qualifications (no filter for qualifications - return all for selected locations)
		SELECT 	[UkPrn],
				[ProviderName],
				[Postcode],
				[LocationName],
				[AddressLine1],
				[AddressLine2],
				[Town],
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
