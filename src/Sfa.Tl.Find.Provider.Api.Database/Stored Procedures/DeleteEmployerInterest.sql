CREATE PROCEDURE [dbo].[DeleteEmployerInterest]
	@uniqueId UNIQUEIDENTIFIER
AS

	--SET NOCOUNT ON;
	
	WITH EmployerLocations_CTE AS (
		SELECT *
		FROM [dbo].[EmployerLocation]
		WHERE [EmployerInterestId] = 
				(SELECT [Id] 
				 FROM [dbo].[EmployerInterest]
				 WHERE [UniqueId] = @uniqueId)	 
		)
		DELETE FROM EmployerLocations_CTE;

	DELETE FROM [dbo].[EmployerInterest]
	WHERE [UniqueId] = @uniqueId

