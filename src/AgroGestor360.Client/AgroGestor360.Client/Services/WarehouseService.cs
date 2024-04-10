using AgroGestor360.Client.Models;
using System.Text;
using System.Text.Json;

namespace AgroGestor360.Client.Services;

public interface IWarehouseService
{
    Task<bool> CheckExistence(string serverURL);
    Task<bool> DeleteAsync(string serverURL, string id);
    Task<IEnumerable<WarehouseItemGet>> GetAllAsync(string serverURL);
    Task<WarehouseItemGet?> GetByIdAsync(string serverURL, string id);
    Task<string> InsertAsync(string serverURL, WarehouseItemSend entity);
    Task<bool> UpdateAsync(string serverURL, WarehouseItemSend entity);
}

public class WarehouseService : IWarehouseService
{
    public async Task<bool> CheckExistence(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/warehouse/exist");

            if (response.IsSuccessStatusCode)
            {
                var exist = bool.Parse(await response.Content.ReadAsStringAsync());
                return exist;
            }
        }
        return false;
    }

    public async Task<IEnumerable<WarehouseItemGet>> GetAllAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/warehouse");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<WarehouseItemGet>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<WarehouseItemGet?> GetByIdAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/warehouse/{id}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<WarehouseItemGet>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<string> InsertAsync(string serverURL, WarehouseItemSend entity)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var entityJson = JsonSerializer.Serialize(entity);
            var data = new StringContent(entityJson, Encoding.UTF8, "application/json");
            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/warehouse", data);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
        return string.Empty;
    }

    public async Task<bool> UpdateAsync(string serverURL, WarehouseItemSend entity)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var entityJson = JsonSerializer.Serialize(entity);
            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/warehouse", new StringContent(entityJson, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }
        return false;
    }

    public async Task<bool> DeleteAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.DeleteAsync($"{serverURL}/warehouse/{id}");

            return response.IsSuccessStatusCode;
        }
        return false;
    }
}
