﻿using Sfa.Tl.Find.Provider.Application.Models.Enums;

namespace Sfa.Tl.Find.Provider.Application.Models;
public class NotificationLocationSummaryDto
{
    public int? Id { get; init; }

    public NotificationFrequency Frequency { get; init; }

    public int? SearchRadius { get; init; }
}
