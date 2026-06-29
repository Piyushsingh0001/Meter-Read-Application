using CabconMAUI.ViewModels;
namespace CabconMAUI.Views;
public partial class MeterReadPage : ContentPage
{
    public MeterReadPage(MeterReadViewModel vm){InitializeComponent();BindingContext=vm;}
}