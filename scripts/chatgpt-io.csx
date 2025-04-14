/*
    File I/O for chatgpt.csx
        * called by chatgpt-parser.csx for reading when --c present
        * called by chatgpt.csx for writing
*/

#load "chatgpt-data.csx"

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

string ResponseFolder = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "chatgpt", "responses");

public GPTJson LoadFile(string filename)
{
    try
    {       
        string filepath = string.Concat(Path.Combine(ResponseFolder, filename), ".json");
        Console.WriteLine($"LoadFile filepath = {filepath}");
        if (File.Exists(filepath))
        {
            string fileContents = File.ReadAllText(filepath);
            Console.WriteLine($"LoadFile fileContents:\n{fileContents}");
            try
            {
                GPTJson result = JsonSerializer.Deserialize<GPTJson>(fileContents); 
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"LoadFile Error: {e.Message}");
                return null;
            }
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"chatgpt-io.LoadFile error: {e.Message}");
    }
    
    return null; 
}

// conversation should only contain NEW messages

        //WriteMessagesToFile(args.UserMessage, messages, string.IsNullOrEmpRoleStringsty(args.Filename) ? GetFilename() : args.Filename);
        // TODO replace with chatgpt-io.SaveFile
            // add TokenUsage to sent Prompt
            // transform response into GPTMessage with GPTMessageRole.Assistant, include TokenUsage
            // if System message was overwritten, overwrite entire file (check for GPTMessage with Role=System .NewMessage)
public void SaveFile(GPTJson conversation, string filename)
{  
    string filepath = Path.Combine(ResponseFolder, filename);

    string jsonString = JsonSerializer.Serialize(conversation);
    using var jsonDocument = JsonDocument.Parse(jsonString);
    var options = new JsonWriterOptions
    {
        Indented = true
    };

    using var stream = new MemoryStream();
    using (var writer = new Utf8JsonWriter(stream, options))
    {
        jsonDocument.WriteTo(writer);
    }

    string formattedJson = Encoding.UTF8.GetString(stream.ToArray());

    File.WriteAllLines($"{filepath}.json", formattedJson.Split(Environment.NewLine));
       
}