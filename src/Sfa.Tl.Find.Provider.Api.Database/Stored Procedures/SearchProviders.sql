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
			l.[Name],
			l.[AddressLine1],
			l.[AddressLine2],
			l.[Town],
			l.[County],
			l.[Postcode],
			l.[Email],
			COALESCE(NULLIF(l.[Email],''), p.[Email]) AS [Email],
			COALESCE(NULLIF(l.[Telephone],''), p.[Telephone]) AS [Telephone],
			COALESCE(NULLIF(l.[Website],''), p.[Website]) AS [Website],
			l.[Location].STDistance(@fromLocation) / 1609.3399999999999E0 AS [DistanceInMiles] 
	FROM	[dbo].[Provider] p
	INNER JOIN	[dbo].[Location] l
	ON		p.[Id] = l.[ProviderId]
	WHERE	p.IsDeleted = 0	
	ORDER BY [DistanceInMiles],
			p.[Name],
			l.[Name]
	OFFSET @page * @pageSize ROWS
	FETCH NEXT @pageSize ROWS ONLY

