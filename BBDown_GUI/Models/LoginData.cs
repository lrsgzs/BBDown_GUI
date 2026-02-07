namespace BBDown_GUI.Models;

public class LoginData
{
    public string QrCodePath { get; set; } = string.Empty;
    public string QrCodeKey { get; set; } = string.Empty;

    public LoginData() { }
    
    public LoginData(string qrCodePath, string qrCodeKey)
    {
        QrCodePath = qrCodePath;
        QrCodeKey = qrCodeKey;
    }
}