using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Sfa.Tl.Find.Provider.Api;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    })
    .Build()
    .Run();

