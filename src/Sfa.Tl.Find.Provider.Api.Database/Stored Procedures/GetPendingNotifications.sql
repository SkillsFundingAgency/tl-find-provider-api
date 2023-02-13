CREATE PROCEDURE [dbo].[GetPendingNotifications]
	@frequency [INT]
AS
	SET NOCOUNT ON;

	DECLARE @metersPerMile DECIMAL = 1609.3399999999999E0;

	WITH Notifications_CTE AS (
			SELECT	nl.[Id] AS [NotificationLocationId],
					pn.[Id] AS [ProviderNotificationId],
					pn.[Email],
					pn.[ProviderId],
					nl.[LocationId],
					nl.[SearchRadius] * @metersPerMile AS [SearchRadiusInMeters],
					nl.[LastNotificationDate],
					nl.[CreatedOn] AS [NotificationLocationCreatedOn]
			FROM [NotificationLocation] nl
			INNER JOIN [ProviderNotification] pn
			ON pn.[Id] = nl.[ProviderNotificationId]
			WHERE nl.[Frequency] = @frequency
			  AND pn.[EmailVerificationToken] IS NULL --Verified	
			),

		--Locations where an individual location was chosen
		IndividualLocations_CTE AS (
			SELECT n.[NotificationLocationId],
				  n.[ProviderNotificationId],
				  l.[Location]
			FROM Notifications_CTE n
			INNER JOIN	[dbo].[Location] l
			ON		l.[Id] = n.[LocationId]
			AND	l.[IsDeleted] = 0
		  ),

		--locations where "All" was selected 
		ProviderLocations_CTE AS (
			SELECT	n.[NotificationLocationId],
					n.[ProviderNotificationId],
					l.[Location]
			FROM Notifications_CTE n
			INNER JOIN	[dbo].[Location] l
			ON		n.[ProviderId] = l.[ProviderId]
			  AND	l.[IsDeleted] = 0
		  WHERE	n.[LocationId] IS NULL
		  ),

		Combined_CTE AS (
			SELECT	n.[Email],
					n.[NotificationLocationId],
					n.[LastNotificationDate],
					n.[NotificationLocationCreatedOn],
					n.[SearchRadiusInMeters],
					COALESCE(l.[Location], pl.[Location]) AS [Location],
					nlr.[RouteId]
			FROM Notifications_CTE n
			LEFT JOIN IndividualLocations_CTE l
			ON l.[NotificationLocationId] = n.[NotificationLocationId]
			LEFT JOIN ProviderLocations_CTE pl
			ON pl.[NotificationLocationId] = n.[NotificationLocationId]
			LEFT JOIN [dbo].[NotificationLocationRoute] nlr
			ON nlr.[NotificationLocationId] = n.[NotificationLocationId]
			)

		SELECT	c.[NotificationLocationId],
				c.[Email]
		FROM Combined_CTE c
		WHERE EXISTS(
				SELECT 1 
				FROM [dbo].[EmployerInterest] ei
				INNER JOIN EmployerInterestLocation el
				ON el.[EmployerInterestId] = ei.[Id]
				LEFT JOIN [EmployerInterestRoute] eir
				ON eir.[EmployerInterestId] = ei.[Id]
				WHERE c.[Location].STDistance(el.[Location]) <= c.[SearchRadiusInMeters]
				AND (ei.CreatedOn >= c.[NotificationLocationCreatedOn])
				AND (c.LastNotificationDate IS NULL 
					OR (ei.CreatedOn > c.LastNotificationDate))
				AND (c.[RouteId] IS NULL
					OR c.[RouteId] = eir.[RouteId])
		)
		GROUP BY c.[NotificationLocationId],
			c.[Email]
