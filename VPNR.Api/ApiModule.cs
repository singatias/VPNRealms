using PoweredSoft.Module.Abstractions;
using VPNR.Node;

namespace VPNRealms;

public class ApiModule : IModule
{
    public IServiceCollection ConfigureServices(IServiceCollection services)
    {
        return services.AddTransient<IIdentityService, IIdentityService>();
    }
}