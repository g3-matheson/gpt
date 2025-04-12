/*
    Data Structures for chatgpt.csx
*/

using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

class ResponseWrapper
{
    [JsonPropertyName("id")] public string ID { get; set; }
    [JsonPropertyName("object")] public string Object { get; set; }
    [JsonPropertyName("created")] public long TimeUTC { get; set; }
    [JsonPropertyName("model")] public string Model { get; set; }
    [JsonPropertyName("choices")] public List<ResponseChoice> Choices { get; set; }
    [JsonPropertyName("usage")] public ResponseTokenUsage TokenUsage { get; set; }
    [JsonPropertyName("service_tier")] public string ServiceTier { get; set; }
    [JsonPropertyName("system_fingerprint")] public string SystemFingerprint { get; set; }
}

class ResponseChoice
{
    [JsonPropertyName("index")] public int Index { get; set; }
    [JsonPropertyName("message")] public ResponseMessage Response { get; set; }
    [JsonPropertyName("logprobs")] public object LogProbs { get; set; }
    [JsonPropertyName("finish_reason")] public string FinishReason { get; set; }
}

class ResponseMessage
{
    [JsonPropertyName("role")] public string Role { get; set; }
    [JsonPropertyName("content")] public string Message { get; set; }
    [JsonPropertyName("refusal")] public object Refusal { get; set; } // bool? ?
    [JsonPropertyName("annotations")] public List<object> Annotations { get; set; }
}

class ResponseTokenUsage
{
    [JsonPropertyName("prompt_tokens")] public int PromptTokens { get; set; }
    [JsonPropertyName("completion_tokens")] public int CompletionTokens { get; set; }
    [JsonPropertyName("total_tokens")] public int TotalTokens { get; set; }
    [JsonPropertyName("prompt_tokens_details")] public Dictionary<string, int> PromptTokensDetails { get; set; }
    [JsonPropertyName("completion_tokens_details")] public Dictionary<string, int> CompletionTokensDetails { get; set; }
}

public class GPTJson
{
    [JsonPropertyName("messages")] public List<GPTMessage> Messages { get; set; }
}

public class GPTMessage
{
    public GPTMessage() {}
    public GPTMessage(GPTMessageRole role, string message, bool newMessage = false)
    {
        Role = role;
        Message = message;
        NewMessage = newMessage;
    }

    [JsonPropertyName("role")][JsonConverter(typeof(GPTMessageRoleConverter))]
    public GPTMessageRole Role { get; set; }
    [JsonPropertyName("content")] public string Message { get; set; }
    [JsonPropertyName("tokens-used-in")] public int TokensIn { get; set; }
    [JsonPropertyName("tokens-used-out")] public int TokensOut { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)] public bool NewMessage { get; set; }

    public bool Equals(GPTMessage other)
    {
        if (other is null) return false;
        if (this.Role == GPTMessageRole.System && other.Role == GPTMessageRole.System)
        {
            return true;
        }
        return Role == other.Role && Message == other.Message;
    }

    public override bool Equals(object obj) => Equals(obj as GPTMessage);

    public override int GetHashCode()
    {
        return HashCode.Combine(Role, Message);
    }

}

public enum GPTMessageRole
{
    System,
    User,
    Assistant
}
 
public static Dictionary<GPTMessageRole, string> RoleStrings = new()
{
    { GPTMessageRole.System, "system" },
    { GPTMessageRole.User, "user" },
    { GPTMessageRole.Assistant, "assistant" } 
};

public class GPTMessageRoleConverter : JsonConverter<GPTMessageRole>
{
    public override GPTMessageRole Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
       throw new NotImplementedException(); 
    }

    public override void Write(Utf8JsonWriter writer, GPTMessageRole value, JsonSerializerOptions options)
    {
        writer.WriteString("role", RoleStrings[value]); 
    }
}
