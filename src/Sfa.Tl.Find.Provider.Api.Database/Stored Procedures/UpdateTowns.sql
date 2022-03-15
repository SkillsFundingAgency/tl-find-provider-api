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
		t.[Id] = s.[Id]
	)
	WHEN MATCHED 
		 AND (t.[Name] <> s.[Name]
			  OR t.[County] <> s.[County]
			  OR t.[LocalAuthorityName] <> s.[LocalAuthorityName]
			  OR t.[Latitude] <> s.[Latitude]
			  OR t.[Longitude] <> s.[Longitude])
	THEN UPDATE SET
		t.[Name] = s.[Name],
		t.[County] = s.[County],
		t.[LocalAuthorityName] = s.[LocalAuthorityName],
		t.[Latitude] = s.[Latitude],
		t.[Longitude] = s.[Longitude]

	WHEN NOT MATCHED BY TARGET THEN INSERT
	(
		[Id],
		[Name],
		[County],
		[LocalAuthorityName],
		[Latitude],
		[Longitude]
	)
	VALUES
	(
		s.[Id],
		s.[Name],
		s.[County],
		s.[LocalAuthorityName],
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
