using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Renci.SshNet;
using VPNR.Dal;
using VPNR.Dal.Entities;
using VPNR.Dal.Entities.Nodes;

namespace VPNR.Node
{
    public class NodeService
    {
        private readonly DbContext _dbContext;
        private readonly NodeSettings _nodeSettings;
        private readonly NodeSshSettings _sshSettings;
        private readonly PrivateKeyFile _privateKeyFile;

        public NodeService(IConfiguration configuration, DbContext dbContext)
        {
            _dbContext = dbContext;
            _nodeSettings = new NodeSettings();
            _sshSettings = new NodeSshSettings();
            configuration.Bind("Node", _nodeSettings);
            configuration.Bind("Ssh", _sshSettings);

            var test = @"
-----BEGIN OPENSSH PRIVATE KEY-----
b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW
QyNTUxOQAAACCzfQrKvfT/MTg2MLQctsItOShhVtFhBM5s5N6ZxzUfoAAAAJC8yDr0vMg6
9AAAAAtzc2gtZWQyNTUxOQAAACCzfQrKvfT/MTg2MLQctsItOShhVtFhBM5s5N6ZxzUfoA
AAAECeSxqg6hV9L5eeAo40w+qocNfZZLsZPoqBWK1hvdIPCbN9Csq99P8xODYwtBy2wi05
KGFW0WEEzmzk3pnHNR+gAAAACnRoZW5pQE1haW4BAgM=
-----END OPENSSH PRIVATE KEY-----
";
            
            _privateKeyFile = new PrivateKeyFile(new MemoryStream(Encoding.UTF8.GetBytes(test)));
        }
    }
}