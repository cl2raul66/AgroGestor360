using AgroGestor360.Client.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AgroGestor360.Client.Services;

public interface ITypesDiscountsCustomersService
{
    Task<bool> CheckExistence(string serverURL);
    Task<bool> DeleteAsync(string serverURL, int id);
    Task<IEnumerable<CustomerDiscountClass>> GetAllAsync(string serverURL);
    Task<IEnumerable<CustomerDiscountClass>> GetAllDefaultAsync(string serverURL);
    Task<CustomerDiscountClass?> GetByIdDefaultAsync(string serverURL, int id);
    Task<int> InsertAsync(string serverURL, CustomerDiscountClass entity);
    Task<bool> UpdateAsync(string serverURL, CustomerDiscountClass entity);
}

public class TypesDiscountsCustomersService : ITypesDiscountsCustomersService
{
    public async Task<bool> CheckExistence(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/typesdiscountscustomers/exist");

            if (response.IsSuccessStatusCode)
            {
                var exist = bool.Parse(await response.Content.ReadAsStringAsync());
                return exist;
            }
        }
        return false;
    }

    public async Task<IEnumerable<CustomerDiscountClass>> GetAllAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/typesdiscountscustomers");

            if (response.StatusCode is System.Net.HttpStatusCode.NotFound)
            {
                return [];
            }
            else
            {
                response.EnsureSuccessStatusCode();
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<CustomerDiscountClass>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<int> InsertAsync(string serverURL, CustomerDiscountClass entity)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var entityJson = JsonSerializer.Serialize(entity);
            var data = new StringContent(entityJson, Encoding.UTF8, "application/json");
            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/typesdiscountscustomers", data);

            if (response.IsSuccessStatusCode)
            {
                return int.Parse(await response.Content.ReadAsStringAsync());
            }
        }
        return 0;
    }

    public async Task<bool> UpdateAsync(string serverURL, CustomerDiscountClass entity)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var entityJson = JsonSerializer.Serialize(entity);
            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/typesdiscountscustomers", new StringContent(entityJson, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }
        return false;
    }

    public async Task<bool> DeleteAsync(string serverURL, int id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.DeleteAsync($"{serverURL}/typesdiscountscustomers/{id}");

            return response.IsSuccessStatusCode;
        }
        return false;
    }

    public async Task<IEnumerable<CustomerDiscountClass>> GetAllDefaultAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/typesdiscountscustomers/getalldefault");

            if (response.StatusCode is HttpStatusCode.NotFound)
            {
                return [];
            }
            else
            {
                response.EnsureSuccessStatusCode();
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<CustomerDiscountClass>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<CustomerDiscountClass?> GetByIdDefaultAsync(string serverURL, int id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/typesdiscountscustomers/getbyiddefault/{id}");

            if (response.StatusCode is HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                response.EnsureSuccessStatusCode();
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CustomerDiscountClass>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }
}
