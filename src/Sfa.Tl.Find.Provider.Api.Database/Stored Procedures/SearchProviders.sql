CREATE PROCEDURE [dbo].[SearchProviders]
	@fromLatitude DECIMAL(9, 6),
	@fromLongitude DECIMAL(9, 6),
	@qualificationId INT,
	@page INT,
	@pageSize INT
AS

	SET NOCOUNT ON;

	DECLARE @fromLocation GEOGRAPHY = geography::Point(@fromLatitude, @fromLongitude, 4326)
	
	SELECT	p.[UkPrn],
			p.[Name] AS [ProviderName],
			l.[Postcode],
			l.[Name] AS [LocationName],
			l.[AddressLine1],
			l.[AddressLine2],
			l.[Town],
			l.[County],
			COALESCE(NULLIF(l.[Email],''), p.[Email]) AS [Email],
			COALESCE(NULLIF(l.[Telephone],''), p.[Telephone]) AS [Telephone],
			COALESCE(NULLIF(l.[Website],''), p.[Website]) AS [Website],
			l.[Location].STDistance(@fromLocation) / 1609.3399999999999E0 AS [Distance], --(Miles)
			lq.[DeliveryYear] AS [Year],
			q.[Id] AS [Id],
			q.[Name] AS [Name]
	FROM	[dbo].[Provider] p
	INNER JOIN	[dbo].[Location] l
	ON		p.[Id] = l.[ProviderId]
	INNER JOIN	[dbo].[LocationQualification] lq
	ON		lq.[LocationId] = l.[Id]
	INNER JOIN	[dbo].[Qualification] q
	ON		q.[Id] = lq.[QualificationId]
	WHERE	p.IsDeleted = 0	
	  AND	l.IsDeleted = 0
	  AND	q.IsDeleted = 0
	ORDER BY [Distance],
			p.[Name],
			l.[Name]
	OFFSET @page * @pageSize ROWS
	FETCH NEXT @pageSize ROWS ONLY


