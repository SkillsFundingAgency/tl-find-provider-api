using System.Text.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sfa.Tl.Find.Provider.Api.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public string ApiAppId { get; }
    public string ApiKey { get; }
    public string QualificationArticleMap { get; }
    public string ArticleBaseUrl { get; }
    public string SearchApiUrl { get; }
        
    public IndexModel(IConfiguration config, ILogger<IndexModel> logger)
    {
        _logger = logger;

        ApiAppId = config["ApiAppId"];
        ApiKey = config["ApiKey"];
        SearchApiUrl = config["SearchApiUrl"];
        ArticleBaseUrl = config["ArticleBaseUrl"];

        var map = config
            .GetSection("QualificationArticleMap").GetChildren()
            .ToDictionary(x => x.Key, x => x.Value);
            
        foreach (var q in map)
        {
            map[q.Key] = $"{ArticleBaseUrl.Trim('/')}/{q.Value}";
        }

        QualificationArticleMap = JsonSerializer.Serialize(map);
    }

    public void OnGet()
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Page model {name} GET called",
                nameof(IndexModel));
        }
    }
}