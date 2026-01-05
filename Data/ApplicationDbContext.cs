using Microsoft.EntityFrameworkCore;
using VietLab.Models;
using VietLab.Data.Configurations;

namespace VietLab.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<SampleMatrixGroup> SampleMatrixGroups { get; set; }
    public DbSet<SampleMatrix> SampleMatrices { get; set; }
    public DbSet<EquipmentType> EquipmentTypes { get; set; }
    public DbSet<AnalysisGroup> AnalysisGroups { get; set; }
    public DbSet<AnalysisItem> AnalysisItems { get; set; }
    public DbSet<DepartmentAnalysisCapability> DepartmentAnalysisCapabilities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply Fluent API configurations
        modelBuilder.ApplyConfiguration(new AnalysisGroupConfiguration());
        modelBuilder.ApplyConfiguration(new AnalysisItemConfiguration());
        modelBuilder.ApplyConfiguration(new DepartmentAnalysisCapabilityConfiguration());

        // Map tên bảng cho SampleMatrixGroup, SampleMatrix và EquipmentType trước (database dùng số ít)
        // Phải đặt trước phần convert tự động để không bị ghi đè
        modelBuilder.Entity<SampleMatrixGroup>()
            .ToTable("sample_matrix_group");

        modelBuilder.Entity<SampleMatrix>()
            .ToTable("sample_matrix");

        modelBuilder.Entity<EquipmentType>()
            .ToTable("equipment_type");

        // Convert tất cả tên cột sang snake_case
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Convert tên bảng sang snake_case (nếu chưa được set cụ thể)
            var tableName = entityType.GetTableName();
            if (!string.IsNullOrEmpty(tableName))
            {
                // Chỉ convert nếu tên bảng chưa được set cụ thể (không phải snake_case đã có)
                // Kiểm tra xem có phải là tên đã được set thủ công không
                var isManuallySet = entityType.ClrType.Name == "SampleMatrixGroup" || 
                                   entityType.ClrType.Name == "SampleMatrix" ||
                                   entityType.ClrType.Name == "EquipmentType" ||
                                   entityType.ClrType.Name == "AnalysisGroup" ||
                                   entityType.ClrType.Name == "AnalysisItem" ||
                                   entityType.ClrType.Name == "DepartmentAnalysisCapability";
                
                if (!isManuallySet)
                {
                    entityType.SetTableName(ToSnakeCase(tableName));
                }
            }

            // Convert tên cột sang snake_case (chỉ nếu chưa được set cụ thể)
            foreach (var property in entityType.GetProperties())
            {
                var columnName = property.GetColumnName();
                if (!string.IsNullOrEmpty(columnName))
                {
                    // Kiểm tra xem có phải là cột đã được set thủ công không
                    // (ví dụ: SampleMatrix.SampleMatrixGroupId -> sample_matrix_group_id)
                    // Hoặc các entity đã được config bằng IEntityTypeConfiguration
                    var isManuallySet = (entityType.ClrType.Name == "SampleMatrix" && 
                                       property.Name == "SampleMatrixGroupId" &&
                                       columnName == "sample_matrix_group_id") ||
                                       entityType.ClrType.Name == "AnalysisGroup" ||
                                       entityType.ClrType.Name == "AnalysisItem" ||
                                       entityType.ClrType.Name == "DepartmentAnalysisCapability";
                    
                    if (!isManuallySet)
                    {
                        property.SetColumnName(ToSnakeCase(columnName));
                    }
                }
            }

            // Convert tên foreign key constraints sang snake_case
            foreach (var foreignKey in entityType.GetForeignKeys())
            {
                var constraintName = foreignKey.GetConstraintName();
                if (!string.IsNullOrEmpty(constraintName))
                {
                    foreignKey.SetConstraintName(ToSnakeCase(constraintName));
                }
            }

            // Convert tên index sang snake_case
            foreach (var index in entityType.GetIndexes())
            {
                var indexName = index.GetDatabaseName();
                if (!string.IsNullOrEmpty(indexName))
                {
                    index.SetDatabaseName(ToSnakeCase(indexName));
                }
            }
        }

        // Precision cho DiscountRate để tránh truncate
        modelBuilder.Entity<Client>()
            .Property(c => c.DiscountRate)
            .HasPrecision(5, 2);

        // Quan hệ 1-n: Client - Contacts
        modelBuilder.Entity<Contact>()
            .HasOne(c => c.Client)
            .WithMany(c => c.Contacts)
            .HasForeignKey(c => c.ClientId);

        // Quan hệ 1-n: Branch - Departments
        modelBuilder.Entity<Department>()
            .HasOne(d => d.Branch)
            .WithMany(b => b.Departments)
            .HasForeignKey(d => d.BranchId);

        // Quan hệ 1-1: Employee - Account
        modelBuilder.Entity<Account>()
            .HasOne(a => a.Employee)
            .WithOne(e => e.Account)
            .HasForeignKey<Account>(a => a.EmployeeId);

        // Quan hệ 1-n: Permission - Accounts (một account có 1 permission)
        modelBuilder.Entity<Account>()
            .HasOne(a => a.Permission)
            .WithMany(p => p.Accounts)
            .HasForeignKey(a => a.PermissionId);

        // Quan hệ 1-n: Account - RefreshTokens
        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.Account)
            .WithMany()
            .HasForeignKey(rt => rt.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // Quan hệ 1-n: SampleMatrixGroup - SampleMatrix
        modelBuilder.Entity<SampleMatrix>()
            .HasOne(sm => sm.SampleMatrixGroup)
            .WithMany(smg => smg.SampleMatrices)
            .HasForeignKey(sm => sm.SampleMatrixGroupId);

        // Map tên cột foreign key: SampleMatrixGroupId -> sample_matrix_group_id
        // (EF Core tự động thêm _id vào tên foreign key)
        modelBuilder.Entity<SampleMatrix>()
            .Property(sm => sm.SampleMatrixGroupId)
            .HasColumnName("sample_matrix_group_id");

        // Seed Employees (Guid cố định)
        var emp1Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var emp2Id = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var emp3Id = Guid.Parse("11111111-eeee-eeee-eeee-eeeeeeeeeeee");
        var emp4Id = Guid.Parse("22222222-ffff-ffff-ffff-ffffffffffff");
        var emp5Id = Guid.Parse("33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var emp6Id = Guid.Parse("44444444-1111-1111-1111-111111111111");
        var emp7Id = Guid.Parse("55555555-2222-2222-2222-222222222222");
        var emp8Id = Guid.Parse("66666666-3333-3333-3333-333333333333");
        
        var permAdminId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var permUserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var permManagerId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        var permSalesId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
        
        var acc1Id = Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1");
        var acc2Id = Guid.Parse("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2");
        var acc3Id = Guid.Parse("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3");
        var acc4Id = Guid.Parse("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4");
        var acc5Id = Guid.Parse("e5e5e5e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5");
        var acc6Id = Guid.Parse("f6f6f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6");
        var acc7Id = Guid.Parse("17171717-1717-1717-1717-171717171717");
        var acc8Id = Guid.Parse("28282828-2828-2828-2828-282828282828");

        modelBuilder.Entity<Employee>().HasData(
            new Employee
            {
                EmployeeId = emp1Id,
                EmployeeCode = "EMP-001",
                Department = "Kinh doanh",
                Role = "Sales Manager",
                FullName = "Nguyễn Văn An",
                Title = "Giám đốc Kinh doanh",
                Email = "an.nguyen@viet-labs.com",
                Notes = "Phụ trách khách hàng miền Bắc",
                Status = "Active"
            },
            new Employee
            {
                EmployeeId = emp2Id,
                EmployeeCode = "EMP-002",
                Department = "Kỹ thuật",
                Role = "Tech Lead",
                FullName = "Lê Thị Hương",
                Title = "Trưởng phòng Kỹ thuật",
                Email = "huong.le@viet-labs.com",
                Notes = "Phụ trách tích hợp kỹ thuật",
                Status = "Active"
            },
            new Employee
            {
                EmployeeId = emp3Id,
                EmployeeCode = "EMP-003",
                Department = "Kinh doanh",
                Role = "Sales Executive",
                FullName = "Trần Văn Bình",
                Title = "Chuyên viên Kinh doanh",
                Email = "binh.tran@viet-labs.com",
                Notes = "Phụ trách khách hàng miền Trung",
                Status = "Active"
            },
            new Employee
            {
                EmployeeId = emp4Id,
                EmployeeCode = "EMP-004",
                Department = "Kinh doanh",
                Role = "Sales Executive",
                FullName = "Phạm Thị Mai",
                Title = "Chuyên viên Kinh doanh",
                Email = "mai.pham@viet-labs.com",
                Notes = "Phụ trách khách hàng miền Nam",
                Status = "Active"
            },
            new Employee
            {
                EmployeeId = emp5Id,
                EmployeeCode = "EMP-005",
                Department = "Kỹ thuật",
                Role = "Senior Developer",
                FullName = "Hoàng Văn Đức",
                Title = "Kỹ sư phần mềm",
                Email = "duc.hoang@viet-labs.com",
                Notes = "Phát triển hệ thống CRM",
                Status = "Active"
            },
            new Employee
            {
                EmployeeId = emp6Id,
                EmployeeCode = "EMP-006",
                Department = "Hành chính",
                Role = "HR Manager",
                FullName = "Vũ Thị Lan",
                Title = "Trưởng phòng Nhân sự",
                Email = "lan.vu@viet-labs.com",
                Notes = "Quản lý nhân sự và tuyển dụng",
                Status = "Active"
            },
            new Employee
            {
                EmployeeId = emp7Id,
                EmployeeCode = "EMP-007",
                Department = "Tài chính",
                Role = "Finance Manager",
                FullName = "Đỗ Văn Hùng",
                Title = "Trưởng phòng Tài chính",
                Email = "hung.do@viet-labs.com",
                Notes = "Quản lý tài chính và kế toán",
                Status = "Active"
            },
            new Employee
            {
                EmployeeId = emp8Id,
                EmployeeCode = "EMP-008",
                Department = "Marketing",
                Role = "Marketing Specialist",
                FullName = "Bùi Thị Hoa",
                Title = "Chuyên viên Marketing",
                Email = "hoa.bui@viet-labs.com",
                Notes = "Phụ trách marketing và truyền thông",
                Status = "Active"
            }
        );

        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                PermissionId = permAdminId,
                PermissionCode = "PERM-ADMIN",
                Name = "Quản trị hệ thống",
                Notes = "Toàn quyền",
                Status = "Active"
            },
            new Permission
            {
                PermissionId = permUserId,
                PermissionCode = "PERM-USER",
                Name = "Người dùng",
                Notes = "Quyền người dùng tiêu chuẩn",
                Status = "Active"
            },
            new Permission
            {
                PermissionId = permManagerId,
                PermissionCode = "PERM-MANAGER",
                Name = "Quản lý",
                Notes = "Quyền quản lý phòng ban",
                Status = "Active"
            },
            new Permission
            {
                PermissionId = permSalesId,
                PermissionCode = "PERM-SALES",
                Name = "Kinh doanh",
                Notes = "Quyền truy cập module kinh doanh",
                Status = "Active"
            }
        );

        modelBuilder.Entity<Account>().HasData(
            new Account
            {
                AccountId = acc1Id,
                EmployeeId = emp1Id,
                PermissionId = permAdminId,
                UserName = "an.nguyen",
                PasswordHash = "hashed-password-1",
                Status = "Active"
            },
            new Account
            {
                AccountId = acc2Id,
                EmployeeId = emp2Id,
                PermissionId = permManagerId,
                UserName = "huong.le",
                PasswordHash = "hashed-password-2",
                Status = "Active"
            },
            new Account
            {
                AccountId = acc3Id,
                EmployeeId = emp3Id,
                PermissionId = permSalesId,
                UserName = "binh.tran",
                PasswordHash = "hashed-password-3",
                Status = "Active"
            },
            new Account
            {
                AccountId = acc4Id,
                EmployeeId = emp4Id,
                PermissionId = permSalesId,
                UserName = "mai.pham",
                PasswordHash = "hashed-password-4",
                Status = "Active"
            },
            new Account
            {
                AccountId = acc5Id,
                EmployeeId = emp5Id,
                PermissionId = permUserId,
                UserName = "duc.hoang",
                PasswordHash = "hashed-password-5",
                Status = "Active"
            },
            new Account
            {
                AccountId = acc6Id,
                EmployeeId = emp6Id,
                PermissionId = permManagerId,
                UserName = "lan.vu",
                PasswordHash = "hashed-password-6",
                Status = "Active"
            },
            new Account
            {
                AccountId = acc7Id,
                EmployeeId = emp7Id,
                PermissionId = permManagerId,
                UserName = "hung.do",
                PasswordHash = "hashed-password-7",
                Status = "Active"
            },
            new Account
            {
                AccountId = acc8Id,
                EmployeeId = emp8Id,
                PermissionId = permUserId,
                UserName = "hoa.bui",
                PasswordHash = "hashed-password-8",
                Status = "Active"
            }
        );

        // Seed Branches (Guid cố định)
        var branch1Id = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var branch2Id = Guid.Parse("77777777-7777-7777-7777-777777777777");
        var branch3Id = Guid.Parse("88888888-8888-8888-8888-888888888888");
        var branch4Id = Guid.Parse("99999999-9999-9999-9999-999999999999");

        modelBuilder.Entity<Branch>().HasData(
            new Branch
            {
                BranchId = branch1Id,
                BranchCode = "BR-001",
                NameVi = "Chi nhánh Hà Nội",
                NameEn = "Hanoi Branch",
                License = "CN-2023-HN-001",
                Notes = "Trụ sở chính miền Bắc",
                Status = "Active"
            },
            new Branch
            {
                BranchId = branch2Id,
                BranchCode = "BR-002",
                NameVi = "Chi nhánh TP. Hồ Chí Minh",
                NameEn = "HCMC Branch",
                License = "CN-2023-HCM-002",
                Notes = "Trụ sở chính miền Nam",
                Status = "Active"
            },
            new Branch
            {
                BranchId = branch3Id,
                BranchCode = "BR-003",
                NameVi = "Chi nhánh Đà Nẵng",
                NameEn = "Da Nang Branch",
                License = "CN-2023-DN-003",
                Notes = "Chi nhánh miền Trung",
                Status = "Active"
            },
            new Branch
            {
                BranchId = branch4Id,
                BranchCode = "BR-004",
                NameVi = "Chi nhánh Cần Thơ",
                NameEn = "Can Tho Branch",
                License = "CN-2023-CT-004",
                Notes = "Chi nhánh Đồng bằng sông Cửu Long",
                Status = "Active"
            }
        );

        // Seed Departments (gắn với Branch)
        var dept1Id = Guid.Parse("88888888-8888-8888-8888-888888888888");
        var dept2Id = Guid.Parse("99999999-9999-9999-9999-999999999999");
        var dept3Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var dept4Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var dept5Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        var dept6Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
        var dept7Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
        var dept8Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        modelBuilder.Entity<Department>().HasData(
            new Department
            {
                DepartmentId = dept1Id,
                DepartmentCode = "DEP-HN-KD",
                BranchId = branch1Id,
                NameVi = "Phòng Kinh doanh Hà Nội",
                NameEn = "Hanoi Sales Department",
                Notes = "Phòng ban kinh doanh tại chi nhánh Hà Nội",
                Status = "Active"
            },
            new Department
            {
                DepartmentId = dept2Id,
                DepartmentCode = "DEP-HCM-KT",
                BranchId = branch2Id,
                NameVi = "Phòng Kỹ thuật HCM",
                NameEn = "HCMC Engineering Department",
                Notes = "Phòng ban kỹ thuật tại chi nhánh HCM",
                Status = "Active"
            },
            new Department
            {
                DepartmentId = dept3Id,
                DepartmentCode = "DEP-HN-KT",
                BranchId = branch1Id,
                NameVi = "Phòng Kỹ thuật Hà Nội",
                NameEn = "Hanoi Engineering Department",
                Notes = "Phòng ban kỹ thuật tại chi nhánh Hà Nội",
                Status = "Active"
            },
            new Department
            {
                DepartmentId = dept4Id,
                DepartmentCode = "DEP-HCM-KD",
                BranchId = branch2Id,
                NameVi = "Phòng Kinh doanh HCM",
                NameEn = "HCMC Sales Department",
                Notes = "Phòng ban kinh doanh tại chi nhánh HCM",
                Status = "Active"
            },
            new Department
            {
                DepartmentId = dept5Id,
                DepartmentCode = "DEP-HN-HC",
                BranchId = branch1Id,
                NameVi = "Phòng Hành chính Hà Nội",
                NameEn = "Hanoi Administration Department",
                Notes = "Phòng ban hành chính tại chi nhánh Hà Nội",
                Status = "Active"
            },
            new Department
            {
                DepartmentId = dept6Id,
                DepartmentCode = "DEP-DN-KD",
                BranchId = branch3Id,
                NameVi = "Phòng Kinh doanh Đà Nẵng",
                NameEn = "Da Nang Sales Department",
                Notes = "Phòng ban kinh doanh tại chi nhánh Đà Nẵng",
                Status = "Active"
            },
            new Department
            {
                DepartmentId = dept7Id,
                DepartmentCode = "DEP-DN-KT",
                BranchId = branch3Id,
                NameVi = "Phòng Kỹ thuật Đà Nẵng",
                NameEn = "Da Nang Engineering Department",
                Notes = "Phòng ban kỹ thuật tại chi nhánh Đà Nẵng",
                Status = "Active"
            },
            new Department
            {
                DepartmentId = dept8Id,
                DepartmentCode = "DEP-CT-KD",
                BranchId = branch4Id,
                NameVi = "Phòng Kinh doanh Cần Thơ",
                NameEn = "Can Tho Sales Department",
                Notes = "Phòng ban kinh doanh tại chi nhánh Cần Thơ",
                Status = "Active"
            }
        );

        // Seed data mẫu (sử dụng Guid cố định)
        var client1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var client2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var client3Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var client4Id = Guid.Parse("44444444-1111-1111-1111-111111111111");
        var client5Id = Guid.Parse("55555555-2222-2222-2222-222222222222");
        var client6Id = Guid.Parse("66666666-3333-3333-3333-333333333333");
        var client7Id = Guid.Parse("77777777-4444-4444-4444-444444444444");
        var client8Id = Guid.Parse("88888888-5555-5555-5555-555555555555");
        var client9Id = Guid.Parse("99999999-6666-6666-6666-666666666666");
        var client10Id = Guid.Parse("aaaaaaaa-7777-7777-7777-777777777777");
        var client11Id = Guid.Parse("bbbbbbbb-8888-8888-8888-888888888888");
        var client12Id = Guid.Parse("cccccccc-9999-9999-9999-999999999999");
        var client13Id = Guid.Parse("dddddddd-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var client14Id = Guid.Parse("eeeeeeee-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var client15Id = Guid.Parse("ffffffff-cccc-cccc-cccc-cccccccccccc");

        modelBuilder.Entity<Client>().HasData(
            new Client
            {
                ClientId = client1Id,
                CompanyName = "Công ty ABC",
                InternalCode = "CLI-ABC-001",
                TaxCode = "0101234567",
                BankAccountNumber = "1234567890",
                Address = "123 Đường XYZ",
                City = "Hà Nội",
                Country = "Việt Nam",
                Profession = "Công nghệ thông tin",
                Scale = "200 nhân sự",
                CustomerType = "Enterprise",
                DiscountRate = 5,
                RepresentativeName = "Nguyễn Văn A",
                RepresentativeEmail = "contact@abc.com",
                RepresentativePhone = "0123456789",
                RepresentativeTitle = "Giám đốc",
                SalesOwnerName = "Nguyễn Văn An",
                SalesOwnerEmail = "an.nguyen@viet-labs.com",
                SalesOwnerPhone = "0900000001",
                IsBlacklisted = false,
                BlacklistReason = string.Empty,
                CreatedDate = DateTime.Now,
                LastContactDate = DateTime.Now.AddDays(-5),
                Status = "Active",
                Notes = "Khách hàng tiềm năng cao"
            },
            new Client
            {
                ClientId = client2Id,
                CompanyName = "Công ty XYZ",
                InternalCode = "CLI-XYZ-002",
                TaxCode = "0202345678",
                BankAccountNumber = "2233445566",
                Address = "456 Đường ABC",
                City = "TP. Hồ Chí Minh",
                Country = "Việt Nam",
                Profession = "Thương mại điện tử",
                Scale = "120 nhân sự",
                CustomerType = "SMB",
                DiscountRate = 3,
                RepresentativeName = "Trần Thị B",
                RepresentativeEmail = "info@xyz.com",
                RepresentativePhone = "0987654321",
                RepresentativeTitle = "Trưởng phòng mua hàng",
                SalesOwnerName = "Phạm Thị Mai",
                SalesOwnerEmail = "mai.pham@viet-labs.com",
                SalesOwnerPhone = "0900000002",
                IsBlacklisted = false,
                BlacklistReason = string.Empty,
                CreatedDate = DateTime.Now.AddDays(-30),
                LastContactDate = DateTime.Now.AddDays(-2),
                Status = "Active",
                Notes = "Khách hàng thân thiết"
            },
            new Client
            {
                ClientId = client3Id,
                CompanyName = "Công ty DEF",
                InternalCode = "CLI-DEF-003",
                TaxCode = "0303456789",
                BankAccountNumber = "9988776655",
                Address = "789 Đường DEF",
                City = "Đà Nẵng",
                Country = "Việt Nam",
                Profession = "Sản xuất",
                Scale = "80 nhân sự",
                CustomerType = "Prospect",
                DiscountRate = 0,
                RepresentativeName = "Lê Văn C",
                RepresentativeEmail = "hello@def.com",
                RepresentativePhone = "0912345678",
                RepresentativeTitle = "Phó giám đốc",
                SalesOwnerName = "Trần Văn Bình",
                SalesOwnerEmail = "binh.tran@viet-labs.com",
                SalesOwnerPhone = "0900000003",
                IsBlacklisted = true,
                BlacklistReason = "Đang rà soát công nợ",
                CreatedDate = DateTime.Now.AddDays(-60),
                LastContactDate = null,
                Status = "Prospect",
                Notes = "Đang trong quá trình tư vấn"
            },
            new Client
            {
                ClientId = client4Id,
                CompanyName = "Công ty GHI",
                InternalCode = "CLI-GHI-004",
                TaxCode = "0404567890",
                BankAccountNumber = "1122334455",
                Address = "321 Đường GHI",
                City = "Hà Nội",
                Country = "Việt Nam",
                Profession = "Tài chính - Ngân hàng",
                Scale = "350 nhân sự",
                CustomerType = "Enterprise",
                DiscountRate = 7,
                RepresentativeName = "Phạm Văn D",
                RepresentativeEmail = "contact@ghi.com",
                RepresentativePhone = "0123456780",
                RepresentativeTitle = "Tổng giám đốc",
                SalesOwnerName = "Nguyễn Văn An",
                SalesOwnerEmail = "an.nguyen@viet-labs.com",
                SalesOwnerPhone = "0900000001",
                IsBlacklisted = false,
                BlacklistReason = string.Empty,
                CreatedDate = DateTime.Now.AddDays(-15),
                LastContactDate = DateTime.Now.AddDays(-1),
                Status = "Active",
                Notes = "Khách hàng VIP"
            },
            new Client
            {
                ClientId = client5Id,
                CompanyName = "Công ty JKL",
                InternalCode = "CLI-JKL-005",
                TaxCode = "0505678901",
                BankAccountNumber = "5566778899",
                Address = "654 Đường JKL",
                City = "TP. Hồ Chí Minh",
                Country = "Việt Nam",
                Profession = "Bán lẻ",
                Scale = "150 nhân sự",
                CustomerType = "SMB",
                DiscountRate = 4,
                RepresentativeName = "Hoàng Thị E",
                RepresentativeEmail = "info@jkl.com",
                RepresentativePhone = "0987654320",
                RepresentativeTitle = "Giám đốc Kinh doanh",
                SalesOwnerName = "Phạm Thị Mai",
                SalesOwnerEmail = "mai.pham@viet-labs.com",
                SalesOwnerPhone = "0900000002",
                IsBlacklisted = false,
                BlacklistReason = string.Empty,
                CreatedDate = DateTime.Now.AddDays(-45),
                LastContactDate = DateTime.Now.AddDays(-3),
                Status = "Active",
                Notes = "Khách hàng ổn định"
            },
            new Client
            {
                ClientId = client6Id,
                CompanyName = "Công ty MNO",
                InternalCode = "CLI-MNO-006",
                TaxCode = "0606789012",
                BankAccountNumber = "9988776655",
                Address = "987 Đường MNO",
                City = "Hải Phòng",
                Country = "Việt Nam",
                Profession = "Logistics",
                Scale = "90 nhân sự",
                CustomerType = "SMB",
                DiscountRate = 2,
                RepresentativeName = "Vũ Văn F",
                RepresentativeEmail = "contact@mno.com",
                RepresentativePhone = "0912345670",
                RepresentativeTitle = "Giám đốc",
                SalesOwnerName = "Nguyễn Văn An",
                SalesOwnerEmail = "an.nguyen@viet-labs.com",
                SalesOwnerPhone = "0900000001",
                IsBlacklisted = false,
                BlacklistReason = string.Empty,
                CreatedDate = DateTime.Now.AddDays(-20),
                LastContactDate = DateTime.Now.AddDays(-7),
                Status = "Active",
                Notes = "Khách hàng mới"
            },
            new Client
            {
                ClientId = client7Id,
                CompanyName = "Công ty PQR",
                InternalCode = "CLI-PQR-007",
                TaxCode = "0707890123",
                BankAccountNumber = "3344556677",
                Address = "147 Đường PQR",
                City = "Đà Nẵng",
                Country = "Việt Nam",
                Profession = "Du lịch",
                Scale = "60 nhân sự",
                CustomerType = "Prospect",
                DiscountRate = 0,
                RepresentativeName = "Đỗ Thị G",
                RepresentativeEmail = "info@pqr.com",
                RepresentativePhone = "0123456709",
                RepresentativeTitle = "Trưởng phòng",
                SalesOwnerName = "Trần Văn Bình",
                SalesOwnerEmail = "binh.tran@viet-labs.com",
                SalesOwnerPhone = "0900000003",
                IsBlacklisted = false,
                BlacklistReason = string.Empty,
                CreatedDate = DateTime.Now.AddDays(-10),
                LastContactDate = DateTime.Now.AddDays(-4),
                Status = "Prospect",
                Notes = "Đang tư vấn"
            },
            new Client
            {
                ClientId = client8Id,
                CompanyName = "Công ty STU",
                InternalCode = "CLI-STU-008",
                TaxCode = "0808901234",
                BankAccountNumber = "7788990011",
                Address = "258 Đường STU",
                City = "Cần Thơ",
                Country = "Việt Nam",
                Profession = "Nông nghiệp",
                Scale = "100 nhân sự",
                CustomerType = "SMB",
                DiscountRate = 3,
                RepresentativeName = "Bùi Văn H",
                RepresentativeEmail = "contact@stu.com",
                RepresentativePhone = "0987654309",
                RepresentativeTitle = "Giám đốc",
                SalesOwnerName = "Phạm Thị Mai",
                SalesOwnerEmail = "mai.pham@viet-labs.com",
                SalesOwnerPhone = "0900000002",
                IsBlacklisted = false,
                BlacklistReason = string.Empty,
                CreatedDate = DateTime.Now.AddDays(-25),
                LastContactDate = DateTime.Now.AddDays(-6),
                Status = "Active",
                Notes = "Khách hàng tiềm năng"
            },
            new Client
            {
                ClientId = client9Id,
                CompanyName = "Công ty VWX",
                InternalCode = "CLI-VWX-009",
                TaxCode = "0909012345",
                BankAccountNumber = "2233445566",
                Address = "369 Đường VWX",
                City = "Hà Nội",
                Country = "Việt Nam",
                Profession = "Giáo dục",
                Scale = "180 nhân sự",
                CustomerType = "Enterprise",
                DiscountRate = 6,
                RepresentativeName = "Lý Thị I",
                RepresentativeEmail = "info@vwx.com",
                RepresentativePhone = "0912345608",
                RepresentativeTitle = "Hiệu trưởng",
                SalesOwnerName = "Nguyễn Văn An",
                SalesOwnerEmail = "an.nguyen@viet-labs.com",
                SalesOwnerPhone = "0900000001",
                IsBlacklisted = false,
                BlacklistReason = string.Empty,
                CreatedDate = DateTime.Now.AddDays(-40),
                LastContactDate = DateTime.Now.AddDays(-2),
                Status = "Active",
                Notes = "Khách hàng thân thiết"
            },
            new Client
            {
                ClientId = client10Id,
                CompanyName = "Công ty YZA",
                InternalCode = "CLI-YZA-010",
                TaxCode = "1010123456",
                BankAccountNumber = "4455667788",
                Address = "741 Đường YZA",
                City = "TP. Hồ Chí Minh",
                Country = "Việt Nam",
                Profession = "Y tế",
                Scale = "250 nhân sự",
                CustomerType = "Enterprise",
                DiscountRate = 8,
                RepresentativeName = "Ngô Văn J",
                RepresentativeEmail = "contact@yza.com",
                RepresentativePhone = "0123456708",
                RepresentativeTitle = "Giám đốc",
                SalesOwnerName = "Phạm Thị Mai",
                SalesOwnerEmail = "mai.pham@viet-labs.com",
                SalesOwnerPhone = "0900000002",
                IsBlacklisted = false,
                BlacklistReason = string.Empty,
                CreatedDate = DateTime.Now.AddDays(-35),
                LastContactDate = DateTime.Now.AddDays(-1),
                Status = "Active",
                Notes = "Khách hàng VIP"
            },
            new Client
            {
                ClientId = client11Id,
                CompanyName = "Công ty BCD",
                InternalCode = "CLI-BCD-011",
                TaxCode = "1111234567",
                BankAccountNumber = "6677889900",
                Address = "852 Đường BCD",
                City = "Đà Nẵng",
                Country = "Việt Nam",
                Profession = "Xây dựng",
                Scale = "110 nhân sự",
                CustomerType = "SMB",
                DiscountRate = 3,
                RepresentativeName = "Trương Thị K",
                RepresentativeEmail = "info@bcd.com",
                RepresentativePhone = "0987654308",
                RepresentativeTitle = "Giám đốc",
                SalesOwnerName = "Trần Văn Bình",
                SalesOwnerEmail = "binh.tran@viet-labs.com",
                SalesOwnerPhone = "0900000003",
                IsBlacklisted = false,
                BlacklistReason = string.Empty,
                CreatedDate = DateTime.Now.AddDays(-50),
                LastContactDate = DateTime.Now.AddDays(-8),
                Status = "Active",
                Notes = "Khách hàng ổn định"
            },
            new Client
            {
                ClientId = client12Id,
                CompanyName = "Công ty EFG",
                InternalCode = "CLI-EFG-012",
                TaxCode = "1212345678",
                BankAccountNumber = "8899001122",
                Address = "963 Đường EFG",
                City = "Hà Nội",
                Country = "Việt Nam",
                Profession = "Truyền thông",
                Scale = "70 nhân sự",
                CustomerType = "Prospect",
                DiscountRate = 0,
                RepresentativeName = "Đinh Văn L",
                RepresentativeEmail = "contact@efg.com",
                RepresentativePhone = "0912345607",
                RepresentativeTitle = "Trưởng phòng",
                SalesOwnerName = "Nguyễn Văn An",
                SalesOwnerEmail = "an.nguyen@viet-labs.com",
                SalesOwnerPhone = "0900000001",
                IsBlacklisted = false,
                BlacklistReason = string.Empty,
                CreatedDate = DateTime.Now.AddDays(-5),
                LastContactDate = DateTime.Now.AddDays(-1),
                Status = "Prospect",
                Notes = "Đang tư vấn"
            },
            new Client
            {
                ClientId = client13Id,
                CompanyName = "Công ty HIJ",
                InternalCode = "CLI-HIJ-013",
                TaxCode = "1313456789",
                BankAccountNumber = "0011223344",
                Address = "159 Đường HIJ",
                City = "TP. Hồ Chí Minh",
                Country = "Việt Nam",
                Profession = "Thực phẩm",
                Scale = "140 nhân sự",
                CustomerType = "SMB",
                DiscountRate = 4,
                RepresentativeName = "Phan Thị M",
                RepresentativeEmail = "info@hij.com",
                RepresentativePhone = "0123456707",
                RepresentativeTitle = "Giám đốc",
                SalesOwnerName = "Phạm Thị Mai",
                SalesOwnerEmail = "mai.pham@viet-labs.com",
                SalesOwnerPhone = "0900000002",
                IsBlacklisted = false,
                BlacklistReason = string.Empty,
                CreatedDate = DateTime.Now.AddDays(-55),
                LastContactDate = DateTime.Now.AddDays(-4),
                Status = "Active",
                Notes = "Khách hàng thân thiết"
            },
            new Client
            {
                ClientId = client14Id,
                CompanyName = "Công ty KLM",
                InternalCode = "CLI-KLM-014",
                TaxCode = "1414567890",
                BankAccountNumber = "2233445566",
                Address = "357 Đường KLM",
                City = "Cần Thơ",
                Country = "Việt Nam",
                Profession = "Năng lượng",
                Scale = "95 nhân sự",
                CustomerType = "SMB",
                DiscountRate = 2,
                RepresentativeName = "Võ Văn N",
                RepresentativeEmail = "contact@klm.com",
                RepresentativePhone = "0987654307",
                RepresentativeTitle = "Giám đốc",
                SalesOwnerName = "Phạm Thị Mai",
                SalesOwnerEmail = "mai.pham@viet-labs.com",
                SalesOwnerPhone = "0900000002",
                IsBlacklisted = false,
                BlacklistReason = string.Empty,
                CreatedDate = DateTime.Now.AddDays(-70),
                LastContactDate = DateTime.Now.AddDays(-10),
                Status = "Inactive",
                Notes = "Khách hàng tạm dừng"
            },
            new Client
            {
                ClientId = client15Id,
                CompanyName = "Công ty NOP",
                InternalCode = "CLI-NOP-015",
                TaxCode = "1515678901",
                BankAccountNumber = "5566778899",
                Address = "753 Đường NOP",
                City = "Hà Nội",
                Country = "Việt Nam",
                Profession = "Công nghệ thông tin",
                Scale = "300 nhân sự",
                CustomerType = "Enterprise",
                DiscountRate = 10,
                RepresentativeName = "Lê Văn O",
                RepresentativeEmail = "info@nop.com",
                RepresentativePhone = "0912345606",
                RepresentativeTitle = "Tổng giám đốc",
                SalesOwnerName = "Nguyễn Văn An",
                SalesOwnerEmail = "an.nguyen@viet-labs.com",
                SalesOwnerPhone = "0900000001",
                IsBlacklisted = false,
                BlacklistReason = string.Empty,
                CreatedDate = DateTime.Now.AddDays(-90),
                LastContactDate = DateTime.Now.AddDays(-1),
                Status = "Active",
                Notes = "Khách hàng chiến lược"
            }
        );

        // Seed contacts
        modelBuilder.Entity<Contact>().HasData(
            new Contact
            {
                ContactId = Guid.Parse("aaaaaaa1-0000-0000-0000-000000000001"),
                ClientId = client1Id,
                FullName = "Nguyễn Văn An",
                Email = "an.nguyen@company.com",
                Phone = "0901234567",
                Department = "Kinh doanh",
                Title = "Giám đốc Kinh doanh",
                IsPrimary = true
            },
            new Contact
            {
                ContactId = Guid.Parse("aaaaaaa1-0000-0000-0000-000000000002"),
                ClientId = client1Id,
                FullName = "Lê Thị Hương",
                Email = "huong.le@company.com",
                Phone = "0912345678",
                Department = "Kỹ thuật",
                Title = "Trưởng phòng Kỹ thuật",
                IsPrimary = false
            },
            new Contact
            {
                ContactId = Guid.Parse("aaaaaaa1-0000-0000-0000-000000000003"),
                ClientId = client2Id,
                FullName = "Trần Thị B",
                Email = "b.tran@xyz.com",
                Phone = "0987654321",
                Department = "Mua hàng",
                Title = "Trưởng phòng mua hàng",
                IsPrimary = true
            },
            new Contact
            {
                ContactId = Guid.Parse("aaaaaaa1-0000-0000-0000-000000000004"),
                ClientId = client2Id,
                FullName = "Phạm Văn C",
                Email = "c.pham@xyz.com",
                Phone = "0987654322",
                Department = "Kế toán",
                Title = "Kế toán trưởng",
                IsPrimary = false
            },
            new Contact
            {
                ContactId = Guid.Parse("aaaaaaa1-0000-0000-0000-000000000005"),
                ClientId = client4Id,
                FullName = "Phạm Văn D",
                Email = "d.pham@ghi.com",
                Phone = "0123456780",
                Department = "Điều hành",
                Title = "Tổng giám đốc",
                IsPrimary = true
            },
            new Contact
            {
                ContactId = Guid.Parse("aaaaaaa1-0000-0000-0000-000000000006"),
                ClientId = client4Id,
                FullName = "Nguyễn Thị E",
                Email = "e.nguyen@ghi.com",
                Phone = "0123456781",
                Department = "Tài chính",
                Title = "Giám đốc Tài chính",
                IsPrimary = false
            },
            new Contact
            {
                ContactId = Guid.Parse("aaaaaaa1-0000-0000-0000-000000000007"),
                ClientId = client5Id,
                FullName = "Hoàng Thị E",
                Email = "e.hoang@jkl.com",
                Phone = "0987654320",
                Department = "Kinh doanh",
                Title = "Giám đốc Kinh doanh",
                IsPrimary = true
            },
            new Contact
            {
                ContactId = Guid.Parse("aaaaaaa1-0000-0000-0000-000000000008"),
                ClientId = client6Id,
                FullName = "Vũ Văn F",
                Email = "f.vu@mno.com",
                Phone = "0912345670",
                Department = "Điều hành",
                Title = "Giám đốc",
                IsPrimary = true
            },
            new Contact
            {
                ContactId = Guid.Parse("aaaaaaa1-0000-0000-0000-000000000009"),
                ClientId = client9Id,
                FullName = "Lý Thị I",
                Email = "i.ly@vwx.com",
                Phone = "0912345608",
                Department = "Điều hành",
                Title = "Hiệu trưởng",
                IsPrimary = true
            },
            new Contact
            {
                ContactId = Guid.Parse("aaaaaaa1-0000-0000-0000-000000000010"),
                ClientId = client9Id,
                FullName = "Trần Văn J",
                Email = "j.tran@vwx.com",
                Phone = "0912345609",
                Department = "Hành chính",
                Title = "Phó hiệu trưởng",
                IsPrimary = false
            },
            new Contact
            {
                ContactId = Guid.Parse("aaaaaaa1-0000-0000-0000-000000000011"),
                ClientId = client10Id,
                FullName = "Ngô Văn J",
                Email = "j.ngo@yza.com",
                Phone = "0123456708",
                Department = "Điều hành",
                Title = "Giám đốc",
                IsPrimary = true
            },
            new Contact
            {
                ContactId = Guid.Parse("aaaaaaa1-0000-0000-0000-000000000012"),
                ClientId = client10Id,
                FullName = "Lê Thị K",
                Email = "k.le@yza.com",
                Phone = "0123456709",
                Department = "Y tế",
                Title = "Trưởng khoa",
                IsPrimary = false
            },
            new Contact
            {
                ContactId = Guid.Parse("aaaaaaa1-0000-0000-0000-000000000013"),
                ClientId = client15Id,
                FullName = "Lê Văn O",
                Email = "o.le@nop.com",
                Phone = "0912345606",
                Department = "Điều hành",
                Title = "Tổng giám đốc",
                IsPrimary = true
            },
            new Contact
            {
                ContactId = Guid.Parse("aaaaaaa1-0000-0000-0000-000000000014"),
                ClientId = client15Id,
                FullName = "Phạm Thị P",
                Email = "p.pham@nop.com",
                Phone = "0912345607",
                Department = "Kỹ thuật",
                Title = "CTO",
                IsPrimary = false
            },
            new Contact
            {
                ContactId = Guid.Parse("aaaaaaa1-0000-0000-0000-000000000015"),
                ClientId = client15Id,
                FullName = "Hoàng Văn Q",
                Email = "q.hoang@nop.com",
                Phone = "0912345608",
                Department = "Kinh doanh",
                Title = "Giám đốc Kinh doanh",
                IsPrimary = false
            }
        );
    }

    /// <summary>
    /// Chuyển đổi PascalCase hoặc camelCase sang snake_case
    /// Ví dụ: EmployeeId -> employee_id, FullName -> full_name
    /// </summary>
    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = new System.Text.StringBuilder();
        result.Append(char.ToLowerInvariant(input[0]));

        for (int i = 1; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]))
            {
                result.Append('_');
                result.Append(char.ToLowerInvariant(input[i]));
            }
            else
            {
                result.Append(input[i]);
            }
        }

        return result.ToString();
    }
}

