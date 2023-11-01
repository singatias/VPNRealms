using System.Threading;
using System.Threading.Tasks;
using VPNR.Dal.Entities;

namespace VPNR.Node
{
    public interface IIdentityService
    {
        bool IsAuthenticated { get; }
        string Email { get; }
        string FirstName { get; }
        string LastName { get; }
        string FullName { get; }
        Task<User> GetUserAsync(CancellationToken cancellationToken = default);
    }
}