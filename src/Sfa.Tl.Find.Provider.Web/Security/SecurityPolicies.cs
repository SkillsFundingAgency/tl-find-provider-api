using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Primitives;

namespace Sfa.Tl.Find.Provider.Web.Security;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
public static class SecurityPolicies
{
    public static StringValues PermissionsList = new(
        "accelerometer=()," +
        "autoplay=()," +
        "camera=()," +
        "encrypted-media=()," +
        "fullscreen=(self)," +
        "geolocation=()," +
        "gyroscope=()," +
        "microphone=()," +
        "midi=()," +
        "picture-in-picture=()," +
        "publickey-credentials-get=()," +
        "sync-xhr=()," +
        "usb=()," +
        "xr-spatial-tracking=()");
}
