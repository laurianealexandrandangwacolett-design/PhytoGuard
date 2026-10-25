using PhytoGuard.ViewModels;

namespace PhytoGuard.Pages;

public partial class WelcomePage : ContentPage
{
    private readonly WelcomeViewModel _vm;
    public WelcomePage(WelcomeViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Session déjà ouverte : aller directement à l'accueil.
        if (_vm.IsLoggedIn) Dispatcher.Dispatch(async () => await Shell.Current.GoToAsync("//home"));
    }
}

