using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using VietLab.Data;
using VietLab.Filters;
using VietLab.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình base path cho IIS sub application
// Trong Development, không dùng base path. Chỉ dùng khi deploy lên IIS
var basePath = Environment.GetEnvironmentVariable("ASPNETCORE_BASEPATH");
if (string.IsNullOrEmpty(basePath))
{
    // Chỉ đọc từ appsettings.json nếu không phải Development
    var environment = builder.Environment.EnvironmentName;
    if (environment != "Development")
    {
        basePath = builder.Configuration["BasePath"] ?? "/crm-api";
    }
    else
    {
        basePath = ""; // Không dùng base path trong Development
    }
}

if (!string.IsNullOrEmpty(basePath))
{
    if (!basePath.StartsWith("/"))
    {
        basePath = "/" + basePath;
    }
    if (basePath.EndsWith("/"))
    {
        basePath = basePath.TrimEnd('/');
    }
}
else
{
    basePath = ""; // Empty string cho Development
}

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Convert tất cả property names sang camelCase cho frontend
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        // Giữ nguyên tên dictionary keys
        options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        // Format enum sang string (camelCase)
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase));
    })
    .AddOData(options =>
    {
        options.Select()
               .Filter()
               .Expand()
               .OrderBy()
               .SetMaxTop(5000)
               .Count();

        // Cấu hình OData route options
        // OData yêu cầu ít nhất một trong hai tùy chọn phải được bật
        options.RouteOptions.EnableKeyInParenthesis = true;  // Cho phép /Clients(123)
        options.RouteOptions.EnableKeyAsSegment = false;   // Không dùng /Clients/123
        options.RouteOptions.EnableControllerNameCaseInsensitive = true;

        // Sử dụng EDM model
        // OData sẽ tự động sử dụng JsonSerializerOptions từ AddJsonOptions ở trên
        options.AddRouteComponents("odata", ODataEdmModel.GetEdmModel());
    });

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

    // Cấu hình Swagger sử dụng camelCase cho schema (để khớp với JSON response)
    c.UseInlineDefinitionsForEnums();

    // Schema Filter để convert property names sang camelCase
    c.SchemaFilter<CamelCaseSchemaFilter>();

    // Sử dụng System.Text.Json naming policy cho Swagger schema
    c.UseAllOfToExtendReferenceSchemas();
    c.SupportNonNullableReferenceTypes();

    // Cấu hình base path cho Swagger khi deploy trên IIS sub application
    if (!string.IsNullOrEmpty(basePath) && basePath != "/")
    {
        c.AddServer(new Microsoft.OpenApi.Models.OpenApiServer
        {
            Url = basePath,
            Description = "IIS Sub Application Base Path"
        });
    }

    // Cấu hình Bearer Token Authentication cho Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
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

builder.Services.AddScoped<VietLab.Data.Layer0ReferenceDataSeeder>();

// Add Repositories
builder.Services.AddScoped<VietLab.Repositories.IStoreRepository, VietLab.Repositories.StoreRepository>();

// Add Services
builder.Services.AddScoped<VietLab.Services.IClientHistoryService, VietLab.Services.ClientHistoryService>();
builder.Services.AddScoped<VietLab.Services.IQuotationHistoryService, VietLab.Services.QuotationHistoryService>();
builder.Services.AddScoped<VietLab.Services.ModulePermissionService>();

builder.Services.Configure<VietLab.Configuration.SmtpOptions>(builder.Configuration.GetSection(VietLab.Configuration.SmtpOptions.SectionName));
builder.Services.AddScoped<VietLab.Services.IEmailSender, VietLab.Services.SmtpEmailSender>();

// Luôn đăng ký Authentication + scheme Bearer (tránh lỗi "No authentication handlers are registered" khi gọi [Authorize(AuthenticationSchemes = "Bearer")])
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
    options.DefaultScheme = "Bearer";
})
.AddScheme<AuthenticationSchemeOptions, TokenAuthenticationHandler>("Bearer", options => { });

// Kiểm tra config để disable authentication (tạm thời cho test)
var disableAuth = builder.Configuration.GetValue<bool>("DisableAuthentication", false);

if (!disableAuth)
{
    builder.Services.AddAuthorization();
}
else
{
    // Khi disable authentication, tạo policy cho phép tất cả
    builder.Services.AddAuthorization(options =>
    {
        options.FallbackPolicy = options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
            .RequireAssertion(_ => true) // Luôn cho phép
            .Build();
    });
}

// CORS: thêm origin từ config; cho phép IP mạng nội bộ (LAN) khi bật — tránh 403 khi mở FE bằng http://192.168.x.x:port
var corsAllowedOriginsExtra = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
var corsExtraOriginSet = new HashSet<string>(
    corsAllowedOriginsExtra.Where(o => !string.IsNullOrWhiteSpace(o)).Select(o => o.TrimEnd('/')),
    StringComparer.OrdinalIgnoreCase);
var corsAllowPrivateLan = builder.Configuration.GetValue("Cors:AllowPrivateNetworkOrigins", false);

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
    // Lưu ý: Không dùng AllowCredentials() khi dùng WithOrigins() với http://localhost
    // vì browser sẽ từ chối credentials từ non-https origins
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "http://localhost:5174",
                "http://localhost:5175",
                "http://localhost:5176",
                "http://localhost:4200",
                "http://localhost:8080",
                "https://localhost:3000",
                "https://localhost:5173",
                "https://localhost:5174",
                "https://localhost:5175",
                "https://localhost:5176",
                "https://localhost:4200"
              )
              .AllowAnyMethod()
              .AllowAnyHeader();
        // Không dùng AllowCredentials() để tránh lỗi với http://localhost
    });

    // Policy cho localhost development - luôn cho phép localhost trong mọi môi trường
    // Và cũng cho phép các origins được cấu hình trong AllowSpecificOrigins
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
            {
                if (string.IsNullOrEmpty(origin))
                {
                    return false;
                }

                try
                {
                    if (corsExtraOriginSet.Contains(origin.TrimEnd('/')))
                    {
                        return true;
                    }

                    var uri = new Uri(origin);
                    // Localhost mọi port + IPv6 loopback
                    if (uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
                        || uri.Host == "127.0.0.1"
                        || uri.Host == "::1")
                    {
                        return true;
                    }

                    if (corsAllowPrivateLan && CorsPrivateNetwork.IsPrivateLanHost(uri.Host))
                    {
                        return true;
                    }

                    var allowedOrigins = new[]
                    {
                        "http://localhost:3000",
                        "http://localhost:5173",
                        "http://localhost:5174",
                        "http://localhost:5175",
                        "http://localhost:5176",
                        "http://localhost:4200",
                        "http://localhost:8080",
                        "https://localhost:3000",
                        "https://localhost:5173",
                        "https://localhost:5174",
                        "https://localhost:5175",
                        "https://localhost:5176",
                        "https://localhost:4200"
                    };

                    return allowedOrigins.Contains(origin);
                }
                catch
                {
                    return false;
                }
            })
            .AllowAnyMethod()
            .AllowAnyHeader();
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

// Log base path để debug
var startupLogger = app.Services.GetRequiredService<ILogger<Program>>();
startupLogger.LogInformation("Base Path configured: {BasePath}", basePath);

// Cấu hình base path middleware cho IIS sub application
// Lưu ý: Với IIS sub application, IIS có thể đã tự động xử lý base path
// Chỉ enable UsePathBase khi có base path và không phải Development
var usePathBaseMiddleware = builder.Configuration.GetValue<bool>("UsePathBaseMiddleware", false);
if (usePathBaseMiddleware && !string.IsNullOrEmpty(basePath) && basePath != "/")
{
    app.UsePathBase(new PathString(basePath));
    startupLogger.LogInformation("PathBase middleware enabled with: {BasePath}", basePath);
}
else
{
    startupLogger.LogInformation("PathBase middleware disabled. BasePath: {BasePath}", basePath ?? "empty");
}

// Configure the HTTP request pipeline.
var isAuthDisabled = app.Configuration.GetValue<bool>("DisableAuthentication", false);
var corsPolicyName = app.Environment.IsDevelopment() ? "AllowAll" : "AllowLocalhost";

// UseRouting + UseCors trước Swagger/Auth — đúng thứ tự doc ASP.NET Core cho preflight OPTIONS
app.UseRouting();
app.UseCors(corsPolicyName);

{
    var corsLogger = app.Services.GetRequiredService<ILogger<Program>>();
    corsLogger.LogInformation("CORS policy: {Policy} (env: {Env})", corsPolicyName, app.Environment.EnvironmentName);
    if (isAuthDisabled)
    {
        corsLogger.LogWarning("⚠️  AUTHENTICATION IS DISABLED - CORS active");
    }
}

app.UseSwagger(c =>
{
    c.RouteTemplate = "swagger/{documentName}/swagger.json";
    if (!string.IsNullOrEmpty(basePath) && basePath != "/")
    {
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            swaggerDoc.Servers = new List<Microsoft.OpenApi.Models.OpenApiServer>
            {
                new Microsoft.OpenApi.Models.OpenApiServer
                {
                    Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{basePath}"
                }
            };
        });
    }
});

app.UseSwaggerUI(c =>
{
    // SwaggerEndpoint là relative path từ RoutePrefix
    // Vì RoutePrefix = "swagger", nên endpoint chỉ cần "v1/swagger.json"
    // Hoặc có thể dùng absolute path "/swagger/v1/swagger.json"
    c.SwaggerEndpoint("v1/swagger.json", "VietLab CRM API v1");
    c.RoutePrefix = "swagger";
    c.DisplayRequestDuration();
    c.EnableDeepLinking();
    c.EnableFilter();
    c.ShowExtensions();
    c.EnableValidator();
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
});

// Development: không redirect HTTP→HTTPS để tránh preflight OPTIONS bị redirect và browser báo lỗi CORS khi gọi http://localhost:5000
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// CamelCase middleware cho OData - đảm bảo OData response là camelCase
// Tạm thời disable để test - OData có thể đã tự động sử dụng JsonSerializerOptions
// app.UseMiddleware<CamelCaseODataMiddleware>();

// Authentication & Authorization middleware
// Chỉ enable nếu không disable trong config
if (!isAuthDisabled)
{
    app.UseAuthentication();
    app.UseAuthorization();
}
else
{
    // Vẫn cần UseAuthorization để xử lý [Authorize] attributes nhưng với policy cho phép tất cả
    app.UseAuthorization();
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning("⚠️  AUTHENTICATION IS DISABLED - All requests will be allowed! This should only be used for testing!");
}

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

        if (app.Configuration.GetValue("Seed:Layer0:Enabled", false))
        {
            var layer0Logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            try
            {
                var layer0 = scope.ServiceProvider.GetRequiredService<VietLab.Data.Layer0ReferenceDataSeeder>();
                layer0.SyncAsync(dbContext).GetAwaiter().GetResult();
                layer0Logger.LogInformation("Seed:Layer0 completed");
            }
            catch (Exception layer0Ex)
            {
                layer0Logger.LogError(layer0Ex, "Seed:Layer0 failed");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database. Please check your connection string in appsettings.json");
        // Không throw exception để ứng dụng vẫn có thể chạy
    }
}

app.Run();

/// <summary>IP private RFC1918 — dùng cho CORS khi FE truy cập qua IP LAN.</summary>
internal static class CorsPrivateNetwork
{
    public static bool IsPrivateLanHost(string host)
    {
        if (string.IsNullOrEmpty(host) || !IPAddress.TryParse(host, out var ip))
        {
            return false;
        }

        if (ip.AddressFamily != AddressFamily.InterNetwork)
        {
            return false;
        }

        var b = ip.GetAddressBytes();
        if (b[0] == 10)
        {
            return true;
        }

        if (b[0] == 172 && b[1] >= 16 && b[1] <= 31)
        {
            return true;
        }

        if (b[0] == 192 && b[1] == 168)
        {
            return true;
        }

        return false;
    }
}

