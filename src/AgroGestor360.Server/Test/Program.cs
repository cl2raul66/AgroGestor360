using AgroGestor360.Client.Services;
using AgroGestor360.Client.Tools;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"))
    .Build();

var clientAccessToken = configuration["License:ClientAccessToken"];
if (string.IsNullOrEmpty(clientAccessToken))
{
    Console.WriteLine($"{nameof(clientAccessToken)} es nulo o vacío");
    return;
}

IApiService apiServ = new ApiService();
apiServ.OnReceiveStatusMessage += HandleServerStatus;

apiServ.ConnectToHttpClient();
apiServ.SetClientDevicePlatform(Environment.OSVersion.ToString());
apiServ.SetClientAccessToken(clientAccessToken!);

var url = configuration["Server:Url"];
if (string.IsNullOrEmpty(url))
{
    Console.WriteLine($"{nameof(url)} es nulo o vacío");
    return;
}

var resultCheckUrl = await apiServ.CheckUrl(url!);
Console.WriteLine($"{nameof(resultCheckUrl)}: {resultCheckUrl}");

var ConnectToServerHub = await apiServ.ConnectToServerHub(url!);
Console.WriteLine($"{nameof(ConnectToServerHub)}: {ConnectToServerHub}");

var cts = new CancellationTokenSource();

// Iniciar una tarea que escucha las teclas del usuario
_ = Task.Run(() =>
{
    while (!cts.Token.IsCancellationRequested)
    {
        if (Console.KeyAvailable)
        {
            cts.Cancel();
        }
    }
});

// Esperar hasta que el usuario presione una tecla
while (!cts.Token.IsCancellationRequested)
{
    await Task.Delay(1000);
}

void HandleServerStatus(ServerStatus status)
{
    Console.WriteLine($"El estado del servidor es: {status}");
}
