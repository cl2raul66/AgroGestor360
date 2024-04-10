using AgroGestor360.Client.Models;
using System.Text;
using System.Text.Json;

namespace AgroGestor360.Client.Services;

public interface IMerchandiseService
{
    Task<bool> CheckExistence(string serverURL);
    Task<bool> DeleteAsync(string serverURL, string id);
    Task<IEnumerable<Merchandise>?> GetAllByNameAsync(string serverURL, string name);
    Task<IEnumerable<string>?> GetAllCategoriesAsync(string serverURL);
    Task<IEnumerable<Merchandise>?> GetAllEnabledAsync(string serverURL);
    Task<Merchandise?> GetByIdAsync(string serverURL, string id);
    Task<string> InsertAsync(string serverURL, Merchandise entity);
    Task<bool> UpdateAsync(string serverURL, Merchandise entity);
}

public class MerchandiseService : IMerchandiseService
{
    public async Task<bool> CheckExistence(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/merchandise/exist");

            if (response.IsSuccessStatusCode)
            {
                var exist = bool.Parse(await response.Content.ReadAsStringAsync());
                return exist;
            }
        }
        return false;
    }

    public async Task<IEnumerable<Merchandise>?> GetAllEnabledAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/merchandise");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<Merchandise>>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<IEnumerable<Merchandise>?> GetAllByNameAsync(string serverURL, string name)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/merchandise/byname/{name}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<Merchandise>>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<IEnumerable<string>?> GetAllCategoriesAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/merchandise/allcategories");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<string>>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<Merchandise?> GetByIdAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/merchandise/{id}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Merchandise>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<string> InsertAsync(string serverURL, Merchandise entity)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var entityJson = JsonSerializer.Serialize(entity);
            var data = new StringContent(entityJson, Encoding.UTF8, "application/json");
            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/merchandise", data);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
        return string.Empty;
    }

    public async Task<bool> UpdateAsync(string serverURL, Merchandise entity)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var entityJson = JsonSerializer.Serialize(entity);
            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/merchandise", new StringContent(entityJson, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }
        return false;
    }

    public async Task<bool> DeleteAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.DeleteAsync($"{serverURL}/merchandise/{id}");

            return response.IsSuccessStatusCode;
        }
        return false;
    }
}
