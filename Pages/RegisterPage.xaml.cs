using PhytoGuard.ViewModels;

namespace PhytoGuard.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly RegisterViewModel _vm;
    public RegisterPage(RegisterViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }
}

