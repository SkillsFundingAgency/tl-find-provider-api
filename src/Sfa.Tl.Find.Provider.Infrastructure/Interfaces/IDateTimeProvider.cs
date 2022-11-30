// ReSharper disable UnusedMember.Global

namespace Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

public interface IDateTimeProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    DateTime Today { get; }
}