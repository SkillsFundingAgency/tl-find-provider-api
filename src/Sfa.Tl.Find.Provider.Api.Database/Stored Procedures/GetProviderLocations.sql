CREATE PROCEDURE [dbo].[GetProviderLocations]
	@ukPrn BIGINT,
	@includeAdditionalData BIT
AS
	SET NOCOUNT ON;

	WITH ProvidersCTE AS (
	SELECT	p.[Id],
			ROW_NUMBER() OVER(PARTITION BY p.[UkPrn] ORDER BY p.[IsAdditionalData]) AS ProviderRowNum
	FROM	[Provider] p
	WHERE	p.[UkPrn] = @ukPrn
	  AND	p.[IsDeleted] = 0
	  AND	(@includeAdditionalData = 1 OR (@includeAdditionalData = 0 AND p.[IsAdditionalData] = 0))
	)
	  SELECT p.[Id],
			 l.[Postcode],
			 l.[Name] AS [LocationName],
			 l.[Latitude],
			 l.[Longitude]
		FROM ProvidersCTE p
		INNER JOIN	[dbo].[Location] l
	  	ON		p.[Id] = l.[ProviderId]
		WHERE	l.[IsDeleted] = 0
		  --Only include the first row to make sure main data set takes priority
		  AND	p.[ProviderRowNum] = 1
		ORDER BY l.[Postcode]

