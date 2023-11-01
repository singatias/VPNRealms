using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Renci.SshNet;
using VPNR.Dal;
using VPNR.Dal.Entities.Nodes;

namespace VPNR.Node.Wireguard
{
    public class WireguardNodeService
    {
        private readonly DbContext _dbContext;
        private readonly NodeSettings _nodeSettings;
        private readonly NodeSshSettings _sshSettings;
        private readonly PrivateKeyFile _privateKeyFile;

        public WireguardNodeService(IConfiguration configuration, DbContext dbContext)
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
        
        public async Task CreateInstallAsync(string instanceId, string[] dns, string? clientPublicKey = null, string serverAddress = "10.0.0.1", int port = 51820, CancellationToken cancellationToken = default)
        {
            var instance = await _dbContext.ProviderInstances.GetAsync(instanceId, cancellationToken);
            
            var client = new SshClient(instance.Ipv4, _sshSettings.User, _sshSettings.Password);
            var sftpClient = new SftpClient(instance.Ipv4, _sshSettings.User, _sshSettings.Password);
            
            WireguardNode? wireguardNode = null;
            try
            {
                client.Connect();
                sftpClient.Connect();
                
                // delete all files if they exists since we we will be overriding them
                try
                {
                    sftpClient.DeleteFile("/etc/sysctl.conf");
                    sftpClient.DeleteFile("/etc/wireguard/wg0.conf");
                    sftpClient.DeleteFile("/etc/init.d/wg-quick-wghome");
                }
                catch (Exception e)
                {
                }
                
                // make sure ip forwarding is persisted
                sftpClient.WriteAllText("/etc/sysctl.conf", "net.ipv4.ip_forward=1");

                SshCommand sshCommand = null;

                // set ip forwarding
                client.RunCommand("sysctl -w net.ipv4.ip_forward=1");

                // install wireguard
                client.RunCommand("apk add -U wireguard-tools");

                // generate wireguard server keys
                client.RunCommand("wg genkey | tee privatekey | wg pubkey > publickey");
                sshCommand = client.RunCommand("cat publickey");
                var serverPublicKey = sshCommand.Result;

                sshCommand = client.RunCommand("cat privatekey");
                var serverPrivateKey = sshCommand.Result;

                string? clientPrivateKey = null;
                // generate wireguard client keys
                if (String.IsNullOrWhiteSpace(clientPublicKey))
                {
                    client.RunCommand("wg genkey | tee clientprivatekey | wg pubkey > clientpublickey");
                    sshCommand = client.RunCommand("cat clientpublickey");
                    clientPublicKey = sshCommand.Result;

                    sshCommand = client.RunCommand("cat clientprivatekey");
                    clientPrivateKey = sshCommand.Result;
                }

                var lastDigit = Int16.Parse(serverAddress.Split('.').Last());
                var clientAddress = $"{serverAddress.Remove(serverAddress.Length - 1)}{lastDigit+1}";

                var clients = new List<WireguardClient>()
                {
                    new WireguardClient()
                    {
                        PublicKey = clientPublicKey,
                        PrivateKey = clientPrivateKey,
                        Address = clientAddress,
                        Dns = dns.ToList()
                    }
                };

                var serverConfigFile = GetServerConfigurationFile(serverAddress, port, serverPrivateKey,
                    clients);
                
                sftpClient.WriteAllText("/etc/wireguard/wg0.conf", serverConfigFile);

                // added init script and make it execute on startup
                var wgInitScriptFile = @"#!/sbin/openrc-run
description=""WireGuard up/down script""
                    
depend() {
    need localmount
    need net
}

start() {
    wg-quick up wg0
}

stop() {
    wg-quick down wg0
}";
                
                sftpClient.WriteAllText("/etc/init.d/wg-quick", wgInitScriptFile);
                
                // make sure to convert dos 2 unix
                client.RunCommand("dos2unix /etc/init.d/wg-quick");
                
                client.RunCommand("chmod a+x /etc/init.d/wg-quick");
                client.RunCommand("rc-update add wg-quick default");
                
                // start the service
                client.RunCommand("/etc/init.d/wg-quick start");
                
                await _dbContext.WireguardNodes.AddAsync(new WireguardNode()
                {
                    InstanceId = instanceId,
                    ServerAddress = serverAddress,
                    Port = port,
                    ServerPublicKey = serverPublicKey,
                    Clients = clients,
                    CreatedAt = DateTime.Now
                }, cancellationToken);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                if (client.IsConnected)
                    client.Disconnect();
                
                if (sftpClient.IsConnected)
                    sftpClient.Disconnect();
            }
        }
        
        public async Task<Stream> GetClientConfigurationFileAsync(string id, int index = 0, CancellationToken cancellationToken = default)
        {
            var wireguardNode = await _dbContext.WireguardNodes.GetAsync(id, cancellationToken);
            var instance = await _dbContext.ProviderInstances.GetAsync(wireguardNode.InstanceId, cancellationToken);

            var client = wireguardNode.Clients.ElementAt(index);
            
            var fileContent = $@"
[Interface]
PrivateKey = {client.PrivateKey}
Address = {_nodeSettings.DefaultClientAddress}/8
DNS = {_nodeSettings.DefaultDns}

[Peer]
PublicKey = {wireguardNode.ServerPublicKey}
AllowedIPs = 0.0.0.0/0
Endpoint = {instance.Ipv4}:{_nodeSettings.DefaultPort}
PersistentKeepalive = 30";

            return new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        }
        
        public static string GetServerConfigurationFile(string address, int port, string serverPrivateKey, List<WireguardClient> clients)
        {
            var fileContent = $@"
[Interface]
Address={address}/8
SaveConfig=true
PostUp=iptables -A FORWARD -i wg0 -j ACCEPT; iptables -t nat -A POSTROUTING -o eth0 -j MASQUERADE;
PostDown=iptables -D FORWARD -i wg0 -j ACCEPT; iptables -t nat -D POSTROUTING -o eth0 -j MASQUERADE;
ListenPort={port}
PrivateKey={serverPrivateKey}
";

            foreach (var client in clients)
            {
                fileContent += $@"
[Peer]
PublicKey={client.PublicKey}
AllowedIPs={client.Address}/32";
            }

            return fileContent;
        }
    }
}