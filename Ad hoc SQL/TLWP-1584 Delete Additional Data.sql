/*
TLWP-1584 - Remove temporary provider data 
TLWP-1921 - Transfer notifications linked to the workaround Providers to the Course Directory equivalent
*/

/*
For local run:
Restore from D:\DBBackup\TLevelProviders_24Feb2023_2.bak
USE TLevelProviders
USE TLevelProviders_TEST
*/

BEGIN TRANSACTION

--Integrity checks - shoul show no records
SELECT * FROM [Location] l
WHERE ProviderId in
(SELECT Id FROM [Provider]
 WHERE IsAdditionalData = 1)
 AND IsAdditionalData <> 1

 SELECT * FROM [LocationQualification] l
WHERE LocationId in
(SELECT Id FROM [Location]
 WHERE IsAdditionalData = 1)
 AND IsAdditionalData <> 1


--SELECT * FROM [dbo].[Provider] WHERE [IsAdditionalData] = 1
/*
--Set up test provider simulating case where user added filters/notifications 
-- then moved to course directory.
--Need to set up the filters and notifications in the web page after running this
UPDATE [dbo].[Provider] 
SET [IsDeleted] = 1
WHERE [UkPrn] = 10000055 
  AND [IsAdditionalData] = 0

--SELECT * FROM [dbo].[Provider] WHERE [UkPrn] = 10000055 
--UPDATE [dbo].[Provider] 
--SET [IsDeleted] = 0
--WHERE [UkPrn] = 10000055 

DECLARE @providerId INT
DECLARE @locationId INT

select @providerId = id 
from [dbo].[Provider]
where ukprn = 10000055
and IsAdditionalData = 1

INSERT INTO [Location]
			([ProviderId]
			,[Postcode]
			,[Name]
			,[AddressLine1]
			,[AddressLine2]
			,[Town]
			,[County]
			,[Email]
			,[Telephone]
			,[Website]
			,[Latitude]
			,[Longitude]
			,[Location]
			,[IsAdditionalData]
)
    VALUES
		(@providerId, 'OX2 9EG', 'Bovis Sales', 'Harcourt Hill', NULL, 'Oxford', NULL, 'enquiries@vistry.ac.uk', '01235 555585', 'https://www.vistry.co.uk/', 51.743496, -1.296747, geography::Point(51.743496, -1.296747, 4326), 1)

SELECT @locationId = SCOPE_IDENTITY()
INSERT INTO [dbo].[LocationQualification]
           ([DeliveryYear]
           ,[LocationId]
           ,[QualificationId])
     VALUES
		 (2022,	@locationId, 41)
		,(2022,	@locationId, 43)
		,(2023,	@locationId, 37)
		,(2023,	@locationId, 47)
		,(2023,	@locationId, 52)

SELECT * FROM [dbo].[Provider] p
LEFT JOIN [Location] l
ON l.[ProviderId] = p.[Id]
WHERE [UkPrn] = 10000055
ORDER BY p.[Name],
		 p.[IsAdditionalData],
		 l.[Postcode]
*/

--Copy search filters to the "Clean" providers
--Copy notifications to the "Clean" providers

SELECT * 
FROM SearchFilter sf
INNER JOIN [Location] l
ON l.[Id] = sf.[LocationId]
INNER JOIN [Provider] p
ON p.[Id] = l.[ProviderId]

--EXEC DeleteSearchFilter 1002
--EXEC DeleteSearchFilter 2002

--EXEC DeleteProviderNotification 2002
--EXEC DeleteProviderNotification 5023
--EXEC DeleteProviderNotification 5024
--EXEC DeleteProviderNotification 5025
--EXEC DeleteProviderNotification 5026
--EXEC DeleteProviderNotification 5027
--EXEC DeleteProviderNotification 5028
--EXEC DeleteProviderNotification 5029
--EXEC DeleteProviderNotification 5030
--EXEC DeleteProviderNotification 5031
--EXEC DeleteProviderNotification 5032
EXEC DeleteProviderNotification 5033
EXEC DeleteProviderNotification 5034
EXEC DeleteProviderNotification 5035
EXEC DeleteProviderNotification 5036
--EXEC DeleteProviderNotification 5037 

SELECT * FROM ProviderNotification pn
INNER JOIN [Provider] p
ON p.[Id] = pn.[ProviderId]
LEFT JOIN [NotificationLocation] nl
ON nl.[ProviderNotificationId] = pn.[ProviderId]
LEFT JOIN [NotificationLocationRoute] nlr
ON nlr.[NotificationLocationId] = nl.[Id]
LEFT JOIN Location l
ON l.[Id] = nl.[LocationId]
LEFT JOIN Route r
ON r.[Id] = nlr.[RouteId]

--Will need to delete any search filters or notifications that can't be migrated
--Get provider ids for SearchFilter join provider where IsAdditionalData = 1 
--call EXEC DeleteSearchProvider for those
/*
CREATE TABLE #A (ID INT IDENTITY(1,1) NOT NULL, Name VARCHAR(50))
After records are inserted in to this temp table, find the total number of records in the table.

DECLARE @TableLength INTEGER
SELECT @TableLength  = MAX(ID) FROM #A

DECLARE @Index INT
SET @Index = 1

WHILE (@Index <=@TableLength)
BEGIN

-- DO your work here 

SET @Index = @Index + 1

END*/

DECLARE @SearchFilterCursor CURSOR 
    , @searchFilterId INT;

SET @SearchFilterCursor = CURSOR FOR
    SELECT sf.[Id]
	FROM [SearchFilter] sf
	INNER JOIN [Location] l
	ON l.[Id] = sf.[LocationId]
	INNER JOIN [Provider] p
	ON p.[Id] = l.[ProviderId]
	WHERE p.[IsAdditionalData] = 1;

OPEN @SearchFilterCursor;  

FETCH NEXT FROM @SearchFilterCursor 
INTO @searchFilterId;

WHILE @@FETCH_STATUS = 0  
BEGIN  
SELECT @searchFilterId
	EXEC DeleteSearchFilter @searchFilterId
    FETCH NEXT FROM @SearchFilterCursor 
        INTO @searchFilterId
END;  
CLOSE @SearchFilterCursor;  
DEALLOCATE @SearchFilterCursor;  

DECLARE @ProviderNotificationCursor  CURSOR 
    , @providerNotificationId INT;

SET @ProviderNotificationCursor  = CURSOR FOR
    SELECT pn.[Id]
	FROM ProviderNotification pn
	INNER JOIN [Provider] p
	ON p.[Id] = pn.[ProviderId]
	WHERE p.[IsAdditionalData] = 1;

OPEN @ProviderNotificationCursor ;  

FETCH NEXT FROM @ProviderNotificationCursor 
INTO @providerNotificationId;

WHILE @@FETCH_STATUS = 0  
BEGIN  
SELECT @searchFilterId
	EXEC DeleteProviderNotification @providerNotificationId
    FETCH NEXT FROM @ProviderNotificationCursor  
        INTO @providerNotificationId
END;  
CLOSE @ProviderNotificationCursor ;  
DEALLOCATE @ProviderNotificationCursor ;  

DELETE [dbo].[LocationQualification] WHERE [IsAdditionalData] = 1
DELETE [dbo].[Location] WHERE [IsAdditionalData] = 1
--
SELECT * FROM [dbo].[Provider] p
INNER JOIN [dbo].[Location] l
ON l.[ProviderId] = p.[Id]
WHERE p.[IsAdditionalData] = 1

DELETE [dbo].[Provider] WHERE [IsAdditionalData] = 1

--SELECT * FROM [dbo].[Provider] WHERE [IsAdditionalData] = 1


-----
--Test search

declare
	@fromLatitude DECIMAL(9, 6) = 51.743496,
	@fromLongitude DECIMAL(9, 6) = -1.296747,
	@routeIds [dbo].[IdListTableType] ,
	@qualificationIds [dbo].[IdListTableType] ,
	@page INT = 0,
	@pageSize INT = 5,
	@totalLocationsCount INT 

		DECLARE @fromLocation GEOGRAPHY = geography::Point(@fromLatitude, @fromLongitude, 4326);

	DECLARE @locations TABLE (
				[UkPrn] BIGINT,
				[ProviderName] NVARCHAR(400),
				[LocationId] INT,
				[Postcode] NVARCHAR(10),
				[LocationName] NVARCHAR(400),
				[AddressLine1] NVARCHAR(400),
				[AddressLine2] NVARCHAR(400),
				[Town] NVARCHAR(100),
				[County] NVARCHAR(50),
				[Email] NVARCHAR(320),
				[Telephone] NVARCHAR(150),
				[Website] NVARCHAR(500),
				[EmployerContactEmail] NVARCHAR(320),
				[EmployerContactTelephone] NVARCHAR(150),
				[EmployerContactWebsite] NVARCHAR(500),
				[StudentContactEmail] NVARCHAR(320),
				[StudentContactTelephone] NVARCHAR(150),
				[StudentContactWebsite] NVARCHAR(500),
				[Distance] FLOAT,
				INDEX IX_Distance NONCLUSTERED(Distance, ProviderName, LocationName)
				)

	DECLARE @allQualificationIds TABLE (
				[Id] INT,
				UNIQUE CLUSTERED (Id)
			)
	DECLARE @hasRouteOrQualificationIds BIT = 0
	DECLARE @metersPerMile DECIMAL = 1609.3399999999999E0;
		
	IF(EXISTS(SELECT 1 from @routeIds) OR 
	   EXISTS(SELECT 1 from @qualificationIds))
	BEGIN
		INSERT INTO @allQualificationIds
		SELECT [QualificationId] 
		FROM @routeIds r
		INNER JOIN [RouteQualification] rq
		ON	rq.[RouteId] = r.[Id]
		UNION
		SELECT [Id]
		FROM @qualificationIds
								   
		SET @hasRouteOrQualificationIds = 1
	END;

WITH ProvidersCTE AS (
		SELECT	p.[Id],
				p.[UkPrn],
				p.[Name], 
				p.[Email],
				p.[Telephone],
				p.[Website],
				p.[EmployerContactEmail],
				p.[EmployerContactTelephone],
				p.[EmployerContactWebsite],
				p.[StudentContactEmail],
				p.[StudentContactTelephone],
				p.[StudentContactWebsite]
		FROM	[Provider] p
		WHERE	p.[IsDeleted] = 0
		),

	NearestLocationsCTE_Inner AS (
		SELECT	p.[UkPrn],
				p.[Name] AS [ProviderName],
				l.[Id] AS [LocationId],
				l.[Postcode],
				l.[Name] AS [LocationName],
				l.[AddressLine1],
				l.[AddressLine2],
				l.[Town],
				l.[County],
				COALESCE(NULLIF(l.[Email], ''), p.[Email]) AS [Email],
				COALESCE(NULLIF(l.[Telephone], ''), p.[Telephone]) AS [Telephone],
				COALESCE(NULLIF(l.[Website], ''), p.[Website]) AS [Website],
				COALESCE(NULLIF(l.[EmployerContactEmail], ''), p.[EmployerContactEmail]) AS [EmployerContactEmail],
				COALESCE(NULLIF(l.[EmployerContactTelephone], ''), p.[EmployerContactTelephone]) AS [EmployerContactTelephone],
				COALESCE(NULLIF(l.[EmployerContactWebsite], ''), p.[EmployerContactWebsite]) AS [EmployerContactWebsite],
				COALESCE(NULLIF(l.[StudentContactEmail], ''), p.[StudentContactEmail]) AS [StudentContactEmail],
				COALESCE(NULLIF(l.[StudentContactTelephone], ''), p.[StudentContactTelephone]) AS [StudentContactTelephone],
				COALESCE(NULLIF(l.[StudentContactWebsite], ''), p.[StudentContactWebsite]) AS [StudentContactWebsite],
				l.[Location].STDistance(@fromLocation) / @metersPerMile AS [Distance] --(Miles)
		FROM	[ProvidersCTE] p
		INNER JOIN	[dbo].[Location] l
		ON		p.[Id] = l.[ProviderId]
		WHERE	l.[IsDeleted] = 0
		  AND	EXISTS (SELECT	lq.[QualificationId]
						FROM	[dbo].[LocationQualification] lq
						INNER JOIN	[dbo].[Qualification] q
						ON		q.[Id] = lq.[QualificationId]
						  AND	q.[IsDeleted] = 0
						INNER JOIN	[dbo].[RouteQualification] rq
						ON		rq.[QualificationId] = q.[Id]
						INNER JOIN	[dbo].[Route] r
						ON		r.[Id] = rq.[RouteId]
						  AND	r.[IsDeleted] = 0
						WHERE	lq.[LocationId] = l.[Id]
						  AND	(@hasRouteOrQualificationIds = 0
								 OR q.[Id] IN (SELECT [Id] FROM @allQualificationIds)))
		)
	INSERT INTO @locations
	SELECT *
	FROM NearestLocationsCTE_Inner;

	SELECT @totalLocationsCount = COUNT(1) 
	from @locations;

	WITH NearestLocationsCTE AS (
		SELECT * 
		FROM @locations
		ORDER BY [Distance],
				 [ProviderName],
				 [LocationName]
		OFFSET @page * @pageSize ROWS
		FETCH NEXT @pageSize ROWS ONLY
		),
		
		TownNamesCTE AS (
			SELECT	[LocationId],
					COALESCE(t.[Name], l.[Town]) AS [TownName]
			FROM NearestLocationsCTE l
			OUTER APPLY (SELECT Top(1) [Name] 
						 FROM [dbo].[Town]
						 WHERE [Name] = l.[Town]
						 OR [Name] = REPLACE(l.[Town], ' ', '-')
						 ) t
			GROUP BY [LocationId],
					 l.[Town],
					 t.[Name])

		--Step 2 - add in the qualifications 
		SELECT 	[UkPrn],
				[ProviderName],
				[Postcode],
				[LocationName],
				[AddressLine1],
				[AddressLine2],
				t.[TownName] AS [Town],
				[County],
				[Email],
				[Telephone],
				[Website],				
				[EmployerContactEmail],
				[EmployerContactTelephone],
				[EmployerContactWebsite],
				[StudentContactEmail],
				[StudentContactTelephone],
				[StudentContactWebsite],				
				[Distance],
				lq.[DeliveryYear] AS [Year],
				rq.[RouteId],
				r.[Name] AS [RouteName],
				q.[Id] AS [QualificationId],
				q.[Name] AS [QualificationName]
		FROM NearestLocationsCTE l
		INNER JOIN	[dbo].[LocationQualification] lq
		ON		lq.[LocationId] = l.[LocationId]
		INNER JOIN [TownNamesCTE] t
		ON		t.[LocationId] = l.[LocationId]
		INNER JOIN	[dbo].[Qualification] q
		ON		q.[Id] = lq.[QualificationId]
		  AND	q.[IsDeleted] = 0
		INNER JOIN	[dbo].[RouteQualification] rq
		ON		rq.[QualificationId] = q.[Id]
		INNER JOIN	[dbo].[Route] r
		ON		r.[Id] = rq.[RouteId]
		  AND	r.[IsDeleted] = 0
		  AND	(@hasRouteOrQualificationIds = 0
				 OR q.[Id] IN (SELECT [Id] FROM @allQualificationIds))
		ORDER BY [Distance],
				 [ProviderName],
				 [LocationName],
				 lq.[DeliveryYear],
				 r.[Name],
				 q.[Name];

select @totalLocationsCount;


ROLLBACK
--COMMIT
