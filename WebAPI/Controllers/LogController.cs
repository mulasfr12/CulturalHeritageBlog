using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Services.Interfaces;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Securing endpoints with JWT authentication
    public class LogController : ControllerBase
    {
        private readonly ILogServices _logServices;

        public LogController(ILogServices logServices)
        {
            _logServices = logServices;
        }

[HttpGet("get/{count:int?}")]
        public async Task<IActionResult> GetLogs(int count = 10) // Default N=10
        {
            var logs = await _logServices.GetLogs(count);
            return Ok(logs);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetLogCount()
        {
            var logCount = await _logServices.GetLogCount();
            return Ok(new { TotalLogs = logCount });
        }
    }
}
