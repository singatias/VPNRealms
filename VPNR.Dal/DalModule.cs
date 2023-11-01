using Microsoft.Extensions.DependencyInjection;
using PoweredSoft.CQRS.DynamicQuery.Abstractions;
using PoweredSoft.DynamicQuery.Core;
using PoweredSoft.Module.Abstractions;

namespace VPNR.Dal
{
    public class DalModule : IModule
    {
        public IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<DbContext>();
            services.AddTransient(typeof(IQueryableProvider<>), typeof(DefaultQueryableProvider<>));
            services.AddTransient<IQueryInterceptorProvider, GlobalQueryInterceptorProvider>();
            return services;
        }
    }
}