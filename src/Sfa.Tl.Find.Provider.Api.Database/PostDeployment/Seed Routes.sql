/*
Insert initial data for Routes
*/

SET IDENTITY_INSERT [dbo].[Route] ON

MERGE INTO [dbo].[Route] AS Target 
USING (VALUES 
  (1, N'Agriculture, Environment and Animal Care', 0),
  (2, N'Business and Administration', 0),
  (3, N'Catering', 0),
  (4, N'Construction and the Built Environment', 0),
  (5, N'Creative and Design', 0),
  (6, N'Digital and IT', 0),
  (7, N'Education and Early Years', 0),
  (8, N'Engineering and Manufacturing', 0),
  (9, N'Hair and Beauty', 1),
  (10, N'Health and Science', 0),
  (11, N'Legal, Finance and Accounting', 0)
  )
  AS Source ([Id], [Name], [IsDeleted]) 
ON Target.[Id] = Source.[Id] 
WHEN MATCHED 
	 AND (Target.[Name] <> Source.[Name] COLLATE Latin1_General_CS_AS
	  OR Target.[IsDeleted] <> Source.[IsDeleted])
THEN 
UPDATE SET 
	[Name] = Source.[Name],
	[ModifiedOn] = GETUTCDATE(),
    [IsDeleted] = Source.[IsDeleted]
WHEN NOT MATCHED BY TARGET THEN 
	INSERT ([Id], [Name], [IsDeleted]) 
	VALUES ([Id], [Name], [IsDeleted]) 
WHEN NOT MATCHED BY SOURCE THEN 
UPDATE SET 
	[ModifiedOn] = GETUTCDATE(),
    [IsDeleted] = 1
;

SET IDENTITY_INSERT [dbo].[Route] OFF
