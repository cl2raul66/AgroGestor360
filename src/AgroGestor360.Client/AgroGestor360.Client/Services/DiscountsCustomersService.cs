using AgroGestor360.Client.Models;
using System.Net;
using System.Text.Json;
using System.Text;

namespace AgroGestor360.Client.Services;

public interface IDiscountsCustomersService
{
    Task<bool> DeleteAsync(string serverURL, int id);
    Task<bool> ExistAsync(string serverURL);
    Task<IEnumerable<DiscountForCustomer>> GetAllAsync(string serverURL);
    Task<DiscountForCustomer?> GetByIdAsync(string serverURL, int id);
    Task<int> InsertAsync(string serverURL, DiscountForCustomer entity);
    Task<bool> UpdateAsync(string serverURL, DiscountForCustomer entity);
}

public class DiscountsCustomersService : IDiscountsCustomersService
{
    public async Task<bool> ExistAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/discountscustomers/exist");
            if (response.IsSuccessStatusCode)
            {
                return bool.Parse(await response.Content.ReadAsStringAsync());
            }
        }
        return false;
    }

    public async Task<IEnumerable<DiscountForCustomer>> GetAllAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/discountscustomers");
            if (response.StatusCode is HttpStatusCode.NotFound)
            {
                return [];
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<DiscountForCustomer>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<DiscountForCustomer?> GetByIdAsync(string serverURL, int id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/discountscustomers/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<DiscountForCustomer>(content, ApiServiceBase.ProviderJSONOptions);
            }
        }
        return null;
    }

    public async Task<int> InsertAsync(string serverURL, DiscountForCustomer entity)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var content = new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json");
            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/discountscustomers", content);
            if (response.IsSuccessStatusCode)
            {
                return int.Parse(await response.Content.ReadAsStringAsync());
            }
        }
        return 0;
    }

    public async Task<bool> UpdateAsync(string serverURL, DiscountForCustomer entity)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var content = new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json");
            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/discountscustomers", content);
            if (response.IsSuccessStatusCode)
            {
                return bool.Parse(await response.Content.ReadAsStringAsync());
            }
        }
        return false;
    }

    public async Task<bool> DeleteAsync(string serverURL, int id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.DeleteAsync($"{serverURL}/discountscustomers/{id}");
            if (response.IsSuccessStatusCode)
            {
                return bool.Parse(await response.Content.ReadAsStringAsync());
            }
        }
        return false;
    }
}
