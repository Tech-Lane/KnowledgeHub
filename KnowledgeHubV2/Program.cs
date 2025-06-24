using KnowledgeHubV2;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using KnowledgeHubV2.Services;
using KnowledgeHubV2.Data;
using Microsoft.EntityFrameworkCore;
using Radzen;

// This is required to load the native SQLite library in the browser.
SQLitePCL.Batteries_V2.Init();

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register the FileSystemService as a singleton so the file handle is preserved across pages.
builder.Services.AddSingleton<FileSystemService>();

// Register the DatabaseStateService as a singleton to manage the DB lifecycle.
builder.Services.AddSingleton<DatabaseStateService>();

// Register a factory for creating DbContext instances.
// This is the recommended pattern for Blazor to avoid issues with context lifetime.
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite("DataSource=file:memdb1?mode=memory&cache=shared", 
    sqliteOptions => {
        sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    }));

// Register the repository for notes and folders.
builder.Services.AddScoped<NoteRepository>();

// Register the state container for inter-component communication.
builder.Services.AddScoped<StateContainer>();

// Add custom services
builder.Services.AddScoped<ReferenceProcessingService>();
builder.Services.AddScoped<LocalBackupService>();

builder.Services.AddRadzenComponents();

builder.Services.AddHttpClient();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add Radzen services
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

await builder.Build().RunAsync();
