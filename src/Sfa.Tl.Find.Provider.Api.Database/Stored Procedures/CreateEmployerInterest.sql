﻿CREATE PROCEDURE [dbo].[CreateEmployerInterest]
	@data [dbo].[EmployerInterestDataTableType] READONLY,
	@industryIds [dbo].[IdListTableType] READONLY,
	@routeIds [dbo].[IdListTableType] READONLY
AS
	SET NOCOUNT ON;

    --Make sure only one row is passed in - this might be changed in a future version
    IF((SELECT COUNT(*) FROM @data) > 1)
	    THROW  60000, 'Only one row can be passed to CreateEmployerInterest.', 1

    DECLARE @newId INT

    INSERT INTO [dbo].[EmployerInterest] (
        [UniqueId], 
        [OrganisationName], 
        [ContactName], 
        [OtherIndustry],
        [Email], 
        [Telephone], 
        [Website], 
        [ContactPreferenceType],
        [AdditionalInformation],
        [ExpiryDate]) 
    SELECT [UniqueId], 
        [OrganisationName], 
        [ContactName], 
        [OtherIndustry],
        [Email], 
        [Telephone], 
        [Website], 
        [ContactPreferenceType],
        [AdditionalInformation],
        [ExpiryDate]
    FROM @data

    --This works for one insert - if we have multiple see https://stackoverflow.com/questions/55606203/get-multiple-scope-identity-while-inserting-data-with-table-valued-parameter
    SELECT @newId = SCOPE_IDENTITY();

    --Create location record
    INSERT INTO [dbo].[EmployerInterestLocation] (
	    [EmployerInterestId],
        [Name],
	    [Postcode],
	    [Latitude],
	    [Longitude],
	    [Location])
    SELECT @newId,
        [LocationName],
	    [Postcode],
	    [Latitude],
	    [Longitude],
        geography::Point([Latitude], [Longitude], 4326)
    FROM @data

    --Create industry record
    INSERT INTO [dbo].[EmployerInterestIndustry] (
        [EmployerInterestId],
        [IndustryId])
    SELECT  @newId,
            [Id]
    FROM @industryIds

    --Create route mappings
    INSERT INTO [dbo].[EmployerInterestRoute] (
        [EmployerInterestId],
        [RouteId])
    SELECT  @newId,
            [Id]
    FROM @routeIds

    RETURN @newId
