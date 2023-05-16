using System.Security.Authentication;
using System.Security.Claims;
using App.Backend.DTO;
using App.Backend.Service;
using MongoDB.Bson;

namespace App.Backend.Auth;

public class AuthContext
{
    public static string TenantHeader = "X-Tenant";

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TenantService _tenantService;
    private readonly UserService _userService;
    private readonly IDictionary<User, Tenant> _currentTentant;
    private readonly IDictionary<string, User> _currentUser;

    public AuthContext(
        IHttpContextAccessor httpContextAccessor,
        TenantService tenantService,
        UserService userService)
    {
        _httpContextAccessor = httpContextAccessor;
        _tenantService = tenantService;
        _userService = userService;
        _currentUser = new Dictionary<string, User>();
        _currentTentant = new Dictionary<User, Tenant>();
    }

    public async Task<User> GetUser(CancellationToken cancellationToken = default)
    {
        var externalId = _httpContextAccessor?.HttpContext?.User?.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        if (externalId == null)
        {
            throw new AuthenticationException("User not authenticated");
        }

        return await GetUser(externalId, cancellationToken);
    }

    public async Task<User> GetUser(string externalId, CancellationToken cancellationToken = default)
    {
        if (!_currentUser.ContainsKey(externalId))
        {
            var user = await _userService.GetOrCreate(externalId, cancellationToken);
            _currentUser[externalId] = new User
            {
                Id = user.Id.ToString(),
                ExternalId = user.ExternalId,
            };
        }
        return _currentUser[externalId];
    }

    public async Task<Tenant> GetTenant(CancellationToken cancellationToken = default)
    {
        var user = await GetUser(cancellationToken);
        return await GetTenant(user, cancellationToken);
    }

    public async Task<Tenant> GetTenant(string externalId, CancellationToken cancellationToken = default)
    {
        var user = await GetUser(externalId, cancellationToken);
        return await GetTenant(user, cancellationToken);
    }

    public async Task<Tenant> GetTenant(User user, CancellationToken cancellationToken = default)
    {
        if (!_currentTentant.ContainsKey(user))
        {
            var tenant = await _tenantService.GetOrCreate(userId: new ObjectId(user.Id), cancellationToken: cancellationToken);
            _currentTentant[user] = new Tenant
            {
                Id = tenant.Id.ToString(),
                Name = tenant.Name,
            };
        }
        return _currentTentant[user];
    }
}