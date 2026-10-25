using PhytoGuard.ViewModels;

namespace PhytoGuard.Pages;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _vm;
    public HomePage(HomeViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.Load();
    }
}

