using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Interfaces;

public interface IPostcodeLookupService
{
    Task<PostcodeLocation> GetPostcode(string postcode);

    Task<PostcodeLocation> GetOutcode(string outcode);

    Task<PostcodeLocation> GetNearestPostcode(double latitude, double longitude);
}