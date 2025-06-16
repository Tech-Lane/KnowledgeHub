using KnowledgeHub.Shared.Models;
using KnowledgeHub.Shared.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KnowledgeHub.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WidgetsController : ControllerBase
    {
        private readonly IDashboardDataService _dashboardDataService;

        public WidgetsController(IDashboardDataService dashboardDataService)
        {
            _dashboardDataService = dashboardDataService;
        }

        [HttpPost]
        public async Task<ActionResult<Widget>> PostWidget(Widget widget)
        {
            var newWidget = await _dashboardDataService.AddWidgetAsync(widget);
            return Ok(newWidget);
        }
    }
} 