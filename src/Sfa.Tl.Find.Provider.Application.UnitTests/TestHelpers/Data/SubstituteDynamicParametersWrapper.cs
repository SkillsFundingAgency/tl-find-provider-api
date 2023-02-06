using Dapper;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.TestHelpers.Data;

public class SubstituteDynamicParametersWrapper
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

    public SubstituteDynamicParametersWrapper()
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