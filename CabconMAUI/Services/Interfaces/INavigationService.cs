namespace CabconMAUI.Services.Interfaces;
public interface INavigationService
{
    Task NavigateToAsync(string route); Task GoBackAsync();
}
