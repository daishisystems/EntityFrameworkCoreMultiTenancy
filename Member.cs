namespace EntityFrameworkCoreMultiTenancy;

public class Member
{
    public int MemberId { get; init; }
    public string? Name { get; init; }
    public string? Address { get; init; }
    public string? Tenant { get; init; }
}