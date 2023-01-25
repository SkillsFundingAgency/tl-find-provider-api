﻿CREATE PROCEDURE [dbo].[GetNotificationSummary]
	@ukPrn BIGINT,
	@includeAdditionalData BIT
AS
	SET NOCOUNT ON;
		
	WITH ProvidersCTE AS (
	SELECT	p.[Id],
			ROW_NUMBER() OVER(PARTITION BY p.[UkPrn] ORDER BY p.[IsAdditionalData]) AS ProviderRowNum
	FROM	[Provider] p
	WHERE	p.[UkPrn] = @ukPrn
	  AND	p.[IsDeleted] = 0
	  AND	(@includeAdditionalData = 1 OR (@includeAdditionalData = 0 AND p.[IsAdditionalData] = 0))
	)
	 SELECT n.[Id],
		   e.[Email],
		   CASE	WHEN e.VerificationToken IS NULL THEN 1
				ELSE 0
			END AS [IsEmailVerified],
		   l.[Id] AS [LocationId],
		   l.[Name] AS [LocationName],
		   l.[Postcode]
		FROM ProvidersCTE p
		INNER JOIN	[dbo].[ProviderNotification] pn
	  	ON	pn.[ProviderId] = p.[Id]
		INNER JOIN [dbo].[Notification] n
		ON n.[Id] = pn.[NotificationId]
		INNER JOIN [dbo].[NotificationEmail] e
		ON e.[NotificationId] = n.[Id]
		LEFT JOIN	[dbo].[Location] l
	  	ON	l.[Id] = n.[LocationId]
		WHERE	ISNULL(l.[IsDeleted], 0) = 0
		  --Only include the first row to make sure main data set takes priority
		  AND	p.[ProviderRowNum] = 1
	ORDER BY	e.[Email],
				[LocationName]
