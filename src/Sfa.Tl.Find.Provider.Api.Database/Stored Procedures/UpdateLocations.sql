CREATE PROCEDURE [dbo].[UpdateLocations]
	@data [dbo].[LocationDataTableType] READONLY,
	@isAdditionalData BIT
AS
	SET NOCOUNT ON;

	DECLARE @ChangeSummary TABLE(
		Change VARCHAR(20),
		Id INT,
		IsDeleted BIT
	);

	--Need to convert to a table with the provider ids
	WITH ProviderLocationsCTE AS (
	SELECT p.[Id] AS [ProviderId],
		d.[Postcode],
		d.[Name],
		d.[AddressLine1],
		d.[AddressLine2],
		d.[Town],
		d.[County],
		d.[Email],
		d.[Telephone],
		d.[Website],
		d.[Latitude],
		d.[Longitude],
		d.[IsAdditionalData]
	FROM @data d
	INNER JOIN [dbo].[Provider] p
	ON p.[UkPrn] = d.[UkPrn]
	--Make sure "normal" and additional data are kept separate
	AND p.[IsAdditionalData] = d.[IsAdditionalData]) 
	
		MERGE INTO [dbo].[Location] AS t
		USING ProviderLocationsCTE AS s
		ON
		(
		  t.[ProviderId] = s.[ProviderId] 
		  AND t.[Postcode] = s.[Postcode]
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
				  OR ISNULL(NULLIF(t.[Email] COLLATE Latin1_General_CS_AS, s.[Email] COLLATE Latin1_General_CS_AS), 
							NULLIF(s.[Email] COLLATE Latin1_General_CS_AS, t.[Email] COLLATE Latin1_General_CS_AS)) 
					 IS NOT NULL
				  OR ISNULL(NULLIF(t.[Telephone] COLLATE Latin1_General_CS_AS, s.[Telephone] COLLATE Latin1_General_CS_AS), 
							NULLIF(s.[Telephone] COLLATE Latin1_General_CS_AS, t.[Telephone] COLLATE Latin1_General_CS_AS)) 
					 IS NOT NULL
				  OR ISNULL(NULLIF(t.[Website] COLLATE Latin1_General_CS_AS, s.[Website] COLLATE Latin1_General_CS_AS), 
							NULLIF(s.[Website] COLLATE Latin1_General_CS_AS, t.[Website] COLLATE Latin1_General_CS_AS)) 
					 IS NOT NULL
				  OR t.[Latitude] <> s.[Latitude]
				  OR t.[Longitude] <> s.[Longitude]
				  OR t.[IsDeleted] = 1) --To undelete
			THEN UPDATE SET
				t.[Name] = s.[Name],
				t.[AddressLine1] = s.[AddressLine1],
				t.[AddressLine2] = s.[AddressLine2],
				t.[Town] = s.[Town],
				t.[County] = s.[County],
				t.[Email] = s.[Email],
				t.[Telephone] = s.[Telephone],
				t.[Website] = s.[Website],
				t.[Latitude] = s.[Latitude],
				t.[Longitude] = s.[Longitude],
				t.[Location] = geography::Point(s.[Latitude], s.[Longitude], 4326),
				t.[IsAdditionalData] = s.[IsAdditionalData],
				t.[IsDeleted] = 0,
				t.[ModifiedOn] = GETUTCDATE()

			WHEN NOT MATCHED BY TARGET THEN INSERT
			(
				[ProviderId],
				[Postcode],
				[Name],
				[AddressLine1],
				[AddressLine2],
				[Town],
				[County],
				[Email],
				[Telephone],
				[Website],
				[Latitude],
				[Longitude],
				[Location],
				[IsAdditionalData]
			)
			VALUES
			(
				s.[ProviderId],
				s.[Postcode],
				s.[Name],
				s.[AddressLine1],
				s.[AddressLine2],
				s.[Town],
				s.[County],
				s.[Email],
				s.[Telephone],
				s.[Website],
				s.[Latitude],
				s.[Longitude],
				geography::Point(s.[Latitude], s.[Longitude], 4326),
				s.[IsAdditionalData]
			)

			WHEN NOT MATCHED BY SOURCE 
			 AND t.[IsDeleted] <> 1 --No need to delete again
			 AND t.[IsAdditionalData] = @isAdditionalData
			THEN UPDATE SET
			  t.[IsDeleted] = 1,
			  t.[ModifiedOn] = GETUTCDATE() --Soft delete

			OUTPUT $action, 
				INSERTED.[Id], 
				INSERTED.[IsDeleted]
			INTO @ChangeSummary;

	WITH changesCTE (Change) AS
	(SELECT	CASE
				WHEN Change = 'UPDATE' AND IsDeleted = 1
				THEN 'DELETE'
				ELSE Change
			END
		FROM @ChangeSummary)
		SELECT	Change, 
				COUNT(*) AS CountPerChange	 
		FROM changesCTE
		GROUP BY Change;
