
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

//Minimal API for loading locations
//app.MapGet("/api/locations/{name}", 
//    async ([FromServices] ILocationService locationService, 
//        string name) => await locationService.SayHello(name));

//call with https://localhost:7026/api/locations/search?term=xxx
//app.MapGet("/api/locations/search/",
//    async ([FromServices] ILocationService locationService,
//        SearchTerms searchTerms) => 
//        await locationService.Search(searchTerms));

app.Run();

