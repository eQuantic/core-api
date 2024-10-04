using System.Text.Json.Serialization;

namespace eQuantic.Core.Api.Error.Results;

public class ErrorResult
{
    public string Code { get; set; } = string.Empty;
    public string? Message { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Details { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, string[]>? Errors { get; set; }
    
    public ErrorResult()
    {
    }

    public ErrorResult(
        string code,
        string? message, 
        IDictionary<string, string[]>? errors = null,
        string? details = null
        )
    {
        Code = code;
        Message = message;
        Details = details;
        Errors = errors;
    }
}