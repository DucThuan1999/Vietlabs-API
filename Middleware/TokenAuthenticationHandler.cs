using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using VietLab.Data;
using VietLab.Services;

namespace VietLab.Middleware;

public class TokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ApplicationDbContext _context;
    private readonly AccessTokenService _tokenService;

    public TokenAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ApplicationDbContext context,
        AccessTokenService tokenService)
        : base(options, logger, encoder)
    {
        _context = context;
        _tokenService = tokenService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.NoResult();
        }

        var authHeader = Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.NoResult();
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        if (string.IsNullOrEmpty(token))
        {
            return AuthenticateResult.Fail("Token is empty");
        }

        if (!_tokenService.TryValidate(token, out var accountId, out var userName))
        {
            Logger.LogWarning("Token rejected: invalid signature, format, or expired");
            return AuthenticateResult.Fail("Invalid or expired token");
        }

        // Yêu cầu khớp CHÍNH XÁC AccountId + UserName + Active — không có fallback mạo danh theo UserName
        // (trước đây nếu AccountId không khớp sẽ âm thầm đăng nhập bằng account đầu tiên trùng UserName).
        var account = await _context.Accounts
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.AccountId == accountId && a.UserName == userName && a.Status == "Active");

        if (account == null)
        {
            Logger.LogWarning("Token authentication failed: account not found or inactive - AccountId={AccountId}, UserName={UserName}", accountId, userName);
            return AuthenticateResult.Fail("Invalid token or account not found");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
            new Claim(ClaimTypes.Name, account.UserName),
            new Claim("AccountId", account.AccountId.ToString()),
            new Claim("EmployeeId", account.EmployeeId.ToString()),
        };

        if (account.Employee != null)
        {
            claims.Add(new Claim(ClaimTypes.GivenName, account.Employee.FullName ?? ""));
            claims.Add(new Claim(ClaimTypes.Email, account.Employee.Email ?? ""));
        }

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        Logger.LogInformation("Token authentication successful for user: {UserName}, AccountId: {AccountId}", userName, accountId);
        return AuthenticateResult.Success(ticket);
    }
}

