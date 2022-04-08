/*
Insert initial data for Routes
*/

SET IDENTITY_INSERT [dbo].[Route] ON

MERGE INTO [dbo].[Route] AS Target 
USING (VALUES 
  (1, N'Agriculture, environment and animal care', 0),
  (2, N'Business and administration', 0),
  (3, N'Catering', 0),
  (4, N'Construction and the built environment', 0),
  (5, N'Creative and design', 0),
  (6, N'Digital and IT', 0),
  (7, N'Education and childcare', 0),
  (8, N'Engineering and manufacturing', 0),
  (9, N'Hair and beauty', 0),
  (10, N'Health and science', 0),
  (11, N'Legal, finance and accounting', 0)
  )
  AS Source ([Id], [Name], [IsDeleted] ) 
ON Target.[Id] = Source.[Id] 
WHEN MATCHED 
	 AND (Target.[Name] <> Source.[Name] COLLATE Latin1_General_CS_AS
	  OR Target.[IsDeleted] <> Source.[IsDeleted])
THEN 
UPDATE SET 
	[Name] = Source.[Name],
	[ModifiedOn] = GETDATE(),
    [IsDeleted] = Source.[IsDeleted]
WHEN NOT MATCHED BY TARGET THEN 
	INSERT ([Id], [Name], [IsDeleted]) 
	VALUES ([Id], [Name], [IsDeleted]) 
WHEN NOT MATCHED BY SOURCE THEN 
UPDATE SET 
	[ModifiedOn] = GETDATE(),
    [IsDeleted] = 1
;

SET IDENTITY_INSERT [dbo].[Route] OFF
