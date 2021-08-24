CREATE PROCEDURE [dbo].[UpdateQualifications]
	@data [dbo].[QualificationDataTableType] READONLY
AS
	SET NOCOUNT ON;

	DECLARE @ChangeSummary TABLE(
		Change VARCHAR(20),
		Id INT,
		IsDeleted BIT
	);

	MERGE INTO [dbo].[Qualification] AS t
	USING @data AS s
	ON
	(
		t.[Id] = s.[Id]
	)
	WHEN MATCHED 
		 AND (t.[Name] <> s.[Name] COLLATE Latin1_General_CS_AS
			  OR t.[IsDeleted] = 1) --To undelete
	THEN UPDATE SET
		t.[Name] = s.[Name],
		t.[IsDeleted] = 0,
		t.[ModifiedOn] = GETUTCDATE()

	WHEN NOT MATCHED BY TARGET THEN INSERT
	(
		[Id],
		[Name]
	)
	VALUES
	(
		s.[Id],
		s.[Name]
	)

	WHEN NOT MATCHED BY SOURCE THEN 
	UPDATE SET
		t.[IsDeleted] = 1	  ,
		t.[ModifiedOn] = GETUTCDATE() --Soft delete

	OUTPUT $action, 
		INSERTED.Id, 
		INSERTED.IsDeleted
	INTO @ChangeSummary	;

	WITH ChangesCTE (Change) AS
	(SELECT	CASE
				WHEN Change = 'UPDATE' AND IsDeleted = 1
				THEN 'DELETE'
				ELSE Change
			END
		FROM @ChangeSummary)
		SELECT	Change, 
				COUNT(*) AS CountPerChange	 
		FROM ChangesCTE
		GROUP BY Change;
