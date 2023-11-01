using System.Collections.Generic;

namespace VPNR.Linode.Models
{
    public class LinodePaginationResponse<T>
    {
        public List<T> Data { get; set; }
        public int Page { get; set; }
        public int Pages { get; set; }
        public int Results { get; set; }
    }
}