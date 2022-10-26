CREATE PROCEDURE [dbo].[SearchEmployerInterest]
	@fromLatitude DECIMAL(9, 6),
	@fromLongitude DECIMAL(9, 6),
	@searchRadius INT,
	@totalEmployerInterestsCount INT OUTPUT
AS
	SET NOCOUNT ON;

	DECLARE @metersPerMile DECIMAL = 1609.3399999999999E0;
	DECLARE @searchRadiusInMeters DECIMAL = @searchRadius * @metersPerMile;

	DECLARE @fromLocation GEOGRAPHY = geography::Point(@fromLatitude, @fromLongitude, 4326);

	SELECT @totalEmployerInterestsCount = COUNT(1) 
	FROM [dbo].[EmployerInterest];

	WITH EmployerInterest_CTE AS (
		SELECT ei.[Id],
			ei.[OrganisationName],
			ei.[OtherIndustry],
			el.[Location].STDistance(@fromLocation) / @metersPerMile AS [Distance], --(Miles)
			ei.[CreatedOn],
			ei.[ModifiedOn]
		FROM [dbo].[EmployerInterest] ei
		INNER JOIN EmployerInterestLocation el
		ON el.[EmployerInterestId] = ei.[Id]
		WHERE el.[Location].STDistance(@fromLocation) <= @searchRadiusInMeters
		),

	EmployerInterestIndustry_CTE AS (
		SELECT eii.[EmployerInterestId], 
			i.[Name],
			ROW_NUMBER() OVER(PARTITION BY eii.[EmployerInterestId] ORDER BY eii.[CreatedOn]) AS IndustryRowNum
		FROM [dbo].[EmployerInterestIndustry] eii
		INNER JOIN [dbo].[Industry] i
		ON eii.[IndustryId] = i.[Id]
	)
	SELECT	ei.[Id],
			ei.[OrganisationName],
			ei.[Distance],
			COALESCE (i.[Name], ei.[OtherIndustry]) AS Industry,
			ei.[CreatedOn],
			ei.[ModifiedOn],
			r.[Id] AS [RouteId],
			r.[Name] AS [RouteName]
	FROM EmployerInterest_CTE ei
	LEFT JOIN EmployerInterestIndustry_CTE i
	ON i.[EmployerInterestId] = ei.[Id]
	 AND i.[IndustryRowNum] = 1	
	LEFT JOIN [EmployerInterestRoute] eir
	ON eir.[EmployerInterestId] = ei.[Id]
	LEFT JOIN [Route] r
	ON r.[Id] = eir.[RouteId]
	ORDER BY	ei.[CreatedOn] DESC,
				ei.[OrganisationName], 
				ei.[Id], 
				r.[Name]
