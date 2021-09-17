using System;
// ReSharper disable UnusedMember.Global

namespace Sfa.Tl.Find.Provider.Api.Interfaces
{
    public interface IDateTimeService
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
        DateTime Today { get; }
    }
}
