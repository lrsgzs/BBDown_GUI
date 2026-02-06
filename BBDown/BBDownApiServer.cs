using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BBDown;

public class BBDownApiServer
{
    public void SetUpServer() { }

    public void Run(string url) { }
}

public record DownloadTask(string Aid, string Url, long TaskCreateTime)
{
    [JsonInclude]
    public string? Title = null;
    [JsonInclude]
    public string? Pic = null;
    [JsonInclude]
    public long? VideoPubTime = null;
    [JsonInclude]
    public long? TaskFinishTime = null;
    [JsonInclude]
    public double Progress = 0f;
    [JsonInclude]
    public double DownloadSpeed = 0f;
    [JsonInclude]
    public double TotalDownloadedBytes = 0f;
    [JsonInclude]
    public bool IsSuccessful = false;

    [JsonInclude]
    public List<string> SavePaths = new();
}
