using AgroGestor360.Server.Tools.Helpers;

namespace AgroGestor360.Server.Tools.Middleware;

public class DeviceTypeRestrictionMiddleware(RequestDelegate next, IConfiguration configuration)
{
    private readonly RequestDelegate _next = next;
    private readonly string? _organizationToken = HashHelper.GenerateHash(configuration["Organization:Id"] + configuration["License:ClientId"]);

    public async Task InvokeAsync(HttpContext context)
    {
        var connection = context.Connection;
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var organizationId = context.Request.Headers["OrganizationToken"].ToString();

        if (organizationId != _organizationToken)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Token de organización inválido.");
            return;
        }

        if (connection.RemoteIpAddress?.Equals(connection.LocalIpAddress) ?? false)
        {
            if (!userAgent.Contains("Windows"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Solo se permite un cliente PC local.");
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
    }
}

