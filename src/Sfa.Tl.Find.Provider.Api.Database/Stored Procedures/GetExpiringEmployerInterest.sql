CREATE PROCEDURE [dbo].[GetExpiringEmployerInterest]
	@date [DateTime2]
AS
	--SET NOCOUNT ON;

	SELECT		ei.[Id],
				ei.[UniqueId],
				ei.[OrganisationName],
				ei.[ContactName],
				COALESCE(eil.[Postcode], ei.[Postcode]) AS [Postcode],
				ei.[Email],
				ei.[Website],
				ei.[Telephone],
				ei.[ContactPreferenceType],
				ei.[AdditionalInformation],
				eil.[Latitude],
				eil.[Longitude],
				ei.[CreatedOn],
				ei.[ModifiedOn],
				ei.[OtherIndustry],
				(SELECT TOP(1) [Id]
				 FROM	[dbo].[EmployerInterestIndustry] eii
				 WHERE eii.[EmployerInterestId] = ei.[Id]
				 ORDER BY eii.[IndustryId]
				) AS [IndustryId],
				eir.[RouteId]
	FROM		[dbo].[EmployerInterest] ei
	LEFT JOIN	[dbo].[EmployerInterestLocation] eil 
	ON			eil.[EmployerInterestId] = ei.[Id]
	LEFT JOIN	[dbo].[EmployerInterestRoute] eir
	ON			eir.[EmployerInterestId] = ei.[Id]
	WHERE		--(ei.[RenewalEmailSentOn] IS NULL 
				-- OR ei.[RenewalEmailSentOn] < ei.[ModifiedOn])
		--AND		
				((ei.[CreatedOn] < @date AND ei.[ModifiedOn] IS NULL)
				 OR	ei.[ModifiedOn] < @date)
