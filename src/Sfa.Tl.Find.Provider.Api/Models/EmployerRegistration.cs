using System.Collections.Generic;

namespace Sfa.Tl.Find.Provider.Api.Models;

public class EmployerRegistration
{
    public string EmployerName { get; init; }
    public string EmployerTelephone { get; init; }
    public string EmployerEmail { get; init; }
    public IEnumerable<Provider> Providers { get; init; }
}
