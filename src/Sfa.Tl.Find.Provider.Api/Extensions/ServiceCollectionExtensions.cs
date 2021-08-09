using System;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Sfa.Tl.Find.Provider.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCorsPolicy(
            this IServiceCollection services, 
            string policyName, 
            string allowedOrigins)
        {
            if (!string.IsNullOrWhiteSpace(allowedOrigins))
            {
                var corsOrigins = allowedOrigins.Split(';', ',');
                services.AddCors(options => options.AddPolicy(policyName, builder =>
                    builder
                        .WithMethods(HttpMethod.Get.Method)
                        .AllowAnyHeader()
                        .WithOrigins(corsOrigins)));
            }

            return services;
        }

        public static IServiceCollection AddSwagger(
            this IServiceCollection services, 
            string name, 
            string title,
            string version,
            string xmlFile = null)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(name,
                    new OpenApiInfo
                    {
                        Title = title,
                        Version = version
                    });

                if (xmlFile != null)
                {
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }
  }
}
