using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyWebApi.Infrastructure
{
    public interface IWeatherForecastService
    {
        Task<WeatherForecastResponse> GetWeatherForecast(GetWeatherForecastRequest request);
    }
    public class WeatherForecastService : IWeatherForecastService
    {
        private readonly HttpClient _metaWeatherHttpClient;
        public WeatherForecastService(HttpClient httpClient)
        {
            _metaWeatherHttpClient = httpClient;
        }
        public async Task<WeatherForecastResponse> GetWeatherForecast(GetWeatherForecastRequest request)
        {
            var cityId = await GetCityId(request.CityName);
            var uriBuilder = new UriBuilder();
            var response = await _metaWeatherHttpClient.GetAsync($"location/{cityId}");

            var content = await response.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };

            var forecast = await JsonSerializer.DeserializeAsync<WeatherForecastResponse>(content, options);

            return forecast;
        }

        private async Task<int> GetCityId(string cityName)
        {
            var uriBuilder = new UriBuilder();
            var response = await _metaWeatherHttpClient.GetAsync($"location/search?query={cityName}");

            var content = await response.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };

            var locations = await JsonSerializer.DeserializeAsync<IEnumerable<Location>>(content, options);
            return locations.First().Woeid;
        }
    }

    public record Location(string Title, int Woeid);
    public record GetWeatherForecastRequest( string CityName );
    public record Forecast(DateTime Applicable_Date, string Weather_State_Name, decimal Min_Temp, decimal Max_Temp, decimal The_Temp);
    public record WeatherForecastResponse(string Title, IEnumerable<Forecast> Consolidated_Weather);

}