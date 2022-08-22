CREATE PROCEDURE [dbo].[GetAllProviderDetails]
AS

	SET NOCOUNT ON;

WITH ProvidersCTE AS (
	SELECT	p.[Id],
			p.[UkPrn],
			p.[Name], 
			p.[Email],
			p.[Telephone],
			p.[Website],
			p.[IsAdditionalData],
			--Need to filter so providers in the additional data set are overidden by ones in the main data set
			ROW_NUMBER() OVER(PARTITION BY p.[UkPrn] ORDER BY p.[IsAdditionalData]) AS ProviderRowNum
	FROM	[Provider] p
	WHERE	p.[IsDeleted] = 0),

NearestLocationsCTE AS (
	SELECT	p.[UkPrn],
			p.[Name] AS [ProviderName],
			l.[Id] AS [LocationId],
			l.[Postcode],
			l.[Name] AS [LocationName],
			ISNULL([AddressLine1], '') AS [AddressLine1],
			ISNULL([AddressLine2], '') AS [AddressLine2],
			ISNULL(l.[Town], '') AS [Town],
			ISNULL(l.[County], '') AS [County],
			COALESCE(NULLIF(l.[Email], ''), p.[Email], '') AS [Email],
			COALESCE(NULLIF(l.[Telephone], ''), p.[Telephone], '') AS [Telephone],
			COALESCE(NULLIF(l.[Website], ''), p.[Website], '') AS [Website],
			p.[IsAdditionalData]
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
					WHERE	lq.[LocationId] = l.[Id])
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
	ORDER BY [ProviderName],
				[LocationName],
				lq.[DeliveryYear],
				r.[Name],
				q.[Name];
