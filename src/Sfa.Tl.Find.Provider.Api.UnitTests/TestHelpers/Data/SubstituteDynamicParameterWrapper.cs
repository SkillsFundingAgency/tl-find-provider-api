using Dapper;
using Intertech.Facade.DapperParameters;
using NSubstitute;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Data
{
    internal class SubstituteDynamicParameterWrapper
    {
        public IDapperParameters DapperParameters
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
            var parameters = Substitute.For<IDapperParameters>();
            parameters
                .When(x =>
                    x.CreateParmsWithTemplate(Arg.Any<object>()))
                .Do(x =>
                {
                    var p = x.Arg<object>();
                    DynamicParameters = new DynamicParameters(p);
                });

            parameters.DynamicParameters
                .Returns(_ => DynamicParameters);

            DapperParameters = parameters;
        }
    }
}
