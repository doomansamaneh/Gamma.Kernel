namespace Gamma.Next.Application.Queries.Customer;

public class CustomerQuery
{
    public string? Name { get; set; }
    public string? City { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
