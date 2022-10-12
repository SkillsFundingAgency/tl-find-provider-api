CREATE PROCEDURE [dbo].[UpdateProviders]
	@data [dbo].[ProviderDataTableType] READONLY,
	@isAdditionalData BIT
AS
	SET NOCOUNT ON;

	DECLARE @ChangeSummary TABLE(
		Change VARCHAR(20),
		Id INT,
		IsDeleted BIT
	);

	MERGE INTO [dbo].[Provider] AS t
	USING @data AS s
	ON
	(
		t.[UkPrn] = s.[UkPrn]
		AND t.[IsAdditionalData] = s.[IsAdditionalData]
	)

	WHEN MATCHED 
			AND (t.[Name] <> s.[Name] COLLATE Latin1_General_CS_AS
				 OR ISNULL(NULLIF(t.[AddressLine1] COLLATE Latin1_General_CS_AS, s.[AddressLine1] COLLATE Latin1_General_CS_AS), 
				 		   NULLIF(s.[AddressLine1] COLLATE Latin1_General_CS_AS, t.[AddressLine1] COLLATE Latin1_General_CS_AS))
					IS NOT NULL
				 OR ISNULL(NULLIF(t.[AddressLine2] COLLATE Latin1_General_CS_AS, s.[AddressLine2] COLLATE Latin1_General_CS_AS), 
						   NULLIF(s.[AddressLine2] COLLATE Latin1_General_CS_AS, t.[AddressLine2] COLLATE Latin1_General_CS_AS)) 
					IS NOT NULL
				 OR ISNULL(NULLIF(t.[Town] COLLATE Latin1_General_CS_AS, s.[Town] COLLATE Latin1_General_CS_AS), 
						   NULLIF(s.[Town] COLLATE Latin1_General_CS_AS, t.[Town] COLLATE Latin1_General_CS_AS)) 
					IS NOT NULL
				 OR ISNULL(NULLIF(t.[County] COLLATE Latin1_General_CS_AS, s.[County] COLLATE Latin1_General_CS_AS), 
						   NULLIF(s.[County] COLLATE Latin1_General_CS_AS, t.[County] COLLATE Latin1_General_CS_AS)) 
					IS NOT NULL
				 OR ISNULL(NULLIF(t.[Postcode] COLLATE Latin1_General_CS_AS, s.[Postcode] COLLATE Latin1_General_CS_AS), 
						   NULLIF(s.[Postcode] COLLATE Latin1_General_CS_AS, t.[Postcode] COLLATE Latin1_General_CS_AS)) 
					IS NOT NULL
				 OR ISNULL(NULLIF(t.[Email] COLLATE Latin1_General_CS_AS, s.[Email] COLLATE Latin1_General_CS_AS), 
						   NULLIF(s.[Email] COLLATE Latin1_General_CS_AS, t.[Email] COLLATE Latin1_General_CS_AS)) 
					IS NOT NULL
				 OR ISNULL(NULLIF(t.[Telephone] COLLATE Latin1_General_CS_AS, s.[Telephone] COLLATE Latin1_General_CS_AS), 
						   NULLIF(s.[Telephone] COLLATE Latin1_General_CS_AS, t.[Telephone] COLLATE Latin1_General_CS_AS)) 
					IS NOT NULL
				 OR ISNULL(NULLIF(t.[Website] COLLATE Latin1_General_CS_AS, s.[Website] COLLATE Latin1_General_CS_AS), 
						   NULLIF(s.[Website] COLLATE Latin1_General_CS_AS, t.[Website] COLLATE Latin1_General_CS_AS)) 
					IS NOT NULL
			/*
			--TODO: The following will be needed when we are updating
			--		provider data directly from the web site, but not while 
			--		we are just uploading provider contact files.
			--		For now, update should ignore these columns when data is 
			--		imported from course directory.
				 OR ISNULL(NULLIF(t.[EmployerContactEmail] COLLATE Latin1_General_CS_AS, s.[EmployerContactEmail] COLLATE Latin1_General_CS_AS), 
						   NULLIF(s.[EmployerContactEmail] COLLATE Latin1_General_CS_AS, t.[EmployerContactEmail] COLLATE Latin1_General_CS_AS)) 
					IS NOT NULL
				 OR ISNULL(NULLIF(t.[EmployerContactTelephone] COLLATE Latin1_General_CS_AS, s.[EmployerContactTelephone] COLLATE Latin1_General_CS_AS), 
						   NULLIF(s.[EmployerContactTelephone] COLLATE Latin1_General_CS_AS, t.[EmployerContactTelephone] COLLATE Latin1_General_CS_AS)) 
					IS NOT NULL
				 OR ISNULL(NULLIF(t.[EmployerContactWebsite] COLLATE Latin1_General_CS_AS, s.[EmployerContactWebsite] COLLATE Latin1_General_CS_AS), 
						   NULLIF(s.[EmployerContactWebsite] COLLATE Latin1_General_CS_AS, t.[EmployerContactWebsite] COLLATE Latin1_General_CS_AS)) 
					IS NOT NULL
				 OR ISNULL(NULLIF(t.[StudentContactEmail] COLLATE Latin1_General_CS_AS, s.[StudentContactEmail] COLLATE Latin1_General_CS_AS), 
						   NULLIF(s.[StudentContactEmail] COLLATE Latin1_General_CS_AS, t.[StudentContactEmail] COLLATE Latin1_General_CS_AS)) 
					IS NOT NULL
				 OR ISNULL(NULLIF(t.[StudentContactTelephone] COLLATE Latin1_General_CS_AS, s.[StudentContactTelephone] COLLATE Latin1_General_CS_AS), 
						   NULLIF(s.[StudentContactTelephone] COLLATE Latin1_General_CS_AS, t.[StudentContactTelephone] COLLATE Latin1_General_CS_AS)) 
					IS NOT NULL
				 OR ISNULL(NULLIF(t.[StudentContactWebsite] COLLATE Latin1_General_CS_AS, s.[StudentContactWebsite] COLLATE Latin1_General_CS_AS), 
						   NULLIF(s.[StudentContactWebsite] COLLATE Latin1_General_CS_AS, t.[StudentContactWebsite] COLLATE Latin1_General_CS_AS)) 
					IS NOT NULL
			*/
		OR t.[IsDeleted] = 1) --To undelete
	THEN UPDATE SET
		t.[Name] = s.[Name],
		t.[AddressLine1] = s.[AddressLine1],
		t.[AddressLine2] = s.[AddressLine2],
		t.[Town] = s.[Town],
		t.[County] = s.[County],
		t.[Postcode] = s.[Postcode],
		t.[Email] = s.[Email],
		t.[Telephone] = s.[Telephone],
		t.[Website] = s.[Website],
		t.[IsAdditionalData] = s.[IsAdditionalData],
		--t.[EmployerContactEmail] = s.[EmployerContactEmail],
		--t.[EmployerContactTelephone] = s.[EmployerContactTelephone],
		--t.[EmployerContactWebsite] = s.[EmployerContactWebsite],
		--t.[StudentContactEmail] = s.[StudentContactEmail],
		--t.[StudentContactTelephone] = s.[StudentContactTelephone],
		--t.[StudentContactWebsite] = s.[StudentContactWebsite],
		t.[IsDeleted] = 0,
		t.[ModifiedOn] = GETUTCDATE()

	WHEN NOT MATCHED BY TARGET THEN INSERT
	(
		[UkPrn],
		[Name],
		[AddressLine1],
		[AddressLine2],
		[Town],
		[County],
		[Postcode],
		[Email],
		[Telephone],
		[Website],
		[EmployerContactEmail],
		[EmployerContactTelephone],
		[EmployerContactWebsite],
		[StudentContactEmail],
		[StudentContactTelephone],
		[StudentContactWebsite],
		[IsAdditionalData]
	)
	VALUES
	(
		s.[UkPrn],
		s.[Name],
		s.[AddressLine1],
		s.[AddressLine2],
		s.[Town],
		s.[County],
		s.[Postcode],
		s.[Email],
		s.[Telephone],
		s.[Website],
		s.[EmployerContactEmail],
		s.[EmployerContactTelephone],
		s.[EmployerContactWebsite],
		s.[StudentContactEmail],
		s.[StudentContactTelephone],
		s.[StudentContactWebsite],
		s.[IsAdditionalData]
	)

	WHEN NOT MATCHED BY SOURCE 
			AND t.[IsDeleted] <> 1
			AND t.[IsAdditionalData] = @isAdditionalData
	THEN UPDATE SET
		t.[IsDeleted] = 1,
		t.[ModifiedOn] = GETUTCDATE() --Soft delete

	OUTPUT $action, 
		INSERTED.[Id], 
		INSERTED.[IsDeleted]
	INTO @ChangeSummary;

	WITH ChangesCTE (Change) AS
	(SELECT	CASE
				WHEN Change = 'UPDATE' AND [IsDeleted] = 1
				THEN 'DELETE'
				ELSE Change
			END
		FROM @ChangeSummary)
		SELECT	Change, 
				COUNT(*) AS CountPerChange	 
		FROM ChangesCTE
		GROUP BY Change;
