using AgroGestor360.Client.Tools.Helpers;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net;
using System.Runtime.InteropServices;

namespace AgroGestor360.Client.Services;

public interface IApiService
{
    Task<bool> CheckUrl(string serverURL);
    Task<bool> ConnectToServerHub(string serverURL);
    void SetClientAccessToken(string clientaccesstoken);
    void ConnectToHttpClient();
}

public class ApiService : IApiService
{
    public void ConnectToHttpClient()
    {
        ApiServiceBase.ProviderHttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        ApiServiceBase.ProviderHttpClient.DefaultRequestHeaders.UserAgent.ParseAdd(RuntimeInformation.OSDescription);
    }

    public void SetClientAccessToken(string accessToken)
    {
        var token = HashHelper.GenerateHash(accessToken);
        ApiServiceBase.ProviderHttpClient!.DefaultRequestHeaders.Add("ClientAccessToken", token);
        ApiServiceBase.IsSetClientAccessToken = true;
    }

    public async Task<bool> CheckUrl(string serverURL)
    {
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            try
            {
                var response = await ApiServiceBase.ProviderHttpClient!.GetAsync(new Uri(new Uri(serverURL), "healthchecks").ToString());
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
        if (ApiServiceBase.IsSetClientAccessToken && Uri.IsWellFormedUriString(serverURL, UriKind.Absolute))
        {
            try
            {
                ApiServiceBase.ProviderHubConnection = new HubConnectionBuilder()
                    .WithUrl($"{serverURL}/serverStatusHub", options =>
                    {
                        options.Headers.Add("ClientAccessToken", ApiServiceBase.ProviderHttpClient!.DefaultRequestHeaders.GetValues("ClientAccessToken").First());
                        options.Headers.Add("UserAgent", RuntimeInformation.OSDescription);
                    })
                    .WithAutomaticReconnect(new RetryPolicy())
                    .Build();

                ApiServiceBase.ProviderHubConnection.On<string>("ReceiveStatusMessage", (message) =>
                {
                    Console.WriteLine($"Server Status: {message}");
                });

                await ApiServiceBase.ProviderHubConnection.StartAsync();
                return ApiServiceBase.ProviderHubConnection.State is HubConnectionState.Connected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al intentar conectar al hub del servidor: {ex.Message}");
            }
        }

        return false;
    }

    class RetryPolicy : IRetryPolicy
    {
        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            return TimeSpan.FromSeconds(Math.Pow(2, retryContext.PreviousRetryCount));
        }
    }
}
