/*
    Data Structures for chatgpt.csx
*/
using System;
using System.Collections.Generic;
using System.Text;
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
