﻿CREATE PROCEDURE [dbo].[UpdateLocationQualifications]
	@data [dbo].[LocationQualificationDataTableType] READONLY
AS
	SET NOCOUNT ON;

	DECLARE @ChangeSummary TABLE(
		Change VARCHAR(20),
		Id INT
	);
	
	--Need to convert to a table with the location ids
	WITH LocationQualificationYearsCTE AS (
	SELECT l.[Id] AS [LocationId],
		d.[Postcode],
		d.[DeliveryYear],
		d.[QualificationId]
	FROM @data d
	INNER JOIN [dbo].[Provider] p
	ON p.[UkPrn] = d.[UkPrn]
	INNER JOIN [dbo].[Location] l
	ON l.[ProviderId] = p.[Id]
	AND l.[Postcode] = d.[Postcode]
	)
		MERGE INTO [dbo].[LocationQualification] AS t
		USING LocationQualificationYearsCTE AS s
		ON
		(
		  t.[LocationId] = s.[LocationId]
		  AND t.[DeliveryYear] = s.[DeliveryYear]
		  AND t.[QualificationId] = s.[QualificationId]
		)

		WHEN NOT MATCHED BY TARGET THEN INSERT
		(
			[LocationId],
			[DeliveryYear],
			[QualificationId]
		)
		VALUES
		(
			s.[LocationId],
			s.[DeliveryYear],
			s.[QualificationId]
		)

		WHEN NOT MATCHED BY SOURCE 
		THEN DELETE

		OUTPUT $action, 
			INSERTED.[Id]
		INTO @ChangeSummary;

	SELECT	Change, 
			COUNT(*) AS CountPerChange	 
	FROM @ChangeSummary
	GROUP BY Change;
