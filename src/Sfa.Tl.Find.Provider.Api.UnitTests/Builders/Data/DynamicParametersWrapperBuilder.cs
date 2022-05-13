using System;
using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models.Configuration;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Data;

public class DynamicParametersWrapperBuilder
{
    public DynamicParametersWrapper Build()
    {
        return new DynamicParametersWrapper();
    }
}