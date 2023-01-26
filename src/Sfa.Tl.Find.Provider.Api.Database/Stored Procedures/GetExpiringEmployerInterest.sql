CREATE PROCEDURE [dbo].[GetExpiringEmployerInterest]
	@daysToExpiry [INT]
AS
	SET NOCOUNT ON;

	DECLARE @expiryDate [DATETIME2] = 
			DATEADD(day, 
				@daysToExpiry, 
				CAST(CONVERT(CHAR(8), GETUTCDATE(), 112) + ' 23:59:59.9999999' AS DATETIME2))

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
				(SELECT TOP(1) eii.[IndustryId]
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
	--We want items that expire in next n days, 
	--but not where email has been sent in the previous n days
	WHERE		ei.[ExpiryDate] < @expiryDate
	  AND		(ei.[ExtensionEmailSentDate] IS NULL 
				 OR	ei.[ExtensionEmailSentDate] < DATEADD(day, -@daysToExpiry, ei.[ExpiryDate]))
