using EntityFrameworkCoreMultiTenancy;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// tenant setter & getter
builder.Services.AddScopedAs<TenantService>(new[] { typeof(ITenantGetter), typeof(ITenantSetter) });

// IOptions version of tenants
builder.Services.Configure<TenantConfigurationSection>(builder.Configuration);

// middleware that sets the current tenant
builder.Services.AddScoped<MultiTenantServiceMiddleware>();
builder.Services.AddDbContext<Database>((s, o) =>
{
    var tenant = s.GetService<ITenantGetter>()?.Tenant;
    // for migrations
    var connectionString = tenant?.ConnectionString ?? "Data Source=client01.db";
    // multi-tenant databases
    o.UseSqlite(connectionString);
});

var app = builder.Build();
await Database.Initialize(app);

// middleware that reads and sets the tenant
app.UseMiddleware<MultiTenantServiceMiddleware>();

// multi-tenant request, try adding ?tenant=client01 (default) or ?tenant=client02
app.MapGet("/", async (Database db) => await db
    .Members
    // hide the tenant, which is response noise
    .Select(m => new { m.MemberId, m.Name, m.Address })
    .ToListAsync());

app.Run();