using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RestSharp;
using VPNR.Linode.Models;

namespace VPNR.Linode
{
    public class LinodeService
    {
        private readonly LinodeSettings _settings;
        private readonly RestClient _client;
        
        public LinodeService(IConfiguration configuration)
        {
            _settings = new LinodeSettings();
            configuration.Bind("Linode", _settings);
            _client = new RestClient(_settings.Endpoint);
        }

        public async Task ConfigureLinodeNode(int id, CancellationToken cancellationToken = default)
        {
            
        }
        
        public async Task<List<LinodeInstance>> ListLinodeInstancesAsync(CancellationToken cancellationToken = default)
        {
            var request = CreateAuthorizedRequest("/linode/instances", Method.Get);

            var response = await _client.ExecuteAsync(request, cancellationToken);
            
            var responseModel = Newtonsoft.Json.JsonConvert.DeserializeObject<LinodePaginationResponse<LinodeInstance>>(response.Content);
            return responseModel.Data;
        }
        
        public async Task<LinodeInstance> GetLinodeInstanceAsync(int id, CancellationToken cancellationToken = default)
        {
            var request = CreateAuthorizedRequest($"/linode/instances/{id}", Method.Get);

            var response = await _client.ExecuteAsync(request, cancellationToken);
            
            var responseModel = Newtonsoft.Json.JsonConvert.DeserializeObject<LinodeInstance>(response.Content);
            return responseModel;
        }

        public async Task<LinodeInstance> CreateLinodeInstanceAsync(CreateLinodeInstanceModel model, CancellationToken cancellationToken = default)
        {
            var request = CreateAuthorizedRequest("/linode/instances", Method.Post);

            var serializeObject = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            request.AddJsonBody(serializeObject);
            
            var response = await _client.ExecuteAsync(request, cancellationToken);
            
            var responseModel = Newtonsoft.Json.JsonConvert.DeserializeObject<LinodeInstance>(response.Content);
            return responseModel;
        }

        public async Task<bool> DeleteLinodeInstance(int id, CancellationToken cancellationToken = default)
        {
            var request = CreateAuthorizedRequest($"/linode/instances/{id}", Method.Delete);
            var response = await _client.ExecuteAsync(request, cancellationToken);
            return response.IsSuccessStatusCode;
        }

        private RestRequest CreateAuthorizedRequest(string resource, Method method = Method.Post)
        {
            RestRequest request = new RestRequest(resource, method);
            request.AddHeader("Authorization", $"Bearer {_settings.Token}");
            return request;
        }
    }
}