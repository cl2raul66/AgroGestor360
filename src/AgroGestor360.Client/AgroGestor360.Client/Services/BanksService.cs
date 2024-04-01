using AgroGestor360.Client.Models;
using AgroGestor360.Server.Controllers;
using System.Text;
using System.Text.Json;

namespace AgroGestor360.Client.Services;

public interface IBanksService
{
    Task<bool> CheckExistence(string serverURL);
    Task<bool> DeleteBankAsync(string serverURL, string id);
    Task<Bank?> GetBankByIdAsync(string serverURL, string id);
    Task<IEnumerable<Bank>?> GetBanksAsync(string serverURL);
    Task<string> InsertBankAsync(string serverURL, Bank bank);
    Task<bool> UpdateBankAsync(string serverURL, Bank bank);
}

public class BanksService : IBanksService
{
    public async Task<bool> CheckExistence(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/exist");

            if (response.IsSuccessStatusCode)
            {
                var exist = bool.Parse(await response.Content.ReadAsStringAsync());
                return exist;
            }
        }
        return false;
    }

    public async Task<IEnumerable<Bank>?> GetBanksAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/banks");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<Bank>>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<Bank?> GetBankByIdAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/banks/{id}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Bank>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<string> InsertBankAsync(string serverURL, Bank bank)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var bankJson = JsonSerializer.Serialize(bank);
            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/banks", new StringContent(bankJson, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        return string.Empty;
    }

    public async Task<bool> UpdateBankAsync(string serverURL, Bank bank)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var bankJson = JsonSerializer.Serialize(bank);
            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/banks", new StringContent(bankJson, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
        }
        return false;
    }

    public async Task<bool> DeleteBankAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.DeleteAsync($"{serverURL}/banks/{id}");

            response.EnsureSuccessStatusCode();
        }
        return false;
    }
}

