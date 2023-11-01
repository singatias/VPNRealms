using System.Collections.Generic;

namespace VPNR.Dal.Entities.Nodes
{
    public class WireguardClient
    {
        public string PublicKey { get; set; }
        public string? PrivateKey { get; set; }
        public string Address { get; set; }    
        public List<string> Dns { get; set; }
    }
    
    public class WireguardNode : Node
    {
        public int Port { get; set; }
        public string ServerPublicKey { get; set; }
        public string ServerAddress { get; set; }
        public List<WireguardClient> Clients { get; set; }
    }
}