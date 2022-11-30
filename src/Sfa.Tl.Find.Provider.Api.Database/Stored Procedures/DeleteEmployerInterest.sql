CREATE PROCEDURE [dbo].[DeleteEmployerInterest]
	@employerInterestIds [dbo].[IdListTableType] READONLY,
	@employerInterestsDeleted INT OUTPUT
AS

	SET NOCOUNT ON;
	
	DELETE FROM [dbo].[EmployerInterestLocation]
	WHERE [EmployerInterestId] IN (SELECT [Id] FROM @employerInterestIds);

	SELECT @employerInterestsDeleted = @@ROWCOUNT

	DELETE FROM [dbo].[EmployerInterestIndustry]
	WHERE [EmployerInterestId] IN (SELECT [Id] FROM @employerInterestIds);

	DELETE FROM [dbo].[EmployerInterestRoute]
	WHERE [EmployerInterestId] IN (SELECT [Id] FROM @employerInterestIds);

	DELETE FROM [dbo].[EmployerInterest]
	WHERE [Id] IN (SELECT [Id] FROM @employerInterestIds);
