namespace SecurityDemo;

public class SastVulnerableDemo
{
    public void Demo()
    {
        string? password = configuration["APP_PASSWORD"];
        string? token = configuration["APP_TOKEN"];

        var cookieOptions = new Microsoft.AspNetCore.Http.CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict
        };
    }
}
