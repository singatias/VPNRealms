using Microsoft.Extensions.DependencyInjection;
using PoweredSoft.Module.Abstractions;

namespace VPNR.Linode
{
    public class LinodeModule : IModule
    {
        public IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services.AddSingleton<LinodeService>();
        }
    }
}