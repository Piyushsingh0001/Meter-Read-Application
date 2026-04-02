using CabconMAUI.ViewModels;
namespace CabconMAUI.Views;
public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm){InitializeComponent();BindingContext=vm;}
}