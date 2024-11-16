using AgroGestor360Client.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AgroGestor360Client.Services;

public interface ISaleRecordsService
{
    Task<bool> ChangeByStatusAsync(string serverURL, DTO10_3 dTO);
    Task<bool> CheckExistence(string serverURL);
    Task<bool> DeleteAsync(string serverURL, string code);
    Task<bool> DeleteConceptAsync(string serverURL, int id);
    Task<bool> RepaymentAsync(string serverURL, DTO10_2 dTO);
    Task<IEnumerable<DTO10>> GetAllAsync(string serverURL);
    Task<DTO10?> GetByCodeAsync(string serverURL, string code);
    Task<IEnumerable<ConceptForDeletedSaleRecord>> GetConceptsAsync(string serverURL);
    Task<IEnumerable<int>> GetCreditTimeAsync(string serverURL);
    Task<DTO_SB1?> GetDTO_SB1FromOrderAsync(string serverURL, string code);
    Task<DTO10_4?> GetProductsByCodeAsync(string serverURL, string code);
    Task<string> InsertAsync(string serverURL, DTO10_1 dTO);
    Task<int> InsertConceptAsync(string serverURL, ConceptForDeletedSaleRecord entity);
    Task<string> InsertFromOrderAsync(string serverURL, DTO8 dTO);
    Task<string> InsertFromOrderWithModificationsAsync(string serverURL, DTO10_1 dTO);
    Task<string> InsertFromQuoteAsync(string serverURL, DTO10_2 dTO);
}

public class SaleRecordsService : ISaleRecordsService
{
    public async Task<IEnumerable<int>> GetCreditTimeAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/SaleRecords/getcredittime");

            if (response.StatusCode is HttpStatusCode.NotFound)
            {
                return [];
            }
            else
            {
                response.EnsureSuccessStatusCode();
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<int>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<bool> CheckExistence(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/SaleRecords/exist");

            if (response.IsSuccessStatusCode)
            {
                var result = bool.Parse(await response.Content.ReadAsStringAsync());
                return result;
            }
        }
        return false;
    }

    public async Task<IEnumerable<DTO10>> GetAllAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/SaleRecords");

            if (response.StatusCode is HttpStatusCode.NotFound)
            {
                return [];
            }
            else
            {
                response.EnsureSuccessStatusCode();
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<DTO10>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<DTO10?> GetByCodeAsync(string serverURL, string code)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/SaleRecords/{code}");

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
            return JsonSerializer.Deserialize<DTO10>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<DTO10_4?> GetProductsByCodeAsync(string serverURL, string code)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/SaleRecords/getproductsbycode/{code}");

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
            return JsonSerializer.Deserialize<DTO10_4>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<DTO_SB1?> GetDTO_SB1FromOrderAsync(string serverURL, string code)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/SaleRecords/getdto_sb1fromorder/{code}");

            if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.BadRequest)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }

            return JsonSerializer.Deserialize<DTO_SB1>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<string> InsertAsync(string serverURL, DTO10_1 dTO)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(dTO, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/SaleRecords", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
        return string.Empty;
    }

    public async Task<string> InsertFromQuoteAsync(string serverURL, DTO10_2 dTO)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(dTO, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/SaleRecords/insertfromquote", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
        return string.Empty;
    }

    public async Task<string> InsertFromOrderAsync(string serverURL, DTO8 dTO)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(dTO, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/SaleRecords/insertfromorder", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
        return string.Empty;
    }

    public async Task<string> InsertFromOrderWithModificationsAsync(string serverURL, DTO10_1 dTO)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(dTO, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/SaleRecords/InsertFromOrderWithModifications", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
        return string.Empty;
    }

    public async Task<bool> RepaymentAsync(string serverURL, DTO10_2 dTO)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(dTO, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/SaleRecords/Repayment", content);

            return response.IsSuccessStatusCode;
        }
        return false;
    }

    public async Task<bool> ChangeByStatusAsync(string serverURL, DTO10_3 dTO)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(dTO, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/SaleRecords/ChangeByStatus", content);

            return response.IsSuccessStatusCode;
        }
        return false;
    }

    public async Task<bool> DeleteAsync(string serverURL, string code)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.DeleteAsync($"{serverURL}/SaleRecords/{code}");

            return response.IsSuccessStatusCode;
        }
        return false;
    }


    public async Task<IEnumerable<ConceptForDeletedSaleRecord>> GetConceptsAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/SaleRecords/concepts");

            if (response.StatusCode is not HttpStatusCode.OK)
            {
                return [];
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<ConceptForDeletedSaleRecord>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<int> InsertConceptAsync(string serverURL, ConceptForDeletedSaleRecord entity)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(entity, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/SaleRecords/concepts", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return int.Parse(result);
            }
        }
        return 0;
    }

    public async Task<bool> DeleteConceptAsync(string serverURL, int id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.DeleteAsync($"{serverURL}/SaleRecords/concepts/{id}");

            return response.IsSuccessStatusCode;
        }
        return false;
    }
}
