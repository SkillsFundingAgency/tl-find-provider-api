using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Data
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly IDbContextWrapper _dbContextWrapper;

        public ProviderRepository(IDbContextWrapper dbContextWrapper)
        {
            _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
        }

        public async Task<IEnumerable<Models.Provider>> GetAll()
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

            };
        }

        public async Task Save(IEnumerable<Models.Provider> providers)
        {
        }
    }
}
