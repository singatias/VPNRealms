using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VPNR.Dal
{
    public interface IQueryableProviderOverride<T>
    {
        Task<IQueryable<T>> GetQueryableAsync(object query, CancellationToken cancellationToken = default);
    }
}