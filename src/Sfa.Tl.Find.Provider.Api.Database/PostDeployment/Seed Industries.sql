/*
Insert initial data for Industries
*/

SET IDENTITY_INSERT [dbo].[Industry] ON

MERGE INTO [dbo].[Industry] AS Target 
USING (VALUES 
  (1,  N'Admin and business support', 0),
  (2,  N'Agriculture', 0),
  (3,  N'Construction', 0),
  (4,  N'Retail and wholesale', 0),
  (5,  N'Health and social care', 0),
  (6,  N'Education and childcare', 0),
  (7,  N'Transport', 0),
  (8,  N'Utilities', 0),
  (9,  N'Manufacturing and engineering', 0),
  (10, N'Arts, entertainment, and recreation', 0),
  (11, N'Financial services', 0),
  (12, N'Public sector', 0),
  (13, N'IT and communications', 0),
  (14, N'Hospitality', 0),
  (15, N'Other', 0)
  )
  AS Source ([Id], [Name], [IsDeleted]) 
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

SET IDENTITY_INSERT [dbo].[Industry] OFF
