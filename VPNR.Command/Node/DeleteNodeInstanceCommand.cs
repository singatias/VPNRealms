using PoweredSoft.CQRS.Abstractions;
using VPNR.Linode;

namespace VPNR.Command.Node;

public class DeleteNodeInstanceCommand
{
    public int Id { get; set; }
}

public class DeleteNodeInstanceCommandHandler : ICommandHandler<DeleteNodeInstanceCommand, bool>
{
    private readonly LinodeService _linodeService;

    public DeleteNodeInstanceCommandHandler(LinodeService linodeService)
    {
        _linodeService = linodeService;
    }

    public Task<bool> HandleAsync(DeleteNodeInstanceCommand command, CancellationToken cancellationToken = new CancellationToken())
    {
        return _linodeService.DeleteLinodeInstance(command.Id, cancellationToken);
    }
}