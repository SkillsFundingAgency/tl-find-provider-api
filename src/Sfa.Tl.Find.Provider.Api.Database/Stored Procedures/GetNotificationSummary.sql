CREATE PROCEDURE [dbo].[GetNotificationSummary]
	@ukPrn BIGINT
AS
	SET NOCOUNT ON;
	
	WITH ProvidersCTE AS (
	SELECT	p.[Id]
	FROM	[Provider] p
	WHERE	p.[UkPrn] = @ukPrn
	  AND	p.[IsDeleted] = 0
	)
		SELECT n.[Id],
		   n.[Email],
		   CASE	WHEN n.EmailVerificationToken IS NULL THEN 1
				ELSE 0
			END AS [IsEmailVerified],
		   nl.[Id] AS [NotificationLocationId],
		   l.[Id] AS [LocationId],
		   l.[Name] AS [LocationName],
		   l.[Postcode]
		FROM ProvidersCTE p
		INNER JOIN [dbo].[ProviderNotification] n
	  	ON	n.[ProviderId] = p.[Id]
		LEFT JOIN [dbo].[NotificationLocation] nl
  		ON	nl.[ProviderNotificationId] = n.[Id]
		LEFT JOIN	[dbo].[Location] l
  		ON	l.[Id] = nl.[LocationId]		
		WHERE	ISNULL(l.[IsDeleted], 0) = 0
		ORDER BY	n.[Email],
				[LocationName]
