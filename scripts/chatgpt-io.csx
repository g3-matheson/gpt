/*
    File I/O for chatgpt.csx
        * called by chatgpt-parser.csx for reading when --c present
        * called by chatgpt.csx for writing
*/

#load "chatgpt-data.csx"

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

string ResponseFolder = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
    "chatgpt", "responses");

public GPTJson LoadFile(string filename)
{
    string filepath = Path.Combine(ResponseFolder, filename);
    if (File.Exists(filepath))
    {
        GPTJson result = JsonSerializer.Deserialize<GPTJson>(File.ReadAllText(filepath));
        return result;
    }
    return null; 
}

// conversation should only contain NEW messages

        //WriteMessagesToFile(args.UserMessage, messages, string.IsNullOrEmpRoleStringsty(args.Filename) ? GetFilename() : args.Filename);
        // TODO replace with chatgpt-io.SaveFile
            // add TokenUsage to sent Prompt
            // transform response into GPTMessage with GPTMessageRole.Assistant, include TokenUsage
            // if System message was overwritten, overwrite entire file (check for GPTMessage with Role=System .NewMessage)
public void SaveFile(GPTJson conversation, string filename, bool append = false)
{  
    string filepath = Path.Combine(ResponseFolder, filename);

    GPTMessage systemMessage = conversation.Messages.First((GPTMessage m) => m.Role == GPTMessageRole.System);
    if (systemMessage.NewMessage)
    {
        append = false;
    }
    else
    {
        conversation.Messages.RemoveAll(m => !m.NewMessage);
    }

    string s = JsonSerializer.Serialize(conversation);
    Console.WriteLine("SaveFile: \n${s}");
    if (append && File.Exists(filepath))
    {
        File.AppendAllText($"{filepath}.json", s);
    }
    else 
    {
        File.WriteAllText($"{filepath}.json", s);
    }
}

// use chatgpt-data.GPTJson here to store conversation and update accordingly
// expose public methods that add or remove from this structure
    // e.g AddPrompt() that adds the user message in the right place