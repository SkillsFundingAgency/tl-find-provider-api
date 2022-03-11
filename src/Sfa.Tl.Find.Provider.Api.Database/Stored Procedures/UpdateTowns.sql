CREATE PROCEDURE [dbo].[UpdateTowns]
	@data [dbo].[TownDataTableType] READONLY
AS
	SET NOCOUNT ON;

	DECLARE @ChangeSummary TABLE(
		Change VARCHAR(20),
		Id INT
	);

	MERGE INTO [dbo].[Town] AS t
	USING @data AS s
	ON
	(
		t.[Name] = s.[Name]
		AND t.[County] = s.[County]
	)
	WHEN MATCHED 
		 AND (t.[Latitude] <> s.[Latitude]
			  OR t.[Longitude] <> s.[Longitude])
	THEN UPDATE SET
		t.[Latitude] = s.[Latitude],
		t.[Longitude] = s.[Longitude]

	WHEN NOT MATCHED BY TARGET THEN INSERT
	(
		[Name],
		[County],
		[Latitude],
		[Longitude]
	)
	VALUES
	(
		s.[Name],
		s.[County],
		s.[Latitude],
		s.[Longitude]
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
