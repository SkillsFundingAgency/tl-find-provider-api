/*
Insert initial data for Email Templates
*/

MERGE INTO [dbo].[EmailTemplate] AS Target 
USING (VALUES 
	(N'TestWithoutPersonalisation', N'1379cac6-2e92-49f3-b2a4-fbd5698c48e1'),
	(N'EmailDeliveryStatus', N'42c160e8-8cf5-4ca0-96cf-202aa224dc67')
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
