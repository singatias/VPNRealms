using Microsoft.Extensions.DependencyInjection;
using PoweredSoft.CQRS.Abstractions;
using PoweredSoft.Module.Abstractions;
using VPNR.Linode.Models;
using VPNR.Query.Node;

namespace VPNR.Query;

public class QueryModule : IModule
{
    public IServiceCollection ConfigureServices(IServiceCollection services)
    {
        return services
            .AddQuery<ListLinodeInstancesQuery, IEnumerable<LinodeInstance>, ListLinodeInstanceQueryHandler>();
    }
}