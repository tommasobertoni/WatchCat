using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EventsSource
{
    class Program
    {
        private const string TargetUrl = "http://localhost:7071/api/Gate";

        static async Task Main(string[] args)
        {
            //var bursts = 4;
            //var delays = new[] { 2000, 500, 100, 0 };
            //var messagesPerBurst = new[] { 101 };

            var bursts = 2;
            var delays = new[] { 10000 };
            var messagesPerBurst = new[] { 101, 500 };

            await ProduceEvents(bursts, delays, messagesPerBurst);

            Console.WriteLine("Completed");
        }

        private static async Task ProduceEvents(int bursts, int[] delays, int[] messagesPerBurst)
        {
            var httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                AllowAutoRedirect = false,
            });

            for (int b = 0; b < bursts; b++)
            {
                var messagesToSend = GetMessagesToSend(b);
                Console.WriteLine($"Burst {b + 1}: sending {messagesToSend} messages.");

                if (messagesToSend > 0)
                {
                    var messagesTasks = new List<Task>();

                    for (int i = 0; i < messagesToSend; i++)
                    {
                        var json = JsonConvert.SerializeObject(new { message = $"Hello {b}:{i}" });
                        messagesTasks.Add(SendJsonAsync(httpClient, json));
                    }

                    await Task.WhenAll(messagesTasks);
                }

                int delay = GetDelay(b);
                await Task.Delay(delay);
            }

            int GetMessagesToSend(int b)
            {
                return b < messagesPerBurst.Length
                    ? messagesPerBurst[b] : messagesPerBurst[messagesPerBurst.Length - 1];
            }

            int GetDelay(int b)
            {
                return b < delays.Length
                    ? delays[b] : delays[delays.Length - 1];
            }
        }

        private static async Task SendJsonAsync(HttpClient httpClient, string json)
        {
            try
            {
                await httpClient.PostAsync(TargetUrl, new StringContent(json, Encoding.UTF8, "application/json"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Task.Delay(100);
                await SendJsonAsync(httpClient, json);
            }
        }
    }
}
