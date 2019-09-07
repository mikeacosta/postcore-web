using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Postcore.AdApi.Shared.Models;
using Postcore.Web.Core.ApiModels;
using Postcore.Web.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Postcore.Web.Infrastructure.ApiClients
{
    public class AdApiClient : IAdApiClient
    {
        private readonly string _baseUrl;
        private readonly HttpClient _client;
        private readonly IMapper _mapper;

        public AdApiClient(IConfiguration configuration, HttpClient client, IMapper mapper)
        {
            _client = client;
            _baseUrl = configuration.GetSection("AdApi").GetValue<string>("BaseUrl");
            _mapper = mapper;
        }

        public async Task<List<Ad>> GetAllAsync()
        {
            var response = await _client.GetAsync(new Uri($"{_baseUrl}/all")).ConfigureAwait(false);
            var ads = await response.Content.ReadAsAsync<List<AdDto>>().ConfigureAwait(false);
            return ads.Select(a => _mapper.ToAd(a)).ToList();
        }

        public async Task<CreateResponse> CreateAsync(Ad ad)
        {
            var dto = _mapper.ToAdDto(ad);

            var json = JsonConvert.SerializeObject(dto);
            var response = await _client.PostAsync(new Uri($"{_baseUrl}/create"), 
                new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var createAdResponse = JsonConvert.DeserializeObject<Postcore.AdApi.Shared.Models.CreateAdResponseDto>(jsonResponse);

            return _mapper.ToCreateResponse(createAdResponse);
        }

        public async Task<bool> ConfirmAsync(ConfirmRequest request)
        {
            var dto = _mapper.ToConfirmAdDto(request);
            var json = JsonConvert.SerializeObject(dto);
            var response = await _client.PutAsync(new Uri($"{_baseUrl}/confirm"),
                    new StringContent(json, Encoding.UTF8, "application/json")) .ConfigureAwait(false);

            return response.StatusCode == HttpStatusCode.OK;

        }
    }
}
