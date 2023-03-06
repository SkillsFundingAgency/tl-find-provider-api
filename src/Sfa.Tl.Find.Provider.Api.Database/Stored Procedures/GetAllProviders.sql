CREATE PROCEDURE [dbo].[GetAllProviders]
AS
	SET NOCOUNT ON;

	WITH ProvidersCTE AS (
		SELECT	p.[Id],
				p.[UkPrn],
				p.[Name], 
				p.[Email],
				p.[Telephone],
				p.[Website]
		FROM	[Provider] p
		WHERE	p.[IsDeleted] = 0),

	NearestLocationsCTE AS (
		SELECT	p.[UkPrn],
				p.[Name],
				p.[Email],
				p.[Telephone],
				p.[Website],
				l.[Id] AS [LocationId],
				l.[Postcode],
				l.[Name] AS [LocationName],
				l.[AddressLine1] AS [LocationAddressLine1],
				l.[AddressLine2] AS [LocationAddressLine2],
				l.[Town] AS [Town],
				l.[County] AS [County],
				l.[Email] AS [LocationEmail],
				l.[Telephone] AS [LocationTelephone],
				l.[Website] AS [LocationWebsite],
				l.[Latitude],
				l.[Longitude]
		FROM	[ProvidersCTE] p
		INNER JOIN	[dbo].[Location] l
		ON		p.[Id] = l.[ProviderId]
		WHERE	l.[IsDeleted] = 0
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
				l.[Name],
				[Email],
				[Telephone],
				[Website],
				[Postcode],
				[LocationName],
				[LocationAddressLine1],
				[LocationAddressLine2],
				t.[TownName] AS [Town],
				[County],
				[Email],
				[Telephone],
				[Website],
				[Latitude],
				[Longitude],
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
		ORDER BY	l.[Name],
					[LocationName],
					lq.[DeliveryYear],
					r.[Name],
					q.[Name];
