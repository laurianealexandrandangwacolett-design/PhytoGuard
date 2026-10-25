using PhytoGuard.ViewModels;

namespace PhytoGuard.Pages;

public partial class DiagCulturePage : ContentPage
{
    private readonly DiagCultureViewModel _vm;
    public DiagCulturePage(DiagCultureViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.Load();
    }
}

