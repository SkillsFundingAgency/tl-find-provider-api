CREATE PROCEDURE [dbo].[SearchProviders]
	@fromLatitude DECIMAL(9, 6),
	@fromLongitude DECIMAL(9, 6),
	@qualificationId INT,
	@page INT,
	@pageSize INT
AS

	SET NOCOUNT ON;
	
	SELECT	[UkPrn],
			[Name],
			[AddressLine1],
			[AddressLine2],
			[Town],
			[County],
			[Postcode],
			[Email],
			[Telephone],
			[Website]
	FROM	[dbo].[Provider] p
	WHERE	p.IsDeleted = 0
	ORDER BY [Name]
	OFFSET @page * @pageSize ROWS
	FETCH NEXT @pageSize ROWS ONLY

