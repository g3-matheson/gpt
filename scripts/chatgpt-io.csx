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
        if (File.Exists(filepath))
        {
            string fileContents = File.ReadAllText(filepath);
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