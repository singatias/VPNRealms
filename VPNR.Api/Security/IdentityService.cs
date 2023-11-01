using System.Security.Claims;
using VPNR.Dal;
using VPNR.Dal.Entities;
using VPNR.Node;

namespace VPNRealms.Security;

public class IdentityService : IIdentityService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DbContext _context;
    
    private User? user;
    
    
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    public string Email => ResolveClaim(ClaimTypes.Email);
    public string FirstName => ResolveClaim(ClaimTypes.GivenName);
    public string LastName => ResolveClaim(ClaimTypes.Surname);
    public string FullName => $"{FirstName} {LastName}";

    public IdentityService(IHttpContextAccessor httpContextAccessor, DbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public async Task<User> GetUserAsync(CancellationToken cancellationToken = default)
    {
        if (!IsAuthenticated)
            return null;

        if (user == null)
        {
            var emailLower = Email?.ToLower();
            user = await _context.Users.FirstOrDefaultAsync(e => e.Email.ToLower() == emailLower, cancellationToken);
        }
            
        return user;
    }
    
    private string ResolveClaim(params string[] name)
    {
        if (!IsAuthenticated)
            return null;

        var claim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(t => name.Contains(t.Type));
        return claim?.Value;
    }
}