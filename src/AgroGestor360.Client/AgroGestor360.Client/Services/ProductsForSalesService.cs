using AgroGestor360.Client.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AgroGestor360.Client.Services;

public interface IProductsForSalesService
{
    Task<bool> CheckExistence(string serverURL);
    Task<bool> DeleteAsync(string serverURL, string id);
    Task<IEnumerable<DTO4>> GetAllAsync(string serverURL);
    Task<DTO4?> GetByIdAsync(string serverURL, string id);
    Task<IEnumerable<ProductOffering>> GetOffersById(string serverURL, string id);
    Task<string> InsertAsync(string serverURL, DTO4_1 entity);
    Task<bool> UpdateAsync(string serverURL, object dTO);
}

public class ProductsForSalesService : IProductsForSalesService
{
    public async Task<bool> CheckExistence(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/productsforsales/exist");

            if (response.IsSuccessStatusCode)
            {
                var exist = bool.Parse(await response.Content.ReadAsStringAsync());
                return exist;
            }
        }
        return false;
    }

    public async Task<IEnumerable<DTO4>> GetAllAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/productsforsales");

            if (response.StatusCode is HttpStatusCode.NotFound)
            {
                return [];
            }
            else
            {
                response.EnsureSuccessStatusCode();
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<DTO4>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<IEnumerable<ProductOffering>> GetOffersById(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/productsforsales/offers/{id}");

            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                case HttpStatusCode.BadRequest:
                    return [];
                default:
                    response.EnsureSuccessStatusCode();
                    break;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<ProductOffering>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<DTO4?> GetByIdAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/productsforsales/{id}");

            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                case HttpStatusCode.BadRequest:
                    return null;
                default:
                    response.EnsureSuccessStatusCode();
                    break;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DTO4>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<string> InsertAsync(string serverURL, DTO4_1 entity)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var entityJson = JsonSerializer.Serialize(entity);
            var data = new StringContent(entityJson, Encoding.UTF8, "application/json");
            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/productsforsales", data);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
        return string.Empty;
    }

    public async Task<bool> UpdateAsync(string serverURL, object dTO)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var entityJson = JsonSerializer.Serialize(dTO);
            string route = dTO switch
            {
                DTO4_2 => "send2",
                DTO4_3 => "send3",
                DTO4_4 => "send4",
                _ => string.Empty
            };
            if (string.IsNullOrEmpty(route))
            {
                return false;
            }
            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/productsforsales/{route}", new StringContent(entityJson, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }
        return false;
    }

    public async Task<bool> DeleteAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.DeleteAsync($"{serverURL}/productsforsales/{id}");

            return response.IsSuccessStatusCode;
        }
        return false;
    }
}
