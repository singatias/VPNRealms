namespace VPNR.Linode.Models
{
    public class LinodeInstanceSpecificationModel
    {
        public int Disk { get; set; }
        public int Memory { get; set; }
        public int Transfer { get; set; }
        public int vcpus { get; set; }
    }
}