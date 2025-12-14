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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Tự động tạo database nếu chưa tồn tại
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Database initialized successfully");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database. Please check your connection string in appsettings.json");
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

