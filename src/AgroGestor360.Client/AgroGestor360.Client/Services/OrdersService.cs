using AgroGestor360.Client.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AgroGestor360.Client.Services;

public interface IOrdersService
{
    Task<bool> ChangeByStatusAsync(string serverURL, DTO8_6 dTO);
    Task<bool> CheckExistence(string serverURL);
    Task<bool> DeleteAsync(string serverURL, string code);
    Task<IEnumerable<DTO8>> GetAllAsync(string serverURL);
    Task<DTO8?> GetByCodeAsync(string serverURL, string code);
    Task<DTO_SB1?> GetDTO_SB1FromQuotationAsync(string serverURL, string code);
    Task<DTO8_5?> GetProductsByCodeAsync(string serverURL, string code);
    Task<string> InsertAsync(string serverURL, DTO8_1 dTO);
    Task<string> InsertFromQuoteAsync(string serverURL, DTO7 dTO);
    Task<bool> UpdateAsync(string serverURL, DTO8_3 dTO);
    Task<bool> UpdateByProductsAndStatus(string serverURL, DTO10_2 dTO);
}

public class OrdersService : IOrdersService
{
    public async Task<bool> CheckExistence(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/orders/exist");

            if (response.IsSuccessStatusCode)
            {
                var result = bool.Parse(await response.Content.ReadAsStringAsync());
                return result;
            }
        }
        return false;
    }

    public async Task<IEnumerable<DTO8>> GetAllAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/orders");

            if (response.StatusCode is HttpStatusCode.NotFound)
            {
                return [];
            }
            else
            {
                response.EnsureSuccessStatusCode();
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<DTO8>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<DTO_SB1?> GetDTO_SB1FromQuotationAsync(string serverURL, string code)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/orders/getdto_sb1fromquotation/{code}");

            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<DTO_SB1>(responseContent, ApiServiceBase.ProviderJSONOptions);
            }
        }
        return null;
    }

    public async Task<DTO8?> GetByCodeAsync(string serverURL, string code)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/orders/{code}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<DTO8>(content, ApiServiceBase.ProviderJSONOptions);
            }

        }
        return null;
    }

    public async Task<DTO8_5?> GetProductsByCodeAsync(string serverURL, string code)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/orders/getproductsbycode/{code}");

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
            return JsonSerializer.Deserialize<DTO8_5>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<string> InsertAsync(string serverURL, DTO8_1 dTO)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(dTO, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/orders", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
        return string.Empty;
    }

    public async Task<string> InsertFromQuoteAsync(string serverURL, DTO7 dTO)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(dTO, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/orders/insertfromquote", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
        return string.Empty;
    }

    public async Task<bool> UpdateAsync(string serverURL, DTO8_3 dTO)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(dTO, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/orders", content);

            return response.IsSuccessStatusCode;
        }
        return false;
    }

    public async Task<bool> ChangeByStatusAsync(string serverURL, DTO8_6 dTO)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(dTO, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/orders/changebystatus", content);

            return response.IsSuccessStatusCode;
        }
        return false;
    }

    public async Task<bool> UpdateByProductsAndStatus(string serverURL, DTO10_2 dTO)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(dTO, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/orders/updatebyproductsandstatus", content);

            return response.IsSuccessStatusCode;
        }
        return false;
    }

    public async Task<bool> DeleteAsync(string serverURL, string code)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.DeleteAsync($"{serverURL}/orders/{code}");

            return response.IsSuccessStatusCode;
        }
        return false;
    }
}
