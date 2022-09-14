using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json.Linq;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;

namespace Sfa.Tl.Find.Provider.Web.Authorization;

public static class ProviderAuthenticationExtensions
{
    public const string AuthenticationCookieName = "tl-provider-auth-cookie";
    public const string AuthenticationTypeName = "DfE-SignIn";

    public static void AddProviderAuthentication(this IServiceCollection services, DfeSignInSettings signInSettings)
    {
        var cookieSecurePolicy = CookieSecurePolicy.Always;

        var cookieOptions = new Action<CookieAuthenticationOptions>(options =>
        {
            options.CookieManager = new ChunkingCookieManager { ChunkSize = 3000 };
            options.Cookie.SecurePolicy = cookieSecurePolicy;
            options.AccessDeniedPath = "/error/403";
        });

        var cookieAndSessionTimeout = 20; //settings.Timeout;
        var overallSessionTimeout = TimeSpan.FromMinutes(cookieAndSessionTimeout);

        services.AddAntiforgery(options =>
        {
            options.Cookie.SecurePolicy = cookieSecurePolicy;
        });

        services.AddAuthentication(options =>
        {
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            //TODO: List auth cookie on cookies page?
            options.Cookie.Name = AuthenticationCookieName;
            options.Cookie.SecurePolicy = cookieSecurePolicy;
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = overallSessionTimeout;
            //options.LogoutPath = signInSettings.LogoutPath;
            options.AccessDeniedPath = "/Error/403";
        })
        .AddOpenIdConnect(options =>
        {
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.MetadataAddress = signInSettings.MetadataAddress;
            options.RequireHttpsMetadata = false;
            options.ClientId = signInSettings.ClientId;
            options.ClientSecret = signInSettings.ClientSecret;
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.GetClaimsFromUserInfoEndpoint = true;

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("email");
            options.Scope.Add("profile");
            options.Scope.Add("organisationid");
            options.Scope.Add("organisation");
            options.Scope.Add("Organisation");
            options.Scope.Add("organisation.id");

            // When we expire the session, ensure user is prompted to sign in again at DfE Sign In
            options.MaxAge = overallSessionTimeout;

            options.SaveTokens = true;
            options.CallbackPath = new PathString("/auth/cb");
            //options.CallbackPath = new PathString(signInSettings.CallbackPath);
            //options.SignedOutCallbackPath = new PathString(signInSettings.SignedOutCallbackPath);
            //options.SignedOutCallbackPath = new PathString(signInSettings.SignedOutCallbackPath);
            //options.SignedOutRedirectUri = "/signout-complete";
            options.SecurityTokenValidator = new JwtSecurityTokenHandler
            {
                InboundClaimTypeMap = new Dictionary<string, string>(),
                TokenLifetimeInMinutes = cookieAndSessionTimeout,
                SetDefaultTimesOnTokenCreation = true,
            };
            options.ProtocolValidator = new OpenIdConnectProtocolValidator
            {
                RequireSub = true,
                RequireStateValidation = false,
                NonceLifetime = overallSessionTimeout
            };

            options.DisableTelemetry = true;

            options.Events = new OpenIdConnectEvents
            {
                // Sometimes, problems in the OIDC provider (such as session timeouts)
                // Redirect the user to the /auth/cb endpoint. ASP.NET Core middleware interprets this by default
                // as a successful authentication and throws in surprise when it doesn't find an authorization code.
                // This override ensures that these cases redirect to the root.
                OnMessageReceived = context =>
                {
                    var isSpuriousAuthCbRequest =
                                        context.Request.Path == options.CallbackPath &&
                                        context.Request.Method == "GET" &&
                                        !context.Request.Query.ContainsKey("code");

                    if (isSpuriousAuthCbRequest)
                    {
                        context.HandleResponse();
                        context.Response.Redirect("/");
                    }
                    return Task.CompletedTask;
                },

                // Sometimes the auth flow fails. The most commonly observed causes for this are
                // Cookie correlation failures, caused by obscure load balancing stuff.
                // In these cases, rather than send user to a 500 page, prompt them to re-authenticate.
                // This is derived from the recommended approach: https://github.com/aspnet/Security/issues/1165
                OnRemoteFailure = ctx =>
                {
                    ctx.HandleResponse();
                    return Task.FromException(ctx.Failure);
                },

                // that event is called after the OIDC middleware received the authorisation code,
                // redeemed it for an access token and a refresh token,
                // and validated the identity token
                OnTokenValidated = async ctx =>
                {
                    var claims = new List<Claim>();

                    foreach (var claim in ctx.Principal?.Claims)
                    {
                        Debug.WriteLine($"Claim {claim.Type} = {claim.Value}");
                    }

                    //TODO: Use System.Text.Json
                    var organisationClaim = ctx.Principal.FindFirst("Organisation");
                    
                    var organisation = organisationClaim != null 
                        ? JObject.Parse(ctx.Principal.FindFirst("Organisation").Value)
                        : new JObject();

                    if (organisation.HasValues)
                    {
                        var organisationId = organisation.SelectToken("id").ToString();
                        var userId = ctx.Principal.FindFirst("sub").Value;

                        //var dfeSignInApiClient = ctx.HttpContext.RequestServices.GetService<IDfeSignInApiClient>();
                        //var userInfo = await dfeSignInApiClient.GetDfeSignInUserInfo(organisationId, userId);

                        //if (userInfo.HasAccessToService)
                        //{
                        //    var internalApiClient = ctx.HttpContext.RequestServices.GetService<IResultsAndCertificationInternalApiClient>();
                        //    var loggedInUserTypeResponse = userInfo.Ukprn.Value == 1
                        //                                    ? new LoggedInUserTypeInfo { Ukprn = userInfo.Ukprn.Value, UserType = LoginUserType.AwardingOrganisation }
                        //                                    : await internalApiClient.GetLoggedInUserTypeInfoAsync(userInfo.Ukprn.Value);

                        //    if (loggedInUserTypeResponse != null && loggedInUserTypeResponse.UserType != LoginUserType.NotSpecified)
                        //    {
                        //        claims.AddRange(new List<Claim>
                        //        {
                        //                            new Claim(CustomClaimTypes.UserId, userId),
                        //                            new Claim(CustomClaimTypes.OrganisationId, organisationId),
                        //                            new Claim(CustomClaimTypes.Ukprn, userInfo.Ukprn.HasValue ? userInfo.Ukprn.Value.ToString() : string.Empty),
                        //                            new Claim(ClaimTypes.GivenName, ctx.Principal.FindFirst("given_name").Value),
                        //                            new Claim(ClaimTypes.Surname, ctx.Principal.FindFirst("family_name").Value),
                        //                            new Claim(ClaimTypes.Email, ctx.Principal.FindFirst("email").Value),
                        //                            new Claim(CustomClaimTypes.HasAccessToService, userInfo.HasAccessToService.ToString()),
                        //                            new Claim(CustomClaimTypes.LoginUserType, ((int)loggedInUserTypeResponse.UserType).ToString())
                        //        });

                        //        if (userInfo.Roles != null && userInfo.Roles.Any())
                        //        {
                        //            claims.AddRange(userInfo.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));
                        //        }
                        //    }
                        //}
                    }
                    else
                    {
                        //claims.Add(new Claim(CustomClaimTypes.HasAccessToService, "false"));
                    }

                    ctx.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, AuthenticationTypeName));

                    // so that we don't issue a session cookie but one with a fixed expiration
                    ctx.Properties.IsPersistent = true;
                    ctx.Properties.ExpiresUtc = DateTime.UtcNow.Add(overallSessionTimeout);
                }
            };
        });
        // ReSharper disable CommentTypo
        /*
        services
            .AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme =
                    CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultSignInScheme =
                    CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme =
                    WsFederationDefaults.AuthenticationScheme;
            })
            //.AddWsFederation(options =>
            //{
            //    options.MetadataAddress = settings.MetadataAddress;
            //    options.Wtrealm = settings.Wtrealm;
            //    options.CallbackPath = "/{ukprn}/home";
            //    options.Events.OnSecurityTokenValidated = async (ctx) =>
            //    {
            //        await PopulateProviderClaims(ctx.HttpContext, ctx.Principal);
            //    };
            //})
            //.AddCookie(cookieOptions)
            ;
        */
        // ReSharper restore CommentTypo

    }

    private static Task PopulateProviderClaims(HttpContext httpContext, ClaimsPrincipal principal)
    {
        var providerId = principal.Claims.First(c => c.Type.Equals(ProviderClaims.ProviderUkprn)).Value;
        var displayName = principal.Claims.First(c => c.Type.Equals(ProviderClaims.DisplayName)).Value;
        httpContext.Items.Add(ClaimsIdentity.DefaultNameClaimType, providerId);
        httpContext.Items.Add(ProviderClaims.DisplayName, displayName);
        principal.Identities.First().AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, providerId));
        principal.Identities.First().AddClaim(new Claim(ProviderClaims.DisplayName, displayName));
        return Task.CompletedTask;
    }
}
