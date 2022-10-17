CREATE PROCEDURE [dbo].[DeleteEmployerInterestByUniqueId]
	@uniqueId UNIQUEIDENTIFIER
AS

	--SET NOCOUNT ON;
	
	DECLARE @returnCount INT
	DECLARE @employerInterestIds TABLE (
		[Id] INT);

	INSERT INTO @employerInterestIds
	SELECT [Id] 
	FROM [dbo].[EmployerInterest]
    WHERE [UniqueId] = @uniqueId

	EXEC @returnCount = [dbo].[DeleteEmployerInterest] @employerInterestIds
	RETURN @returnCount

