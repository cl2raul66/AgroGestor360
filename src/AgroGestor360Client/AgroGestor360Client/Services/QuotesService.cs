using AgroGestor360Client.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AgroGestor360Client.Services;

public interface IQuotesService
{
    Task<bool> ChangesByStatusAsync(string serverURL, DTO7_3 dTO);
    Task<bool> CheckExistence(string serverURL);
    Task<bool> DeleteAsync(string serverURL, string code);
    Task<IEnumerable<DTO7>> GetAllAsync(string serverURL);
    Task<DTO7?> GetByCodeAsync(string serverURL, string code);
    Task<DTO8_2?> GetOrderByCodeAsync(string serverURL, string code);
    Task<DTO7_4?> GetProductsByCodeAsync(string serverURL, string code);
    Task<string> InsertAsync(string serverURL, DTO7_1 dTO);
    Task<bool> UpdateAsync(string serverURL, DTO7_2 dTO);
}

public class QuotesService : IQuotesService
{
    public async Task<bool> CheckExistence(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/quotes/exist");

            if (response.IsSuccessStatusCode)
            {
                var result = bool.Parse(await response.Content.ReadAsStringAsync());
                return result;
            }
        }
        return false;
    }

    public async Task<IEnumerable<DTO7>> GetAllAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/quotes");

            if (response.StatusCode is HttpStatusCode.NotFound)
            {
                return [];
            }
            else
            {
                response.EnsureSuccessStatusCode();
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<DTO7>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<DTO7?> GetByCodeAsync(string serverURL, string code)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/quotes/{code}");

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
            return JsonSerializer.Deserialize<DTO7>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<DTO7_4?> GetProductsByCodeAsync(string serverURL, string code)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/quotes/getproductsbycode/{code}");

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
            return JsonSerializer.Deserialize<DTO7_4>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<DTO8_2?> GetOrderByCodeAsync(string serverURL, string code)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/quotes/getorderbycode/{code}");

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
            return JsonSerializer.Deserialize<DTO8_2>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<string> InsertAsync(string serverURL, DTO7_1 dTO)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(dTO, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/quotes", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
        return string.Empty;
    }

    public async Task<bool> UpdateAsync(string serverURL, DTO7_2 dTO)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(dTO, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/quotes", content);

            return response.IsSuccessStatusCode;
        }
        return false;
    }

    public async Task<bool> ChangesByStatusAsync(string serverURL, DTO7_3 dTO)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(dTO, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/quotes/changerbystatus", content);

            return response.IsSuccessStatusCode;
        }
        return false;
    }

    public async Task<bool> DeleteAsync(string serverURL, string code)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.DeleteAsync($"{serverURL}/quotes/{code}");

            return response.IsSuccessStatusCode;
        }
        return false;
    }
}
