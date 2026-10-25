using PhytoGuard.ViewModels;

namespace PhytoGuard.Pages;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _vm;
    public LoginPage(LoginViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }
}

