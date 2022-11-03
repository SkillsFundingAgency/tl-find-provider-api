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

--Clean up employer interest - it will be changed a lot in next release
--These statements should be removed in the Employer Interest branch
--IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmployerLocation]') AND type in (N'U'))
--	DROP TABLE [dbo].[EmployerLocation]
--GO
--BEGIN
--IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmployerInterest]') AND type in (N'U'))
--	AND NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmployerInterestLocation]') AND type in (N'U'))
--	DROP TABLE [dbo].[EmployerInterest]
--END
--GO
