namespace VPNRealms;

public class HangfireActivator : Hangfire.JobActivator
{
    private readonly IServiceProvider serviceProvider;

    public HangfireActivator(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }
        
    public override object ActivateJob(Type type)
    {
        return serviceProvider.GetService(type);
    }
}