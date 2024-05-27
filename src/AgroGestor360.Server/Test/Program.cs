using AgroGestor360.Client.Services;
using Microsoft.AspNetCore.SignalR.Client;

IApiService apiServ = new ApiService();
apiServ.ConnectToHttpClient();
apiServ.SetClientDevicePlatform(Environment.OSVersion.ToString());
apiServ.SetClientAccessToken("38D941C88617485496B07AF837C5E64E");

IOrganizationService organizationServ = new OrganizationService();
IAuthService authServ = new AuthService();

string url = "http://localhost:5010";

Console.WriteLine("Hola root");
var checkurl = await apiServ.CheckUrl(url);

if (!checkurl)
{
    Console.WriteLine("El servidor no está disponible en este momento. Por favor, inténtalo más tarde.");
    return;
}
else
{
    var org = await organizationServ.GetOrganization(url);
    if (org is not null)
    {
        Console.WriteLine("Información de la organización:");
        Console.WriteLine(org.Id);
        Console.WriteLine(org.Name);
        Console.WriteLine(org.Address);
        Console.WriteLine(org.Phone);
        Console.WriteLine(org.Email);
    }
    else
    {
        Console.WriteLine("No se pudo obtener la información de la organización.");
    }
}

// Conectar al hub del servidor
var connected = await apiServ.ConnectToServerHub(url);

if (connected)
{
    apiServ.OnReceiveStatusMessage += status =>
    {
        Console.WriteLine($"Received status message: {status}");
    };
}
else
{
    Console.WriteLine("No se pudo conectar al hub del servidor.");
}

//Console.Write("Ingrese su contraseña: ");
//string? password = Console.ReadLine();

//if (!string.IsNullOrEmpty(password))
//{
//    var resultAuth = await authServ.AuthRoot(url, password);

//    if (resultAuth)
//    {
//        Console.WriteLine("Autenticación exitosa.");
//    }
//    else
//    {
//        Console.WriteLine("Autenticación fallida.");
//    }
//}

//var service = new MeasurementService();
//while (true)
//{
//    Console.WriteLine("Seleccione una medida:");
//    var measurements = service.GetMeasurementNames().ToList();
//    for (int i = 0; i < measurements.Count; i++)
//    {
//        Console.WriteLine($"{i + 1}. {measurements[i]}");
//    }

//    if (int.TryParse(Console.ReadLine(), out int selectedMeasurement) && selectedMeasurement > 0 && selectedMeasurement <= measurements.Count)
//    {
//        var units = service.GetNamesAndUnitsMeasurement(measurements[selectedMeasurement - 1]);
//        Console.WriteLine($"Unidades para {measurements[selectedMeasurement - 1]}:");
//        //for (int i = 0; i < units.Count; i++)
//        //{
//        //    Console.WriteLine($"{i + 1}. {units[i]}");
//        //}
//        foreach (var item in units)
//        {
//            Console.WriteLine(item);
//        }
//    }
//    else
//    {
//        Console.WriteLine("Entrada inválida. Por favor, intente de nuevo.");
//    }

//    Console.WriteLine("¿Desea continuar? (s/n)");
//    var r = Console.ReadLine();
//    if (r?.ToLower() != "s")
//    {
//        break;
//    }
//}
