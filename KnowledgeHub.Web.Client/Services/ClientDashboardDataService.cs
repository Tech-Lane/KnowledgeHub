using KnowledgeHub.Shared.Models;
using KnowledgeHub.Shared.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace KnowledgeHub.Web.Client.Services
{
    public class ClientDashboardDataService : IDashboardDataService
    {
        private readonly HttpClient _http;

        public ClientDashboardDataService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Dashboard>> GetDashboardsAsync()
        {
            return await _http.GetFromJsonAsync<List<Dashboard>>("api/Dashboards");
        }

        public async Task<Dashboard> GetDashboardAsync(int id)
        {
            return await _http.GetFromJsonAsync<Dashboard>($"api/Dashboards/{id}");
        }

        public async Task<Dashboard> CreateDashboardAsync(Dashboard dashboard)
        {
            var response = await _http.PostAsJsonAsync("api/Dashboards", dashboard);
            return await response.Content.ReadFromJsonAsync<Dashboard>();
        }

        public async Task<Widget> AddWidgetAsync(Widget widget)
        {
            var response = await _http.PostAsJsonAsync("api/Widgets", widget);
            return await response.Content.ReadFromJsonAsync<Widget>();
        }
    }
} 