--Set expiry dates for any old data - add 84 days (12 weeks) and round up to end of day

DECLARE @retentionDays INT = 84

UPDATE	[EmployerInterest]
SET		[ExpiryDate] = 
			DATEADD(day, 
				@retentionDays, 
				CAST(CONVERT(CHAR(8), [CreatedOn], 112) + ' 23:59:59.9999999' AS DATETIME2))
WHERE	[ExpiryDate] IS NULL
