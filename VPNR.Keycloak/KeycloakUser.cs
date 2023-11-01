using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VPNR.Keycloak
{
    public class KeycloakUser
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("createdTimestamp")]
        public long CreatedTimestamp { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        [JsonProperty("totp")]
        public bool Totp { get; set; }
        [JsonProperty("emailVerified")]
        public bool EmailVerified { get; set; }
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("notBefore")]
        public int NotBefore { get; set; }
        [JsonProperty("requiredActions")]
        public List<string> RequiredActions { get; set; }
        [JsonProperty("attributes")]
        public JObject Attributes { get; set; }
        [JsonProperty("access")]
        public Dictionary<string, object> Access { get; set; }
    }
}