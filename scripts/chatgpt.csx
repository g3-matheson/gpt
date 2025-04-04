/*
    Usage:
        Call from CLI with:
        
            gpt [prompt] [filename] [-d]
        
        filename is optional: will default to {current datetime}.txt
        -d is for debug: prints GPT response .json
*/

#load "./chatgpt-data.csx"

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
    Console.WriteLine("Usage: gpt 'prompt' 'filename(opt)'");
    return;
}

string prompt = Args[0];
string filename = Args.Count > 1 ? Args[1] : string.Empty;
string debugFlag = Args.Count > 2 ? Args[2] : string.Empty;

await AskChatGpt(prompt, filename, debugFlag);

async Task AskChatGpt(string prompt, string filename, string debugFlag)
{
    try
    {
        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
        
        var requestBody = new
        {
            model = "gpt-4o",
            messages = new[] { new { role = "user", content = prompt } },
            max_tokens = 500
        };

        string jsonRequest = JsonSerializer.Serialize(requestBody);
        HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
    
        HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
        response.EnsureSuccessStatusCode();
    
        string jsonResponse = await response.Content.ReadAsStringAsync();
        var gptResponse = JsonSerializer.Deserialize<ResponseWrapper>(jsonResponse);
    
        if (debugFlag == "-d")
        {
            Console.WriteLine(jsonResponse);
        }

        if (gptResponse.Choices == null || gptResponse.Choices.Count == 0)
        {
            Console.WriteLine("ChatGPT sent no response.");
            return;
        }
    
        List<string> messages = gptResponse.Choices.Select(choice => choice.Response.Message).ToList();
        PrintMessages(messages);
        WriteMessagesToFile(prompt, messages, string.IsNullOrEmpty(filename) ? GetFilename() : filename);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

List<string> ExtractMessages(ResponseWrapper response) 
{
    List<string> messages = new();
    foreach (ResponseChoice choice in response.Choices)
    {
        messages.Append($"({choice.Index}): {choice.Response.Message}");
    }
    return messages;
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