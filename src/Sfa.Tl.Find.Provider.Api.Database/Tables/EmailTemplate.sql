﻿CREATE TABLE [dbo].[EmailTemplate]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[Name] NVARCHAR(50) NOT NULL, 
	[TemplateId] NVARCHAR(50) NOT NULL, 
	[CreatedOn] DATETIME2 NOT NULL DEFAULT getutcdate(), 
	[ModifiedOn] DATETIME2 NULL, 

    CONSTRAINT [PK_EmailTemplates] PRIMARY KEY ([Id]))
