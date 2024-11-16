using AgroGestor360Server.Tools.Helpers;

namespace AgroGestor360Server.Tools.Middleware;

public class DeviceTypeRestrictionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DeviceTypeRestrictionMiddleware> _logger;
    private readonly string clientAccessToken;

    public DeviceTypeRestrictionMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<DeviceTypeRestrictionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        clientAccessToken = HashHelper.GenerateHash(configuration["License:ClientAccessToken"]!);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var connection = context.Connection;
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var token = context.Request.Headers["ClientAccessToken"].ToString();

        // Ignore SignalR requests
        if (context.Request.Path.Value?.StartsWith("/serverStatusHub") ?? false)
        {
            await _next(context);
            return;
        }

        if (token != clientAccessToken)
        {
            _logger.LogError("Token de acceso a clientes inválido. Solo se permite un cliente PC local. Dirección IP: {0}, Sistema operativo: {1}",
                connection.RemoteIpAddress,
                userAgent.Contains("Windows") ? "Windows" : (userAgent.Contains("Android") ? "Android" : "Desconocido"));
            return;
        }

        if (connection.RemoteIpAddress?.Equals(connection.LocalIpAddress) ?? false)
        {
            if (!userAgent.Contains("Windows"))
            {
                _logger.LogError("Solo se permite un cliente PC local. Solo se permiten clientes Android no locales. Dirección IP: {0}, Sistema operativo: {1}",
                    connection.RemoteIpAddress,
                    userAgent.Contains("Windows") ? "Windows" : (userAgent.Contains("Android") ? "Android" : "Desconocido"));
                return;
            }
        }
        else if (!userAgent.Contains("Android"))
        {
            _logger.LogError("Solo se permiten clientes Android no locales.");
            return;
        }

        await _next(context);
    }
}

