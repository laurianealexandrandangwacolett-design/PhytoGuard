using PhytoGuard.ViewModels;

namespace PhytoGuard.Pages;

public partial class HistoryPage : ContentPage
{
    private readonly HistoryViewModel _vm;
    public HistoryPage(HistoryViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.Load();
    }
}

