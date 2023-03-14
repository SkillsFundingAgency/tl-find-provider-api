--Clean up redundant procedures
DROP PROCEDURE IF EXISTS [dbo].[GetSearchFilters]
DROP PROCEDURE IF EXISTS [dbo].[GetNotifications]

IF EXISTS(SELECT * FROM sys.columns WHERE name = 'IsAdditionalData')
EXEC('
	--Remove any search filters on additional data
	DECLARE @SearchFilterCursor CURSOR 
		   ,@searchFilterId INT;

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
		EXEC DeleteSearchFilter @searchFilterId
		FETCH NEXT FROM @SearchFilterCursor 
			INTO @searchFilterId
	END;  
	CLOSE @SearchFilterCursor;  
	DEALLOCATE @SearchFilterCursor;  

	DECLARE @ProviderNotificationCursor  CURSOR 
		   ,@providerNotificationId INT;

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
		EXEC DeleteProviderNotification @providerNotificationId
		FETCH NEXT FROM @ProviderNotificationCursor  
			INTO @providerNotificationId
	END;  
	CLOSE @ProviderNotificationCursor ;  
	DEALLOCATE @ProviderNotificationCursor ;  

	DELETE [dbo].[LocationQualification] WHERE [IsAdditionalData] = 1
	DELETE [dbo].[Location] WHERE [IsAdditionalData] = 1
	DELETE [dbo].[Provider] WHERE [IsAdditionalData] = 1
')
