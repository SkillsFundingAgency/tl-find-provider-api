using Sfa.Tl.Find.Provider.Api.Data;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    public class DbContextWrapperBuilder
    {
        public DbContextWrapper Build(
            string connectionString = null)
        {
            connectionString ??= "Data Source=Test;Initial Catalog=Test;Integrated Security=True;";

            return new DbContextWrapper(connectionString);
        }
    }
}
