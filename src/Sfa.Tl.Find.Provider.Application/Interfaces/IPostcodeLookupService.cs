using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

public interface IPostcodeLookupService
{
    Task<GeoLocation> GetPostcode(string postcode);

    Task<GeoLocation> GetOutcode(string outcode);

    Task<GeoLocation> GetNearestPostcode(double latitude, double longitude);
}