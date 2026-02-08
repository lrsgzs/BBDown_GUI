namespace BBDown_GUI.Models;

public class WebLoginData
{
    public string QrCodePath { get; set; } = string.Empty;
    public string QrCodeKey { get; set; } = string.Empty;

    public WebLoginData() { }
    
    public WebLoginData(string qrCodePath, string qrCodeKey)
    {
        QrCodePath = qrCodePath;
        QrCodeKey = qrCodeKey;
    }
}