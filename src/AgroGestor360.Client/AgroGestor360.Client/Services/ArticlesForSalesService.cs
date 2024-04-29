using AgroGestor360.Client.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AgroGestor360.Client.Services;

public interface IArticlesForSalesService
{
    Task<IEnumerable<DTO3>> GetAllAsync(string serverURL);
    Task<DTO3?> GetByIdAsync(string serverURL, string id);
    Task<bool> UpdateAsync(string serverURL, DTO3_1 entity);
}

public class ArticlesForSalesService : IArticlesForSalesService
{
    public async Task<IEnumerable<DTO3>> GetAllAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/articlesforsales");

            if (response.StatusCode is System.Net.HttpStatusCode.NotFound)
            {
                return [];
            }
            else
            {
                response.EnsureSuccessStatusCode();
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<DTO3>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<DTO3?> GetByIdAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/articlesforsales/{id}");

            if (response.StatusCode is HttpStatusCode.NotFound || response.StatusCode is HttpStatusCode.BadRequest)
            {
                return null;
            }
            else
            {
                response.EnsureSuccessStatusCode();
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DTO3>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<bool> UpdateAsync(string serverURL, DTO3_1 entity)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var entityJson = JsonSerializer.Serialize(entity);
            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/articlesforsales", new StringContent(entityJson, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }
        return false;
    }
}
