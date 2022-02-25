CREATE PROCEDURE [dbo].[UpdateProviderReferences]
	@data [dbo].[ProviderReferenceDataTableType] READONLY
AS
	SET NOCOUNT ON;

	DECLARE @ChangeSummary TABLE(
		Change VARCHAR(20),
		Id INT
	);

	MERGE INTO [dbo].[ProviderReference] AS t
	USING @data AS s
	ON
	(
		t.[UkPrn] = s.[UkPrn]
	)
	WHEN MATCHED 
		 AND (t.[Name] <> s.[Name] COLLATE Latin1_General_CS_AS
			  OR t.[Urn] <> s.[Urn])
	THEN UPDATE SET
		t.[Name] = s.[Name],
		t.[Urn] = s.[Urn],
		t.[ModifiedOn] = GETUTCDATE()

	WHEN NOT MATCHED BY TARGET THEN INSERT
	(
		[UkPrn],
		[Urn],
		[Name]
	)
	VALUES
	(
		s.[UkPrn],
		s.[Urn],
		s.[Name]
	)

	WHEN NOT MATCHED BY SOURCE 
	THEN DELETE

	OUTPUT $action, 
		INSERTED.[Id]
	INTO @ChangeSummary	;

	SELECT	Change, 
			COUNT(*) AS CountPerChange	 
	FROM @ChangeSummary
	GROUP BY Change;
