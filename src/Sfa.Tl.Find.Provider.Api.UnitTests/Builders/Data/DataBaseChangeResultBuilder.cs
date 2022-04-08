using System;
using System.Collections.Generic;
using System.Linq;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Data;

public class DataBaseChangeResultBuilder
{
    private readonly IList<(string Change, int ChangeCount)> _changeResults
        = new List<(string Change, int ChangeCount)>();

    public IList<(string Change, int ChangeCount)> Build() => _changeResults;

    public DataBaseChangeResultBuilder WithInserts(int insertCount)
    {
        AddItem("INSERT", insertCount);
        return this;
    }

    public DataBaseChangeResultBuilder WithUpdates(int updateCount)
    {
        AddItem("UPDATE", updateCount);
        return this;
    }

    public DataBaseChangeResultBuilder WithDeletes(int deleteCount)
    {
        AddItem("DELETE", deleteCount);
        return this;
    }

    private void AddItem(string key, int count)
    {
        if (_changeResults.Any(x => x.Change == key))
            throw new InvalidOperationException($"An item with key '{key}' already exists");

        _changeResults.Add((key, count));
    }
}