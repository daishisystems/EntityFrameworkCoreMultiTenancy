using Microsoft.Extensions.Options;

namespace EntityFrameworkCoreMultiTenancy;

public class MultiTenantServiceMiddleware : IMiddleware
{
    private readonly IOptions<TenantConfigurationSection> _config;
    private readonly ILogger<MultiTenantServiceMiddleware> _logger;
    private readonly ITenantSetter _setter;

    public MultiTenantServiceMiddleware(
        ITenantSetter setter,
        IOptions<TenantConfigurationSection> config,
        ILogger<MultiTenantServiceMiddleware> logger)
    {
        _setter = setter;
        _config = config;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var tenant = _config.Value.Tenants.First();

        if (context.Request.Query.TryGetValue("tenant", out var values))
        {
            var key = values.First();
            tenant = _config.Value
                .Tenants
                .FirstOrDefault(t => t.Name.Equals(key?.Trim(), StringComparison.OrdinalIgnoreCase)) ?? tenant;

            // This is Q module middleware. The 'tenant' should be specified in the query issued from qCentral.
            context.Response.Cookies.Append("tenant", tenant.Name);
        }

        _logger.LogInformation("Using the tenant {tenant}", tenant.Name);
        _setter.SetTenant(tenant);

        await next(context);
    }
}