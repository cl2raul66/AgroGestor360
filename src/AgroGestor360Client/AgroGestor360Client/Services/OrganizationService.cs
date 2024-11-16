using AgroGestor360Client.Models;
using System.Text.Json;

namespace AgroGestor360Client.Services;

public interface IOrganizationService
{
    Task<Organization?> GetOrganization(string serverURL);
}

public class OrganizationService : IOrganizationService
{
    public async Task<Organization?> GetOrganization(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            var response = await ApiServiceBase.ProviderHttpClient!.GetAsync($"{serverURL}/Organization");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Organization>(content, ApiServiceBase.ProviderJSONOptions)!;
            }
            else
            {
                Console.WriteLine("No se pudo obtener la información de la organización.");
            }
        }
        return null;
    }
}
