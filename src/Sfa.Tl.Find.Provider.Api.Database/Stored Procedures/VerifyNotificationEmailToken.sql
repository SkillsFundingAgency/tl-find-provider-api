CREATE PROCEDURE [dbo].[VerifyNotificationEmailToken]
	@emailVerificationToken UNIQUEIDENTIFIER
AS
	SET NOCOUNT ON;

	DECLARE @id INT;
	DECLARE @email NVARCHAR(320);

	SELECT @id = [Id], 
		@email = [Email]
	FROM dbo.ProviderNotification 
	WHERE EmailVerificationToken = @emailVerificationToken

	IF( @id IS NOT NULL)
	BEGIN
		UPDATE dbo.ProviderNotification 
		SET EmailVerificationToken = NULL, 
			ModifiedOn = GETUTCDATE() 
		WHERE [Id] = @id
	END

	--select to return verified email (if any)
	SELECT @email AS [Email]
