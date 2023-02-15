CREATE PROCEDURE [dbo].[GetEmployerInterestSummary]
	--@page INT,
	--@pageSize INT,
	--@totalEmployerInterestsCount INT OUTPUT
AS
	SET NOCOUNT ON;

	--SELECT @totalEmployerInterestsCount = COUNT(1) 
	--FROM [dbo].[EmployerInterest];

	WITH EmployerInterest_CTE AS (
		SELECT ei.[Id],
			ei.[OrganisationName],
			ei.[OtherIndustry],
			ei.[ExpiryDate],
			ei.[CreatedOn],
			ei.[ModifiedOn]
		FROM [dbo].[EmployerInterest] ei
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
			eil.[Postcode],
			NULL AS [Distance],
			COALESCE (i.[Name], ei.[OtherIndustry]) AS Industry,
			ei.[ExpiryDate],
			ei.[CreatedOn],
			ei.[ModifiedOn],
			r.[Id] AS [RouteId],
			r.[Name] AS [RouteName]
	FROM EmployerInterest_CTE ei
	LEFT JOIN	[dbo].[EmployerInterestLocation] eil 
	ON			eil.[EmployerInterestId] = ei.[Id]
	LEFT JOIN EmployerInterestIndustry_CTE i
	ON i.[EmployerInterestId] = ei.[Id]
	 AND i.[IndustryRowNum] = 1	
	LEFT JOIN [EmployerInterestRoute] eir
	ON eir.[EmployerInterestId] = ei.[Id]
	LEFT JOIN [Route] r
	ON r.[Id] = eir.[RouteId]
	ORDER BY	ei.[OrganisationName],
				ei.[CreatedOn] DESC,
				ei.[Id],
				r.[Name]
