using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using KnowledgeHub.Web.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Get the current directory and build a configuration object to read appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        // Create a new DbContextOptionsBuilder
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Get the connection string from your configuration file
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Tell the builder to use SQLite with that connection string
        builder.UseSqlite(connectionString);

        // Create and return a new instance of the ApplicationDbContext
        return new ApplicationDbContext(builder.Options);
    }
}