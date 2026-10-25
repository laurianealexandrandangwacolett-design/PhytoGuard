using PhytoGuard.ViewModels;

namespace PhytoGuard.Pages;

public partial class ExpertsPage : ContentPage
{
    private readonly ExpertsViewModel _vm;
    public ExpertsPage(ExpertsViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }
}

