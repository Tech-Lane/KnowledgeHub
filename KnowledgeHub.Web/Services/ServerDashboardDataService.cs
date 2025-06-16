using KnowledgeHub.Shared.Models;
using KnowledgeHub.Shared.Services;
using KnowledgeHub.Web.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KnowledgeHub.Web.Services
{
    public class ServerDashboardDataService : IDashboardDataService
    {
        private readonly ApplicationDbContext _context;

        public ServerDashboardDataService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Dashboard>> GetDashboardsAsync()
        {
            return await _context.Dashboards.ToListAsync();
        }

        public async Task<Dashboard> GetDashboardAsync(int id)
        {
            return await _context.Dashboards
                .Include(d => d.Widgets)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Dashboard> CreateDashboardAsync(Dashboard dashboard)
        {
            _context.Dashboards.Add(dashboard);
            await _context.SaveChangesAsync();
            return dashboard;
        }

        public async Task<Widget> AddWidgetAsync(Widget widget)
        {
            _context.Widgets.Add(widget);
            await _context.SaveChangesAsync();
            return widget;
        }
    }
} 