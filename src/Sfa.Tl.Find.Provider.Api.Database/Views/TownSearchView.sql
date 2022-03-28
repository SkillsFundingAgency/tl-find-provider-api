CREATE VIEW TownSearchView
WITH SCHEMABINDING
AS 
SELECT  [Id],
		[Name],
		[County],
		[LocalAuthorityName],
		[Latitude],
		[Longitude],
		--Replace known special characters, 
		-- and replace ampersand with and
		CONCAT(
			dbo.ReplaceAllFunction(
				dbo.ReplaceAllFunction(
					CASE WHEN County IS NULL THEN
						CONCAT([Name], ' ',  [LocalAuthorityName])
					ELSE
						CONCAT([Name], ' ',  [County])
					END, 
					'%[ ,()/]%', 1, ''), 
				'%[&]%', 1, 'and'),
			--Performance trick - append id so clustered index will work
			'+', convert(nvarchar, Id)) AS Search

FROM dbo.[Town]
