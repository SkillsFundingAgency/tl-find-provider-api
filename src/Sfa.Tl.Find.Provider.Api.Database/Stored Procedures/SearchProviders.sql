CREATE PROCEDURE [dbo].[SearchProviders]
	@fromLatitude DECIMAL(9, 6),
	@fromLongitude DECIMAL(9, 6),
	@qualificationId INT,
	@page INT,
	@pageSize INT
AS

	SET NOCOUNT ON;

	DECLARE @fromLocation GEOGRAPHY = geography::Point(@fromLatitude, @fromLongitude, 4326);
	
	WITH NearestLocationsCTE AS (
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
	FROM	[dbo].[Provider] p
	INNER JOIN	[dbo].[Location] l
	ON		p.[Id] = l.[ProviderId]
	WHERE	p.IsDeleted = 0	
	  AND	l.IsDeleted = 0
	  AND	EXISTS (
	  		SELECT	lq.[QualificationId]
			FROM	[dbo].[LocationQualification] lq
			INNER JOIN	[dbo].[Qualification] q
			ON		q.[Id] = lq.[QualificationId]
			  AND	q.IsDeleted = 0
			WHERE	lq.[LocationId] = l.[Id])
	ORDER BY [Distance],
			p.[Name],
			l.[Name]
	OFFSET @page * @pageSize ROWS
	FETCH NEXT @pageSize ROWS ONLY)
	--Step 2 - add in the qualifications 
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
				lq.[DeliveryYear] AS [Year],
				q.[Id] AS [Id],
				q.[Name] AS [Name]
		FROM NearestLocationsCTE l
		INNER JOIN	[dbo].[LocationQualification] lq
		ON		lq.[LocationId] = l.[LocationId]
		INNER JOIN	[dbo].[Qualification] q
		ON		q.[Id] = lq.[QualificationId]
		  AND	q.IsDeleted = 0;
