using PoweredSoft.CQRS.Abstractions;
using VPNR.Linode;
using VPNR.Linode.Models;

namespace VPNR.Query.Node;

public class ListLinodeInstancesQuery
{
    
}

public class ListLinodeInstanceQueryHandler : IQueryHandler<ListLinodeInstancesQuery, IEnumerable<LinodeInstance>>
{
    private readonly LinodeService _linodeService;

    public ListLinodeInstanceQueryHandler(LinodeService linodeService)
    {
        _linodeService = linodeService;
    }

    public async Task<IEnumerable<LinodeInstance>> HandleAsync(ListLinodeInstancesQuery query, CancellationToken cancellationToken)
    {
        var instances = await _linodeService.ListLinodeInstancesAsync(cancellationToken);
        return instances;
    }
}