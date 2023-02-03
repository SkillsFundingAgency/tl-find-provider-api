CREATE PROCEDURE [dbo].[GetProviderNotificationLocations]
	@providerNotificationId INT
AS
	SET NOCOUNT ON;

	WITH ProviderLocationsCTE AS (
		SELECT pn.Id AS [ProviderNotificationId],
				l.[Id] AS [LocationId],
				 l.[Name],
				 l.[Postcode]
		FROM [ProviderNotification] pn
		INNER JOIN [dbo].[Provider] p
  		ON	p.[Id] = pn.[ProviderId]
		INNER JOIN	[dbo].[Location] l
		ON		p.[Id] = l.[ProviderId]
		WHERE	pn.[Id] = @providerNotificationId
		  AND	l.[IsDeleted] = 0),

	NotificationLocationsCTE AS (
		SELECT pn.Id AS [ProviderNotificationId],
				nl.[Id] AS [NotificationLocationId],
				nl.[LocationId] AS [LocationId]
		FROM [ProviderNotification] pn
		LEFT JOIN [dbo].[NotificationLocation] nl
		ON	nl.[ProviderNotificationId] = pn.[Id]
		WHERE	pn.[Id] = @providerNotificationId
	)

	SELECT  nl.[NotificationLocationId] AS [Id],
			pl.[LocationId],
			pl.[Name],
			pl.[Postcode]
	FROM NotificationLocationsCTE nl
	FULL JOIN ProviderLocationsCTE pl
	ON pl.[LocationId] = nl.[LocationId]
	ORDER BY pl.[Name]
	;