using Microsoft.Extensions.DependencyInjection;
using PoweredSoft.Module.Abstractions;

namespace VPNR.Keycloak
{
    public class KeycloakModule : IModule
    {
        public IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services.AddTransient<KeycloakService>();
        }
    }
}