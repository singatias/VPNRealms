using Microsoft.Extensions.DependencyInjection;

namespace VPNR.Dal
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQueryableProviderOverride<T, TService>(this IServiceCollection services)
            where TService : class, IQueryableProviderOverride<T>
        {
            return services.AddTransient<IQueryableProviderOverride<T>, TService>();
        }
    }
}