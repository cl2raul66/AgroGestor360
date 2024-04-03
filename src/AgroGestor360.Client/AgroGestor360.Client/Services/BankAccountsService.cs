using AgroGestor360.Client.Models;
using AgroGestor360.Server.Controllers;
using System.Text;
using System.Text.Json;

namespace AgroGestor360.Client.Services;

public interface IBankAccountsService
{
    Task<bool> CheckExistence(string serverURL);
    Task<bool> CheckExistenceByNumber(string serverURL, string number);
    Task<bool> DeleteBankAsync(string serverURL, string number);
    Task<IEnumerable<string>> GetAllNumbersAsync(string serverURL);
    Task<BankAccount?> GetBankByNumberAsync(string serverURL, string number);
    Task<IEnumerable<BankAccount>> GetBanksAsync(string serverURL);
    Task<bool> InsertBankAsync(string serverURL, BankAccount bankAccount);
    Task<bool> UpdateBankAsync(string serverURL, BankAccount bankAccount);
}

public class BankAccountsService : IBankAccountsService
{
    public async Task<bool> CheckExistenceByNumber(string serverURL, string number)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/bankaccounts/{number}");

            return response.IsSuccessStatusCode;
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

    public async Task<IEnumerable<BankAccount>> GetBanksAsync(string serverURL)
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

    public async Task<IEnumerable<string>> GetAllNumbersAsync(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/bankaccounts/numbers");

            if (response.StatusCode is System.Net.HttpStatusCode.NotFound)
            {
                return [];
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<string>>(content, ApiServiceBase.ProviderJSONOptions) ?? [];
        }
        return [];
    }

    public async Task<bool> InsertBankAsync(string serverURL, BankAccount bankAccount)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var bankJson = JsonSerializer.Serialize(bankAccount);
            var data = new StringContent(bankJson, Encoding.UTF8, "application/json");
            var response = await ApiServiceBase.ProviderHttpClient!.PostAsync($"{serverURL}/bankaccounts", data);

            if (response.IsSuccessStatusCode)
            {
                return bool.Parse(await response.Content.ReadAsStringAsync());
            }
        }
        return false;
    }

    public async Task<bool> UpdateBankAsync(string serverURL, BankAccount bankAccount)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var bankJson = JsonSerializer.Serialize(bankAccount);
            var response = await ApiServiceBase.ProviderHttpClient!.PutAsync($"{serverURL}/bankaccounts", new StringContent(bankJson, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }
        return false;
    }

    // Este método no tiene una ruta correspondiente en el controlador.
    // Deberías agregar un método Delete en tu controlador que acepte un número de cuenta como parámetro.
    public async Task<bool> DeleteBankAsync(string serverURL, string number)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.DeleteAsync($"{serverURL}/bankaccounts/{number}");

            return response.IsSuccessStatusCode;
        }
        return false;
    }
}
