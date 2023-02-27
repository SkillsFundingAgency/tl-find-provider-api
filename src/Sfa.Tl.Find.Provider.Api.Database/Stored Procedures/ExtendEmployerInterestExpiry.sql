CREATE PROCEDURE [dbo].[ExtendEmployerInterestExpiry]
	@uniqueId UNIQUEIDENTIFIER, 
    @numberOfDaysToExtend INT,
    @expiryNotificationDays INT,
    @maximumExtensions INT
AS
	SET NOCOUNT ON;

	DECLARE @extensionSucceeded BIT
    DECLARE @remainingExtensions INT

	UPDATE [dbo].[EmployerInterest]
	SET [ExpiryDate] = DATEADD(day, @numberOfDaysToExtend, [ExpiryDate]),
	    [ExtensionCount] = ExtensionCount + 1,
	    [ModifiedOn] = GETUTCDATE()
	WHERE [UniqueId] = @uniqueId
	  AND [ExpiryDate] < DATEADD(day, @expiryNotificationDays + 1, GETUTCDATE())
	  AND [ExtensionCount] < @maximumExtensions

	SELECT @extensionSucceeded = 
		CASE 
			WHEN @@ROWCOUNT > 0 THEN 1 
			ELSE 0 
		END

	SELECT 	@remainingExtensions = 
		CASE
			WHEN [ExtensionCount] < @maximumExtensions
			THEN @maximumExtensions - [ExtensionCount]
		ELSE 0 END
	FROM [dbo].[EmployerInterest]
	WHERE [UniqueId] = @uniqueId

	SELECT @extensionSucceeded AS [Success], 
		COALESCE(@remainingExtensions, 0) AS [ExtensionsRemaining]
