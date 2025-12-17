using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using VietLab.Data;
using VietLab.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddOData(options => options
        .Select()
        .Filter()
        .OrderBy()
        .SetMaxTop(100)
        .Count()
        .AddRouteComponents("odata", GetEdmModel()));

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "VietLab CRM API",
        Version = "v1",
        Description = "API quản lý khách hàng với OData support",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "VietLab",
            Email = "admin@viet-labs.com"
        }
    });

    // Giải quyết xung đột route OData (Swagger không hiểu template "()")
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add CORS
builder.Services.AddCors(options =>
{
    // Policy cho Development - cho phép tất cả origins
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    
    // Policy cho Production - chỉ cho phép specific origins
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "http://localhost:4200",
                "http://localhost:8080",
                "https://localhost:3000",
                "https://localhost:5173",
                "https://localhost:4200"
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
    
    // Policy mặc định - luôn enable cho tất cả môi trường
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "VietLab CRM API v1");
        c.RoutePrefix = "swagger";
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
        c.ShowExtensions();
        c.EnableValidator();
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
    });
}

// Enable CORS - phải đặt trước UseHttpsRedirection, UseAuthorization và MapControllers
// Luôn enable CORS cho tất cả môi trường
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll");
}
else
{
    app.UseCors("AllowSpecificOrigins");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Tự động tạo database nếu chưa tồn tại
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        // Kiểm tra xem database có tồn tại và có bảng không
        if (!dbContext.Database.CanConnect())
        {
            logger.LogInformation("Database does not exist. Creating database...");
            dbContext.Database.EnsureCreated();
            logger.LogInformation("Database created successfully");
        }
        else
        {
            // Kiểm tra xem bảng Contacts có tồn tại không
            try
            {
                var testQuery = dbContext.Contacts.Count();
                logger.LogInformation("Database and tables are ready");
            }
            catch (Exception tableEx)
            {
                logger.LogWarning(tableEx, "Some tables are missing. Recreating database...");
                // Xóa và tạo lại database
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                logger.LogInformation("Database recreated successfully with all tables");
            }
        }
        
        logger.LogInformation("Database initialized successfully");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database. Please check your connection string in appsettings.json");
        // Không throw exception để ứng dụng vẫn có thể chạy
    }
}

app.Run();

static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<Client>("Clients");
    builder.EntitySet<Contact>("Contacts");
    builder.EntitySet<Employee>("Employees");
    builder.EntitySet<Branch>("Branches");
    builder.EntitySet<Department>("Departments");
    builder.EntitySet<Account>("Accounts");
    builder.EntitySet<Permission>("Permissions");
    return builder.GetEdmModel();
}

