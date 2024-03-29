﻿/*
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

--Set up tables for Quartz Job Store
:r ".\CreateQuartzJobStore.sql"
