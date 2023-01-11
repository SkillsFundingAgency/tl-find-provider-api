using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Application.Models;

[DebuggerDisplay("Id {" + nameof(Id) + "}" +
                 " SearchRadius {" + nameof(SearchRadius) + ", nq}")]
public class SearchFilterDto
{
    public int Id { get; init; }

    public int LocationId { get; init; }

    public string LocationName { get; init; }

    public string Postcode { get; init; }

    public int? SearchRadius { get; init; }
}