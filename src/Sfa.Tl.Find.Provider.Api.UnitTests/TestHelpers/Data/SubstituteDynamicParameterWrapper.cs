using Dapper;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Data;

internal class SubstituteDynamicParameterWrapper
{
    public IDynamicParametersWrapper DapperParameterFactory
    {
        get;
    }

    public DynamicParameters DynamicParameters
    {
        get; 
        private set;
    }

    public SubstituteDynamicParameterWrapper()
    {
        var parameters = Substitute.For<IDynamicParametersWrapper>();
        parameters
            .When(x =>
                x.CreateParameters(Arg.Any<object>()))
            .Do(x =>
            {
                var p = x.Arg<object>();
                DynamicParameters = new DynamicParameters(p);
            });

        parameters.DynamicParameters
            .Returns(_ => DynamicParameters);

        DapperParameterFactory = parameters;
    }
}