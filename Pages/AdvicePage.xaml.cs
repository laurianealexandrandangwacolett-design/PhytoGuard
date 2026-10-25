using PhytoGuard.ViewModels;

namespace PhytoGuard.Pages;

public partial class AdvicePage : ContentPage
{
    private readonly AdviceViewModel _vm;
    public AdvicePage(AdviceViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.Load();
    }
}

