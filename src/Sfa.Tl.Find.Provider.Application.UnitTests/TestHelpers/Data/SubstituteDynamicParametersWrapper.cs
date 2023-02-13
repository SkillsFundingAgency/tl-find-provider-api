using System.Data;
using Dapper;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.TestHelpers.Data;

public class SubstituteDynamicParametersWrapper
{
    public IDynamicParametersWrapper DapperParameterFactory { get; }

    public DynamicParameters DynamicParameters { get; set; }
    
    public object? ReturnValue { get; set; }

    public SubstituteDynamicParametersWrapper()
    {
        var parameters = Substitute.For<IDynamicParametersWrapper>();
        parameters
            .When(x =>
                x.CreateParameters(Arg.Any<object>()))
            .Do(x =>
            {
                DynamicParameters = new DynamicParameters(x.Arg<object>());
            });

        parameters
            .When(x =>
                x.AddReturnValueParameter(Arg.Any<string>(), Arg.Any<DbType?>(), Arg.Any<int?>()))
            .Do(x =>
            {
                DynamicParameters ??= new DynamicParameters();
                DynamicParameters.Add(x.Arg<string>(), ReturnValue, dbType: x.Arg<DbType?>(), size: x.Arg<int?>());
            });

        parameters.DynamicParameters
            .Returns(_ => DynamicParameters);

        DapperParameterFactory = parameters;
    }
}