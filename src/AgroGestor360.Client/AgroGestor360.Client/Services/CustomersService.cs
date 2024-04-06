using System.Text;
using System.Text.Json;
using vCardLib.Models;

namespace AgroGestor360.Client.Services;

public interface ICustomersService
{
    Task<bool> DeleteAsync(string serverURL, string id);
    Task<bool> ExistAsync(string serverURL);
    Task<IEnumerable<vCard>> GetAllAsync(string serverURL);
    Task<IEnumerable<vCard>> GetAllByNameAsync(string serverURL, string name);
    Task<vCard?> GetByIdAsync(string serverURL, string id);
    Task<bool> InsertAsync(string serverURL, vCard card);
    Task<bool> UpdateAsync(string serverURL, vCard card);
}

public class CustomersService : ICustomersService
{
    public async Task<bool> ExistAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/customers/exist");
            if (response.IsSuccessStatusCode)
            {
                return bool.Parse(await response.Content.ReadAsStringAsync());
            }
        }
        return false;
    }

    public async Task<IEnumerable<vCard>> GetAllAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/customers");
            if (response.StatusCode is System.Net.HttpStatusCode.NotFound)
            {
                return [];
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<vCard>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<IEnumerable<vCard>> GetAllByNameAsync(string serverURL, string name)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/customers/byname/{name}");
            if (response.StatusCode is System.Net.HttpStatusCode.NotFound)
            {
                return [];
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<vCard>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<vCard?> GetByIdAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/customers/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<vCard>(content, ApiServiceBase.ProviderJSONOptions);
            }
        }
        return null;
    }

    public async Task<bool> InsertAsync(string serverURL, vCard card)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var content = new StringContent(JsonSerializer.Serialize(card), Encoding.UTF8, "application/json");
            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/customers", content);
            if (response.IsSuccessStatusCode)
            {
                return bool.Parse(await response.Content.ReadAsStringAsync());
            }
        }
        return false;
    }

    public async Task<bool> UpdateAsync(string serverURL, vCard card)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var content = new StringContent(JsonSerializer.Serialize(card), Encoding.UTF8, "application/json");
            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/customers", content);
            if (response.IsSuccessStatusCode)
            {
                return bool.Parse(await response.Content.ReadAsStringAsync());
            }
        }
        return false;
    }

    public async Task<bool> DeleteAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.DeleteAsync($"{serverURL}/customers/{id}");
            if (response.IsSuccessStatusCode)
            {
                return bool.Parse(await response.Content.ReadAsStringAsync());
            }
        }
        return false;
    }
}
