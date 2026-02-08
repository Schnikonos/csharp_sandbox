using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Application.service
{
    public class ClientCallService
    {
        private readonly HttpClient _httpClient;

        public ClientCallService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("demoApiClient");
        }

        public async Task<string> GetDataFromApi()
        {
            var response = await _httpClient.GetAsync("/api/data");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
