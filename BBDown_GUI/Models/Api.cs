using System.Text.Json.Serialization;

namespace BBDown_GUI.Models;

public static class Api
{
    public static class X
    {
        public static class WebInterface
        {
            /// <summary>
            /// https://api.bilibili.com/x/web-interface/nav
            /// </summary>
            public class NavData
            {
                public class ModelLevelInfo
                {
                    [JsonPropertyName("current_level")] public int CurrentLevel { get; set; } = -1;
                }

                public class ModelWbiImg
                {
                    [JsonPropertyName("img_url")] public string ImgUrl { get; set; } = string.Empty;
                    [JsonPropertyName("sub_url")] public string SubUrl { get; set; } = string.Empty;
                }
    
                [JsonPropertyName("isLogin")] public bool IsLogin { get; set; } = false;
                
                [JsonPropertyName("mid")] public long MId { get; set; } = -1;
                [JsonPropertyName("uname")] public string UName { get; set; } = string.Empty;
                [JsonPropertyName("face")] public string Face { get; set; } = string.Empty;
                [JsonPropertyName("level_info")] public ModelLevelInfo LevelInfo { get; set; } = new();

                [JsonPropertyName("wbi_img")] public ModelWbiImg WbiImg { get; set; } = new();
            }
        }
    }
}