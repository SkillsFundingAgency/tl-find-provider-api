CREATE PROCEDURE [dbo].[DeleteSearchFilter]
	@searchFilterId INT
AS
	SET NOCOUNT ON;

	DELETE FROM [dbo].[SearchFilterRoute]
	WHERE [SearchFilterId] = @searchFilterId;

	DELETE FROM [dbo].[SearchFilter]
	WHERE [Id] = @searchFilterId;
