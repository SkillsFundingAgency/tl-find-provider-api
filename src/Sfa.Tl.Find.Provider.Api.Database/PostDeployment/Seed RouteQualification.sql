/*
Insert initial data for Routes.
The values below group the qualifications with ids (framework codes) within routes.
As new qualifications are created, the mapping can be set here.
*/

MERGE INTO [dbo].[RouteQualification] AS Target 
USING (VALUES 
  -- Agriculture, environment and animal car
  (1, 57), -- Agriculture, Land management and Production
  (1, 58), -- Animal Care and Management

  -- Business and administration
  (2, 51), -- Management and Administration

  -- Construction and the built environment
  (4, 45),  -- Building Services Engineering for Construction
  (4, 36),  -- Design, Surveying and Planning for Construction

  -- Creative and design
  (5, 54),  -- Craft and Design
  (5, 55),  -- Media, Broadcast and Production

  -- Digital and IT
  (6, 39),  -- Digital Business Services
  (6, 37),  -- Digital Production, Design and Development
  (6, 40),  -- Digital Support Services

  -- Education and early years
  (7, 38),  -- Education and Early Years

  -- Engineering and manufacturing
  (8, 48),  -- Design and Development for Engineering and Manufacturing
  (8, 49),  -- Maintenance, Installation and Repair for Engineering and Manufacturing
  (8, 50),  -- Engineering, Manufacturing, Processing and Control

  -- Hair and beauty
  (9, 53),  -- Hairdressing, Barbering and Beauty Therapy

  -- Health and science
  (10, 41), -- Health
  (10, 42), -- Healthcare Science
  (10, 43), -- Science

  --Legal, finance and accounting
  (11, 46), -- Finance
  (11, 47), -- Accounting
  (11, 52)  -- Legal Services
  )
  AS Source ([RouteId], [QualificationId])
ON Target.[RouteId] = Source.[RouteId] 
  AND Target.[QualificationId] = Source.[QualificationId] 
WHEN NOT MATCHED BY TARGET 
  AND EXISTS(SELECT * 
			 FROM [Qualification] 
			 WHERE [Id] = [QualificationId]) 
  THEN 
	INSERT ([RouteId], [QualificationId]) 
	VALUES ([RouteId], [QualificationId]) 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE
;
