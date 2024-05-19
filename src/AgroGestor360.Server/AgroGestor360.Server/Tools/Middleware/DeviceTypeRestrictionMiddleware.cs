using AgroGestor360.Server.Tools.Helpers;

namespace AgroGestor360.Server.Tools.Middleware;

public class DeviceTypeRestrictionMiddleware(RequestDelegate next, IConfiguration configuration)
{
    private readonly RequestDelegate _next = next;
    private readonly string clientAccessToken = HashHelper.GenerateHash(configuration["License:ClientAccessToken"]!);

    public async Task InvokeAsync(HttpContext context)
    {
        var connection = context.Connection;
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var token = context.Request.Headers["ClientAccessToken"].ToString();

        // Registra la dirección IP y el sistema operativo
        Console.WriteLine($"Dirección IP: {connection.RemoteIpAddress}");
        Console.WriteLine($"Sistema operativo: {(userAgent.Contains("Windows") ? "Windows" : (userAgent.Contains("Android") ? "Android" : "Desconocido"))}");


        if (token != clientAccessToken)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Token de acceso a clientes inválido.");
            Console.WriteLine("Resultado: Solo se permite un cliente PC local."); // Registra el resultado
            return;
        }

        if (connection.RemoteIpAddress?.Equals(connection.LocalIpAddress) ?? false)
        {
            if (!userAgent.Contains("Windows"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Solo se permite un cliente PC local.");
                Console.WriteLine("Resultado: Solo se permiten clientes Android no locales."); // Registra el resultado
                return;
            }
        }
        else if (!userAgent.Contains("Android")) // Verifica si la solicitud no es de un cliente Android
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Solo se permiten clientes Android no locales.");
            return;
        }

        await _next(context);
        Console.WriteLine("Resultado: Solicitud pasó la verificación."); // Registra el resultado si la solicitud pasó la verificación
    }
}

