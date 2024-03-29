using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

string GenerateHash(string input)
{
    byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
    StringBuilder sb = new();
    for (int i = 0; i < bytes.Length; i++)
    {
        sb.Append(bytes[i].ToString("x2"));
    }
    return sb.ToString();
}

string salt = "2EE6C66E50164DDE9B416E2C2A526438";

HttpClient client = new();
client.DefaultRequestHeaders.UserAgent.ParseAdd(RuntimeInformation.OSDescription);
client.DefaultRequestHeaders.Add("ClientAccessToken", GenerateHash("38D941C88617485496B07AF837C5E64E"));
Console.WriteLine("Hola root");

var healthCheckResponse = await client.GetAsync("http://localhost:5010/healthchecks");
if (!healthCheckResponse.IsSuccessStatusCode)
{
    Console.WriteLine("El servidor no está disponible en este momento. Por favor, inténtalo más tarde.");
    return;
}
else
{
    var orgResponse = await client.GetAsync("http://localhost:5010/Organization");
    if (orgResponse.IsSuccessStatusCode)
    {
        var organization = await orgResponse.Content.ReadAsStringAsync();
        Console.WriteLine("Información de la organización:");
        Console.WriteLine(organization);
    }
    else
    {
        Console.WriteLine("No se pudo obtener la información de la organización.");
    }
}

Console.Write("Ingrese su contraseña: ");
string? password = Console.ReadLine();
string token = GenerateHash("root" + password + salt);

var content = new StringContent($"\"{token}\"", Encoding.UTF8, "application/json");
var response = await client.PostAsync("http://localhost:5010/Auth", content);

if (response.IsSuccessStatusCode)
{
    Console.WriteLine("Autenticación exitosa.");
}
else
{
    Console.WriteLine("Autenticación fallida.");
}
