using Microsoft.Extensions.DependencyInjection;
using PoweredSoft.Module.Abstractions;
using VPNR.Node.Wireguard;

namespace VPNR.Node
{
    public class NodeModule : IModule
    {
        public IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services.AddSingleton<WireguardNodeService>();
        }
    }
}