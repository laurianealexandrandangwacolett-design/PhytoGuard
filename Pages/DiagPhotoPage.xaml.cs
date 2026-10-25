using PhytoGuard.ViewModels;

namespace PhytoGuard.Pages;

public partial class DiagPhotoPage : ContentPage
{
    private readonly DiagPhotoViewModel _vm;
    public DiagPhotoPage(DiagPhotoViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }
}

