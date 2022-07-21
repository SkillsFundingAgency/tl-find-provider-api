/*
Insert initial data for Qualifications

This is an insert-only script - the nightly import will manage any changes

*/

--TODO: Remove this delete when we have the final framework codes below
DELETE FROM [dbo].[Qualification]
WHERE [Id] > 1000

MERGE INTO [dbo].[Qualification] AS Target 
USING (VALUES 
  (36, N'Design, Surveying and Planning for Construction'),
  (37, N'Digital Production, Design and Development'),
  (38, N'Education and Childcare'),
  (39, N'Digital Business Services'),
  (40, N'Digital Support Services'),
  (41, N'Health'),
  (42, N'Healthcare Science'),
  (43, N'Science'),
  (44, N'Onsite Construction'),
  (45, N'Building Services Engineering for Construction'),
  (46, N'Finance'),
  (47, N'Accounting'),
  (48, N'Design and Development for Engineering and Manufacturing'),
  (49, N'Maintenance, Installation and Repair for Engineering and Manufacturing'),
  (50, N'Engineering, Manufacturing, Processing and Control'),
  (51, N'Management and Administration'),
  --TODO: Fix the framework codes and hard-delete > 1000
  (1052, N'Agriculture, Land Management and Production'),
  (1053, N'Animal Care and Management'),
  (1054, N'Catering'),
  (1055, N'Craft and Design'),
  (1056, N'Media, Broadcast and Production'),
  (1057, N'Hair, Beauty and Aesthetics'),
  (1058, N'Legal Services')
  )
  AS Source ([Id], [Name]) 
ON Target.[Id] = Source.[Id] 
WHEN NOT MATCHED BY TARGET THEN 
	INSERT ([Id], [Name], [IsDeleted]) 
	VALUES ([Id], [Name], 0) 
;
