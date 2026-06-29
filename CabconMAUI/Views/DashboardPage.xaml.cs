using CabconMAUI.ViewModels;
namespace CabconMAUI.Views;
public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel vm){InitializeComponent();BindingContext=vm;}
}