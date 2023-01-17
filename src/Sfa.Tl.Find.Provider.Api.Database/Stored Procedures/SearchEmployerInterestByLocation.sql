CREATE PROCEDURE [dbo].[SearchEmployerInterestByLocation]
	@locationId INT,
	@defaultSearchRadius INT,
	@searchFiltersApplied BIT OUTPUT,
	@totalEmployerInterestsCount INT OUTPUT
AS
	SET NOCOUNT ON;

	DECLARE @metersPerMile DECIMAL = 1609.3399999999999E0;
	DECLARE @searchRadius DECIMAL =  @defaultSearchRadius;
	DECLARE @searchRadiusInMeters DECIMAL;

	DECLARE @routeIds [dbo].[IdListTableType];

	DECLARE @searchFilterId INT;
	DECLARE @fromLocation GEOGRAPHY;

	SELECT @totalEmployerInterestsCount = COUNT(1) 
	FROM [dbo].[EmployerInterest];

	SET @searchFiltersApplied = 0;

	IF (@locationId IS NULL)
		RAISERROR ('Either @locationId or both @latitude and @longitude required' ,1 ,1)

	SELECT @fromLocation = [Location],
			@searchFilterId = sf.[Id],
			@searchRadius = CASE WHEN sf.[Id] IS NOT NULL 
						THEN  sf.[SearchRadius]
						ELSE @defaultSearchRadius 
					END
	FROM [dbo].[Location] l
	LEFT JOIN [dbo].[SearchFilter] sf
	ON sf.[LocationId] = l.[Id] 
	WHERE l.[Id] = @locationId

	IF(@searchFilterId IS NOT NULL)
	BEGIN
		SET @searchFiltersApplied = 1

		INSERT INTO @routeIds 
		SELECT [RouteId]
		FROM [dbo].[SearchFilterRoute]
	END

	SET @searchRadiusInMeters = @searchRadius * @metersPerMile;

	WITH EmployerInterest_CTE AS (
		SELECT ei.[Id],
			ei.[OrganisationName],
			ei.[OtherIndustry],
			el.[Location].STDistance(@fromLocation) / @metersPerMile AS [Distance], --(Miles)
			ei.[ExpiryDate],
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
	),

	Route_CTE AS (
		SELECT r.[Id], 
			r.[Name]
		FROM [dbo].[Route] r
		WHERE (SELECT COUNT(*) FROM @routeIds) = 0
		   OR r.[Id] IN (SELECT [Id] FROM @routeIds)
	)

	SELECT	ei.[Id],
			ei.[OrganisationName],
			ei.[Distance],
			COALESCE (i.[Name], ei.[OtherIndustry]) AS Industry,
			ei.[ExpiryDate],
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
	LEFT JOIN Route_CTE r
	ON r.[Id] = eir.[RouteId]
	ORDER BY	ei.[Distance],
				ei.[CreatedOn] DESC,
				ei.[OrganisationName], 
				r.[Name]
