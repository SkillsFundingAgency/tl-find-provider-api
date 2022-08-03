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
	  		  OR ISNULL(NULLIF(t.[County] COLLATE Latin1_General_CS_AS, s.[County] COLLATE Latin1_General_CS_AS), 
						NULLIF(s.[County] COLLATE Latin1_General_CS_AS, t.[County] COLLATE Latin1_General_CS_AS))
				 IS NOT NULL
	  		  OR ISNULL(NULLIF(t.[LocalAuthority] COLLATE Latin1_General_CS_AS, s.[LocalAuthority] COLLATE Latin1_General_CS_AS), 
						NULLIF(s.[LocalAuthority] COLLATE Latin1_General_CS_AS, t.[LocalAuthority] COLLATE Latin1_General_CS_AS))
				 IS NOT NULL
			  OR t.[Latitude] <> s.[Latitude]
			  OR t.[Longitude] <> s.[Longitude])
	THEN UPDATE SET
		t.[Name] = s.[Name],
		t.[County] = s.[County],
		t.[LocalAuthority] = s.[LocalAuthority],
		t.[Latitude] = s.[Latitude],
		t.[Longitude] = s.[Longitude]

	WHEN NOT MATCHED BY TARGET THEN INSERT
	(
		[Id],
		[Name],
		[County],
		[LocalAuthority],
		[Latitude],
		[Longitude]
	)
	VALUES
	(
		s.[Id],
		s.[Name],
		s.[County],
		s.[LocalAuthority],
		s.[Latitude],
		s.[Longitude]
	)

	WHEN NOT MATCHED BY SOURCE 
	THEN DELETE

	OUTPUT $action, 
		INSERTED.[Id]
	INTO @ChangeSummary;

	SELECT	Change, 
			COUNT(*) AS CountPerChange	 
	FROM @ChangeSummary
	GROUP BY Change;
