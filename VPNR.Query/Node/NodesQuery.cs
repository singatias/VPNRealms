using PoweredSoft.CQRS.Abstractions;
using VPNR.Dal;

namespace VPNR.Query.Node;

public class NodesQuery
{
    
}

public class NodesQueryResult
{
    
}

public class NodesQueryHandler : IQueryHandler<NodesQuery, IEnumerable<NodesQueryResult>>
{
    private readonly DbContext _dbContext;

    public NodesQueryHandler(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<IEnumerable<NodesQueryResult>> HandleAsync(NodesQuery query, CancellationToken cancellationToken = new CancellationToken())
    {
        
        throw new NotImplementedException();
    }
}