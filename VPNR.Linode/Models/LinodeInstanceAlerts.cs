namespace VPNR.Linode.Models
{
    public class LinodeInstanceAlerts
    {
        public int Cpu { get; set; }
        public int Io { get; set; }
        public int NetworkIn { get; set; }
        public int NetworkOut { get; set; }
        public int TransferQuota { get; set; }
    }
}