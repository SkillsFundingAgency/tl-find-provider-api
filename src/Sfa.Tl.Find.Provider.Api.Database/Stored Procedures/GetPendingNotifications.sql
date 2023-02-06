CREATE PROCEDURE [dbo].[GetPendingNotifications]
	@frequency [INT]
AS

	SET NOCOUNT ON;

	--TODO: Pass this in
	DECLARE @minutesForImmediateNotification [INT] = 15

	DECLARE @date [DATETIME2] = CASE 
		WHEN @frequency = 1 --Immediately
			THEN DATEADD(minute, -15, GETUTCDATE())
		WHEN  @frequency = 2 --Daily
			THEN DATEADD(day, -1, CAST(CONVERT(CHAR(8), GETUTCDATE(), 112) AS DATETIME2))
		WHEN  @frequency = 3 --Weekly
			THEN DATEADD(day, -7, CAST(CONVERT(CHAR(8), GETUTCDATE(), 112) AS DATETIME2))
	END

	SELECT nl.[Id] AS [NotificationLocationId],
			pn.[Email]
			,nl.[LastNotificationDate]
	FROM [NotificationLocation] nl
	INNER JOIN [ProviderNotification] pn
	ON pn.[Id] = nl.[ProviderNotificationId]
	WHERE nl.[Frequency] = @frequency
	  AND pn.[EmailVerificationToken] IS NULL --Verified
	  AND (nl.[LastNotificationDate] IS NOT NULL 
	       OR nl.[CreatedOn] >= @date)
