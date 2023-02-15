using Dapper;
using Sfa.Tl.Find.Provider.Application.Data;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using System.Data;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Data;

public class DynamicParametersWrapperBuilder
{
    public DynamicParametersWrapper Build()
    {
        return new DynamicParametersWrapper();
    }

    public IDynamicParametersWrapper BuildWithOutputParameter<T>(
        string name,
        T value,
        DbType type = DbType.Int32)
    {
        var dynamicParametersWrapper = Substitute.For<IDynamicParametersWrapper>();
        var parameters = new DynamicParameters();
        parameters.Add(name, value, type, ParameterDirection.Output);
        dynamicParametersWrapper.DynamicParameters.Returns(parameters);

        return dynamicParametersWrapper;
    }

    public IDynamicParametersWrapper BuildWithOutputParameters(
        IList<(string Name, object Value, DbType Type)> parameterList)
    {
        var dynamicParametersWrapper = Substitute.For<IDynamicParametersWrapper>();
        var parameters = new DynamicParameters();

        foreach (var p in parameterList)
        {
            parameters.Add(p.Name, p.Value, p.Type, ParameterDirection.Output);
        }

        dynamicParametersWrapper.DynamicParameters.Returns(parameters);

        return dynamicParametersWrapper;
    }
}