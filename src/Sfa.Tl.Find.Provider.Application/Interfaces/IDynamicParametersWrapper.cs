using System.Data;
using Dapper;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface IDynamicParametersWrapper
{
    DynamicParameters DynamicParameters { get; }

    IDynamicParametersWrapper CreateParameters(object template);

    // ReSharper disable once UnusedMemberInSuper.Global
    IDynamicParametersWrapper AddParameter(
        string name,
        object value = null,
        DbType? dbType = default,
        ParameterDirection? direction = default,
        int? size = default);

    IDynamicParametersWrapper AddOutputParameter(
        string name, 
        DbType? dbType = default,
        int? size = default);
}