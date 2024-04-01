using AgroGestor360.Client.Services;

IApiService apiServ = new ApiService();
apiServ.ConnectToHttpClient();
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

Console.Write("Ingrese su contraseña: ");
string? password = Console.ReadLine();

if (!string.IsNullOrEmpty(password))
{
    var resultAuth = await authServ.AuthRoot(url, password);

    if (resultAuth)
    {
        Console.WriteLine("Autenticación exitosa.");
    }
    else
    {
        Console.WriteLine("Autenticación fallida.");
    }
}
