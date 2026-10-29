using System.Text.Json.Serialization;

namespace QTKD.Api.Entities;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";

    // Quan hệ 1 - N: Một khách hàng có thể có nhiều hợp đồng
    [JsonIgnore]
    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}