using PhytoGuard.ViewModels;

namespace PhytoGuard.Pages;

public partial class DiagResultPage : ContentPage
{
    private readonly DiagResultViewModel _vm;
    public DiagResultPage(DiagResultViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _vm.Load();
    }
}

