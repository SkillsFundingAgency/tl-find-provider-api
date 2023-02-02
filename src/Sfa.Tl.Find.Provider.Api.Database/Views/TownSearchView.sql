CREATE VIEW TownSearchView
WITH SCHEMABINDING
AS 
SELECT  [Id],
		[Name],
		[County],
		[LocalAuthority],
		[Latitude],
		[Longitude],
		--Replace known special characters, and replace ampersand with and
		LOWER(CONCAT(
			dbo.ReplaceAllFunction(
				dbo.ReplaceAllFunction(
					CASE WHEN County IS NULL  OR County = '' THEN
						CONCAT([Name], ' ',  [LocalAuthority])
					ELSE
						CONCAT([Name], ' ',  [County])
					END,
					'%[ ,.''()/!-]%', 1, ''), 
				'%[&]%', 1, 'and'),
			--Performance trick - append id so clustered index will work
			'+', convert(nvarchar, Id))) AS Search
FROM dbo.[Town]
