using AgroGestor360.Client.Models;
using System.Text;
using System.Text.Json;

namespace AgroGestor360.Client.Services;

public interface ICustomersService
{
    Task<bool> ExistAsync(string serverURL);
    Task<IEnumerable<CustomerDiscountClass>> GetAllDiscountAsync(string serverURL);
    Task<IEnumerable<DTO5_1>> GetAllWithDiscountAsync(string serverURL);
    Task<IEnumerable<DTO5_1>> GetAllWithoutDiscountAsync(string serverURL);
    Task<DTO5_3?> GetByIdAsync(string serverURL, string id);
    Task<string> InsertAsync(string serverURL, DTO5_2 dTO);
    Task<bool> UpdateAsync(string serverURL, DTO5_3 dTO);
    Task<bool> UpdateDiscountAsync(string serverURL, DTO5_4 dTO);
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

    public async Task<IEnumerable<CustomerDiscountClass>> GetAllDiscountAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/customers/getalldiscount");
            if (response.StatusCode is System.Net.HttpStatusCode.NotFound)
            {
                return [];
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<CustomerDiscountClass>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<IEnumerable<DTO5_1>> GetAllWithDiscountAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/customers/getallwithdiscount");
            if (response.StatusCode is System.Net.HttpStatusCode.NotFound)
            {
                return [];
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<DTO5_1>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<IEnumerable<DTO5_1>> GetAllWithoutDiscountAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/customers/getallwithoutdiscount");
            if (response.StatusCode is System.Net.HttpStatusCode.NotFound)
            {
                return [];
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<DTO5_1>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<DTO5_3?> GetByIdAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/customers/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<DTO5_3>(content, ApiServiceBase.ProviderJSONOptions);
            }
        }
        return null;
    }

    public async Task<string> InsertAsync(string serverURL, DTO5_2 dTO)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var content = new StringContent(JsonSerializer.Serialize(dTO), Encoding.UTF8, "application/json");
            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/customers", content);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
        return string.Empty;
    }

    public async Task<bool> UpdateAsync(string serverURL, DTO5_3 dTO)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var content = new StringContent(JsonSerializer.Serialize(dTO), Encoding.UTF8, "application/json");
            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/customers", content);
            if (response.IsSuccessStatusCode)
            {
                return bool.Parse(await response.Content.ReadAsStringAsync());
            }
        }
        return false;
    }

    public async Task<bool> UpdateDiscountAsync(string serverURL, DTO5_4 dTO)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var content = new StringContent(JsonSerializer.Serialize(dTO), Encoding.UTF8, "application/json");
            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/customers/updatediscount", content);
            if (response.IsSuccessStatusCode)
            {
                return bool.Parse(await response.Content.ReadAsStringAsync());
            }
        }
        return false;
    }
}
