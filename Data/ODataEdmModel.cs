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
        var clientEntitySet = builder.EntitySet<Client>("Clients");
        clientEntitySet.EntityType.HasOptional(c => c.UpdatedByAccount).AutoExpand = true;
        var contactEntitySet = builder.EntitySet<Contact>("Contacts");
        contactEntitySet.EntityType.HasOptional(c => c.UpdatedByAccount).AutoExpand = true;
        var employeeEntitySet = builder.EntitySet<Employee>("Employees");
        employeeEntitySet.EntityType.HasOptional(e => e.Department).AutoExpand = true;
        employeeEntitySet.EntityType.HasOptional(e => e.EmployeeTitle).AutoExpand = true;
        employeeEntitySet.EntityType.HasOptional(e => e.Section).AutoExpand = true;
        employeeEntitySet.EntityType.HasOptional(e => e.UpdatedByAccount).AutoExpand = true;
        employeeEntitySet.EntityType.HasOptional(e => e.Account).AutoExpand = true;
        employeeEntitySet.EntityType.HasOptional(e => e.Manager).AutoExpand = true;
        var branchEntitySet = builder.EntitySet<Branch>("Branches");
        branchEntitySet.EntityType.HasOptional(b => b.UpdatedByAccount).AutoExpand = true;
        var departmentEntitySet = builder.EntitySet<Department>("Departments");
        departmentEntitySet.EntityType.HasOptional(d => d.UpdatedByAccount).AutoExpand = true;
        var sectionEntitySet = builder.EntitySet<Section>("Sections");
        sectionEntitySet.EntityType.HasOptional(s => s.Department).AutoExpand = true;
        sectionEntitySet.EntityType.HasOptional(s => s.UpdatedByAccount).AutoExpand = true;
        
        var accountEntitySet = builder.EntitySet<Account>("Accounts");
        var accountEntityType = accountEntitySet.EntityType;
        accountEntityType.HasOptional(a => a.Employee).AutoExpand = true;
        
        builder.EntitySet<Permission>("Permissions");
        
        // Sample and Analysis entities
        var sampleMatrixGroupEntitySet = builder.EntitySet<SampleMatrixGroup>("SampleMatrixGroups");
        sampleMatrixGroupEntitySet.EntityType.HasOptional(smg => smg.UpdatedByAccount).AutoExpand = true;
        var sampleMatrixEntitySet = builder.EntitySet<SampleMatrix>("SampleMatrices");
        sampleMatrixEntitySet.EntityType.HasOptional(sm => sm.UpdatedByAccount).AutoExpand = true;
        var equipmentTypeEntitySet = builder.EntitySet<EquipmentType>("EquipmentTypes");
        equipmentTypeEntitySet.EntityType.HasOptional(e => e.UpdatedByAccount).AutoExpand = true;
        var analysisGroupEntitySet = builder.EntitySet<AnalysisGroup>("AnalysisGroups");
        analysisGroupEntitySet.EntityType.HasOptional(ag => ag.UpdatedByAccount).AutoExpand = true;
        var analysisItemEntitySet = builder.EntitySet<AnalysisItem>("AnalysisItems");
        analysisItemEntitySet.EntityType.HasOptional(ai => ai.ReferenceMethod).AutoExpand = true;
        analysisItemEntitySet.EntityType.HasOptional(ai => ai.Standard).AutoExpand = true;
        analysisItemEntitySet.EntityType.HasOptional(ai => ai.UnitOfMeasure).AutoExpand = true;
        analysisItemEntitySet.EntityType.HasOptional(ai => ai.StandardQuantityUnitOfMeasure).AutoExpand = true;
        analysisItemEntitySet.EntityType.HasOptional(ai => ai.LaboratoryTechnique).AutoExpand = true;
        analysisItemEntitySet.EntityType.HasOptional(ai => ai.UpdatedByAccount).AutoExpand = true;
        var analysisItemTatEntitySet = builder.EntitySet<AnalysisItemTat>("AnalysisItemTats");
        analysisItemTatEntitySet.EntityType.HasOptional(tat => tat.UpdatedByAccount).AutoExpand = true;

        // Quotation entities
        builder.EntitySet<Quotation>("Quotations");
        builder.EntitySet<QuotationSample>("QuotationSamples");
        var quotationItemEntitySet = builder.EntitySet<QuotationItem>("QuotationItems");
        quotationItemEntitySet.EntityType.HasOptional(qi => qi.UpdatedByAccount).AutoExpand = true;
        var quotationNonNd107ItemEntitySet = builder.EntitySet<QuotationNonNd107Item>("QuotationNonNd107Items");
        quotationNonNd107ItemEntitySet.EntityType.HasOptional(x => x.UpdatedByAccount).AutoExpand = true;
        var quotationAnalysisGroupEntitySet = builder.EntitySet<QuotationAnalysisGroup>("QuotationAnalysisGroups");
        quotationAnalysisGroupEntitySet.EntityType.HasOptional(qag => qag.UpdatedByAccount).AutoExpand = true;
        builder.EntitySet<QuotationApprovalThreshold>("QuotationApprovalThresholds");
        builder.EntitySet<VatRate>("VatRates");
        
        // QuotationHistory với AutoExpand cho navigation properties
        var quotationHistoryEntitySet = builder.EntitySet<QuotationHistory>("QuotationHistories");
        var quotationHistoryEntityType = quotationHistoryEntitySet.EntityType;
        quotationHistoryEntityType.HasRequired(qh => qh.ChangedByAccount).AutoExpand = true;
        
        // Package entities
        var packageEntitySet = builder.EntitySet<Package>("Packages");
        packageEntitySet.EntityType.HasOptional(p => p.UpdatedByAccount).AutoExpand = true;
        var packageAnalysisItemEntitySet = builder.EntitySet<PackageAnalysisItem>("PackageAnalysisItems");
        packageAnalysisItemEntitySet.EntityType.HasOptional(pai => pai.UpdatedByAccount).AutoExpand = true;

        // Client related entities
        var clientDebtEntitySet = builder.EntitySet<ClientDebt>("ClientDebts");
        clientDebtEntitySet.EntityType.HasOptional(cd => cd.UpdatedByAccount).AutoExpand = true;
        
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

        // Danh mục ngành nghề khách hàng
        var clientIndustryEntitySet = builder.EntitySet<ClientIndustry>("ClientIndustries");
        clientIndustryEntitySet.EntityType.HasOptional(ci => ci.UpdatedByAccount).AutoExpand = true;

        // Danh mục chức vụ nhân viên
        var employeeTitleEntitySet = builder.EntitySet<EmployeeTitle>("EmployeeTitles");
        employeeTitleEntitySet.EntityType.HasOptional(et => et.UpdatedByAccount).AutoExpand = true;

        // Nhà thầu phụ
        var subcontractorEntitySet = builder.EntitySet<Subcontractor>("Subcontractors");
        subcontractorEntitySet.EntityType.HasOptional(s => s.UpdatedByAccount).AutoExpand = true;
        var subcontractorCapabilityEntitySet = builder.EntitySet<SubcontractorCapability>("SubcontractorCapabilities");
        subcontractorCapabilityEntitySet.EntityType.HasRequired(sc => sc.AnalysisItem).AutoExpand = true;
        subcontractorCapabilityEntitySet.EntityType.HasRequired(sc => sc.Subcontractor).AutoExpand = true;
        subcontractorCapabilityEntitySet.EntityType.HasOptional(sc => sc.UpdatedByAccount).AutoExpand = true;

        // Department capability
        var departmentAnalysisCapabilityEntitySet = builder.EntitySet<DepartmentAnalysisCapability>("DepartmentAnalysisCapabilities");
        departmentAnalysisCapabilityEntitySet.EntityType.HasRequired(dac => dac.AnalysisItem).AutoExpand = true;
        departmentAnalysisCapabilityEntitySet.EntityType.HasRequired(dac => dac.Department).AutoExpand = true;
        departmentAnalysisCapabilityEntitySet.EntityType.HasOptional(dac => dac.UpdatedByAccount).AutoExpand = true;

        // Năng lực phòng ban - Chỉ định (có ngày hết hạn)
        builder.EntitySet<DepartmentAnalysisCapabilityDesignation>("DepartmentAnalysisCapabilityDesignations");

        // Danh mục Chỉ định
        var designationEntitySet = builder.EntitySet<Designation>("Designations");
        designationEntitySet.EntityType.HasOptional(d => d.UpdatedByAccount).AutoExpand = true;

        // Chỉ tiêu - Chỉ định (có ngày hết hạn)
        builder.EntitySet<AnalysisItemDesignation>("AnalysisItemDesignations");

        // Năng lực nhân viên (chỉ tiêu - nhân viên thực hiện)
        var employeeAnalysisCapabilityEntitySet = builder.EntitySet<EmployeeAnalysisCapability>("EmployeeAnalysisCapabilities");
        employeeAnalysisCapabilityEntitySet.EntityType.HasRequired(eac => eac.Employee).AutoExpand = true;
        employeeAnalysisCapabilityEntitySet.EntityType.HasRequired(eac => eac.AnalysisItem).AutoExpand = true;

        // Năng lực nhà thầu phụ - Chỉ định (có ngày hết hạn)
        builder.EntitySet<SubcontractorCapabilityDesignation>("SubcontractorCapabilityDesignations");

        // Danh mục Quy chuẩn/Tiêu chuẩn
        var standardEntitySet = builder.EntitySet<Standard>("Standards");
        standardEntitySet.EntityType.HasOptional(s => s.UpdatedByAccount).AutoExpand = true;

        // Danh mục Phương pháp tham chiếu (Reference Method)
        var referenceMethodEntitySet = builder.EntitySet<ReferenceMethod>("ReferenceMethods");
        referenceMethodEntitySet.EntityType.HasOptional(rm => rm.UpdatedByAccount).AutoExpand = true;

        // Danh mục Đơn vị tính (Unit of Measure)
        var unitOfMeasureEntitySet = builder.EntitySet<UnitOfMeasure>("UnitOfMeasures");
        unitOfMeasureEntitySet.EntityType.HasOptional(u => u.UpdatedByAccount).AutoExpand = true;

        var laboratoryTechniqueEntitySet = builder.EntitySet<LaboratoryTechnique>("LaboratoryTechniques");
        laboratoryTechniqueEntitySet.EntityType.HasOptional(lt => lt.UpdatedByAccount).AutoExpand = true;

        return builder.GetEdmModel();
    }
}

