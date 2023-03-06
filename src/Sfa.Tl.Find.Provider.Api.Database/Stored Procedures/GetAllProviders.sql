CREATE PROCEDURE [dbo].[GetAllProviders]
AS
	SET NOCOUNT ON;

	WITH ProvidersCTE AS (
		SELECT	p.[Id],
				p.[UkPrn],
				p.[Name], 
				p.[Postcode],
				p.[AddressLine1],
				p.[AddressLine2],
				p.[Town] AS [Town],
				p.[County] AS [County],
				p.[Email],
				p.[Telephone],
				p.[Website]
		FROM	[Provider] p
		WHERE	p.[IsDeleted] = 0),

	NearestLocationsCTE AS (
		SELECT	p.[UkPrn],
				p.[Name],
				p.[Postcode],
				p.[AddressLine1],
				p.[AddressLine2],
				p.[Town] AS [Town],
				p.[County] AS [County],
				p.[Email],
				p.[Telephone],
				p.[Website],
				l.[Id] AS [LocationId],
				l.[Postcode]  AS [LocationPostcode],
				l.[Name] AS [LocationName],
				l.[AddressLine1] AS [LocationAddressLine1],
				l.[AddressLine2] AS [LocationAddressLine2],
				l.[Town] AS [LocationTown],
				l.[County] AS [LocationCounty],
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
		)

		--add in the qualifications 
		SELECT 	--provider details
				l.[UkPrn],
				l.[Name],
				l.[Postcode],
				l.[AddressLine1],
				l.[AddressLine2],
				l.[Town],
				l.[County],
				l.[Email],
				l.[Telephone],
				l.[Website],
				--location details
				l.[LocationName],
				l.[LocationPostcode],
				l.[LocationAddressLine1],
				l.[LocationAddressLine2],
				l.[LocationTown],
				l.[LocationCounty],
				l.[LocationEmail],
				l.[LocationTelephone],
				l.[LocationWebsite],
				[Latitude],
				[Longitude],
				--delivery year details with routes and qualifications
				lq.[DeliveryYear] AS [Year],
				rq.[RouteId],
				r.[Name] AS [RouteName],
				q.[Id] AS [QualificationId],
				q.[Name] AS [QualificationName]				
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
		ORDER BY	l.[Name],
					[LocationName],
					lq.[DeliveryYear],
					r.[Name],
					q.[Name];
