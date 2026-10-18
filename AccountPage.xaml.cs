using Microsoft.Maui.Storage;

namespace PhytoGuard;

public partial class AccountPage : ContentPage
{
    public AccountPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        AccountNameLabel.Text =
            $"Nom : {Preferences.Get("UserName", "Non renseigné")}";

        AccountPhoneLabel.Text =
            $"Téléphone : {Preferences.Get("UserPhone", "Non renseigné")}";
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}