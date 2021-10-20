using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Api.Filters;

namespace Sfa.Tl.Find.Provider.Api.Attributes
{
    public class HeaderLoggerAttribute : TypeFilterAttribute
    {
        public HeaderLoggerAttribute()
            : base(typeof(HeaderLoggerFilter))
        { }
    }
}
