CREATE PROCEDURE [dbo].[UpdateNotification]
	@id INT,
	@email NVARCHAR(320),
	@frequency INT,
	@searchRadius INT,
	@locationId INT,
	@routeIds [dbo].[IdListTableType] READONLY
AS
	SET NOCOUNT ON;

	