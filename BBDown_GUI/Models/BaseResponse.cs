using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace BBDown_GUI.Models;

public class BaseResponse<TData>
{
    [JsonPropertyName("code")] public int Code { get; set; } = -1;
    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;
    [JsonPropertyName("ttl")] public int Ttl { get; set; } = 1;
    [JsonPropertyName("data")] public TData? Data { get; set; } = default;

    public bool TryGetData([NotNullWhen(true)] out TData? data)
    {
        if (Code == 0 && Data != null)
        {
            data = Data;
            return true;
        }

        data = default;
        return false;
    }
}