CREATE TABLE [dbo].[EmployerInterestIndustry]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[EmployerInterestId] INT NOT NULL,
	[IndustryId] INT NOT NULL,
	[CreatedOn] DATETIME2(7) NOT NULL DEFAULT (GETUTCDATE())
	CONSTRAINT [PK_EmployerInterestIndustry] PRIMARY KEY ([Id])
	CONSTRAINT [FK_EmployerInterestIndustry_EmployerInterest] FOREIGN KEY([EmployerInterestId]) REFERENCES [dbo].[EmployerInterest] ([Id])
	CONSTRAINT [FK_EmployerInterestIndustry_Industry] FOREIGN KEY([IndustryId]) REFERENCES [dbo].[Industry] ([Id])
)
