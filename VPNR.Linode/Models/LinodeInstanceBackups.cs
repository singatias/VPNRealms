using System;

namespace VPNR.Linode.Models
{
    public class LinodeInstanceBackups
    {
        public bool Available { get; set; }
        public bool Enabled { get; set; }
        public DateTime LastSuccessful { get; set; }
        public LinodeInstanceBackupSchedule Schedule { get; set; }
    }
}