﻿using libs.contracts;
using libs.Contracts;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace api.Controllers
{
    /// <summary>
    /// Health check end point
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly INewsService _newsService;
        private readonly ICache _cache;

        public HealthController(INewsService newsService, ICache cache)
        {
            _newsService = newsService;
            _cache = cache;
        }

        /// <summary>
        /// Return OK (200) if all services and providers are available and healthy 
        /// Return Bad (500) if any of the services or providers are unavailable
        /// </summary>
        /// <returns></returns>
        [HttpGet("Hello")]
        public async Task<IActionResult> Hello()
        {
            try
            {
                //check that hacker news is reachable
                var newsAvailable = await _newsService.Ping();

                if (!newsAvailable)
                {
                    return BadRequest();
                }

                var cacheAvailable = await _cache.Ping();
                if (!cacheAvailable)
                {
                    return BadRequest();
                }

                return Ok();

            }
            catch (Exception ex)
            {
                Log.Error($"Failed while checking dependency upness.{ex.Message}.{ex.StackTrace}", ex);

                return BadRequest();
            }
        }
    }
}
