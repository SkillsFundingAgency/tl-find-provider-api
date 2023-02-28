using Microsoft.AspNetCore.Mvc.Rendering;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;
using Route = Sfa.Tl.Find.Provider.Application.Models.Route;

namespace Sfa.Tl.Find.Provider.Web.Extensions;

public static class SelectListHelperExtensions
{
    public static IList<Route> GetSelectedSkillAreas(SelectListItem[]? selectList)
    {
        return selectList != null
            ? selectList
                .Where(s => s.Selected)
                .Select(s =>
                    new Route
                    {
                        Id = int.Parse(s.Value)
                    })
                .ToList()
            : new List<Route>();
    }

    public static SelectListItem[] LoadFrequencyOptions(NotificationFrequency selectedValue)
    {
        return Enum.GetValues<NotificationFrequency>()
            .Select(f => new SelectListItem
            {
                Value = ((int)f).ToString(),
                Text = f.ToString(),
                Selected = f == selectedValue
            })
            .ToArray();
    }
    
    public static SelectListItem[] LoadProviderLocationOptions(IList<NotificationLocationName> providerLocations, int? selectedValue)
    {
        var selectList = providerLocations
            .Select(p
                => new SelectListItem(
                    $"{p.Name.TruncateWithEllipsis(15).ToUpper()} [{p.Postcode}]",
                    p.LocationId.ToString(),
                    p.LocationId == selectedValue))
            .OrderBy(x => x.Text)
            .ToList();

        return providerLocations.Count == 1
            ? selectList.ToArray()
            : selectList
                .Prepend(new SelectListItem("Select a campus", "", true))
                .ToArray();
    }

    public static SelectListItem[] LoadProviderLocationOptionsWithAllOption(IEnumerable<LocationPostcode> providerLocations, int? selectedValue)
    {
        return providerLocations.Select(p
                => new SelectListItem(
                    $"{p.Name.TruncateWithEllipsis(15).ToUpper()} [{p.Postcode}]",
                    p.Id.ToString(),
                    p.Id == selectedValue)
            )
            .OrderBy(x => x.Text)
            .Prepend(new SelectListItem("All", "0", selectedValue is null or 0))
            .ToArray();
    }

    public static SelectListItem[] LoadSearchRadiusOptions(int? selectedValue, bool setDefaultSelectedValue = false)
    {
        if(selectedValue == null)
        {
            }

        
        var values = new List<int> { 5, 10, 20, 30, 40, 50 };
        return values
            .Select(p => new SelectListItem(
                $"{p} miles",
                p.ToString(),
                p == selectedValue || (selectedValue == null && setDefaultSelectedValue && p == values[0]))
            )
            .ToArray();
    }

    public static SelectListItem[] LoadSkillAreaOptions(IEnumerable<Route>  routes, IEnumerable<Route> selectedRoutes)
    {
        return routes
            .Select(r => new SelectListItem(
                r.Name,
                r.Id.ToString(),
                selectedRoutes.Any(x => r.Id == x.Id))
            )
            .OrderBy(x => x.Text)
            .ToArray();
    }
}
