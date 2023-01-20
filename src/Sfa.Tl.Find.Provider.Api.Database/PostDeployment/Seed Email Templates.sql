/*
Insert initial data for Email Templates
*/

MERGE INTO [dbo].[EmailTemplate] AS Target 
USING (VALUES 
	(N'EmailDeliveryStatus', N'42c160e8-8cf5-4ca0-96cf-202aa224dc67'),
	(N'EmployerRegisterInterest', N'e4e773c1-09ed-4f66-82f9-289e6184b325'),
	(N'EmployerExtendInterest', N'e0ead036-07c9-4b67-80b0-9249be59908f'),
	(N'EmployerInterestRemoved', N'f250df3a-e23a-4762-bde5-938a48cfd2c2'),
	(N'ProviderNotification', N'e86be431-209d-4e58-9995-5b25d6ea73df'),
	(N'ProviderVerification', N'4cb980f5-b740-4e1c-9e20-36dcf808904e')
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
