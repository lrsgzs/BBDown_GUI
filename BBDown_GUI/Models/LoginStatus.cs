namespace BBDown_GUI.Models;

public class LoginStatus
{
    public bool IsSuccessfully { get; set; } = false;
    public int Status { get; set; } = -1;
    public string Reason { get; set; } = "还未开始登录";

    public LoginStatus() { }
    
    public LoginStatus(bool isSuccessfully, int status, string reason)
    {
        IsSuccessfully = isSuccessfully;
        Status = status;
        Reason = reason;
    }
}