using AgroGestor360Client.Models;
using System.Text.Json;
using System.Text;
using System.Net;

namespace AgroGestor360Client.Services;

public interface IReconciliationService
{
    Task<ReconciliationPolicy?> GetPolicyAsync(string serverURL);
    Task<bool> HasPolicyAsync(string serverURL);
    Task<int> InsertPolicyAsync(string serverURL, ReconciliationPolicy policy);
    Task<bool> UpdatePolicyAsync(string serverURL, ReconciliationPolicy policy);
}

public class ReconciliationService : IReconciliationService
{
    public async Task<ReconciliationPolicy?> GetPolicyAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/Reconciliation/policy");

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
            return JsonSerializer.Deserialize<ReconciliationPolicy>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<bool> HasPolicyAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/Reconciliation/haspolicy");

            if (response.IsSuccessStatusCode)
            {
                var result = bool.Parse(await response.Content.ReadAsStringAsync());
                return result;
            }
        }
        return false;
    }

    public async Task<int> InsertPolicyAsync(string serverURL, ReconciliationPolicy policy)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(policy, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/Reconciliation/policy", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<int>(responseContent);
            }
        }
        return 0;
    }

    public async Task<bool> UpdatePolicyAsync(string serverURL, ReconciliationPolicy policy)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(policy, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/Reconciliation/policy", content);

            return response.IsSuccessStatusCode;
        }
        return false;
    }
}
