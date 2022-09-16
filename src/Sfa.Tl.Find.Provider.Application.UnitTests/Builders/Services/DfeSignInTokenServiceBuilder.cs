﻿using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
public class DfeSignInTokenServiceBuilder
{
    public IDfeSignInTokenService Build(
        DfeSignInSettings? signInSettings = null)
    {
        signInSettings ??= new SettingsBuilder().BuildDfeSignInSettings();
        var signInOptions = signInSettings.ToOptions();

        return new DfeSignInTokenService(
            signInOptions);
    }
}
