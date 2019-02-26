using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WatchCat.Core.Events
{
    public class EventsExtractor
    {
        private readonly static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };

        public List<string> NamePropertyKeys { get; } = new List<string>
        {
            "event-name", "x-event-name",
            "name", "x-name",
            "e-name", "x-e-name",
            "ev-name", "x-ev-name", 
        };

        public async Task<EventEntry> FromHttpRequest(HttpRequest req)
        {
            var name = TryGetName(req);
            var content = await TryGetContentAsync(req);

            return new EventEntry(content, name, apiVersion: 1);
        }

        private string TryGetName(HttpRequest req)
        {
            string name = null;

            if (NamePropertyKeys.Any())
            {
                name = NamePropertyKeys.Select(k => TryGetQueryStringValue(k))
                       .Union(NamePropertyKeys.Select(k => TryGetHeaderValue(k)))
                       .Where(n => !string.IsNullOrWhiteSpace(n))
                       .FirstOrDefault();
            }

            return name;

            // Local functions

            string TryGetQueryStringValue(string key)
            {
                return req.Query != null && req.Query.Count > 0
                       ? req.Query.FirstOrDefault(q => q.Key.ToLowerInvariant() == key.ToLowerInvariant()).Value.ToString()
                       : null;
            }

            string TryGetHeaderValue(string key)
            {
                return req.Headers != null && req.Headers.Count > 0
                    ? req.Headers.FirstOrDefault(h => h.Key.ToLowerInvariant() == key.ToLowerInvariant()).Value.ToString()
                    : null;
            }
        }

        private async Task<object> TryGetContentAsync(HttpRequest req)
        {
            object content = null;

            if (req.ContentLength.GetValueOrDefault() > 0)
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                content = JsonConvert.DeserializeObject(requestBody, _jsonSettings);
            }

            return content;
        }
    }
}
