/*
    File I/O for chatgpt.csx
        * called by chatgpt-parser.csx for reading when --c present
        * called by chatgpt.csx for writing
*/

#load "chatgpt-data.csx"

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

string ResponseFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "chatgpt", "responses");

public GPTJson LoadFile(string filename)
{
    if (File.Exists(Path.Combine(ResponseFolder, filename)))
    {
        GPTJson result = JsonSerializer.Deserialize<GPTJson>(File.ReadAllText(filename));
        return result;
    }
    return null; 
}

// conversation should only contain NEW messages
public void SaveFile(GPTJson conversation, string filename, bool append = false)
{
    string s = JsonSerializer.Serialize(conversation);
    if (append && File.Exists(Path.Combine(ResponseFolder, filename)))
    {
        File.AppendAllText($"{filename}.json", s);
    }
    else 
    {
        File.WriteAllText($"{filename}.json", s);
    }
}

// use chatgpt-data.GPTJson here to store conversation and update accordingly
// expose public methods that add or remove from this structure
    // e.g AddPrompt() that adds the user message in the right place