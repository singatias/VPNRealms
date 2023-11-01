namespace VPNR.Keycloak
{
    public class KeycloakSettings
    {
        public string Endpoint { get; set; }
        public string TokenEndpoint { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}