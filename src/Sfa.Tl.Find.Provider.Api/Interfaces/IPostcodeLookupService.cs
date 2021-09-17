using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Interfaces
{
    public interface IPostcodeLookupService
    {
        Task<PostcodeLocation> GetPostcode(string postcode);
    }
}
