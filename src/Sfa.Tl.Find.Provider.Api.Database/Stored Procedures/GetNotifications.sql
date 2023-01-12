CREATE PROCEDURE [dbo].[GetNotifications]
	@ukPrn BIGINT,
	@includeAdditionalData BIT
AS
	SET NOCOUNT ON;

	DECLARE @Notification TABLE (
		[Id] INT IDENTITY(1,1) NOT NULL,
		[LocationId] INT NOT NULL,
		[Email] NVARCHAR(320) NOT NULL,
		[SearchRadius] INT NOT NULL,
		[RouteId] INT NULL
	);

	WITH ProvidersCTE AS (
	SELECT	p.[Id],
			ROW_NUMBER() OVER(PARTITION BY p.[UkPrn] ORDER BY p.[IsAdditionalData]) AS ProviderRowNum
	FROM	[Provider] p
	WHERE	p.[UkPrn] = @ukPrn
	  AND	p.[IsDeleted] = 0
	  AND	(@includeAdditionalData = 1 OR (@includeAdditionalData = 0 AND p.[IsAdditionalData] = 0))
	)
	 SELECT n.[Id],
		   l.[Id] AS [LocationId],
		   l.[Name] AS [LocationName],
		   l.[Postcode],
		   n.[Email],
		   n.[SearchRadius],
		   r.[Id] AS [RouteId],
		   r.[Name] AS [RouteName]
		FROM ProvidersCTE p
		INNER JOIN	[dbo].[Location] l
	  	ON	p.[Id] = l.[ProviderId]
		--INNER JOIN [dbo].[Notification] n
		INNER JOIN @Notification n
		ON n.[LocationId] = l.[Id]
		LEFT JOIN [Route] r
		ON r.[Id] = n.[RouteId]
		WHERE	l.[IsDeleted] = 0
		  --Only include the first row to make sure main data set takes priority
		  AND	p.[ProviderRowNum] = 1
	ORDER BY	[LocationName],
				n.[Email],
				r.[Name]

