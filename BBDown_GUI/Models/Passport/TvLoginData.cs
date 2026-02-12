using System.Collections.Specialized;

namespace BBDown_GUI.Models.Passport;

public class TvLoginData
{
    public NameValueCollection Params { get; set; }
    public string QrCodePath { get; set; } = string.Empty;
    
    public TvLoginData() { }

    public TvLoginData(NameValueCollection tvParams, string qrCodePath)
    {
        Params = tvParams;
        QrCodePath = qrCodePath;
    }
}