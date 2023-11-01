using Hangfire;
using Microsoft.Extensions.Configuration;
using PoweredSoft.CQRS.Abstractions;
using VPNR.Dal;
using VPNR.Linode;
using VPNR.Linode.Models;
using VPNR.Node.Tasks;
using LinodeInstance = VPNR.Dal.Entities.Providers.LinodeInstance;

namespace VPNR.Command.Node;

public class SpawnNodeInstanceCommand
{
    
}

public class SpawnNodeInstanceCommandHandler : ICommandHandler<SpawnNodeInstanceCommand, string>
{
    private readonly IConfiguration _configuration;
    private readonly LinodeService _linodeService;
    private readonly IBackgroundJobClient _backgroundJobs;
    private readonly DbContext _dbContext;

    public SpawnNodeInstanceCommandHandler(IConfiguration configuration, LinodeService linodeService, IBackgroundJobClient backgroundJobs, DbContext dbContext)
    {
        _configuration = configuration;
        _linodeService = linodeService;
        _backgroundJobs = backgroundJobs;
        _dbContext = dbContext;
    }

    public async Task<string> HandleAsync(SpawnNodeInstanceCommand command, CancellationToken cancellationToken = new CancellationToken())
    {
        var publicKey = _configuration["Ssh:PublicKey"];
        var image = _configuration["Linode:Image"];

        var user = "root";
        var password = "5-xZC4FUBkVBJ,6";

        var instance = await _linodeService.CreateLinodeInstanceAsync(new CreateLinodeInstanceModel()
        {
            Booted = true,
            Image = image,
            Region = "ca-central",
            Type = "g6-nanode-1",
            Label = "api-test",
            Group = "test",
            Tags = new string[]
            {
                "vpn",
                "test"
            },
            AuthorizedKeys = new string[]
            {
                publicKey
            },
            RootPass = password,
            PrivateIp = false,
            BackupsEnabled = false
        }, cancellationToken);
        
        var linodeInstance = await _dbContext.LinodeInstances.AddAsync(new LinodeInstance()
        {
            Ipv4 = instance.Ipv4[0],
            User = user,
            Password = password,
            LinodeId = instance.Id
        }, cancellationToken);

        _backgroundJobs.Schedule<ConfigureLinodeInstanceTask>(t => t.Run(linodeInstance.Id),
            new TimeSpan(0, 0, 0, 30));
        
        // TODO: if something bad make sure to delete instance!

        return linodeInstance.Id;
    }
}