using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WatchCat.Connectors.Slack
{
    public class SlackConnector
    {
        private readonly HttpClient _httpClient;

        public SlackConnector(HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                AllowAutoRedirect = false,
            });
        }

        public ISlackWebHookClient CreateWebHookClient(string webHook) => new SlackWebHookClient(_httpClient, webHook);
    }
}
