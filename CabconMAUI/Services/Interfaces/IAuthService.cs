namespace CabconMAUI.Services.Interfaces;
public interface IAuthService
{
    bool IsAuthenticated{get;} string CurrentUser{get;}
    Task<bool> LoginAsync(string userId,string password); void Logout();
}
