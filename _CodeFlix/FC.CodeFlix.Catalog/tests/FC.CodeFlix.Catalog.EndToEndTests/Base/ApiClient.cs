using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FC.CodeFlix.Catalog.EndToEndTests.Base
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient(HttpClient httpClient) => this._httpClient = httpClient;


        private async Task<TOutput?> GetOutput<TOutput> (HttpResponseMessage response)
            where TOutput : class
        {
            var outputString = await response.Content.ReadAsStringAsync();
            TOutput? output = null;
            if (!string.IsNullOrWhiteSpace(outputString))
                output = JsonSerializer.Deserialize<TOutput>(outputString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return output;
        }


        public async Task<(HttpResponseMessage?, TOutput?)> Post<TOutput>(string route, object payload)
            where TOutput : class
        {
            var response = await this._httpClient.PostAsync(route, new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
            var output = await this.GetOutput<TOutput>(response);

            return (response, output);
        }

        public async Task<(HttpResponseMessage?, TOutput?)> Put<TOutput>(string route, object payload)
            where TOutput : class
        {
            var response = await this._httpClient.PutAsync(route, new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
            var output = await this.GetOutput<TOutput>(response);

            return (response, output);
        }

        public async Task<(HttpResponseMessage?, TOutput?)> Delete<TOutput>(string route)
            where TOutput : class
        {
            var response = await this._httpClient.DeleteAsync(route);
            var output = await this.GetOutput<TOutput>(response);

            return (response, output);
        }

        public async Task<(HttpResponseMessage?, TOutput?)> Get<TOutput>(string route, object? queryStringParametersObject = null)
            where TOutput : class
        {
            var url = this.PrepareGetRout(route, queryStringParametersObject);
            var response = await this._httpClient.GetAsync(url);
            var output = await this.GetOutput<TOutput>(response);

            return (response, output);
        }

        private string PrepareGetRout(string route, object? queryStringParametersObject = null)
        {
            if (queryStringParametersObject is null)
                return route;

            var parametersJson = JsonSerializer.Serialize(queryStringParametersObject);
            var parameterDictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(parametersJson);
            return QueryHelpers.AddQueryString(route, parameterDictionary!);
        }
    }
}
