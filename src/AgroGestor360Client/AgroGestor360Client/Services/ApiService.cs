using AgroGestor360Client.Tools;
using AgroGestor360Client.Tools.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net;

namespace AgroGestor360Client.Services;

public interface IApiService
{
    event Action<string[]>? OnReceiveExpiredOrderMessage;
    event Action<string[]>? OnReceiveExpiredQuotationMessage;
    event Action<ServerStatus>? OnReceiveStatusMessage;

    Task<bool> CheckUrl(string serverURL);
    void ConnectToHttpClient();
    Task<bool> ConnectToServerHub(string serverURL);
    Task<bool> JoinOrderViewGroup();
    Task<bool> JoinQuotationViewGroup();
    Task<bool> LeaveOrderViewGroup();
    Task<bool> LeaveQuotationViewGroup();
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
            if (ApiServiceBase.ProviderHubConnection is not null)
            {
                return true;
            }

            try
            {
                ApiServiceBase.ProviderHubConnection = new HubConnectionBuilder().WithUrl($"{serverURL}/serverStatusHub", options =>
                    {
                        options.Headers.Add("ClientAccessToken", ApiServiceBase.ProviderHttpClient!.DefaultRequestHeaders.GetValues("ClientAccessToken").First());
                        var userAgent = ApiServiceBase.ProviderHttpClient.DefaultRequestHeaders.UserAgent.ToString();
                        options.Headers.Add("UserAgent", userAgent);

                        var op = options;
                    })
                    .WithAutomaticReconnect(new RetryPolicy())
                    .Build();

                ApiServiceBase.ProviderHubConnection.On<ServerStatus>("ReceiveStatusMessage", (status) =>
                {
                    OnReceiveStatusMessage?.Invoke(status);
                });

                ApiServiceBase.ProviderHubConnection.On<string[]>("ReceiveExpiredQuotationMessage", (quotationCodes) =>
                {
                    OnReceiveExpiredQuotationMessage?.Invoke(quotationCodes);
                });

                ApiServiceBase.ProviderHubConnection.On<string[]>("ReceiveExpiredOrderMessage", (ordersCodes) =>
                {
                    OnReceiveExpiredOrderMessage?.Invoke(ordersCodes);
                });

                await ApiServiceBase.ProviderHubConnection.StartAsync();
                return ApiServiceBase.ProviderHubConnection.State is HubConnectionState.Connected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al intentar iniciar la conexión con el hub: {ex.Message}");
                return false;
            }
        }

        return false;
    }

    public event Action<ServerStatus>? OnReceiveStatusMessage;

    #region QUOTATION
    public async Task<bool> JoinQuotationViewGroup()
    {
        if (ApiServiceBase.ProviderHubConnection is not null && ApiServiceBase.ProviderHubConnection.State is HubConnectionState.Connected)
        {
            await ApiServiceBase.ProviderHubConnection.InvokeAsync("JoinGroup", "QuotationView");
            return true;
        }
        return false;
    }

    public async Task<bool> LeaveQuotationViewGroup()
    {
        if (ApiServiceBase.ProviderHubConnection is not null && ApiServiceBase.ProviderHubConnection.State is HubConnectionState.Connected)
        {
            await ApiServiceBase.ProviderHubConnection.InvokeAsync("LeaveGroup", "QuotationView");
            return true;
        }
        return false;
    }

    public event Action<string[]>? OnReceiveExpiredQuotationMessage;
    #endregion

    #region ORDER
    public async Task<bool> JoinOrderViewGroup()
    {
        if (ApiServiceBase.ProviderHubConnection is not null && ApiServiceBase.ProviderHubConnection.State is HubConnectionState.Connected)
        {
            await ApiServiceBase.ProviderHubConnection.InvokeAsync("JoinGroup", "OrderView");
            return true;
        }
        return false;
    }

    public async Task<bool> LeaveOrderViewGroup()
    {
        if (ApiServiceBase.ProviderHubConnection is not null && ApiServiceBase.ProviderHubConnection.State is HubConnectionState.Connected)
        {
            await ApiServiceBase.ProviderHubConnection.InvokeAsync("LeaveGroup", "OrderView");
            return true;
        }
        return false;
    }

    public event Action<string[]>? OnReceiveExpiredOrderMessage;
    #endregion
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
