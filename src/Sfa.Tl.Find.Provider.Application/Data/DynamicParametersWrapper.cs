using System.Data;
using Dapper;
using Sfa.Tl.Find.Provider.Application.Interfaces;

namespace Sfa.Tl.Find.Provider.Application.Data;

public class DynamicParametersWrapper : IDynamicParametersWrapper
{
    public DynamicParameters DynamicParameters { get; private set; }

    public DynamicParametersWrapper()
    {
        DynamicParameters = new DynamicParameters();
    }

    public IDynamicParametersWrapper CreateParameters(object template)
    {
        DynamicParameters = new DynamicParameters(template);

        return this;
    }

    public IDynamicParametersWrapper AddParameter(
        string name,
        object value = null,
        DbType? dbType = default,
        ParameterDirection? direction = default,
        int? size = default)
    {
        DynamicParameters.Add(name, value, dbType, direction, size);

        return this;
    }

    public IDynamicParametersWrapper AddOutputParameter(
        string name,
        DbType? dbType = default,
        int? size = default)
    {
        DynamicParameters.Add(name, dbType: dbType, direction: ParameterDirection.Output, size: size);
        
        return this;
    }

    public IDynamicParametersWrapper AddReturnValueParameter(
        string name, 
        DbType? dbType = default, 
        int? size = default)
    {
        DynamicParameters.Add(name, dbType: dbType, direction: ParameterDirection.ReturnValue, size: size);

        return this;
    }
}