using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WatchCat.Core;

namespace WatchCat.Connectors.Slack
{
    public interface ISlackWebHookClient
    {
        Task PublishMessageAsync(Notification notification);
    }

    public class SlackWebHookClient : ISlackWebHookClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _webHook;

        public SlackWebHookClient(HttpClient httpClient, string webHook)
        {
            _httpClient = httpClient;
            _webHook = webHook;
        }

        public async Task PublishMessageAsync(Notification notification)
        {
            var formattedNotification = FormattedNotification.Create(notification);
            string json = JsonConvert.SerializeObject(formattedNotification);
            
            var response = await _httpClient.PostAsync(_webHook, new StringContent(json, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var ex = new Exception("Publish failed");
                ex.Data.Add("response-http-status-code", (int)response.StatusCode);

                if (response.Content.Headers.ContentLength > 0)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    ex.Data.Add("response-content", content);
                }

                throw ex;
            }
        }

        private class FormattedNotification
        {
            [JsonProperty("text")]
            public string Text { get; private set; }

            //[JsonProperty("attachments")]
            //public string Attachments { get; private set; }

            private FormattedNotification() { }

            public static FormattedNotification Create(Notification notification)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"_[{notification.CreatedAtUtc}]_ <!channel> there are (*{notification.Items.Count}*) new notifications:");
                const int maxlen = 1000;
                notification.Items.ForEach(item =>
                {
                    if (sb.Length < maxlen)
                    {
                        var maxLength = Math.Min(item.Content.Length, maxlen - sb.Length + 1);
                        sb.AppendLine($"- `{item.Content.Substring(0, maxLength)}{(maxLength != item.Content.Length ? "..." : "")}`");
                    }
                });

                return new FormattedNotification
                {
                    Text = sb.ToString(),
                };
            }
        }
    }
}
