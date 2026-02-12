using BBDown.Core.Util;

namespace BBDown_GUI.Models.Video;

public class VideoPageInfo
{
    public required int Index { get; init; }
    public required string Aid { get; init; }
    public required string Title { get; init; }
    public required int DurationInt { get; set; }
    
    public string BvId => BilibiliBvConverter.Encode(long.Parse(Aid));
    public string Duration => Utils.FormatTime(DurationInt);
}