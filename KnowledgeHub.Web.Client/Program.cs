using KnowledgeHub.Shared.Services;
using KnowledgeHub.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace KnowledgeHub.Web.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            // Add device-specific services used by the KnowledgeHub.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            builder.Services.AddSingleton<IStateService, StateService>();

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddScoped<INoteDataService, ClientNoteDataService>();

            await builder.Build().RunAsync();
        }
    }
}
