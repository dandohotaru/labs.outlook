using Demo.App.Shared.Extensions;
using Demo.App.Shared.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Demo.App.Chats
{
    public class OpenaiChatService : IChatService
    {
        public OpenaiChatService(ISettingsService settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
        }

        private ISettingsService Settings { get; set; }

        public async Task<string> Send(IEnumerable<ChatMessage> messages)
        {
            try
            {
                var endpoint = Settings.GetValue<string>("Openai:Endpoints:Completion");
                var token = Settings.GetValue<string>("Openai:Key");
                var model = Settings.GetValue<string>("Openai:Model");
                var temperature = Settings.GetValue<double>("Openai:Temperature");
                var max = Settings.GetValue<int>("Openai:Max");

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var requestModel = new
                {
                    messages = messages
                        .Select(p => new
                        {
                            role = p.Role,
                            content = p.Content
                        })
                        .ToArray(),
                    model = model,
                    temperature = temperature,
                    max_completion_tokens = max,
                    top_p = 1,
                    frequency_penalty = 0,
                    presence_penalty = 0,
                    response_format = new
                    {
                        type = "text"
                    },
                };

                var requestContent = JsonSerializer.Serialize(requestModel);
                var request = new StringContent(requestContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(endpoint, request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(responseContent);

                return document.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error: {exception.ToMessage()}");

                var message = new StringBuilder()
                    .AppendLine("Error generating reply. ")
                    .AppendLine(exception.ToMessage())
                    .ToString();

                return message;
            }
        }
    }
}
