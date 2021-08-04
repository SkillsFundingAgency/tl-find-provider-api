using System.Linq;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Data
{
    public class ProviderRepository : IProviderRepository
    {
        public async Task<IQueryable<Models.Provider>> GetAllProviders()
        {
            return new Models.Provider[]
            {
                new()
                {
                    UkPrn = 10000001,
                    Name = "Test provider 1"
                },
                new()
                {
                    UkPrn = 10000001,
                    Name = "Test provider 2"
                }

            }.AsQueryable();
        }
    }
}
