using Microsoft.Extensions.DependencyInjection;
using PoweredSoft.CQRS.Abstractions;
using PoweredSoft.Module.Abstractions;
using VPNR.Command.Node;

namespace VPNR.Command;

public class CommandModule : IModule
{
    public IServiceCollection ConfigureServices(IServiceCollection services)
    {
        services.AddCommand<SpawnNodeInstanceCommand, string, SpawnNodeInstanceCommandHandler>();
        services.AddCommand<DeleteNodeInstanceCommand, bool, DeleteNodeInstanceCommandHandler>();
        return services;
    }
}