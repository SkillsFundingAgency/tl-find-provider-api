CREATE PROCEDURE [dbo].[DeleteEmployerInterestBeforeDate]
	@date DATETIME2
AS

	--SET NOCOUNT ON;
	
	DECLARE @returnCount INT
	DECLARE @employerInterestIds TABLE (
		[Id] INT);

	INSERT INTO @employerInterestIds
	SELECT [Id] 
	FROM [dbo].[EmployerInterest]
    WHERE ([CreatedOn] < @date
	  --TODO: Add logic to allow users to ask for extension on date
	  --		Might add a flag to the table for use here, together with the ModifiedOn
	  --AND [ModifiedOn] IS NULL
	  )
	  --OR [ModifiedOn] < @date

	EXEC @returnCount = [dbo].[DeleteEmployerInterest] @employerInterestIds
	RETURN @returnCount
