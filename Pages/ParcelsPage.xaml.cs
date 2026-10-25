using PhytoGuard.ViewModels;

namespace PhytoGuard.Pages;

public partial class ParcelsPage : ContentPage
{
    private readonly ParcelsViewModel _vm;
    public ParcelsPage(ParcelsViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.Load();
    }
}

