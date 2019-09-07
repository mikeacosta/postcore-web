using Microsoft.Extensions.Configuration;
using Postcore.Web.Core.ApiModels;
using Postcore.Web.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Postcore.Web.Infrastructure.ApiClients
{
    public class SearchApiClient : ISearchApiClient
    {
        private readonly string _url;
        private readonly HttpClient _client;

        public SearchApiClient(IConfiguration configuration, HttpClient client)
        {
            _url = configuration.GetSection("SearchApi").GetValue<string>("Url");
            _client = client;
        }

        public async Task<List<Ad>> Search(string keyword)
        {
            var result = new List<Ad>();
            var searchUrl = $"{_url}?q={keyword}";
            var httpResponse = await _client.GetAsync(new Uri(searchUrl)).ConfigureAwait(false);

            if (httpResponse.StatusCode != HttpStatusCode.OK)
                return result;

            var json = httpResponse.Content.ReadAsStringAsync();

            var ads = await httpResponse.Content.ReadAsAsync<List<Ad>>().ConfigureAwait(false);
            result.AddRange(ads);

            return result;
        }
    }
}
