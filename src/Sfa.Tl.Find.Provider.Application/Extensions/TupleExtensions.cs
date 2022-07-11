namespace Sfa.Tl.Find.Provider.Application.Extensions;

public static class TupleExtensions
{
    public static (int Inserted, int Updated, int Deleted) ConvertToTuple(
        this IEnumerable<(string Change, int ChangeCount)> changeResults)
    {
        var dictionary = changeResults
            .ToDictionary(
                x => x.Change, 
                x => x.ChangeCount);

        var inserted = dictionary.ContainsKey("INSERT") ? dictionary["INSERT"] : 0;
        var updated = dictionary.ContainsKey("UPDATE") ? dictionary["UPDATE"] : 0;
        var deleted = dictionary.ContainsKey("DELETE") ? dictionary["DELETE"] : 0;

        return (inserted, updated, deleted);
    }
}