using AgroGestor360.Client.Models;
using AgroGestor360.Client.Tools.Helpers;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace AgroGestor360.Client.Services;

public class ApiService
{
    bool isSetClientAccessToken;
    readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    HubConnection HubConnection { get; set; }

    public ApiService()
    {
        Httpclient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
        Httpclient.DefaultRequestHeaders.UserAgent.ParseAdd(RuntimeInformation.OSDescription);
    }

    HttpClient Httpclient { get; set; }

    public void SetClientAccessToken(string token)
    {
        Httpclient.DefaultRequestHeaders.Add("ClientAccessToken", token);
        isSetClientAccessToken = true;
    }

    public async Task<bool> CheckUrl(string serverURL)
    {
        if (isSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            try
            {
                var response = await Httpclient.GetAsync(new Uri(new Uri(serverURL), "healthchecks").ToString());
                return response.StatusCode is HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al intentar acceder a la URL: {ex.Message}");
            }
        }

        return false;
    }

    public async Task<bool> ConnectToServerHub(string serverURL)
    {
        if (isSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            try
            {
                HubConnection = new HubConnectionBuilder()
                    .WithUrl($"{serverURL}/serverStatusHub", options =>
                    {
                        options.Headers.Add("ClientAccessToken", Httpclient.DefaultRequestHeaders.GetValues("ClientAccessToken").First());
                        options.Headers.Add("UserAgent", RuntimeInformation.OSDescription);
                    })
                    .Build();

                await HubConnection.StartAsync();
                return HubConnection.State == HubConnectionState.Connected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al intentar conectar al hub del servidor: {ex.Message}");
            }
        }

        return false;
    }

    public async Task<Organization> GetOrganization(string serverURL)
    {
        var response = await Httpclient.GetAsync($"{serverURL}/Organization");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Organization>(content, jsonOptions)!;
        }
        else
        {
            Console.WriteLine("No se pudo obtener la información de la organización.");
        }
        return new Organization();
    }
    
    public async Task<bool> Auth(string serverURL, string password, string salt)
    {
        string token = HashHelper.GenerateHash("root" + password + salt);
        var content = new StringContent($"\"{token}\"", Encoding.UTF8, "application/json");
        var response = await Httpclient.PostAsync($"{serverURL}/Auth", content);
        return response.IsSuccessStatusCode;
    }
}
