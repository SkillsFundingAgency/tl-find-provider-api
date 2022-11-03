using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Infrastructure.Services;
public class SessionService : ISessionService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _environment;

    public SessionService(IHttpContextAccessor httpContextAccessor, string environment)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }

    public void Set(string key, object value)
    {
        _httpContextAccessor.HttpContext.Session.SetString(FormatKey(key),
            JsonSerializer.Serialize(value));
    }

    public void Remove(string key)
    {
        _httpContextAccessor.HttpContext.Session.Remove(FormatKey(key));
    }

    public T? Get<T>(string key)
    {
        var session = _httpContextAccessor.HttpContext.Session;
        key = FormatKey(key);
        
        if (session.Keys.All(k => k != key))
        {
            return default;
        }

        var value = session.GetString(key);
        
        return string.IsNullOrWhiteSpace(value) ? default : JsonSerializer.Deserialize<T>(value);
    }

    public bool Exists(string key)
    {
        return _httpContextAccessor.HttpContext.Session.Keys.Any(k => k == FormatKey(key));
    }

    private string FormatKey(string key) =>
        $"{_environment}_{key}";
}
