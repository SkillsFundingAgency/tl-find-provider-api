CREATE PROCEDURE [dbo].[GetPendingNotifications]
	@frequency [INT]
AS

	SET NOCOUNT ON;

	--TODO: Pass this in
	DECLARE @minutesForImmediateNotification [INT] = 15

	DECLARE @metersPerMile DECIMAL = 1609.3399999999999E0;

	DECLARE @minDate [DATETIME2] = CASE 
		WHEN @frequency = 1 --Immediately
			THEN DATEADD(minute, -15, GETUTCDATE())
		WHEN  @frequency = 2 --Daily
			THEN DATEADD(day, -1, CAST(CONVERT(CHAR(8), GETUTCDATE(), 112) AS DATETIME2))
		WHEN  @frequency = 3 --Weekly
			THEN DATEADD(day, -7, CAST(CONVERT(CHAR(8), GETUTCDATE(), 112) AS DATETIME2))
	END;

	WITH Notifications_CTE AS (
		SELECT	nl.[Id] AS [NotificationLocationId],
				pn.[Id] AS [ProviderNotificationId],
				nl.[Frequency],
				nl.[SearchRadius],
				pn.[Email],
				pn.[ProviderId],
				nl.[LocationId],
				nl.[LastNotificationDate],
				nl.[CreatedOn]
		FROM [NotificationLocation] nl
		INNER JOIN [ProviderNotification] pn
		ON pn.[Id] = nl.[ProviderNotificationId]
		WHERE nl.[Frequency] = @frequency
		  AND pn.[EmailVerificationToken] IS NULL --Verified
--date criteria looks wrong
		  AND ((nl.[LastNotificationDate] IS NOT NULL 
			   AND nl.[LastNotificationDate] >= @minDate)
			   OR (nl.[LastNotificationDate] IS NULL 
			   AND nl.[CreatedOn] >= @minDate))
			   ),

		--locations where there is a set location
		Locations_CTE AS (
			SELECT n.[NotificationLocationId],
				  n.[ProviderNotificationId],
				  n.[Frequency],
				  n.[SearchRadius],
				  n.[LocationId],
				  l.[Postcode],
				  l.[Latitude],
				  l.[Longitude],
				  l.[Location]
		FROM Notifications_CTE n
		--INNER JOIN [NotificationLocation] nl
		--ON pn.[Id] = nl.[ProviderNotificationId]
		INNER JOIN	[dbo].[Location] l
		ON		l.[Id] = n.[LocationId]
		AND	l.[IsDeleted] = 0
		  ),

		--locations where "all" was selected 
		ProviderLocations_CTE AS (
		SELECT	n.[NotificationLocationId],
				n.[ProviderNotificationId],
				n.[Frequency],
				n.[SearchRadius],
				l.[Id] AS [LocationId],
				l.[Name],
				l.[Postcode],
				l.[Latitude],
				l.[Longitude],
				l.[Location]
		FROM Notifications_CTE n
		INNER JOIN	[dbo].[Location] l
		ON		n.[ProviderId] = l.[ProviderId]
		  AND	l.[IsDeleted] = 0
		  WHERE	n.[LocationId] IS NULL
		  ),

		  Combined_CTE AS (
		   SELECT	n.[Email],
					n.[ProviderNotificationId],
					n.[NotificationLocationId],
					n.[LastNotificationDate],
					n.[Frequency],
					n.[SearchRadius],
					n.[SearchRadius] * @metersPerMile AS [SearchRadiusInMeters],
					COALESCE(l.[LocationId], pl.[LocationId]) AS [LocationId],
					COALESCE(l.[Postcode], pl.[Postcode]) AS [Postcode], 
					COALESCE(l.[Latitude], pl.[Latitude]) AS [Latitude], 
					COALESCE(l.[Longitude], pl.[Longitude]) AS [Longitude],
					COALESCE(l.[Location], pl.[Location]) AS [Location]
		   FROM Notifications_CTE n
		   LEFT JOIN Locations_CTE l
		   ON l.[NotificationLocationId] = n.[NotificationLocationId]
		   LEFT JOIN ProviderLocations_CTE pl
		   ON pl.[NotificationLocationId] = n.[NotificationLocationId]
		   )
		   		   
		   SELECT	c.[Email],
					c.[ProviderNotificationId],
					c.[NotificationLocationId],
					c.[LastNotificationDate],
					c.[Frequency],
					c.[SearchRadius],
					c.[SearchRadiusInMeters],
					c.[LocationId],
					c.[Postcode], 
					c.[Latitude], 
					c.[Longitude],
					c.[Location]
		   FROM Combined_CTE c
		   ORDER BY c.[Email],
		   c.[Postcode]
