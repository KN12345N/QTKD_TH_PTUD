namespace QTKD.Api.Entities;

public class Contract
{
    public int Id { get; set; }
    public string ContractNo { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Draft";
    // Nội dung hợp đồng
    public string Description { get; set; } = string.Empty;
    // Tên tài liệu đính kèm
    public string AttachmentName { get; set; } = string.Empty;

    // Khóa ngoại liên kết Customer
    public Customer? Customer { get; set; }
}
