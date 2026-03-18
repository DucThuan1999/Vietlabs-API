using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using VietLab.Data;

namespace VietLab.Middleware;

public class TokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ApplicationDbContext _context;

    public TokenAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ApplicationDbContext context)
        : base(options, logger, encoder)
    {
        _context = context;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Logger.LogDebug("TokenAuthenticationHandler: HandleAuthenticateAsync called for path: {Path}", Request.Path);
        
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            Logger.LogDebug("TokenAuthenticationHandler: No Authorization header found");
            return AuthenticateResult.NoResult();
        }

        var authHeader = Request.Headers["Authorization"].ToString();
        Logger.LogDebug("TokenAuthenticationHandler: Authorization header: {Header}", authHeader?.Substring(0, Math.Min(20, authHeader?.Length ?? 0)) + "...");
        
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            Logger.LogDebug("TokenAuthenticationHandler: Authorization header does not start with 'Bearer '");
            return AuthenticateResult.NoResult();
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        if (string.IsNullOrEmpty(token))
        {
            return AuthenticateResult.Fail("Token is empty");
        }

        try
        {
            // Decode token để lấy thông tin
            var tokenBytes = Convert.FromBase64String(token);
            var tokenString = Encoding.UTF8.GetString(tokenBytes);
            var tokenParts = tokenString.Split(':');

            Logger.LogDebug("Token decoded: {TokenString}, Parts: {PartsCount}", tokenString, tokenParts.Length);

            if (tokenParts.Length < 2)
            {
                Logger.LogWarning("Invalid token format: expected at least 2 parts, got {PartsCount}", tokenParts.Length);
                return AuthenticateResult.Fail("Invalid token format");
            }

            // Parse AccountId và UserName từ token
            if (!Guid.TryParse(tokenParts[0], out var accountId))
            {
                Logger.LogWarning("Invalid account ID in token: {AccountIdString}", tokenParts[0]);
                return AuthenticateResult.Fail("Invalid account ID in token");
            }

            var userName = tokenParts[1];
            Logger.LogDebug("Parsed token - AccountId: {AccountId}, UserName: {UserName}", accountId, userName);

            // Tìm account trong database
            var account = await _context.Accounts
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.AccountId == accountId && a.UserName == userName && a.Status == "Active");

            if (account == null)
            {
                // Kiểm tra xem account có tồn tại nhưng không active không
                var inactiveAccount = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountId == accountId && a.UserName == userName);
                
                if (inactiveAccount != null)
                {
                    Logger.LogWarning("Account found but not active: {AccountId}, Status: {Status}", accountId, inactiveAccount.Status);
                    return AuthenticateResult.Fail($"Account is not active. Status: {inactiveAccount.Status}");
                }
                
                // Nếu không tìm thấy account, có thể là admin bypass token
                // Tìm account đầu tiên có UserName trùng hoặc tìm account admin
                var fallbackAccount = await _context.Accounts
                    .Include(a => a.Employee)
                    .FirstOrDefaultAsync(a => a.UserName == userName && a.Status == "Active");
                
                if (fallbackAccount != null)
                {
                    Logger.LogInformation("Using fallback account for admin bypass token: AccountId={AccountId}, UserName={UserName}", fallbackAccount.AccountId, fallbackAccount.UserName);
                    account = fallbackAccount;
                }
                else
                {
                    Logger.LogWarning("Account not found: AccountId={AccountId}, UserName={UserName}", accountId, userName);
                    return AuthenticateResult.Fail("Invalid token or account not found");
                }
            }

            // Tạo claims
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
        catch (FormatException ex)
        {
            Logger.LogWarning(ex, "Token format error: {Token}", token?.Substring(0, Math.Min(20, token?.Length ?? 0)));
            return AuthenticateResult.Fail($"Token format error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating token: {Error}", ex.Message);
            return AuthenticateResult.Fail($"Token validation failed: {ex.Message}");
        }
    }
}

