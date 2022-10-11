﻿using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Sfa.Tl.Find.Provider.Application.Extensions;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;

namespace Sfa.Tl.Find.Provider.Web.Authorization;

public static class ProviderAuthenticationExtensions
{
    public const string AuthenticationCookieName = "tl-provider-auth-cookie";
    public const string AuthenticationTypeName = "DfE-SignIn";

    public static void AddProviderAuthentication(
        this IServiceCollection services,
        DfeSignInSettings signInSettings,
        IWebHostEnvironment environment)
    {
        var cookieSecurePolicy = environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;

        var cookieAndSessionTimeout = signInSettings.Timeout;
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
            options.Events = new CookieAuthenticationEvents
            {
                OnValidatePrincipal = async x =>
                {
                    // since our cookie lifetime is based on the access token one,
                    // check if we're more than halfway of the cookie lifetime
                    // assume a timeout of 20 minutes.
                    var timeElapsed = DateTimeOffset.UtcNow.Subtract(x.Properties.IssuedUtc.Value);

                    if (timeElapsed > TimeSpan.FromMinutes(19.5))
                    {
                        var identity = (ClaimsIdentity)x.Principal.Identity;
                        var accessTokenClaim = identity.FindFirst("access_token");
                        var refreshTokenClaim = identity.FindFirst("refresh_token");
                    }
                }
            };
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
            //options.Scope.Add("organisation.id");

            //options.Scope.Add("offline_access");

            // When we expire the session, ensure user is prompted to sign in again at DfE Sign In
            options.MaxAge = overallSessionTimeout;

            options.SaveTokens = true;
            options.CallbackPath = new PathString("/auth/cb");
            options.SignedOutCallbackPath = "/signout/complete";
            //options.SignedOutRedirectUri = "/signout/complete";
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
                    
                    var organisationIds = ctx.Principal
                        .FindAll("Organisation")
                        .Select(org => JsonDocument.Parse(org.Value))
                        .Select(jsonDoc => 
                            jsonDoc
                                .RootElement
                                .SafeGetString("id"))
                        .Where(orgId => orgId != null).ToList();

                    if (organisationIds.Any())
                    {
                        var organisationId = organisationIds.First();
                        var userId = ctx.Principal.FindFirst("sub").Value;

                        var dfeSignInApiClient = ctx.HttpContext.RequestServices.GetRequiredService<IDfeSignInApiService>();
                        var userInfo = await dfeSignInApiClient.GetDfeSignInUserInfo(organisationId, userId);

                        if (userInfo.HasAccessToService)
                        {
                            claims.AddRange(new List<Claim>
                            {
                                new(CustomClaimTypes.UserId, userId),
                                new(CustomClaimTypes.OrganisationId, organisationId),
                                new(CustomClaimTypes.UkPrn, userInfo.UkPrn.HasValue ? userInfo.UkPrn.Value.ToString() : string.Empty),
                                new(ClaimTypes.GivenName, ctx.Principal.FindFirst("given_name").Value),
                                new(ClaimTypes.Surname, ctx.Principal.FindFirst("family_name").Value),
                                new(ClaimTypes.Email, ctx.Principal.FindFirst("email").Value),
                                new(CustomClaimTypes.HasAccessToService, userInfo.HasAccessToService.ToString()),
                                //new Claim(CustomClaimTypes.LoginUserType, ((int)loggedInUserTypeResponse.UserType).ToString())
                            });

                            if (userInfo.Roles != null && userInfo.Roles.Any())
                            {
                                claims.AddRange(userInfo.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));
                            }
                        }
                        else
                        {
                            //Temp code to add org name from original claims
                            //claims.Add(new Claim(CustomClaimTypes.HasAccessToService, "false"));
                            var org = ctx.Principal
                                .FindFirst("Organisation");
                            var organisationName = JsonDocument.Parse(org.Value)
                                .RootElement
                                .SafeGetString("name");
                            claims.Add(new Claim(CustomClaimTypes.DisplayName, organisationName));
                        }
                    }

                    ctx.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, AuthenticationTypeName));

                    // so that we don't issue a session cookie but one with a fixed expiration
                    ctx.Properties.IsPersistent = true;
                    ctx.Properties.ExpiresUtc = DateTime.UtcNow.Add(overallSessionTimeout);
                }
            };
        });
    }
}
