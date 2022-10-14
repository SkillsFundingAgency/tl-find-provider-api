CREATE PROCEDURE [dbo].[SearchEmployerInterest]
	@fromLatitude DECIMAL(9, 6),
	@fromLongitude DECIMAL(9, 6),
	@page INT,
	@pageSize INT,
	@totalEmployerInterestsCount INT OUTPUT
AS
	SET NOCOUNT ON;

	DECLARE @fromLocation GEOGRAPHY = geography::Point(@fromLatitude, @fromLongitude, 4326);
	SELECT * FROM [dbo].[EmployerInterest]

	SELECT @totalEmployerInterestsCount = COUNT(1) 
	FROM [dbo].[EmployerInterest];

	SELECT ei.[Id],
		   ei.[OrganisationName],
		   i.[Name] AS [Industry],
		   0 AS [Distance],
		   ei.[CreatedOn],
		   ei.[ModifiedOn]
	FROM [dbo].[EmployerInterest] ei
	INNER JOIN [dbo].[EmployerInterestIndustry] eii
	ON eii.[EmployerInterestId] = ei.[Id]
	INNER JOIN [dbo].[Industry] i
	ON i.[Id] = eii.[IndustryId]
	ORDER BY [OrganisationName]
