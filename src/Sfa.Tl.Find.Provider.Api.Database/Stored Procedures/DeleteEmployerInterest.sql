CREATE PROCEDURE [dbo].[DeleteEmployerInterest]
	@uniqueId UNIQUEIDENTIFIER
AS

	--SET NOCOUNT ON;
	
	DECLARE @employerInterestIds TABLE (
		[Id] INT);

	INSERT INTO @employerInterestIds
	SELECT [Id] 
	FROM [dbo].[EmployerInterest]
    WHERE [UniqueId] = @uniqueId

	DELETE FROM [dbo].[EmployerInterestLocation]
	WHERE [EmployerInterestId] IN (SELECT [Id] FROM @employerInterestIds);

	DELETE FROM [dbo].[EmployerInterestIndustry]
	WHERE [EmployerInterestId] IN (SELECT [Id] FROM @employerInterestIds);

	DELETE FROM [dbo].[EmployerInterestRoute]
	WHERE [EmployerInterestId] IN (SELECT [Id] FROM @employerInterestIds);

	DELETE FROM [dbo].[EmployerInterest]
	WHERE [UniqueId] = @uniqueId

	RETURN (SELECT COUNT(*) FROM @employerInterestIds)
