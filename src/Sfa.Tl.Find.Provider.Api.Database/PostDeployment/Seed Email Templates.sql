/*
Insert initial data for Email Templates
*/

MERGE INTO [dbo].[EmailTemplate] AS Target 
USING (VALUES 
	(N'EmailDeliveryStatus', N'42c160e8-8cf5-4ca0-96cf-202aa224dc67'),
	(N'EmployerRegisterInterest', N'e4e773c1-09ed-4f66-82f9-289e6184b325')
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
