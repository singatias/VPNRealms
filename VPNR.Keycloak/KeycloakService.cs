using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace VPNR.Keycloak
{
    public class KeycloakService
    {
        private readonly KeycloakSettings _settings;

        public KeycloakService(IConfiguration configuration)
        {
            _settings = new KeycloakSettings();
            configuration.Bind("Keycloak", _settings);
        }

        public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            var client = new HttpClient();
            var response = await client.RequestTokenAsync(new TokenRequest
            {
                Address = _settings.TokenEndpoint,
                GrantType = "client_credentials",
                ClientId = _settings.ClientId,
                ClientSecret = _settings.ClientSecret,
            }, cancellationToken: cancellationToken);

            return response.AccessToken;
        }

        public async Task<KeycloakUser> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var httpClient = new HttpClient();
            var url = $"{this._settings.Endpoint}/users?email={email}";
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", await GetTokenAsync(cancellationToken));
            var response = await httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var ret = JsonConvert.DeserializeObject<List<KeycloakUser>>(json);
            return ret.FirstOrDefault();
        }

        public async Task<KeycloakUser> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", await GetTokenAsync(cancellationToken));
            var url = $"{this._settings.Endpoint}/users/{id}";
            var response = await httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var ret = JsonConvert.DeserializeObject<KeycloakUser>(json);
            return ret;
        }

        public async Task SendChangePasswordEmailAsync(string id, CancellationToken cancellationToken = default)
        {
            //PUT /{realm}/users/{id}/execute-actions-email
            var url = $"{this._settings.Endpoint}/users/{id}/execute-actions-email";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", await GetTokenAsync(cancellationToken));
            var putJson = JsonConvert.SerializeObject(new string[] {
                "UPDATE_PASSWORD"
            });
            var response = await httpClient.PutAsync(url, new StringContent(putJson, Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<KeycloakUser>> GetUsersAsync(string search = null, int max = 100, CancellationToken cancellationToken = default)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", await GetTokenAsync(cancellationToken));
            var url = $"{this._settings.Endpoint}/users?max={max}";
            if (!string.IsNullOrWhiteSpace(search))
                url += $"&search={search}";

            var response = await httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var ret = JsonConvert.DeserializeObject<List<KeycloakUser>>(json);
            return ret;
        }

        public async Task ChangePasswordAsync(string id, string newPassword, bool temporary, CancellationToken cancellationToken = default)
        {
            ///auth/admin/realms/{realm}/users/{id}/reset-password
            /////{ "type": "password", "temporary": false, "value": "my-new-password" }
            var url = $"{this._settings.Endpoint}/users/{id}/reset-password";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", await GetTokenAsync(cancellationToken));
            var putJson = JsonConvert.SerializeObject(new {
                type = "password",
                temporary,
                value = newPassword
            });
            var response = await httpClient.PutAsync(url, new StringContent(putJson, Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();
        }



        public async Task UpdateUserByIdAsync(string id, string email, string firstName, string lastName, bool enabled, CancellationToken cancellationToken = default)
        {
            var user = await GetUserByIdAsync(id, cancellationToken);
            if (user == null)
                throw new Exception($"no user {email} on the realm");

            user.Email = email;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.Username = email;
            user.Enabled = enabled;

            var url = $"{_settings.Endpoint}/users/{id}";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", await GetTokenAsync(cancellationToken));
            var putJson = JsonConvert.SerializeObject(user);
            var response = await httpClient.PutAsync(url, new StringContent(putJson, Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return await this.GetUserByEmailAsync(email, cancellationToken) != null;
        }

        public async Task<KeycloakUser> CreateOrUpdateAsync(string email, string firstName, string lastName, bool enabled, CancellationToken cancellationToken = default)
        {
            var existingUser = await GetUserByEmailAsync(email, cancellationToken);
            if (existingUser != null)
            {
                await UpdateUserByIdAsync(existingUser.Id, email, firstName, lastName, enabled, cancellationToken);
                return await GetUserByEmailAsync(email, cancellationToken);
            }
            
            return await CreateUserAsync(email, firstName, lastName, enabled, cancellationToken);
        }

        public async Task<KeycloakUser> CreateUserAsync(string email, string firstName, string lastName, bool enabled, CancellationToken cancellationToken = default)
        {
            long epochTicks = new DateTime(1970, 1, 1).Ticks;
            long unixTime = ((DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond);

            var user = new KeycloakUser()
            {
                CreatedTimestamp = unixTime,
                Username = email,
                FirstName = firstName,
                LastName = lastName,
                Enabled = enabled,
                Totp = false,
                EmailVerified = false,
                RequiredActions = new List<String>(),
                Attributes = null,
                Email = email,
                NotBefore = 0,
                Access = new Dictionary<string, object>
                {
                    { "manageGroupMembership", true },
                    { "view", true },
                    { "mapRoles", true  },
                    { "impersonate", true },
                    { "manage", true }
                }
            };

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", await GetTokenAsync(cancellationToken));
            var url = $"{this._settings.Endpoint}/users";
            var postJson = JsonConvert.SerializeObject(user);
            var response = await httpClient.PostAsync(url, new StringContent(postJson, Encoding.UTF8, "application/json"), cancellationToken);
            response.EnsureSuccessStatusCode();
            var ret = await GetUserByEmailAsync(email, cancellationToken);
            return ret;
        }
    }
}