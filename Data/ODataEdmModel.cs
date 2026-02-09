using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using VietLab.Models;

namespace VietLab.Data;

/// <summary>
/// Cấu hình OData EDM Model với tất cả entities và AutoExpand settings
/// </summary>
public static class ODataEdmModel
{
    public static IEdmModel GetEdmModel()
    {
        var builder = new ODataConventionModelBuilder();
        
        // Core entities
        builder.EntitySet<Client>("Clients");
        builder.EntitySet<Contact>("Contacts");
        builder.EntitySet<Employee>("Employees");
        builder.EntitySet<Branch>("Branches");
        builder.EntitySet<Department>("Departments");
        
        // Account với AutoExpand cho Employee
        var accountEntitySet = builder.EntitySet<Account>("Accounts");
        var accountEntityType = accountEntitySet.EntityType;
        accountEntityType.HasOptional(a => a.Employee).AutoExpand = true;
        
        builder.EntitySet<Permission>("Permissions");
        
        // Sample and Analysis entities
        builder.EntitySet<SampleMatrixGroup>("SampleMatrixGroups");
        builder.EntitySet<SampleMatrix>("SampleMatrices");
        builder.EntitySet<EquipmentType>("EquipmentTypes");
        builder.EntitySet<AnalysisGroup>("AnalysisGroups");
        builder.EntitySet<AnalysisItem>("AnalysisItems");
        builder.EntitySet<AnalysisItemTat>("AnalysisItemTats");
        
        // Quotation entities
        builder.EntitySet<Quotation>("Quotations");
        builder.EntitySet<QuotationItem>("QuotationItems");
        builder.EntitySet<QuotationAnalysisGroup>("QuotationAnalysisGroups");
        
        // Package entities
        builder.EntitySet<Package>("Packages");
        builder.EntitySet<PackageAnalysisGroup>("PackageAnalysisGroups");
        
        // Client related entities
        builder.EntitySet<ClientDebt>("ClientDebts");
        
        // ClientForecast với AutoExpand cho navigation properties
        var clientForecastEntitySet = builder.EntitySet<ClientForecast>("ClientForecasts");
        var clientForecastEntityType = clientForecastEntitySet.EntityType;
        clientForecastEntityType.HasOptional(cf => cf.CreatedByAccount).AutoExpand = true;
        clientForecastEntityType.HasOptional(cf => cf.UpdatedByAccount).AutoExpand = true;
        
        // ClientHistory với AutoExpand cho navigation properties
        var clientHistoryEntitySet = builder.EntitySet<ClientHistory>("ClientHistories");
        var clientHistoryEntityType = clientHistoryEntitySet.EntityType;
        clientHistoryEntityType.HasRequired(ch => ch.ChangedByAccount).AutoExpand = true;
        
        // Location entities
        builder.EntitySet<Country>("Countries");
        builder.EntitySet<Province>("Provinces");
        builder.EntitySet<Ward>("Wards");
        
        // Department capability
        builder.EntitySet<DepartmentAnalysisCapability>("DepartmentAnalysisCapabilities");
        
        return builder.GetEdmModel();
    }
}

