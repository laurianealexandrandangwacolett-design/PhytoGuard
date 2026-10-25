using PhytoGuard.ViewModels;

namespace PhytoGuard.Pages;

public partial class PaymentPage : ContentPage
{
    private readonly PaymentViewModel _vm;
    public PaymentPage(PaymentViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _vm.Load();
    }
}

