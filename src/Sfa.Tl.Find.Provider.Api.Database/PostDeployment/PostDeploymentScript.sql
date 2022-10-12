/*
Post-Deployment Script 
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

:r ".\Seed Email Templates.sql"
:r ".\Seed Qualifications.sql"
:r ".\Seed Routes.sql"
:r ".\Seed RouteQualification.sql"

--This can be removed after qualifications have been cleanup in production
--The seed scripts will soft-delete the old qualifications and replace them in RouteQualification
DELETE FROM [dbo].[Qualification]
WHERE [Id] > 1000;
