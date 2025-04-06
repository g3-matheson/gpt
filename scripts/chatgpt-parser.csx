/*
    CLI Parsing for chatgpt.csx
    Arguments:
        * all strings (?) TODO: check if you can pass int thru bash
        * [required] (optional) {required with flag}

    
*/

using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

public class ArgumentParser
{ 

    private static ArgumentParser _instance;

    private ArgumentParser() { }

    public static ArgumentParser Instance
    {
        get
        {
            _instance ??= new ArgumentParser();
            return _instance;
        }
    }

    // input arguments
    public static string Model { get; private set; }
    public static int MaxTokens { get; private set; }

    // Sets behavioral context/guidelines
    public static string SystemMessage { get; private set; }
    public static string UserMessage { get; private set; }

    // Model's responses fed back into itself?
    public static string AssistantMessage { get; private set; }
       
    // flags
    public static bool Debug { get; private set; }
    public static bool TokensLeft { get; private set; }
    public static bool ContinueChatFromFile { get; private set; }

    public static bool TryParse(string input)
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
                flagCommand(inputs[++i]);
            }
        }
        // if passes validation, parase
        return true;
    }

    private static readonly Dictionary<string, Action<string>> _flags = new()
    {
       { "--max-tokens",
            (string s) => { MaxTokens = Int32.TryParse(s, out int result)? result : _defalutMaxTokens; }},
       { "--continue",
            (string s) => { ContinueChatFromFile = Boolean.TryParse(s, out bool result) && result; }}
    };

    private static readonly int _defalutMaxTokens = 500;
}

