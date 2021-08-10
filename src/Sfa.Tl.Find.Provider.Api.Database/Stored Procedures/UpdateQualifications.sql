CREATE PROCEDURE [dbo].[UpdateQualifications]
  @data [dbo].[QualificationDataTableType] READONLY
AS
	SET NOCOUNT ON;

	MERGE INTO [dbo].[Qualification] AS t
	USING @data AS s
	ON
	(
	  t.[Id] = s.[Id]
	)
	WHEN MATCHED 
		 AND (t.[Id] <> s.[Id] 
			  OR t.[Name] <> s.[Name]
			  OR t.[IsDeleted] = 0)
	THEN UPDATE SET
	  t.[Id]  = s.[Id],
	  t.[Name] = s.[Name],
	  t.[IsDeleted] = 0

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
	  t.[IsDeleted] = 1; --Soft delete
