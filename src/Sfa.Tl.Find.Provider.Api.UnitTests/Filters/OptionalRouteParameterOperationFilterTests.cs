using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Sfa.Tl.Find.Provider.Api.Filters;
using Microsoft.OpenApi.Models;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Filters
{
    public class OptionalRouteParameterOperationFilterTests
    {
        [Fact]
        public void OptionalRouteParameterOperationFilter_Sets_Required_To_False_For_Optional_Path_Parameter()
        {
            const string targetParameterName = "qualificationId";

            var operation = new OpenApiOperation
            {
                OperationId = nameof(FindProvidersController.GetProviders),
                Parameters = new List<OpenApiParameter>
                    {
                        new()
                        {
                            Name = targetParameterName,
                            In = ParameterLocation.Path,
                            Schema = new OpenApiSchema(),
                            Required = true
                        }}
            };

            var methodInfo = typeof(FindProvidersController)
                .GetMethod(nameof(FindProvidersController.GetProviders));

            var filterContext = new OperationFilterContext(
                new ApiDescription(),
                Substitute.For<ISchemaGenerator>(),
                new SchemaRepository(),
                methodInfo);

            var filter = new OptionalRouteParameterOperationFilter();

            filter.Apply(operation, filterContext);

            operation.Parameters.Single(p => p.Name == targetParameterName).Required = false;
        }

        [Fact]
        public void OptionalRouteParameterOperationFilter_Does_Not_Set_Required_To_False_For_Non_Optional_Path_Parameter()
        {
            const string targetParameterName = "postCode";

            var operation = new OpenApiOperation
            {
                OperationId = nameof(FindProvidersController.GetProviders),
                Parameters = new List<OpenApiParameter>
                {
                    new()
                    {
                        Name = targetParameterName,
                        In = ParameterLocation.Path,
                        Schema = new OpenApiSchema(),
                        Required = true
                    }}
            };

            var methodInfo = typeof(FindProvidersController)
                .GetMethod(nameof(FindProvidersController.GetProviders));

            var filterContext = new OperationFilterContext(
                new ApiDescription(),
                Substitute.For<ISchemaGenerator>(),
                new SchemaRepository(),
                methodInfo);

            var filter = new OptionalRouteParameterOperationFilter();

            filter.Apply(operation, filterContext);

            operation.Parameters.Single(p => p.Name == targetParameterName).Required = true;
        }
    }
}
