using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Models.Configuration;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    public class DbContextWrapperBuilder
    {
        public DbContextWrapper Build(
            string connectionString = null,
            ILogger<DbContextWrapper> logger = null)
        {
            connectionString ??= "Data Source=Test;Initial Catalog=Test;Integrated Security=True;";
            logger ??= Substitute.For<ILogger<DbContextWrapper>>();

            var configuration = new SiteConfiguration
            {
                SqlConnectionString = connectionString
            };

            //return new DbContextWrapper(connectionString, logger);
            return new DbContextWrapper(configuration, logger);
        }
    }
}
