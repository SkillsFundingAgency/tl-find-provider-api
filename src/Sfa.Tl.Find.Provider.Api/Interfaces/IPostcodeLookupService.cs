using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Interfaces
{
    public interface IPostcodeLookupService
    {
        public Task<PostcodeLocation> GetPostcode(string postcode);
    }
}
