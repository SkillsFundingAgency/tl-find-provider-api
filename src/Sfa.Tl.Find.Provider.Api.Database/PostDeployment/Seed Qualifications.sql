/*
Insert initial data for Qualifications
*/

MERGE INTO [dbo].[Qualification] AS Target 
USING (VALUES 
  (36, N'Design, Surveying and Planning for Construction', 0),
  (37, N'Digital Production, Design and Development', 0),
  (38, N'Education and Early Years', 0),
  (39, N'Digital Business Services', 0),
  (40, N'Digital Support Services', 0),
  (41, N'Health', 0),
  (42, N'Healthcare Science', 0),
  (43, N'Science', 0),
  (45, N'Building Services Engineering for Construction', 0),
  (46, N'Finance', 0),
  (47, N'Accounting', 0),
  (48, N'Design and Development for Engineering and Manufacturing', 0),
  (49, N'Maintenance, Installation and Repair for Engineering and Manufacturing', 0),
  (50, N'Engineering, Manufacturing, Processing and Control', 0),
  (51, N'Management and Administration', 0),
  (52, N'Legal Services', 0),
  (53, N'Hairdressing, Barbering and Beauty Therapy', 0),
  (54, N'Craft and Design', 0),
  (55, N'Media, Broadcast and Production', 0),
  (56, N'Catering', 1),
  (57, N'Agriculture, Land Management and Production', 0),
  (58, N'Animal Care and Management', 0),
  (59, N'Marketing', 0)
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
