using KnowledgeHub.Shared.Models;
using KnowledgeHub.Shared.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KnowledgeHub.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardsController : ControllerBase
    {
        private readonly IDashboardDataService _dashboardDataService;

        public DashboardsController(IDashboardDataService dashboardDataService)
        {
            _dashboardDataService = dashboardDataService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Dashboard>>> GetDashboards()
        {
            return await _dashboardDataService.GetDashboardsAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Dashboard>> GetDashboard(int id)
        {
            var dashboard = await _dashboardDataService.GetDashboardAsync(id);
            if (dashboard == null)
            {
                return NotFound();
            }
            return dashboard;
        }

        [HttpPost]
        public async Task<ActionResult<Dashboard>> PostDashboard(Dashboard dashboard)
        {
            var newDashboard = await _dashboardDataService.CreateDashboardAsync(dashboard);
            return CreatedAtAction(nameof(GetDashboard), new { id = newDashboard.Id }, newDashboard);
        }
    }
} 