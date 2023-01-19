﻿CREATE NONCLUSTERED INDEX [IX_NotificationRoute_RouteId]
	ON [dbo].[NotificationRoute]([RouteId])
	INCLUDE([NotificationId]) 
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
	ON [PRIMARY]
