/*
    CLI Parsing for chatgpt.csx
    Arguments:
        * all strings (?) TODO: check if you can pass int thru bash
        * [required] (optional) {required with flag}

    
*/

using System.Collections.Generic;

public abstract class IArgumentParser
{
    public bool TryParse(string input)
    {
        try
        {
            // check for errors / invalid format => return falsea
            List<string> inputs = [.. input.Split(' ')];
            bool isFlag;

            for(int i = 0; i < inputs.Count; i++)
            {
                if (inputs[i][..2] == "--")
                {
                    isFlag = false;
                    isFlag = _flags.TryGetValue(inputs[i][2..], out var flagCommand);
                    flagCommand?.Invoke(inputs[++i]);
                    if (flagCommand != null) i++;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"IArgument.TryParse() Error: {e.Message}");
            return false;
        }
        
        // if passes validation, parase
        return true;
    }

    public bool TryParse(IList<string> args)
    {
        try
        { 
            bool isFlag;

            for(int i = 0; i < args.Count; i++)
            {
                if (args[i].Length > 1 && args[i][..2] == "--")
                {
                    Console.WriteLine($"Found flag with value: {args[i]}");
                    isFlag = _flags.TryGetValue(args[i], out var flagCommand);
                    Console.WriteLine($"IsFlag: {isFlag}");
                    flagCommand?.Invoke(args[++i]);
                    if (flagCommand != null) i++;
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

    protected Dictionary<string, Action<string>> _flags;
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
    public bool TokensLeft { get; private set; } = false;
    public bool ContinueChatFromFile { get; private set; } = false;
    public string Filename { get; set; }

    public new Dictionary<string, Action<string>> _flags = new()
    {
        { "--q", 
            (string s) => { Instance.UserMessage = s; Console.WriteLine($"prompt set to: {s}"); }},
        { "--f",
            (string s) => { Instance.Filename = s; }},
        { "--max-tokens",
            (string s) => { Instance.MaxTokens = Int32.TryParse(s, out int opt) ? opt : _defaultMaxTokens; }},
        { "--continue",
            (string s) => { 
                Instance.ContinueChatFromFile = Boolean.TryParse(s, out bool result) && result; 
                // AssistantMessage = ...
                    /* - open file, explain context to chatgpt in system message that you are
                        sending it previous responses and prompts -- TBD what AssistantMessage role is in this
                                                                    -- i.e: send only gpt responses? they need link to prompts
                    - 
                */
                }}
    };

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine($"Model: {Model}");
        sb.AppendLine($"Prompt: {UserMessage}");
        sb.AppendLine($"MaxTokens: {MaxTokens}");

        return sb.ToString();
    }

    private static readonly int _defaultMaxTokens = 500;
}

