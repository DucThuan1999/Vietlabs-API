namespace VietLab.Models;

public class OrderSample
{
    public Guid OrderSampleId { get; set; }
    public Guid OrderId { get; set; }
    public Guid? QuotationId { get; set; }

    // Thông tin mẫu
    public string? SampleIdentifier { get; set; } // Số nhận dạng mẫu
    public string? SampleCode { get; set; } // Mã mẫu
    public Guid SampleMatrixId { get; set; } // Nền mẫu (FK đến SampleMatrix)
    public string? SampleName { get; set; } // Tên mẫu
    public decimal? SampleWeight { get; set; } // Khối lượng mẫu
    public decimal? SampleTemperature { get; set; } // Nhiệt độ mẫu
    public string? ResultTurnaroundTimeRequirement { get; set; } // Yêu cầu thời gian trả kết quả
    public decimal? FeePercentage { get; set; } // Tỉ lệ % tính phí
    public string? SampleConditionDescription { get; set; } // Mô tả tình trạng mẫu
    public string? Notes { get; set; } // Ghi chú
    public int? AnalysisItemCount { get; set; } // Số lượng chỉ tiêu
    public int? NtpAnalysisItemCount { get; set; } // Số lượng chỉ tiêu NTP
    public decimal? Amount { get; set; } // Thành tiền
    public DateTime? SampleReceivedDate { get; set; } // Ngày nhận mẫu

    public Order? Order { get; set; }
    public SampleMatrix? SampleMatrix { get; set; }
    public Quotation? Quotation { get; set; }

    public ICollection<OrderSampleItem> OrderSampleItems { get; set; } = new List<OrderSampleItem>();
    public ICollection<OrderSampleAnalysisGroup> OrderSampleAnalysisGroups { get; set; } = new List<OrderSampleAnalysisGroup>();
    public ICollection<OrderSamplePackage> OrderSamplePackages { get; set; } = new List<OrderSamplePackage>();

    public ICollection<OrderTemplate> OrderTemplates { get; set; } = new List<OrderTemplate>();
}

