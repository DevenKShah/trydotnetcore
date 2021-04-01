using System;
using MyWebApi.Infrastructure;

namespace MyWebApi
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public decimal TemperatureC { get; set; }

        public decimal TemperatureF => 32 + (TemperatureC / 0.5556m);

        public string Summary { get; set; }

        public static implicit operator WeatherForecast(Forecast forecast) => new WeatherForecast
        {
            Date = forecast.Applicable_Date,
            TemperatureC = forecast.The_Temp,
            Summary = forecast.Weather_State_Name
        };
    }
}
