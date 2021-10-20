using System;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Sfa.Tl.Find.Provider.Api.Filters
{
    public class HeaderLoggerFilter : IResourceFilter
    {
        private readonly ILogger<HeaderLoggerFilter> _logger;

        public HeaderLoggerFilter(
            ILogger<HeaderLoggerFilter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            try
            {
                var headerInfo = new StringBuilder("Headers: \n");
                foreach (var (headerKey, headerValue) in context.HttpContext.Request.Headers)
                {
                    headerInfo.AppendLine($"{headerKey} = {headerValue}");
                }

                _logger.LogInformation(headerInfo.ToString());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to log headers");
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}
