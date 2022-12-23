using Microsoft.AspNetCore.Http;
using Sfa.Tl.Find.Provider.Infrastructure.Services;

namespace Sfa.Tl.Find.Provider.Infrastructure.UnitTests.Builders;
public class  SessionServiceBuilder
{
    public SessionService Build(
        IHttpContextAccessor? httpContextAccessor = null, 
        string? environment = null)
    {
        httpContextAccessor ??= Substitute.For<IHttpContextAccessor>();
        environment ??= "TEST";

        return new SessionService(
            httpContextAccessor,
            environment);
    }
}
