using AgroGestor360Client.Models;
using System.Text;
using System.Text.Json;

namespace AgroGestor360Client.Services;

public interface IBankAccountsService
{
    Task<bool> DeleteAsync(string serverURL, string id);
    Task<bool> DeleteBankItemAsync(string serverURL, string id);
    Task<bool> ExistAsync(string serverURL);
    Task<IEnumerable<BankAccount>> GetAllAsync(string serverURL);
    Task<IEnumerable<BankItem>> GetAllBankItemAsync(string serverURL);
    Task<BankItem?> GetBankItemByIdAsync(string serverURL, string id);
    Task<BankAccount?> GetByIdAsync(string serverURL, string id);
    Task<bool> InsertAsync(string serverURL, BankAccount entity);
    Task<bool> InsertBankItemAsync(string serverURL, BankItem entity);
    Task<bool> UpdateAsync(string serverURL, BankAccount entity);
    Task<bool> UpdateBankItemAsync(string serverURL, BankItem entity);
}

public class BankAccountsService : IBankAccountsService
{
    public async Task<bool> ExistAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/bankaccounts/exist");
            if (response.IsSuccessStatusCode)
            {
                return bool.Parse(await response.Content.ReadAsStringAsync());
            }
        }
        return false;
    }

    public async Task<IEnumerable<BankAccount>> GetAllAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/bankaccounts");
            if (response.StatusCode is System.Net.HttpStatusCode.NotFound)
            {
                return [];
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<BankAccount>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<BankAccount?> GetByIdAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/bankaccounts/{id}");
            if (response.StatusCode is System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BankAccount>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<bool> InsertAsync(string serverURL, BankAccount entity)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(entity, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/bankaccounts", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }
        return false;
    }

    public async Task<bool> UpdateAsync(string serverURL, BankAccount entity)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(entity, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/bankaccounts", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }
        return false;
    }

    public async Task<bool> DeleteAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.DeleteAsync($"{serverURL}/bankaccounts/{id}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }
        return false;
    }



    public async Task<IEnumerable<BankItem>> GetAllBankItemAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/bankaccounts");
            if (response.StatusCode is System.Net.HttpStatusCode.NotFound)
            {
                return [];
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<BankItem>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<BankItem?> GetBankItemByIdAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/bankaccounts/{id}");
            if (response.StatusCode is System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<BankItem>(content, ApiServiceBase.ProviderJSONOptions);
        }
        return null;
    }

    public async Task<bool> InsertBankItemAsync(string serverURL, BankItem entity)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(entity, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/bankaccounts/BankItem", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }
        return false;
    }

    public async Task<bool> UpdateBankItemAsync(string serverURL, BankItem entity)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var json = JsonSerializer.Serialize(entity, ApiServiceBase.ProviderJSONOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/bankaccounts/BankItem", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }
        return false;
    }

    public async Task<bool> DeleteBankItemAsync(string serverURL, string id)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.DeleteAsync($"{serverURL}/bankaccounts/BankItem/{id}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }
        return false;
    }
}
