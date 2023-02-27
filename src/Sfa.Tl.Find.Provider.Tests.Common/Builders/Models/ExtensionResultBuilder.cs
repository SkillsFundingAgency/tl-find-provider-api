using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
public class ExtensionResultBuilder
{
    public ExtensionResult Build(
        bool success = true, 
        int remaining = 10) => new()
        {
            Success = success,
            ExtensionsRemaining = remaining
        };
}
