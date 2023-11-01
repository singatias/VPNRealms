using System;
using Newtonsoft.Json;

namespace VPNR.Linode.Models
{
    public class LinodeInstance
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string Group { get; set; }
        public string HostUuid { get; set; }
        public string Hypervisor { get; set; }
        public string[] Ipv4 { get; set; }
        public string Ipv6 { get; set; }
        public string Label { get; set; }
        public string Region { get; set; }

        public LinodeInstanceAlerts Alerts { get; set; }
        
        [JsonProperty("specs")]
        public LinodeInstanceSpecificationModel Specifications { get; set; }

        public string Status { get; set; }
        public string[] Tags { get; set; }
        public string Type { get; set; }
        public DateTime Updated { get; set; }
        public bool WatchdogEnabled { get; set; }        
        
    }
}