using PhytoGuard.ViewModels;

namespace PhytoGuard.Pages;

public partial class DiagSymptomsPage : ContentPage
{
    private readonly DiagSymptomsViewModel _vm;
    public DiagSymptomsPage(DiagSymptomsViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _vm.Load();
    }
}

