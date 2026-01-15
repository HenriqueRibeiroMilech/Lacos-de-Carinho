namespace Ldc.Communication.Requests;

public class ResetPasswordJson
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
