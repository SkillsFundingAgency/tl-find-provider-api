﻿namespace Sfa.Tl.Find.Provider.Infrastructure.Configuration;
public class DfeSignInSettings
{
    public string? Audience { get; set; }
    public string? ApiUri { get; set; }
    public string? ApiSecret { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? Issuer { get; set; }
    public string? MetadataAddress { get; set; }
    public int Timeout { get; set; }
}
