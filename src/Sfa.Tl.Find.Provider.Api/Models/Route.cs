using System.Diagnostics;

namespace Sfa.Tl.Find.Provider.Api.Models
{
    [DebuggerDisplay(" {" + nameof(Id) + "}" +
                     " {" + nameof(Name) + ", nq}")]
    public class Route
    {
        public int Id { get; init; }
        public string Name { get; init; }
    }
}
