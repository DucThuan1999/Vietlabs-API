namespace VietLab.Models;

/// <summary>
/// Template cấu hình cho mẫu đơn hàng (con của OrderSample).
/// </summary>
public class OrderTemplate
{
    public Guid TemplateId { get; set; }
    public Guid OrderSampleId { get; set; }
    public Guid? QuotationId { get; set; }

    public string TemplateName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; } // AccountId người tạo

    public OrderSample? OrderSample { get; set; }
    public Quotation? Quotation { get; set; }
    public Account? CreatedByAccount { get; set; }

    public ICollection<OrderTemplateItem> OrderTemplateItems { get; set; } = new List<OrderTemplateItem>();
    public ICollection<OrderTemplateAnalysisGroup> OrderTemplateAnalysisGroups { get; set; } = new List<OrderTemplateAnalysisGroup>();
    public ICollection<OrderTemplatePackage> OrderTemplatePackages { get; set; } = new List<OrderTemplatePackage>();
}

