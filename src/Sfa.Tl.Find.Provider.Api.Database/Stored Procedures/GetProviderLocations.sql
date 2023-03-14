CREATE PROCEDURE [dbo].[GetProviderLocations]
	@ukPrn BIGINT
AS
	SET NOCOUNT ON;

	WITH ProvidersCTE AS (
	SELECT	p.[Id]
	FROM	[Provider] p
	WHERE	p.[UkPrn] = @ukPrn
	  AND	p.[IsDeleted] = 0
	)
	  SELECT l.[Id],
			 l.[Postcode],
			 l.[Name],
			 l.[Latitude],
			 l.[Longitude]
		FROM ProvidersCTE p
		INNER JOIN	[dbo].[Location] l
	  	ON		p.[Id] = l.[ProviderId]
		WHERE	l.[IsDeleted] = 0
		ORDER BY l.[Postcode]
