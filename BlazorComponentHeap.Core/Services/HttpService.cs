using BlazorComponentHeap.Core.Services.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace BlazorComponentHeap.Core.Services;

public class HttpService : IHttpService
{
    public async Task<TResult> PostAsync<TResult, TRequest>(string uri, TRequest body)
        where TResult : class
        where TRequest : class
    {
        var httpClient = new HttpClient() { BaseAddress = new Uri(uri) };
        var request = new HttpRequestMessage(HttpMethod.Post, "");
        request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

        try
        {
            using HttpResponseMessage response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return null!;
            }

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResult>(content)!;
        }
        catch
        { 
        }

        return null!;
    }
}
