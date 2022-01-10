CREATE TYPE [dbo].[LocationQualificationDataTableType] AS TABLE
(
	[UkPrn] BIGINT NOT NULL,
	[Postcode] NVARCHAR(10) NULL,
	[DeliveryYear] SMALLINT NOT NULL,
	[QualificationId] INT NOT NULL,
	[IsAdditionalData] BIT NOT NULL
)
