using AgroGestor360.Client.Tools.Helpers;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net;

namespace AgroGestor360.Client.Services;

public interface IApiService
{
    Task<bool> CheckUrl(string serverURL);
    void ConnectToHttpClient();
    Task<bool> ConnectToServerHub(string serverURL);
    Task JoinQuotationViewGroup();
    Task LeaveQuotationViewGroup();
    void SetClientAccessToken(string accessToken);
    void SetClientDevicePlatform(string os);
}

public class ApiService : IApiService
{
    public void ConnectToHttpClient()
    {
        ApiServiceBase.ProviderHttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public void SetClientDevicePlatform(string os)
    {
        ApiServiceBase.ProviderHttpClient!.DefaultRequestHeaders.UserAgent.ParseAdd(os);
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

    #region HUB
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
                        var userAgent = ApiServiceBase.ProviderHttpClient.DefaultRequestHeaders.UserAgent.ToString();
                        options.Headers.Add("UserAgent", userAgent);
                    })
                    .WithAutomaticReconnect(new RetryPolicy())
                    .Build();

                ApiServiceBase.ProviderHubConnection.On<string>("ReceiveStatusMessage", (message) =>
                {
                    Console.WriteLine($"Server Status: {message}");
                });

                ApiServiceBase.ProviderHubConnection.On<string[]>("ReceiveExpiredQuotationMessage", (quotationCodes) =>
                {
                    foreach (var code in quotationCodes)
                    {
                        Console.WriteLine($"Quotation expired: {code}");
                        // Actualizar la vista aquí
                    }
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

    public async Task JoinQuotationViewGroup()
    {
        if (ApiServiceBase.ProviderHubConnection is not null)
        {
            await ApiServiceBase.ProviderHubConnection.InvokeAsync("JoinGroup", "QuotationView");
        }
        else
        {
            // Manejar el caso en que ProviderHubConnection es null
        }
    }

    public async Task LeaveQuotationViewGroup()
    {
        if (ApiServiceBase.ProviderHubConnection is not null)
        {
            await ApiServiceBase.ProviderHubConnection.InvokeAsync("LeaveGroup", "QuotationView");
        }
        else
        {
            // Manejar el caso en que ProviderHubConnection es null
        }
    }

    public async Task JoinOrderViewGroup()
    {
        if (ApiServiceBase.ProviderHubConnection is not null)
        {
            await ApiServiceBase.ProviderHubConnection.InvokeAsync("JoinGroup", "OrderView");
        }
        else
        {
            // Manejar el caso en que ProviderHubConnection es null
        }
    }

    public async Task LeaveOrderViewGroup()
    {
        if (ApiServiceBase.ProviderHubConnection is not null)
        {
            await ApiServiceBase.ProviderHubConnection.InvokeAsync("LeaveGroup", "OrderView");
        }
        else
        {
            // Manejar el caso en que ProviderHubConnection es null
        }
    }
    #endregion

    #region EXTRA
    class RetryPolicy : IRetryPolicy
    {
        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            return TimeSpan.FromSeconds(Math.Pow(2, retryContext.PreviousRetryCount));
        }
    }
    #endregion
}
