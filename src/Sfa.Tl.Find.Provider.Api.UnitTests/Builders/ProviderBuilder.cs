using System.Collections.Generic;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    public class ProviderBuilder
    {
        public IEnumerable<Models.Provider> BuildList() =>
            new Models.Provider[]
            {
                new()
                {
                    UkPrn = 10000001,
                    Name = "Provider 1"
                },
                new()
                {
                    UkPrn = 10000002,
                    Name = "Provider 2"
                }
            };
    }
}
