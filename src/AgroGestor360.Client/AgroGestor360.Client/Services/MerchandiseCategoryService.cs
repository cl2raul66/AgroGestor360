using AgroGestor360.Client.Models;
using System.Text;
using System.Text.Json;

namespace AgroGestor360.Client.Services;

public interface IMerchandiseCategoryService
{
    Task<bool> CheckExistence(string serverURL);
    Task<bool> DeleteAsync(string serverURL, string id);
    Task<IEnumerable<MerchandiseCategory>?> GetAllAsync(string serverURL);
    Task<MerchandiseCategory?> GetByIdAsync(string serverURL, string id);
    Task<string> InsertAsync(string serverURL, MerchandiseCategory entity);
    Task<bool> UpdateAsync(string serverURL, MerchandiseCategory entity);
}

public class MerchandiseCategoryService : IMerchandiseCategoryService
{
    public async Task<bool> CheckExistence(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/merchandisecategory/exist");

            if (response.IsSuccessStatusCode)
            {
                var exist = bool.Parse(await response.Content.ReadAsStringAsync());
                return exist;
            }
        }
        return false;
    }

    public async Task<IEnumerable<MerchandiseCategory>?> GetAllAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/merchandisecategory");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<MerchandiseCategory>>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<MerchandiseCategory?> GetByIdAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/merchandisecategory/{id}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<MerchandiseCategory>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<string> InsertAsync(string serverURL, MerchandiseCategory entity)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var entityJson = JsonSerializer.Serialize(entity);
            var data = new StringContent(entityJson, Encoding.UTF8, "application/json");
            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/merchandisecategory", data);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
        return string.Empty;
    }

    public async Task<bool> UpdateAsync(string serverURL, MerchandiseCategory entity)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var entityJson = JsonSerializer.Serialize(entity);
            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/merchandisecategory", new StringContent(entityJson, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }
        return false;
    }

    public async Task<bool> DeleteAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.DeleteAsync($"{serverURL}/merchandisecategory/{id}");

            return response.IsSuccessStatusCode;
        }
        return false;
    }
}
