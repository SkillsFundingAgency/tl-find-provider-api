using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Api.Filters;

namespace Sfa.Tl.Find.Provider.Api.Attributes
{
    public class HmacAuthorizationAttribute : TypeFilterAttribute
    {
        public HmacAuthorizationAttribute()
            : base(typeof(HmacAuthorizationFilter))
        { }
    }
}
