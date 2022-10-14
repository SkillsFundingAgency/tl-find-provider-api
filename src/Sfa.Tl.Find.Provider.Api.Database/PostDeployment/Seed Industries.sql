/*
Insert initial data for Industries
*/

SET IDENTITY_INSERT [dbo].[Industry] ON

MERGE INTO [dbo].[Industry] AS Target 
USING (VALUES 
  (1,  N'Admin and Business Support', 0),
  (2,  N'Agriculture', 0),
  (3,  N'Arts, Entertainment and Recreation', 0),
  (4,  N'Construction', 0),
  (5,  N'Education and Childcare', 0),
  (6,  N'Financial Services', 0),
  (7,  N'Health and Social Care', 0),
  (8,  N'Hospitality', 0),
  (9,  N'IT and Communications', 0),
  (10, N'Manufacturing and Engineering', 0),
  (11, N'Public Sector', 0),
  (12, N'Retail and Wholesale', 0),
  (13, N'Transport', 0),
  (14, N'Utilities', 0)
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
