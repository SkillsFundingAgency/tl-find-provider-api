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

--TODO: Remove this statement and remove column 
--from table definition after it has been deleted in live
ALTER TABLE [dbo].[EmployerInterest] 
DROP COLUMN IF EXISTS [Postcode];

--To undo this:
/*
ALTER TABLE [dbo].[EmployerInterest] 
ADD [Postcode] NVARCHAR(10) NULL;

WITH cte AS (
	SELECT EmployerInterestId, Postcode
	FROM EmployerInterestLocation)
	UPDATE EmployerInterest 
	SET Postcode = cte.postcode
	FROM cte
	WHERE Id = cte.EmployerInterestId;
*/
