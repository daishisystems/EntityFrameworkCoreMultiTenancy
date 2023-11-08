#nullable disable
namespace EntityFrameworkCoreMultiTenancy;

public class Tenant
{
    public string Name { get; set; }
    public string ConnectionString { get; set; }
}