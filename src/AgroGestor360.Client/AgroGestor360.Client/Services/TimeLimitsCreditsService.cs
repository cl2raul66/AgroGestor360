using AgroGestor360.Client.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AgroGestor360.Client.Services;

public interface ITimeLimitsCreditsService
{
    Task<bool> DeleteAsync(string serverURL, int id);
    Task<IEnumerable<TimeLimitForCredit>> GetAllAsync(string serverURL);
    Task<TimeLimitForCredit?> GetByIdTimeLimitForCreditAsync(string serverURL, int id);
    Task<TimeLimitForCredit?> GetDefaultAsync(string serverURL);
    Task<int> InsertAsync(string serverURL, TimeLimitForCredit entity);
    Task<bool> SetDefaultAsync(string serverURL, TimeLimitForCredit entity);
}

public class TimeLimitsCreditsService : ITimeLimitsCreditsService
{
    public async Task<IEnumerable<TimeLimitForCredit>> GetAllAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/linecredits/timeslimit");
            if (response.StatusCode is HttpStatusCode.NotFound)
            {
                return [];
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<TimeLimitForCredit>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<TimeLimitForCredit?> GetByIdTimeLimitForCreditAsync(string serverURL, int id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/linecredits/timeslimit/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TimeLimitForCredit>(content, ApiServiceBase.ProviderJSONOptions);
            }
        }
        return null;
    }

    public async Task<int> InsertAsync(string serverURL, TimeLimitForCredit entity)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var content = new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json");
            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/linecredits/timeslimit", content);
            if (response.IsSuccessStatusCode)
            {
                return int.Parse(await response.Content.ReadAsStringAsync());
            }
        }
        return 0;
    }

    public async Task<bool> DeleteAsync(string serverURL, int id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.DeleteAsync($"{serverURL}/linecredits/timeslimit/{id}");
            if (response.IsSuccessStatusCode)
            {
                return bool.Parse(await response.Content.ReadAsStringAsync());
            }
        }
        return false;
    }

    public async Task<TimeLimitForCredit?> GetDefaultAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/linecredits/defaulttimeslimit");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TimeLimitForCredit>(content, ApiServiceBase.ProviderJSONOptions);
            }
        }
        return null;
    }

    public async Task<bool> SetDefaultAsync(string serverURL, TimeLimitForCredit entity)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var content = new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json");
            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/linecredits/defaulttimeslimit", content);
            return response.IsSuccessStatusCode;
        }
        return false;
    }
}
