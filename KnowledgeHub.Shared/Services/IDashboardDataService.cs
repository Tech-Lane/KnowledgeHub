using KnowledgeHub.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KnowledgeHub.Shared.Services
{
    public interface IDashboardDataService
    {
        Task<List<Dashboard>> GetDashboardsAsync();
        Task<Dashboard> GetDashboardAsync(int id);
        Task<Dashboard> CreateDashboardAsync(Dashboard dashboard);
        Task<Widget> AddWidgetAsync(Widget widget);
    }
} 