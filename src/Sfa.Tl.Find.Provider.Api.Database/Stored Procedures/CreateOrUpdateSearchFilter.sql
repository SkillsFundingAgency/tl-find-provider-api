CREATE PROCEDURE [dbo].[CreateOrUpdateSearchFilter]
	@locationId INT,
	@searchRadius INT,
	@routeIds [dbo].[IdListTableType] READONLY
AS
	SET NOCOUNT ON;

	DECLARE @id INT
	SELECT @id = Id 
	FROM [dbo].[SearchFilter] 
	WHERE LocationId = @locationId
		
	IF @id IS NULL
	BEGIN
		INSERT INTO [dbo].[SearchFilter] ([LocationId], [SearchRadius])
		VALUES (@locationId, @searchRadius)
		SELECT @id = SCOPE_IDENTITY()
	END
	ELSE BEGIN
		UPDATE [dbo].[SearchFilter] 
		SET [SearchRadius] = @searchRadius
		WHERE Id = @id
		  AND [SearchRadius] <> @searchRadius
	END;

	WITH TARGET AS 
	(
		SELECT [SearchFilterId], 
			   [RouteId]
		FROM [SearchFilterRoute]
		WHERE [SearchFilterId] = @id
	)
	MERGE INTO TARGET
	USING 
	(
		SELECT [Id]
		FROM @routeIds
	) AS SOURCE ON
		SOURCE.[Id] = TARGET.[RouteId]
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (SearchFilterId, RouteId)
		VALUES (@id, Id)
	WHEN NOT MATCHED BY SOURCE THEN
		DELETE
	;
