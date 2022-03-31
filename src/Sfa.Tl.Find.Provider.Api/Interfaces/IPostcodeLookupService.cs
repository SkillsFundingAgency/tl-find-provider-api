using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Interfaces;

public interface IPostcodeLookupService
{
    Task<GeoLocation> GetPostcode(string postcode);

    Task<GeoLocation> GetOutcode(string outcode);

    Task<GeoLocation> GetNearestPostcode(double latitude, double longitude);
}