/*
    ChatGPT C# Wrapper
    Kat Matheson 
*/

#load "./chatgpt-parser.csx"
#load "./chatgpt-io.csx"

using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

readonly string ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
readonly string ApiURL = "https://api.openai.com/v1/chat/completions";


if (Args.Count < 1)
{
    Console.WriteLine("Usage: gpt --q \"prompt\" --max-tokens MaxTokens --f \"filename\"");
    return;
}

if(GPTArgumentParser.Instance.TryParse(Args))
{
    if (Args.Contains("-d")) Console.WriteLine($"{GPTArgumentParser.Instance.ToString()}\n");
    await AskChatGpt(Args);
}

async Task AskChatGpt(IList<string> cliArgs)
{
    try
    {
        GPTArgumentParser args = GPTArgumentParser.Instance; 

        using HttpClient client = new();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

        List<GPTMessage> conversation = new();

        if (args.ContinueChatFromFile && !string.IsNullOrEmpty(args.Filename))
        {
            GPTJson previousJson = LoadFile(args.Filename); 
            if (previousJson != null)
            {
                conversation.AddRange(previousJson.Messages);
            }
        }

        if (!string.IsNullOrEmpty(args.SystemMessage))
        {
            // avoid duplicate system messages, keep it at the top
            conversation.Remove(new GPTMessage("system", string.Empty));
            conversation.Insert(0, new GPTMessage("system", args.SystemMessage));
        }

        conversation.Add(new GPTMessage("user", args.UserMessage));

        var requestBody = new
        {
            model = args.Model,
            messages = conversation,
            max_tokens = args.MaxTokens
        }; 

        string jsonRequest = JsonSerializer.Serialize(requestBody);
        if (args.Debug) { Console.WriteLine(jsonRequest); }
        HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
    
        HttpResponseMessage response = await client.PostAsync(ApiURL, content);
        response.EnsureSuccessStatusCode();
    
        string jsonResponse = await response.Content.ReadAsStringAsync();
        var gptResponse = JsonSerializer.Deserialize<ResponseWrapper>(jsonResponse);

        if (gptResponse.Choices == null || gptResponse.Choices.Count == 0)
        {
            Console.WriteLine("ChatGPT sent no response.");
            return;
        }
    
        if (args.TokensUsed) Console.WriteLine($"Tokens used: {gptResponse.TokenUsage.CompletionTokens}.\n");

        PrintResponses(gptResponse.Choices);

        string responseAggregate = string.Join(Environment.NewLine, gptResponse.Choices.Select(c => c.Response.Message));

        GPTMessage gptm = new("assistant", responseAggregate)
        {
            TokensOut = gptResponse.TokenUsage.CompletionTokens
        };
        conversation.Add(gptm);

        SaveFile(new GPTJson(conversation), args.Filename ?? GetFilename());
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void PrintResponses(List<ResponseChoice> choices)
{

    int nChoices = choices.Count;
    List<string> responses = choices.Select(
        choice => string.Concat(nChoices> 1? $"({choice.Index}/{nChoices})" : "",
                                choice.Response.Message, Environment.NewLine)).ToList();

    foreach (string response in responses)
    {
        Console.WriteLine(response);
    }
}


string GetFilename() => DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");