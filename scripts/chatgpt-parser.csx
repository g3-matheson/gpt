/*
    CLI Parsing for chatgpt.csx
    Arguments:
        * all strings (?) TODO: check if you can pass int thru bash
        * [required] (optional) {required with flag}

    
*/

using System.Collections.Generic;

public abstract class IArgumentParser
{
    public bool TryParse(IList<string> args)
    {
        try
        { 
            for(int i = 0; i < args.Count; i++)
            {
                // flags with arguments: --[flag] [argument]
                if (args[i].Length > 1 && args[i][..2] == "--")
                {
                    Flags.TryGetValue(args[i], out var flagCommand);
                    flagCommand?.Invoke(args[++i]);
                }

                // flags with no arguments: -[flag]
                else if (args[i].Length > 0 && args[i][..1] == "-")
                {
                    Flags.TryGetValue(args[i], out var flagCommand);
                    flagCommand?.Invoke(string.Empty);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"IArgumentParser.TryParse() Error: {e.Message}");
            return false;
        }

        return true;
    }

    protected abstract Dictionary<string, Action<string>> Flags { get; set; } 
}

public class GPTArgumentParser : IArgumentParser
{ 
    private static GPTArgumentParser _instance;

    private GPTArgumentParser() { 
        Flags = new()
        {
            { "-d",            (string s) => { Debug = true; }},
            { "--q",            (string s) => { UserMessage = s; }},
            { "--s",            (string s) => { SystemMessage = s; }},
            { "--system",       (string s) => { SystemMessage = s; }},
            { "--f",            (string s) => { Filename = s; }},
            { "--max-tokens",   SetMaxTokens },  
            { "--mt",           SetMaxTokens },
            { "-used",          (string s) => { TokensUsed = true; }}, 
            { "--continue",
                (string s) => { ContinueChatFromFile = true; Filename = s; }},
            { "--c",
                (string s) => { ContinueChatFromFile = true; Filename = s; }}
        };
    }

    public static GPTArgumentParser Instance
    {
        get
        {
            _instance ??= new GPTArgumentParser();
            return _instance;
        }
    }

    public string Model { get; private set; } = "gpt-4o";
    public int MaxTokens { get; private set; } = _defaultMaxTokens;

    // Sets behavioral context/guidelines
    public string SystemMessage { get; private set; }
    public string UserMessage { get; private set; }

    // Model's responses fed back into itself
    public string AssistantMessage { get; private set; }

    // flags
    public bool Debug { get; private set; } = false;
    public bool TokensUsed { get; private set; } = false;
    public bool TokensLeft { get; private set; } = false;
    public bool ContinueChatFromFile { get; private set; } = false;
    public string Filename { get; set; }
    protected override Dictionary<string, Action<string>> Flags { get; set; } 
    
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine($"Model: {Model}");
        sb.AppendLine($"Prompt: {UserMessage}");
        sb.AppendLine($"MaxTokens: {MaxTokens}");
        sb.AppendLine($"TokensUsed: {TokensUsed}");

        return sb.ToString();
    }

    private void SetMaxTokens(string s)
    {
        MaxTokens = Int32.TryParse(s, out int i) ? i : _defaultMaxTokens;
    }

    private static readonly int _defaultMaxTokens = 1000;
}

