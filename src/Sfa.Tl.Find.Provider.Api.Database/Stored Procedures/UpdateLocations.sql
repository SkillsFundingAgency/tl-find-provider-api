CREATE PROCEDURE [dbo].[UpdateLocations]
	@data [dbo].[LocationDataTableType] READONLY
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
		d.[Longitude]
	FROM @data d
	INNER JOIN [dbo].[Provider] p
	ON p.[UkPrn] = d.[UkPrn])
	
		MERGE INTO [dbo].[Location] AS t
		USING ProviderLocationsCTE AS s
		ON
		(
		  t.[ProviderId] = s.[ProviderId] 
		  AND t.[Postcode] = s.[Postcode]
		)
		WHEN MATCHED 
			 AND (t.[Name] <> s.[Name]
				  OR ISNULL(NULLIF(t.[AddressLine1], s.[AddressLine1]), NULLIF(s.[AddressLine1], t.[AddressLine1])) IS NOT NULL
				  OR ISNULL(NULLIF(t.[AddressLine2], s.[AddressLine2]), NULLIF(s.[AddressLine2], t.[AddressLine2])) IS NOT NULL
				  OR ISNULL(NULLIF(t.[Town], s.[Town]), NULLIF(s.[Town], t.[Town])) IS NOT NULL
				  OR ISNULL(NULLIF(t.[County], s.[County]), NULLIF(s.[County], t.[County])) IS NOT NULL
				  OR ISNULL(NULLIF(t.[Email], s.[Email]), NULLIF(s.[Email], t.[Email])) IS NOT NULL
				  OR ISNULL(NULLIF(t.[Telephone], s.[Telephone]), NULLIF(s.[Telephone], t.[Telephone])) IS NOT NULL
				  OR ISNULL(NULLIF(t.[Website], s.[Website]), NULLIF(s.[Website], t.[Website])) IS NOT NULL
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
			[Location] 
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
			geography::Point(s.[Latitude], s.[Longitude], 4326)
		)

		WHEN NOT MATCHED BY SOURCE THEN 
		UPDATE SET
		  t.[IsDeleted] = 1	  ,
		  t.[ModifiedOn] = GETUTCDATE() --Soft delete

		OUTPUT $action, 
			INSERTED.Id, 
			INSERTED.IsDeleted
		INTO @ChangeSummary;

	WITH changesCte (Change) AS
	(SELECT	CASE
				WHEN Change = 'UPDATE' AND IsDeleted = 1
				THEN 'DELETE'
				ELSE Change
			END
		FROM @ChangeSummary)
		SELECT	Change, 
				COUNT(*) AS CountPerChange	 
		FROM changesCte
		GROUP BY Change;
