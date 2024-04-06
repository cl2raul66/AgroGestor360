using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;

namespace AgroGestor360.Client.Services;

public static class ApiServiceBase
{
    public static bool IsSetClientAccessToken;
    public static JsonSerializerOptions ProviderJSONOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public static HttpClient? ProviderHttpClient { get; set; }

    public static HubConnection? ProviderHubConnection { get; set; }
}
