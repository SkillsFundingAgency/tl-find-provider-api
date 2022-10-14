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
:r ".\Seed Industries.sql"
:r ".\Seed Qualifications.sql"
:r ".\Seed Routes.sql"
:r ".\Seed RouteQualification.sql"

--Below code can be removed later
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmployerLocation]') AND type in (N'U'))
	PRINT 'This should cause a merge conflict - all this code can be removed'
GO
	