using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace VPNR.Linode.Models
{
    public class CreateLinodeInstanceModel
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("image")]
        public string Image { get; set; }
        
        [JsonProperty("label")]
        public string Label { get; set; }
        
        [JsonProperty("region")]
        public string Region { get; set; }
        
        [JsonProperty("group")]
        public string? Group { get; set; }
        
        [JsonProperty("root_pass")]
        public string RootPass { get; set; }
        
        [JsonProperty("authorized_users")]
        public string[] AuthorizedUsers { get; set; }
        
        [JsonProperty("authorized_keys")]
        public string[] AuthorizedKeys { get; set; }
        
        [JsonProperty("tags")]
        public string[] Tags { get; set; }
        
        [JsonProperty("booted")]
        public bool Booted { get; set; }
        
        [JsonProperty("private_ip")]
        public bool PrivateIp { get; set; }
        
        [JsonProperty("backups_enabled")]
        public bool BackupsEnabled { get; set; }
    }
}