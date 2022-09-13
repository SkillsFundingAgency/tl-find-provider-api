CREATE PROCEDURE [dbo].[SearchProviders]
	@fromLatitude DECIMAL(9, 6),
	@fromLongitude DECIMAL(9, 6),
	@routeIds [dbo].[IdListTableType] READONLY,
	@qualificationIds [dbo].[IdListTableType] READONLY,
	@page INT,
	@pageSize INT,
	@includeAdditionalData BIT,
	@totalLocationsCount INT OUTPUT
AS

	SET NOCOUNT ON;

	DECLARE @fromLocation GEOGRAPHY = geography::Point(@fromLatitude, @fromLongitude, 4326);

	DECLARE @locations TABLE (
				[UkPrn] BIGINT,
				[ProviderName] NVARCHAR(400),
				[LocationId] INT,
				[Postcode] NVARCHAR(10),
				[LocationName] NVARCHAR(400),
				[AddressLine1] NVARCHAR(400),
				[AddressLine2] NVARCHAR(400),
				[Town] NVARCHAR(100),
				[County] NVARCHAR(50),
				[Email] NVARCHAR(320),
				[Telephone] NVARCHAR(150),
				[Website] NVARCHAR(500),
				[EmployerContactEmail] NVARCHAR(320),
				[EmployerContactTelephone] NVARCHAR(150),
				[EmployerContactWebsite] NVARCHAR(500),
				[StudentContactEmail] NVARCHAR(320),
				[StudentContactTelephone] NVARCHAR(150),
				[StudentContactWebsite] NVARCHAR(500),
				[Distance] FLOAT,
				INDEX IX_Distance NONCLUSTERED(Distance, ProviderName, LocationName)
				)
	
	DECLARE @allQualificationIds TABLE (
				[Id] INT,
				UNIQUE CLUSTERED (Id)
			)
	DECLARE @hasRouteOrQualificationIds BIT = 0
		
	IF(EXISTS(SELECT 1 from @routeIds) OR 
	   EXISTS(SELECT 1 from @qualificationIds))
	BEGIN
		INSERT INTO @allQualificationIds
		SELECT [QualificationId] 
		FROM @routeIds r
		INNER JOIN [RouteQualification] rq
		ON	rq.[RouteId] = r.[Id]
		UNION
		SELECT [Id]
		FROM @qualificationIds
								   
		SET @hasRouteOrQualificationIds = 1
	END;
		
	WITH ProvidersCTE AS (
		SELECT	p.[Id],
				p.[UkPrn],
				p.[Name], 
				p.[Email],
				p.[Telephone],
				p.[Website],
				p.[EmployerContactEmail],
				p.[EmployerContactTelephone],
				p.[EmployerContactWebsite],
				p.[StudentContactEmail],
				p.[StudentContactTelephone],
				p.[StudentContactWebsite],
				--Need to filter so providers in the additional data set are overidden by ones in the main data set
				ROW_NUMBER() OVER(PARTITION BY p.[UkPrn] ORDER BY p.[IsAdditionalData]) AS ProviderRowNum
		FROM	[Provider] p
		WHERE	p.[IsDeleted] = 0
		  --If not merging additional data, have to make sure we only include non-additional data
		  AND	(@includeAdditionalData = 1 OR (@includeAdditionalData = 0 AND p.[IsAdditionalData] = 0))),

	NearestLocationsCTE_Inner AS (
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
				COALESCE(NULLIF(l.[EmployerContactEmail], ''), p.[EmployerContactEmail]) AS [EmployerContactEmail],
				COALESCE(NULLIF(l.[EmployerContactTelephone], ''), p.[EmployerContactTelephone]) AS [EmployerContactTelephone],
				COALESCE(NULLIF(l.[EmployerContactWebsite], ''), p.[EmployerContactWebsite]) AS [EmployerContactWebsite],
				COALESCE(NULLIF(l.[StudentContactEmail], ''), p.[StudentContactEmail]) AS [StudentContactEmail],
				COALESCE(NULLIF(l.[StudentContactTelephone], ''), p.[StudentContactTelephone]) AS [StudentContactTelephone],
				COALESCE(NULLIF(l.[StudentContactWebsite], ''), p.[StudentContactWebsite]) AS [StudentContactWebsite],
				l.[Location].STDistance(@fromLocation) / 1609.3399999999999E0 AS [Distance] --(Miles)
		FROM	[ProvidersCTE] p
		INNER JOIN	[dbo].[Location] l
		ON		p.[Id] = l.[ProviderId]
		WHERE	l.[IsDeleted] = 0
		  --Only include the first row to make sure main data set takes priority
		  AND	p.[ProviderRowNum] = 1
		  AND	EXISTS (SELECT	lq.[QualificationId]
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
						  AND	(@hasRouteOrQualificationIds = 0
								 OR q.[Id] IN (SELECT [Id] FROM @allQualificationIds)))
		)
	INSERT INTO @locations
	SELECT *
	FROM NearestLocationsCTE_Inner;

	SELECT @totalLocationsCount = COUNT(1) 
	from @locations;

	WITH NearestLocationsCTE AS (
		SELECT * 
		FROM @locations
		ORDER BY [Distance],
				 [ProviderName],
				 [LocationName]
		OFFSET @page * @pageSize ROWS
		FETCH NEXT @pageSize ROWS ONLY
		),
		
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

		--Step 2 - add in the qualifications 
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
				[EmployerContactEmail],
				[EmployerContactTelephone],
				[EmployerContactWebsite],
				[StudentContactEmail],
				[StudentContactTelephone],
				[StudentContactWebsite],				
				[Distance],
				lq.[DeliveryYear] AS [Year],
				rq.[RouteId],
				r.[Name] AS [RouteName],
				q.[Id] AS [QualificationId],
				q.[Name] AS [QualificationName]
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
		  AND	(@hasRouteOrQualificationIds = 0
				 OR q.[Id] IN (SELECT [Id] FROM @allQualificationIds))
		ORDER BY [Distance],
				 [ProviderName],
				 [LocationName],
				 lq.[DeliveryYear],
				 r.[Name],
				 q.[Name];
