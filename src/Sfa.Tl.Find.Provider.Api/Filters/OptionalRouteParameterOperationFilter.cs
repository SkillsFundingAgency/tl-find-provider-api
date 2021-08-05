using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sfa.Tl.Find.Provider.Api.Filters
{
    //Adapted from:
    //  https://github.com/fooberichu150/swagger-optional-route-parameters
    //  https://www.seeleycoder.com/blog/optional-route-parameters-with-swagger-asp-net-core/
    public class OptionalRouteParameterOperationFilter : IOperationFilter
    {
        private const string CaptureName = "routeParameter";

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var routeAttributes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Mvc.RouteAttribute>();

            var routeWithOptional = routeAttributes.FirstOrDefault(m => m.Template?.Contains("?") ?? false);
            if (routeWithOptional == null)
                return;

            var regex = $"{{(?<{CaptureName}>\\w+)(:\\w+)*\\?}}";
            var matches = Regex.Matches(routeWithOptional.Template, regex);

            foreach (Match match in matches)
            {
                var name = match.Groups[CaptureName].Value;

                var parameter = operation.Parameters.FirstOrDefault(p => p.In == ParameterLocation.Path && p.Name == name);
                if (parameter != null)
                {
                    parameter.AllowEmptyValue = true;
                    parameter.Description = "Must check \"Send empty value\" or Swagger passes a comma for empty values otherwise";
                    parameter.Required = false;
                    //parameter.Schema.Default = new OpenApiString(string.Empty);
                    parameter.Schema.Nullable = true;
                }
            }
        }
    }
}
