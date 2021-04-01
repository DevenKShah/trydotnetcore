using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyWebApi.Infrastructure;

namespace MyWebApi.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [ApiVersion("2")]
    [Route("/api/v{version:apiVersion}/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> logger;
        private readonly IWeatherForecastService weatherForecastService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecastService weatherForecastService)
        {
            this.logger = logger;
            this.weatherForecastService = weatherForecastService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string cityName)
        {
            var fc = await weatherForecastService.GetWeatherForecast(new GetWeatherForecastRequest(cityName));
            var x = fc.Consolidated_Weather.Select(f => (WeatherForecast)f);
            return Ok(new {CityName = fc.Title, Forecast = x});
        }

        [MapToApiVersion("2")]
        [ApiExplorerSettings(GroupName = "v2")]
        [HttpGet]
        public IEnumerable<WeatherForecast> GetV2()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
            })
            .ToArray();
        }
    }

    [ApiVersion("3")]
    [ApiController]
    [Route("/api/v{version:apiVersion}/WeatherForecast")]
    [ApiExplorerSettings(GroupName = "v3")]
    public class WeatherForecastV2Controller: ControllerBase
    {
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = "v3"
            })
            .ToArray();
        }
    }
}
