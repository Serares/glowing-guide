using Northwind.EntityModels;

#region Configure the webserver host and services.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddNorthwindContext();

var app = builder.Build();
#endregion

#region Configure HTTP pipeline and routes
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.Use(async (HttpContext context, Func<Task> next) =>
{
    RouteEndpoint? rep = context.GetEndpoint() as RouteEndpoint;

    if (rep is not null)
    {
        WriteLine($"Endpont name: {rep.DisplayName}");
        WriteLine($"Endpoint route pattern: {rep.RoutePattern.RawText}");
    }

    if (context.Request.Path == "/bonjour")
    {
        // If endpoint path matches
        // This middleware becomes a terminating delegate returning immediatley 
        await context.Response.WriteAsync("Bonjour Monde!");
        return;
    }

    await next();

    // Here we can modify the response after calling the next delegate
});

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

#endregion

app.MapRazorPages();
app.MapGet("/hello", () => $"Environment is {app.Environment.EnvironmentName}");


app.Run();
