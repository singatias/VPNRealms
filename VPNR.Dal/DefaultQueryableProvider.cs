using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PoweredSoft.CQRS.DynamicQuery.Abstractions;

namespace VPNR.Dal
{
    public class DefaultQueryableProvider<T> : IQueryableProvider<T>
    {
        private readonly DbContext context;
        private readonly IServiceProvider serviceProvider;

        public DefaultQueryableProvider(DbContext context, IServiceProvider serviceProvider)
        {
            this.context = context;
            this.serviceProvider = serviceProvider;
        }

        public Task<IQueryable<T>> GetQueryableAsync(object query, CancellationToken cancellationToken = default)
        {
            if (serviceProvider.GetService(typeof(IQueryableProviderOverride<T>)) is IQueryableProviderOverride<T> queryableProviderOverride)
                return queryableProviderOverride.GetQueryableAsync(query, cancellationToken);

            return Task.FromResult(context.GetCollection<T>().AsQueryable());
        }
    }
}