CREATE PROCEDURE [dbo].[GetEmployerInterestDetail]
	@id [INT]
AS
	SET NOCOUNT ON;

	SELECT		ei.[Id],
				ei.[UniqueId],
				ei.[OrganisationName],
				ei.[ContactName],
				eil.[Name] AS [LocationName],
				eil.[Postcode],
				ei.[Email],
				ei.[Website],
				ei.[Telephone],
				ei.[ContactPreferenceType],
				ei.[AdditionalInformation],
				ei.[ExpiryDate],
				ei.[ExtensionCount],
				eil.[Latitude],
				eil.[Longitude],
				ei.[CreatedOn],
				ei.[ModifiedOn],
				i.[Id] AS [IndustryId],
				CASE
					WHEN i.[Id] IS NOT NULL THEN i.[Name]
					ELSE ei.[OtherIndustry]
				END AS [Industry],
				r.[Id] AS [RouteId],
				r.[Name] AS [RouteName]
	FROM		[dbo].[EmployerInterest] ei
	LEFT JOIN	[dbo].[EmployerInterestLocation] eil 
	ON			eil.[EmployerInterestId] = ei.[Id]
	LEFT JOIN	[dbo].[EmployerInterestIndustry] eii
	ON			eii.[EmployerInterestId] = ei.[Id]
	LEFT JOIN	[dbo].[Industry] i 
	ON			i.[Id] = eii.[IndustryId]
	LEFT JOIN	[dbo].[EmployerInterestRoute] eir
	ON			eir.[EmployerInterestId] = ei.[Id]
	LEFT JOIN	[dbo].[Route] r 
	ON			r.[Id] = eir.[RouteId]
	WHERE		ei.[Id] = @id
	ORDER BY	r.[Name]
