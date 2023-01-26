﻿using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

public class NotificationLocationSummaryDtoBuilder
{
    public IEnumerable<NotificationLocationSummaryDto> BuildList() =>
        new NotificationLocationSummaryBuilder()
            .BuildList()
            .Select(n => new NotificationLocationSummaryDto
            {
                Id = n.Id,
                SearchRadius = n.SearchRadius,
                Frequency = n.Frequency
            })
            .ToList();
}