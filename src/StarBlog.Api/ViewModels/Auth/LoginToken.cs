namespace StarBlog.Api.ViewModels.Auth;

public class LoginToken {
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}