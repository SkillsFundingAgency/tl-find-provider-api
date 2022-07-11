

// ReSharper disable UnusedMember.Global

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface IDateTimeService
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    DateTime Today { get; }
}