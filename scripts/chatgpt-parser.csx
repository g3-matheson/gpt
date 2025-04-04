/*
    CLI Parsing for chatgpt.csx
    Arguments:
        * all strings (?) TODO: check if you can pass int thru bash
        * [required] (optional) {required with flag}

    Standard:                   gpt [prompt] (filename)
    Standard with debug:        gpt [prompt] (filename) -d
*/

public class ArgumentParser
{ 
    // input arguments
    public string Model { get; private set; }
    public int MaxTokens { get; private set; }
    public string SystemMessage { get; private set; }
    public string UserMessage { get; private set; }
    public string AssistantMessage { get; private set; }
       
    // flags
    public bool Debug { get; private set; }
    public bool TokensLeft { get; private set; }
    public bool ContinueChatFromFile { get; private set; }

    public bool TryParse(string input)
    {
        // check for errors / invalid format => return false
        // if passes validation, parse
        return true;
    }

}

