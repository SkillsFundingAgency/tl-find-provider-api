CREATE PROCEDURE [dbo].[UpdateProviders]
	@data [dbo].[ProviderDataTableType] READONLY
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
	)
	WHEN MATCHED 
		 AND (t.[Name] <> s.[Name]
			  OR t.[Name] <> t.[Name]
			  OR t.[AddressLine1] <> t.[AddressLine1] 
			  OR t.[AddressLine2] <> t.[AddressLine2]
			  OR t.[Town] <> t.[Town]
			  OR t.[County] <> t.[County]
			  OR t.[Postcode] <> t.[Postcode]
			  OR t.[Email] <> t.[Email]
			  OR t.[Telephone] <> t.[Telephone]
			  OR t.[Website] <> t.[Website]
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
		[Website]
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
		s.[Website]
	)

	WHEN NOT MATCHED BY SOURCE THEN 
	UPDATE SET
	  t.[IsDeleted] = 1	  ,
	  t.[ModifiedOn] = GETUTCDATE() --Soft delete

	OUTPUT $action, 
		INSERTED.Id, 
		INSERTED.IsDeleted
	INTO @ChangeSummary	;

	WITH cte (Change) AS
	(SELECT	CASE
				WHEN Change = 'UPDATE' AND IsDeleted = 1
				THEN 'DELETE'
				ELSE Change
			END
		FROM @ChangeSummary)
		SELECT	Change, 
				COUNT(*) AS CountPerChange	 
		FROM cte
		GROUP BY Change;
