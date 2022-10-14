namespace Sfa.Tl.Find.Provider.Application.Interfaces;
public interface IGoogleMapsApiService
{
    Task<string> GetAddressDetails(string postcode);
}
