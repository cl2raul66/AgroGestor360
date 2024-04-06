using AgroGestor360.Client.Tools.Helpers;
using System.Text;

namespace AgroGestor360.Client.Services;

public interface IAuthService
{
    Task<bool> AuthRoot(string serverURL, string password);
}

public class AuthService : IAuthService
{
    public async Task<bool> AuthRoot(string serverURL, string password)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/Organization/Id");

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            string salt = await response.Content.ReadAsStringAsync();
            string token = HashHelper.GenerateHash("root" + password + salt);
            var content = new StringContent($"\"{token}\"", Encoding.UTF8, "application/json");
            response = await ApiServiceBase.ProviderHttpClient.PostAsync($"{serverURL}/Auth", content);
            return response.IsSuccessStatusCode;
        }
        return false;
    }
}
