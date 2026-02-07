using Microsoft.AspNetCore.Mvc.Testing;
using MyApp.Domain;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace MyApp.Api.Tests
{
    public class WeatherForecastControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public WeatherForecastControllerTest(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetForecast()
        {
            var forecasts = await _client.GetFromJsonAsync<IEnumerable<WeatherForecast>>("/WeatherForecast");
            Assert.Equal(5, forecasts.Count());
        }
    }
}
