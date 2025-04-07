/*
    ChatGPT C# Wrapper
    Kat Matheson 
*/

#load "./chatgpt-data.csx"
#load "./chatgpt-parser.csx"

using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

string ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
string ResponseFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "chatgpt", "responses");

if (Args.Count < 1)
{
    Console.WriteLine("Usage: gpt --q \"prompt\" --max-tokens MaxTokens --f \"filename\"");
    return;
}

if(GPTArgumentParser.Instance.TryParse(Args))
{
    await AskChatGpt();
}

// await AskChatGpt(Args.Aggregate((string s1, string s2)  ));

async Task AskChatGpt()
{
    try
    {
        GPTArgumentParser args = GPTArgumentParser.Instance;

        using HttpClient client = new();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
        
        var requestBody = new
        {
            model = args.Model,
            messages = new[] { new { role = "user", content = args.UserMessage } },
            max_tokens = args.MaxTokens
        };

        string jsonRequest = JsonSerializer.Serialize(requestBody);
        HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
    
        HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
        response.EnsureSuccessStatusCode();
    
        string jsonResponse = await response.Content.ReadAsStringAsync();
        var gptResponse = JsonSerializer.Deserialize<ResponseWrapper>(jsonResponse);

        if (gptResponse.Choices == null || gptResponse.Choices.Count == 0)
        {
            Console.WriteLine("ChatGPT sent no response.");
            return;
        }
    
        List<string> messages = gptResponse.Choices.Select(
            choice => string.Concat($"({choice.Index}/{gptResponse.Choices.Count})",
                                    choice.Response.Message, Environment.NewLine)).ToList();
        PrintMessages(messages);
        WriteMessagesToFile(args.UserMessage, messages, string.IsNullOrEmpty(args.Filename) ? GetFilename() : args.Filename);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        Console.WriteLine(GPTArgumentParser.Instance.ToString());
    }
}

void PrintMessages(IEnumerable<string> messages)
{
    foreach (string message in messages)
    {
        Console.WriteLine(message);
    }
}

void WriteMessagesToFile(string prompt, IEnumerable<string> messages, string filename)
{
    string writePath = Path.Combine(ResponseFolder, filename);
    List<string> output = new() { $"===Prompt===\n{prompt}\n\n===Response===" };
    output.AddRange(messages);

    if(!File.Exists(writePath))
    {
        File.WriteAllLines(writePath, output);
    }    
    else
    {
        File.AppendAllLines(writePath, output);
    }

    File.AppendAllText(writePath, Environment.NewLine);
}

string GetFilename() => DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";