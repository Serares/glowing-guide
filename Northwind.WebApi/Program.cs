using Microsoft.AspNetCore.Mvc.Formatters; // to use IOutputFormatter
using Northwind.EntityModels;
using Microsoft.Extensions.Caching.Memory;
using Northwind.WebApi.Repositories;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI; // to use OpenApi spec with Swagger

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<IMemoryCache>(
    new MemoryCache(new MemoryCacheOptions())
);

builder.Services.AddControllers(options =>
{
    WriteLine("Default outpul formatters:");

    foreach (IOutputFormatter formatter in options.OutputFormatters)
    {
        OutputFormatter mediaFormatter = formatter as OutputFormatter;

        if (mediaFormatter is null)
        {
            WriteLine($"{formatter.GetType().Name}");
        }
        else // OutputFormatter class has SupportedMediaTypes
        {
            WriteLine(" {0}, Media types: {1}",
                arg0: mediaFormatter.GetType().Name,
                arg1: string.Join(", ",
                mediaFormatter.SupportedMediaTypes));
        }
    }
})
.AddXmlDataContractSerializerFormatters()
.AddXmlSerializerFormatters();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddNorthwindContext();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Northwind Service API Version 1");

        c.SupportedSubmitMethods([
            SubmitMethod.Get,
            SubmitMethod.Post,
            SubmitMethod.Put,
            SubmitMethod.Delete
        ]);
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
