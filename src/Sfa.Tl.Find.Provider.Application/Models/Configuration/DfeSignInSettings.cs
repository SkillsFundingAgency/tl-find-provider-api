using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sfa.Tl.Find.Provider.Application.Models.Configuration;
public class DfeSignInSettings
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string MetadataEndpoint { get; set; }
}
