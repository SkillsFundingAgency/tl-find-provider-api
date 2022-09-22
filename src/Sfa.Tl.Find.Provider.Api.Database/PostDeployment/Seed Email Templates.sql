/*
Insert initial data for Email Templates
*/

MERGE INTO [dbo].[EmailTemplate] AS Target 
USING (VALUES 
	(N'TestWithoutPersonalisation', N'1379cac6-2e92-49f3-b2a4-fbd5698c48e1'),
	(N'EmployerRegisterInterest', N'7aae8b4b-0210-4801-8bfb-87ea36c09c80')
  )
  AS Source ([Name], [TemplateId]) 
ON Target.[Name] = Source.[Name] 
-- Update from Source when Name is Matched
WHEN MATCHED 
	 AND (Target.[TemplateId] <> Source.[TemplateId]) 
THEN 
UPDATE SET 
	[Name] = Source.[Name],
	[TemplateId] = Source.[TemplateId],
	[ModifiedOn] = GETDATE()
WHEN NOT MATCHED BY TARGET THEN 
	INSERT ([Name], [TemplateId]) 
	VALUES ([Name], [TemplateId]) 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;
