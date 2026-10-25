using PhytoGuard.ViewModels;

namespace PhytoGuard.Pages;

public partial class DiseaseDetailPage : ContentPage
{
    private readonly DiseaseDetailViewModel _vm;
    public DiseaseDetailPage(DiseaseDetailViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }
}

