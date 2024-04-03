using AgroGestor360.Client.Models;
using AgroGestor360.Server.Controllers;
using System.Text;
using System.Text.Json;

namespace AgroGestor360.Client.Services;

public interface IBankAccountsService
{
    Task<bool> CheckExistence(string serverURL);
    Task<bool> CheckExistenceByNumber(string serverURL, string number);
    Task<bool> DeleteBankAsync(string serverURL, string id);
    Task<BankAccount?> GetBankByIdAsync(string serverURL, string id);
    Task<BankAccount?> GetBankByNumberAsync(string serverURL, string number);
    Task<IEnumerable<BankAccount>?> GetBanksAsync(string serverURL);
    Task<bool> InsertBankAsync(string serverURL, BankAccount bank);
    Task<bool> UpdateBankAsync(string serverURL, BankAccount bank);
}

public class BankAccountsService : IBankAccountsService
{
    public async Task<bool> CheckExistenceByNumber(string serverURL, string number)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/bankaccounts/exists/{number}");

            if (response.IsSuccessStatusCode)
            {
                var exist = bool.Parse(await response.Content.ReadAsStringAsync());
                return exist;
            }
        }
        return false;
    }

    public async Task<BankAccount?> GetBankByNumberAsync(string serverURL, string number)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/bankaccounts/{number}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<BankAccount>(content, ApiServiceBase.ProviderJSONOptions);
            }
        }
        return null;
    }

    public async Task<bool> CheckExistence(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/bankaccounts/exist");

            if (response.IsSuccessStatusCode)
            {
                var exist = bool.Parse(await response.Content.ReadAsStringAsync());
                return exist;
            }
        }
        return false;
    }

    public async Task<IEnumerable<BankAccount>?> GetBanksAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/bankaccounts");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<BankAccount>>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<BankAccount?> GetBankByIdAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/bankaccounts/{id}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BankAccount>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<bool> InsertBankAsync(string serverURL, BankAccount bank)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var bankJson = JsonSerializer.Serialize(bank);
            var data = new StringContent(bankJson, Encoding.UTF8, "application/json");
            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/bankaccounts", data);

            if (response.IsSuccessStatusCode)
            {
                return bool.Parse(await response.Content.ReadAsStringAsync());
            }
        }
        return false;
    }

    public async Task<bool> UpdateBankAsync(string serverURL, BankAccount bank)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var bankJson = JsonSerializer.Serialize(bank);
            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/bankaccounts", new StringContent(bankJson, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }
        return false;
    }

    public async Task<bool> DeleteBankAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.DeleteAsync($"{serverURL}/bankaccounts/{id}");

            return response.IsSuccessStatusCode;
        }
        return false;
    }
}
