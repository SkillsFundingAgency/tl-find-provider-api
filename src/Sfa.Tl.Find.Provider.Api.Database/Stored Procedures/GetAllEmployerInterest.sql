CREATE PROCEDURE [dbo].[GetAllEmployerInterest]
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
			ei.[CreatedOn],
			ei.[ModifiedOn]
		FROM [dbo].[EmployerInterest] ei),
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
			NULL AS [Distance],
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
