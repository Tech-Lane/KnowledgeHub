using KnowledgeHub.Shared.Services;
using KnowledgeHub.Web.Components;
using KnowledgeHub.Web.Data;
using KnowledgeHub.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();

            builder.Services.AddControllers();

            // Add device-specific services used by the KnowledgeHub.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.AddSingleton<IStateService, StateService>();

            builder.Services.AddScoped<INoteDataService, ServerNoteDataService>();
            builder.Services.AddScoped<IDashboardDataService, ServerDashboardDataService>();

            // connection to SQLite database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(
                    typeof(KnowledgeHub.Shared._Imports).Assembly,
                    typeof(KnowledgeHub.Web.Client._Imports).Assembly);

            app.MapControllers();

            app.Run();
        }
    }
}
