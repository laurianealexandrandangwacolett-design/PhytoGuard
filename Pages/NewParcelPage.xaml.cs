using PhytoGuard.ViewModels;

namespace PhytoGuard.Pages;

public partial class NewParcelPage : ContentPage
{
    private readonly NewParcelViewModel _vm;
    public NewParcelPage(NewParcelViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }
}

