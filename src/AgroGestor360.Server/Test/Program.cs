using AgroGestor360.Client.Services;
using AgroGestor360.Client.Tools;

IApiService apiServ = new ApiService();
apiServ.OnReceiveStatusMessage += HandleServerStatus;

apiServ.ConnectToHttpClient();
apiServ.SetClientDevicePlatform(Environment.OSVersion.ToString());
apiServ.SetClientAccessToken("38D941C88617485496B07AF837C5E64E");

string url = "http://localhost:5010";

var resultCheckUrl = await apiServ.CheckUrl(url);
Console.WriteLine($"{nameof(resultCheckUrl)}: {resultCheckUrl}");

var ConnectToServerHub = await apiServ.ConnectToServerHub(url);
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
