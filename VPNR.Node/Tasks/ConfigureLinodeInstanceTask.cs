using System;
using System.Threading.Tasks;
using Hangfire;
using VPNR.Dal;
using VPNR.Linode;
using VPNR.Node.Wireguard;

namespace VPNR.Node.Tasks
{
    [Queue("linode_services"), AutomaticRetry(Attempts = 0)]
    public class ConfigureLinodeInstanceTask
    {
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly LinodeService _linodeService;
        private readonly WireguardNodeService _wireguardNodeService;
        private readonly DbContext _dbContext;

        public ConfigureLinodeInstanceTask(IBackgroundJobClient backgroundJobs, LinodeService linodeService, WireguardNodeService wireguardNodeService, DbContext dbContext)
        {
            _backgroundJobs = backgroundJobs;
            _linodeService = linodeService;
            _wireguardNodeService = wireguardNodeService;
            _dbContext = dbContext;
        }

        public async Task Run(string id)
        {
            var instance = await _dbContext.LinodeInstances.GetAsync(id);
            try
            {
                var linodeInstance = await _linodeService.GetLinodeInstanceAsync(instance.LinodeId);
                if (linodeInstance.Status != "running")
                    throw new Exception("Instance not running yet!");
            
                if (linodeInstance.Ipv4.Length == 0)
                    throw new Exception("Instance has no IPv4 address!");
            
                await _wireguardNodeService.CreateInstallAsync(instance.Id, new string[] { "1.0.0.1", "1.1.1.1", "8.8.8.8" });
            }
            catch (Exception e)
            {
                _backgroundJobs.Schedule<ConfigureLinodeInstanceTask>(t => t.Run(id), new TimeSpan(0, 0, 0, 5));
            }
        }
    }
}