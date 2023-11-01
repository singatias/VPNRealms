namespace VPNR.Node
{
    public class NodeSshSettings
    {
        public string Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
    
    public class NodeSettings
    {
        public string DefaultServerAddress { get; set; }
        public string DefaultClientAddress { get; set; }
        public string DefaultPort { get; set; }
        public string DefaultDns { get; set; }
    }
}