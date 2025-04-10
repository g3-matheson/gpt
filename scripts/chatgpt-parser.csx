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

    private GPTArgumentParser() { }

    public static GPTArgumentParser Instance
    {
        get
        {
            _instance ??= new GPTArgumentParser();
            return _instance;
        }
    }

    // input arguments
    public string Model { get; private set; } = "gpt-4o";
    public int MaxTokens { get; private set; } = _defaultMaxTokens; // out, not in

    // Sets behavioral context/guidelines
    public string SystemMessage { get; private set; }
    public string UserMessage { get; private set; }

    // Model's responses fed back into itself?
    public string AssistantMessage { get; private set; }

    // flags
    public bool Debug { get; private set; } = false;
    public bool TokensUsed { get; private set; } = false;
    public bool TokensLeft { get; private set; } = false;
    public bool ContinueChatFromFile { get; private set; } = false;
    public string Filename { get; set; }
    protected override Dictionary<string, Action<string>> Flags { get; set; } = new() 
    {
        { "--q", 
            (string s) => { Instance.UserMessage = s; }},
        { "--f",
            (string s) => { Instance.Filename = s; }},
        { "--max-tokens",
            (string s) => { Instance.MaxTokens = Int32.TryParse(s, out int opt) ? opt : _defaultMaxTokens; }},  
        { "-used",
            (string s) => { Instance.TokensUsed = true; }}, 
        { "--continue",
            (string s) => { 
                Instance.ContinueChatFromFile = Boolean.TryParse(s, out bool result) && result; 
                // AssistantMessage = ...
                    /* - open file, explain context to chatgpt in system message that you are
                        sending it previous responses and prompts -- TBD what AssistantMessage role is in this
                                                                    -- i.e: send only gpt responses? they need link to prompts
                    - 
                */
                }},
    };

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine($"Model: {Model}");
        sb.AppendLine($"Prompt: {UserMessage}");
        sb.AppendLine($"MaxTokens: {MaxTokens}");
        sb.AppendLine($"TokensUsed: {TokensUsed}");

        return sb.ToString();
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    private static readonly int _defaultMaxTokens = 1000;
}

