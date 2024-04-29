using AgroGestor360.Client.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AgroGestor360.Client.Services;

public interface IArticlesForWarehouseService
{
    Task<IEnumerable<DTO2>> GetAllAsync(string serverURL);
    Task<DTO2?> GetByIdAsync(string serverURL, string id);
    Task<bool> UpdateAsync(string serverURL, DTO2_1 entity);
}

public class ArticlesForWarehouseService : IArticlesForWarehouseService
{
    public async Task<IEnumerable<DTO2>> GetAllAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/articlesforwarehouse");

            if (response.StatusCode is HttpStatusCode.NotFound)
            {
                return [];
            }
            else
            {
                response.EnsureSuccessStatusCode();
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<DTO2>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<DTO2?> GetByIdAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/articlesforwarehouse/{id}");

            if (response.StatusCode is HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                response.EnsureSuccessStatusCode();
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DTO2>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<bool> UpdateAsync(string serverURL, DTO2_1 entity)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var entityJson = JsonSerializer.Serialize(entity);
            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/articlesforwarehouse", new StringContent(entityJson, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }
        return false;
    }
}
