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
	  SELECT l.[Id],
			 l.[Postcode],
			 l.[Name],
			 l.[Latitude],
			 l.[Longitude],
			 CAST(
				CASE WHEN sf.[Id] IS NOT NULL 
					THEN 1 
					ELSE 0 
				END AS BIT) AS [HasSearchFilters]
		FROM ProvidersCTE p
		INNER JOIN	[dbo].[Location] l
	  	ON		p.[Id] = l.[ProviderId]
		LEFT JOIN [dbo].[SearchFilter] sf
		ON sf.[LocationId] = l.[Id]
		WHERE	l.[IsDeleted] = 0
		  --Only include the first row to make sure main data set takes priority
		  AND	p.[ProviderRowNum] = 1
		ORDER BY l.[Postcode]
