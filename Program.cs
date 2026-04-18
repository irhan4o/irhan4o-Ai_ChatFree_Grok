using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    // ТУК ВЕЧЕ НЯМА КЛЮЧ! Програмата ще го вземе от компютъра ти.
    private static readonly string apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");
    private static readonly string url = "https://api.groq.com/openai/v1/chat/completions";

    static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        // Проверка дали ключът е настроен
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("Грешка: Не е намерен API ключ в Environment Variables!");
            Console.WriteLine("Моля, добавете променлива 'GROQ_API_KEY' с вашия ключ.");
            return;
        }

        Console.WriteLine("=== AI Console Chat (Groq + Llama 3.1) ===");

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\nТи: ");
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) break;

            try
            {
                var requestBody = new
                {
                    model = "llama-3.1-8b-instant",
                    messages = new[] { new { role = "user", content = input } }
                };

                string jsonPayload = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using JsonDocument doc = JsonDocument.Parse(responseString);
                    var aiMessage = doc.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"\nAI: {aiMessage}");
                }
                else
                {
                    Console.WriteLine($"\nГрешка: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nГрешка: {ex.Message}");
            }
        }
    }
}