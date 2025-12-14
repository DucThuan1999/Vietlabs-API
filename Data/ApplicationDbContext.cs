using Microsoft.EntityFrameworkCore;
using VietLab.Models;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
            .WithOne()
            .HasForeignKey<Account>(a => a.EmployeeId);

        // Quan hệ 1-n: Permission - Accounts (một account có 1 permission)
        modelBuilder.Entity<Account>()
            .HasOne(a => a.Permission)
            .WithMany(p => p.Accounts)
            .HasForeignKey(a => a.PermissionId);

        // Seed Employees (Guid cố định)
        var emp1Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var emp2Id = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var permAdminId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var permUserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var acc1Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        var acc2Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

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
                PermissionId = permUserId,
                UserName = "huong.le",
                PasswordHash = "hashed-password-2",
                Status = "Active"
            }
        );

        // Seed Branches (Guid cố định)
        var branch1Id = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var branch2Id = Guid.Parse("77777777-7777-7777-7777-777777777777");

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
            }
        );

        // Seed Departments (gắn với Branch)
        var dept1Id = Guid.Parse("88888888-8888-8888-8888-888888888888");
        var dept2Id = Guid.Parse("99999999-9999-9999-9999-999999999999");

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
            }
        );

        // Seed data mẫu (sử dụng Guid cố định)
        var client1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var client2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var client3Id = Guid.Parse("33333333-3333-3333-3333-333333333333");

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
                SalesOwnerName = "Admin",
                SalesOwnerEmail = "admin@viet-labs.com",
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
                SalesOwnerName = "Admin",
                SalesOwnerEmail = "admin@viet-labs.com",
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
                SalesOwnerName = "Admin",
                SalesOwnerEmail = "admin@viet-labs.com",
                SalesOwnerPhone = "0900000003",
                IsBlacklisted = true,
                BlacklistReason = "Đang rà soát công nợ",
                CreatedDate = DateTime.Now.AddDays(-60),
                LastContactDate = null,
                Status = "Prospect",
                Notes = "Đang trong quá trình tư vấn"
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
            }
        );
    }
}

