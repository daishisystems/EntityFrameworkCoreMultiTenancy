using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCoreMultiTenancy;

public class Database : DbContext
{
    public Database(DbContextOptions<Database> options)
        : base(options)
    {
    }

    public DbSet<Member> Members { get; set; } = default!;

    public static async Task Initialize(WebApplication app)
    {
        var data = new Member[]
        {
            new() { MemberId = 1, Name = "John", Address = "Dublin", Tenant = "client01" },
            new() { MemberId = 3, Name = "Alan", Address = "Sao Paulo", Tenant = "client01" },
            new() { MemberId = 6, Name = "Karen", Address = "Cape Town", Tenant = "client02" }
        };

        // initialize the databases
        var tenantConfig = app.Configuration.Get<TenantConfigurationSection>()!;
        foreach (var tenant in tenantConfig.Tenants)
        {
            using var scope = app.Services.CreateScope();
            var tenantSetter = scope.ServiceProvider.GetRequiredService<ITenantSetter>();
            tenantSetter.SetTenant(tenant);

            var db = scope.ServiceProvider.GetRequiredService<Database>();
            await db.Database.MigrateAsync();

            // unique data
            if (db.Members.Any()) continue;
            var unique = data.Where(a => a.Tenant == tenant.Name).ToList();
            db.Members.AddRange(unique);
            await db.SaveChangesAsync();
        }
    }
}