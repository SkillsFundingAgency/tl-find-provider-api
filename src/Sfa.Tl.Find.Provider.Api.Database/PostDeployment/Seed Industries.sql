/*
Insert initial data for Industries
*/

SET IDENTITY_INSERT [dbo].[Industry] ON

MERGE INTO [dbo].[Industry] AS Target 
USING (VALUES 
  (1, N'Agriculture, Environment and Animal Care', N'Agriculture', 0),
  (2, N'Business and Administration', N'Business', 0),
  (3, N'Catering', N'Catering', 0),
  (4, N'Construction and the Built Environment', N'Construction', 0),
  (5, N'Creative and Design',N'Creative and Design', 0),
  (6, N'Digital and IT', 'Digital', 0),
  (7, N'Education and Childcare', N'Education', 0),
  (8, N'Engineering and Manufacturing', N'Engineering', 0),
  (9, N'Hair and Beauty',N'Hair and Beauty', 0),
  (10, N'Health and Science',N'Health and Science', 0),
  (11, N'Legal, Finance and Accounting', N'Legal, Finance and Accounting', 0),
  (12, N'Other', N'Other', 0)
  )
  AS Source ([Id], [Name], [ShortName], [IsDeleted]) 
ON Target.[Id] = Source.[Id] 
WHEN MATCHED 
	 AND (Target.[Name] <> Source.[Name] COLLATE Latin1_General_CS_AS
	  OR Target.[IsDeleted] <> Source.[IsDeleted])
THEN 
UPDATE SET 
	[Name] = Source.[Name],
	[ShortName] = Source.[ShortName],
	[ModifiedOn] = GETDATE(),
    [IsDeleted] = Source.[IsDeleted]
WHEN NOT MATCHED BY TARGET THEN 
	INSERT ([Id], [Name], [ShortName], [IsDeleted]) 
	VALUES ([Id], [Name], [ShortName], [IsDeleted]) 
WHEN NOT MATCHED BY SOURCE THEN 
UPDATE SET 
	[ModifiedOn] = GETDATE(),
    [IsDeleted] = 1
;

SET IDENTITY_INSERT [dbo].[Industry] OFF
