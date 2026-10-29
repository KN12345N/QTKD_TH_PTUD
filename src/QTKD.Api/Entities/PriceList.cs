namespace QTKD.Api.Entities;

public class PriceList
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }

    // Quan hệ 1 - N: Một bảng giá gồm nhiều hạng mục giá
    public ICollection<PriceItem> PriceItems { get; set; } = new List<PriceItem>();
}