/*
Insert initial data for Routes.
The values below group the qualifications with ids (framework codes) within routes.
Qualifications that don't exist yet are commented ouyt.
As new qualifications are created, the mapping can be set here.
*/

MERGE INTO [dbo].[RouteQualification] AS Target 
USING (VALUES 
  -- Agriculture, environment and animal car
  --(1, ), -- Agriculture, Land Management and Production
  --(1, ), -- Animal Care and Management

  -- Business and administration
  (2, 51), -- Management and Administration

  -- Catering
  --(3, ),  -- Catering

  -- Construction
  (4, 45),  -- Building Services Engineering for Construction
  (4, 36),  -- Design, Surveying and Planning for Construction
  (4, 44),  -- Onsite Construction

  -- Creative and design
  --(5, ),  -- Craft and Design
  --(5, ),  -- Media, Broadcast and Production

  -- Digital and IT
  (6, 39),  -- Digital Business Services
  (6, 37),  -- Digital Production, Design and Development
  (6, 40),  -- Digital Support Services

  -- Education and childcare
  (7, 38),  -- Education and Childcare

  -- Engineering and manufacturing
  (8, 48),  --Design and Development for Engineering and Manufacturing
  (8, 49),  --Maintenance, Installation and Repair for Engineering and Manufacturing
  (8, 50),  --Engineering, Manufacturing, Processing and Control

  -- Hair and beauty
  --(9, ),  -- Hair, Beauty and Aesthetics

  -- Health and science
  (10, 41), -- Health
  (10, 42), -- Healthcare Science
  (10, 43), -- Science

  --Legal, finance and accounting
  (11, 46), -- Finance
  (11, 47) -- Accounting
  --(11, ) -- Legal Services
  )
/*
1	Agriculture, environment and animal care
2	Business and administration
3	Catering
4	Construction
5	Creative and design
6	Digital and IT
7	Education and childcare
8	Engineering and manufacturing
9	Hair and beauty
10	Health and science
11	Legal, finance and accounting
*/
/*
36	Design, Surveying and Planning for Construction
37	Digital Production, Design and Development
38	Education and Childcare
39	Digital Business Services
40	Digital Support Services
41	Health
42	Healthcare Science
43	Science
44	Onsite Construction
45	Building Services Engineering for Construction
46	Finance
47	Accounting
48	Design and Development for Engineering and Manufacturing
49	Maintenance, Installation and Repair for Engineering and Manufacturing
50	Engineering, Manufacturing, Processing and Control
51	Management and Administration
*/
  AS Source ([RouteId], [QualificationId] ) 
ON Target.[RouteId] = Source.[RouteId] 
  AND Target.[QualificationId] = Source.[QualificationId] 
WHEN NOT MATCHED BY TARGET THEN 
	INSERT ([RouteId], [QualificationId]) 
	VALUES ([RouteId], [QualificationId]) 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE
;
