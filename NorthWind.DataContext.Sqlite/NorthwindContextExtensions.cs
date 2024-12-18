using Microsoft.EntityFrameworkCore; // use sqlite
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options; // use IService Collection

namespace Northwind.EntityModels;

public static class NorthwindCotnextExtensions
{

    /// <summary>
    /// Adds NorthwindContext to the specified IServiceCollection. Uses the
    /// Sqlite database provider.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="relativePath"></param>
    /// <param name="databaseName"></param>
    /// <returns>
    /// An IServiceCollection that can be used to add more
    /// services.
    /// </returns>
    /// <exception cref="FileNotFoundException"></exception>
    public static IServiceCollection AddNorthwindContext(
        this IServiceCollection services, // type to extend
        string relativePath = "..",
        string databaseName = "Northwind.db"
    )
    {
        string path = Path.Combine(relativePath, databaseName);
        path = Path.GetFullPath(path);
        NorthwindContextLogger.WriteLine($"database path: {path}");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                message: $"{path} not found.", fileName: path
            );
        }

        services.AddDbContext<NorthwindContext>(options =>
        {
            // Data Source is the modern equivalent of Filename.
            options.UseSqlite($"Data Source={path}");

            options.LogTo(NorthwindContextLogger.WriteLine,
                [Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuting]
                );

        },
        // Register with a transient lifetime to avoid concurrency
        // issues in Blazor server-side projects.
        contextLifetime: ServiceLifetime.Transient,
        optionsLifetime: ServiceLifetime.Transient
        );
        return services;
    }
}
